using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bitmex.Client.Websocket.Validations;
using Serilog;

namespace Bitmex.Client.Websocket.Websockets
{
    public class BitmexWebsocketCommunicator : IDisposable
    {
        private readonly Uri _url;
        private Timer _lastChanceTimer;
        private readonly Func<ClientWebSocket> _clientFactory;

        private DateTime _lastReceivedMsg = DateTime.UtcNow; 

        private bool _disposing = false;
        private bool _started = false;
        private ClientWebSocket _client;
        private CancellationTokenSource _cancelation;

        private readonly Subject<string> _messageReceivedSubject = new Subject<string>();
        private readonly Subject<ReconnectionType> _reconnectionSubject = new Subject<ReconnectionType>();

        private readonly BlockingCollection<string> _messagesToSendQueue = new BlockingCollection<string>();

        public BitmexWebsocketCommunicator(Uri url, Func<ClientWebSocket> clientFactory = null)
        {
            BmxValidations.ValidateInput(url, nameof(url));

            _url = url;
            _clientFactory = clientFactory ?? (() => new ClientWebSocket()
            {
                Options = {KeepAliveInterval = new TimeSpan(0, 0, 5, 0)}
            }); 
        }

        /// <summary>
        /// Stream with received message (raw format)
        /// </summary>
        public IObservable<string> MessageReceived => _messageReceivedSubject.AsObservable();

        /// <summary>
        /// Stream for reconnection event (trigerred after the new connection) 
        /// </summary>
        public IObservable<ReconnectionType> ReconnectionHappened => _reconnectionSubject.AsObservable();

        /// <summary>
        /// Time range in ms, how long to wait before reconnecting if no message comes from server.
        /// Default 60000 ms (1 minute)
        /// </summary>
        public int ReconnectTimeoutMs { get; set; } = 60 * 1000;

        /// <summary>
        /// Time range in ms, how long to wait before reconnecting if last reconnection failed.
        /// Default 60000 ms (1 minute)
        /// </summary>
        public int ErrorReconnectTimeoutMs { get; set; } = 60 * 1000;

        /// <summary>
        /// Terminate the websocket connection and cleanup everything
        /// </summary>
        public void Dispose()
        {
            _disposing = true;
            Log.Debug(L("Disposing.."));
            _lastChanceTimer?.Dispose();
            _cancelation?.Cancel();
            _client?.Abort();
            _client?.Dispose();
            _cancelation?.Dispose();
            _messagesToSendQueue?.Dispose();
            _started = false;
        }
       
        /// <summary>
        /// Start listening to the websocket stream on the background thread
        /// </summary>
        public Task Start()
        {
            if (_started)
            {
                Log.Debug(L("Communicator already started, ignoring.."));
                return Task.CompletedTask;
            }
            _started = true;

            Log.Debug(L("Starting.."));
            _cancelation = new CancellationTokenSource();

            Task.Factory.StartNew(_ => SendFromQueue(), TaskCreationOptions.LongRunning, _cancelation.Token);

            return StartClient(_url, _cancelation.Token, ReconnectionType.Initial);
        }

