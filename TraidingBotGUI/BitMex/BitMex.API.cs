using System;
using System.Threading;
using System.Threading.Tasks;
using Bitmex.Client.Websocket.Client;
using Bitmex.Client.Websocket.Requests;
using Bitmex.Client.Websocket.Websockets;
using Bitmex.Client.Websocket;
using Serilog;
using Serilog.Events;
using System.Linq;
using System.IO;
using System.Reflection;
using Windows.Storage;
using System.Collections.Generic;

namespace Bitmex.API
{
    public static class BitMexAPI
    {
        public static List<double> PriceList = new List<double>();
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);

        private static readonly string API_KEY = "5QRSIVI5g2jofJPRgND3h4l1";
        private static readonly string API_SECRET = "vsfHfrDQ-jcdzuH7pjc-CMuTA8dThz95T0ZkNzL1AYwdXRVI";

        public static void Start()
        {
            InitLogging();

            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;

            Log.Debug("====================================");
            Log.Debug("              STARTING              ");
            Log.Debug("====================================");

            var url = BitmexValues.ApiWebsocketUrl;
            using (var communicator = new BitmexWebsocketCommunicator(url))
            {
                communicator.ReconnectTimeoutMs = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                communicator.ReconnectionHappened.Subscribe(type =>
                    Log.Information($"Reconnection happened, type: {type}"));

                using (var client = new BitmexWebsocketClient(communicator))
                {
                    client.Streams.InfoStream.Subscribe(info =>
                    {
                        Log.Information($"Reconnection happened, Message: {info.Info}, Version: {info.Version:D}");
                        SendSubscriptionRequests(client);
                    });

                    SubscribeToStreams(client);

                    communicator.Start();

                    ExitEvent.WaitOne();
                }
            }

        }

        private static void NewMethod(BitmexWebsocketClient client)
        {
            client.Streams.InfoStream.Subscribe(info =>
            {
                Log.Information($"Reconnection happened, Message: {info.Info}, Version: {info.Version:D}");
                SendSubscriptionRequests(client);
            });
        }

        private static void SendSubscriptionRequests(BitmexWebsocketClient client)
        {
            client.Send(new PingRequest()).Wait();
            client.Send(new BookSubscribeRequest()).Wait();
            client.Send(new TradesSubscribeRequest("XBTUSD")).Wait();
            client.Send(new TradeBinSubscribeRequest("1m", "XBTUSD")).Wait();
            client.Send(new TradeBinSubscribeRequest("5m", "XBTUSD")).Wait();
            client.Send(new QuoteSubscribeRequest("XBTUSD")).Wait();
            client.Send(new LiquidationSubscribeRequest()).Wait();

            
            if (!string.IsNullOrWhiteSpace(API_SECRET))
                client.Send(new AuthenticationRequest(API_KEY, API_SECRET)).Wait();
        }

        private static void SubscribeToStreams(BitmexWebsocketClient client)
        {
            client.Streams.ErrorStream.Subscribe(x =>
                Log.Warning($"Error received, message: {x.Error}, status: {x.Status}"));

            client.Streams.AuthenticationStream.Subscribe(x =>
            {
                Log.Information($"Authentication happened, success: {x.Success}");
                client.Send(new WalletSubscribeRequest()).Wait();
                client.Send(new OrderSubscribeRequest()).Wait();
                client.Send(new PositionSubscribeRequest()).Wait();
            });


            client.Streams.SubscribeStream.Subscribe(x =>
                Log.Information($"Subscribed ({x.Success}) to {x.Subscribe}"));

            client.Streams.PongStream.Subscribe(x =>
                Log.Information($"Pong received ({x.Message})"));


            client.Streams.WalletStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Log.Information($"Wallet {x.Account}, {x.Currency} amount: {x.BalanceBtc}"))
            );
            client.Streams.OrderStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Log.Information(
                        $"Order {x.Symbol} updated. Time: {x.Timestamp:HH:mm:ss.fff}, Amount: {x.OrderQty}, " +
                        $"Price: {x.Price}, Direction: {x.Side}, Working: {x.WorkingIndicator}, Status: {x.OrdStatus}"))
            );

            client.Streams.PositionStream.Subscribe(y =>
                y.Data.ToList().ForEach(x => {
                    Log.Information(
                        $"Position {x.Symbol}, {x.Currency} updated. Time: {x.Timestamp:HH:mm:ss.fff}, Amount: {x.CurrentQty}, " +
                        $"Price: {x.LastPrice}, PNL: {x.UnrealisedPnl}");
                    PriceList.Add((double)x.LastPrice);
                })

                
            );

            client.Streams.TradesStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Log.Information($"Trade {x.Symbol} executed. Time: {x.Timestamp:mm:ss.fff}, [{x.Side}] Amount: {x.Size}, " +
                                    $"Price: {x.Price}, Direction: {x.TickDirection}"))
            );

            client.Streams.BookStream.Subscribe(book =>
                book.Data.Take(100).ToList().ForEach(x => Log.Information(
                    $"Book | {book.Action} pair: {x.Symbol}, price: {x.Price}, amount {x.Size}, side: {x.Side}"))
            );

            client.Streams.QuoteStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Log.Information($"Quote {x.Symbol}. Bid: {x.BidPrice} - {x.BidSize} Ask: {x.AskPrice} - {x.AskSize}"))
            );

            client.Streams.LiquidationStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                    Log.Information(
                        $"Liquadation Action: {y.Action}, OrderID: {x.OrderID}, Symbol: {x.Symbol}, Side: {x.Side}, Price: {x.Price}, LeavesQty: {x.leavesQty}"))
            );

            client.Streams.TradeBinStream.Subscribe(y =>
                y.Data.ToList().ForEach(x =>
                Log.Information($"TradeBin table:{y.Table} {x.Symbol} executed. Time: {x.Timestamp:mm:ss.fff}, Open: {x.Open}, " +
                        $"Close: {x.Close}, Volume: {x.Volume}, Trades: {x.Trades}"))
            );

        }

        private static void InitLogging()
        {
            //var executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var executingDir = ApplicationData.Current.LocalFolder.Path;
            var logPath = Path.Combine(executingDir, "logs", "verbose.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .WriteTo.ColoredConsole(LogEventLevel.Information)
                .CreateLogger();
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            Log.Warning("Exiting process");
            ExitEvent.Set();
        }

    }
}