        /// <summary>
        /// Send message to the websocket channel. 
        /// It inserts the message to the queue and actual sending is done on an other thread
        /// </summary>
        /// <param name="message">Message to be sent</param>
        public Task Send(string message)
        {
            BmxValidations.ValidateInput(message, nameof(message));

            _messagesToSendQueue.Add(message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Send message to the websocket channel. 
        /// It doesn't use a sending queue, 
        /// beware of issue while sending two messages in the exact same time 
        /// on the full .NET Framework platform
        /// </summary>
        /// <param name="message">Message to be sent</param>
        public Task SendInstant(string message)
        {
            BmxValidations.ValidateInput(message, nameof(message));

            return SendInternal(message);
        }

        private async Task SendFromQueue()
        {
            foreach (var message in _messagesToSendQueue.GetConsumingEnumerable(_cancelation.Token))
            {
                SendInternal(message).Wait();
            }
        }

        private async Task SendInternal(string message)
        {
            Log.Verbose(L($"Sending:  {message}"));
            var buffer = Encoding.UTF8.GetBytes(message);
            var messageSegment = new ArraySegment<byte>(buffer);
            var client = await GetClient();
            client.SendAsync(messageSegment, WebSocketMessageType.Text, true, _cancelation.Token).Wait();
        }

        private async Task StartClient(Uri uri, CancellationToken token, ReconnectionType type)
        {
            DeactiveLastChance();
            _client = _clientFactory();
            
            try
            {
                _client.ConnectAsync(uri, token).Wait();
#pragma warning disable 4014
                Listen(_client, token);
#pragma warning restore 4014               
                ActivateLastChance();
                _reconnectionSubject.OnNext(type);
            }
            catch (Exception e)
            {
                Log.Error(e, L("Exception while connecting. " +
                               $"Waiting {ErrorReconnectTimeoutMs/1000} sec before next reconnection try."));
                Task.Delay(ErrorReconnectTimeoutMs, token).Wait();
                Reconnect(ReconnectionType.Error).Wait();
            }
            
        }

        private async Task<ClientWebSocket> GetClient()
        {
            if (_client == null || (_client.State != WebSocketState.Open && _client.State != WebSocketState.Connecting))
            {
                Reconnect(ReconnectionType.Lost).Wait();
            }
            return _client;
        }

        private async Task Reconnect( ReconnectionType type)
        {
            if (_disposing)
                return;
            Log.Debug(L("Reconnecting..."));
            _cancelation.Cancel();
            Task.Delay(1000).Wait();

            _cancelation = new CancellationTokenSource();
            StartClient(_url, _cancelation.Token, type).Wait();
        }

        private async Task Listen(ClientWebSocket client, CancellationToken token)
        {
            try
            {
                do
                {
                    WebSocketReceiveResult result = null;
                    var buffer = new byte[1000];
                    var message = new ArraySegment<byte>(buffer);
                    var resultMessage = new StringBuilder();
                    do
                    {
                        result = client.ReceiveAsync(message, token).Result;
                        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        resultMessage.Append(receivedMessage);
                        if (result.MessageType != WebSocketMessageType.Text)
                            break;

                    } while (!result.EndOfMessage);

                    var received = resultMessage.ToString();
                    Log.Verbose(L($"Received:  {received}"));
                    _lastReceivedMsg = DateTime.UtcNow;
                    _messageReceivedSubject.OnNext(received);

                } while (client.State == WebSocketState.Open && !token.IsCancellationRequested);
            }
            catch (TaskCanceledException)
            {
                // task was canceled, ignore
            }
            catch (Exception e)
            {
                Log.Error(e, L("Error while listening to websocket stream"));
            }
        }

        private void ActivateLastChance()
        {
            var timerMs = 1000 * 5;
            _lastChanceTimer = new Timer(async x => await LastChance(x), null, timerMs, timerMs);
        }

        private void DeactiveLastChance()
        {
            _lastChanceTimer?.Dispose();
            _lastChanceTimer = null;
        }

        private async Task LastChance(object state)
        {
            var timeoutMs = Math.Abs(ReconnectTimeoutMs);
            var halfTimeoutMs = timeoutMs / 1.5;
            var diffMs = Math.Abs(DateTime.UtcNow.Subtract(_lastReceivedMsg).TotalMilliseconds);
            if(diffMs > halfTimeoutMs)
                Log.Debug(L($"Last message received {diffMs:F} ms ago"));
            if (diffMs > timeoutMs)
            {
                Log.Debug(L($"Last message received more than {timeoutMs:F} ms ago. Hard restart.."));

                _client?.Abort();
                _client?.Dispose();
                Reconnect(ReconnectionType.NoMessageReceived).Wait();
            }
        }

        private string L(string msg)
        {
            return $"[BMX WEBSOCKET COMMUNICATOR] {msg}";
        }
    }
}
