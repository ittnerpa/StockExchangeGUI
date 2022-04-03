using Binance.API.Csharp.Client.Models.Enums;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Binance.API.Csharp.Client.Models.Market;
using Binance.API.Csharp.Client.Models.WebSocket;
using Binance.API.Csharp.Client;
using DigitalAsset.Coins.Indikatoren;
using System.Collections.Generic;
using DigitalAsset.Coins;
using System;
using TraidingBotGUI.Utilitys;

namespace TraidingBotGUI
{
    public class BinanceAPI
    {
        //Keep this keys secretly - accesspoints to your Account
        private static readonly string apikey = "HqaKEfPJrBU8DPZbB7tHQlBTH2ywoTIvSUWHs2RardZCXed7VqneUMyy756WdsZW";
        private static readonly string secretkey = "xtXhwDNcCnTH9NYJrjs9KDXQzkNXlQQKlcYgriJPHfiQLMmaRjXwJ4gPikZnXwdj";

        //setup the Binance API
        private static ApiClient apiClient = new ApiClient(apikey, secretkey);
        private static BinanceClient binanceClient = new BinanceClient(apiClient, false);

        //Contains all Coins of the Binance API
        private static List<Coin> coins = new List<Coin>();

        //Initialize the Timer
        private static System.Timers.Timer LoopTimer;

        public BinanceAPI()
        {
        }

        /// <summary>
        /// Start the traiding bot
        /// </summary>
        /// <param name="LoopInterval">Interval for receiving data (ms)</param>
        public void StartServer(int LoopInterval = 60000)
        {
            //Main loop
            LoopTimer = new System.Timers.Timer(LoopInterval);
            LoopTimer.Elapsed += MainLoop;

            Logger.WriteLog("Start Server... ", Logger.log.State);

            InitializeAccount();
            InitializeTraidingHistory();

            LoopTimer.Start();
        }

        /// <summary>
        /// Wont receive any further informatione from exchange
        /// </summary>
        public void StopServer()
        {
            //Stop the loop
            LoopTimer.Stop();

            //Write log
            Logger.WriteLog("Client stopped... ", Logger.log.State);
        }

        /// <summary>
        /// Add further Coins for traiding
        /// </summary>
        /// <param name="name">Name of the coin</param>
        /// <param name="pair">Traidingpair</param>
        /// <param name="interval">Request interval</param>
        public void AddCoin(string name, string pair, TimeInterval interval)
        {
            coins.Add(new Coin(name, pair, true, CoinIndikatoren.RSI));
        }

        /// <summary>
        /// Get all data from a crypto asset
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Coin GetCoin(string name)
        {
            foreach (Coin coin in coins)
            {
                if (coin.Name.Contains(name))
                    return coin;
            }

            return null;
        }

        /// <summary>
        /// Returns a list with all current coins
        /// </summary>
        /// <returns></returns>
        public List<Coin> GetAllCoins()
        {
            return coins;
        }

        /// <summary>
        /// Shows the current value of all listed Coins
        /// </summary>
        public void ShowAllCoins()
        {
            foreach (Coin coin in coins)
            {
                Console.WriteLine(coin.Name + ":");
                coin.ShowAllPrices();
                foreach (string pair in coin.GetTraidingPairs())
                    coin.ShowIndicator(pair);

            }
        }

        /// <summary>
        /// Setup the traiding Account of the Binance Client
        /// Get all current assets of the account
        /// </summary>
        private void InitializeAccount()
        {
            try
            {
                //Get Account information from binance
                var accountInfo = binanceClient.GetAccountInfo();

                //Sort balances of the account
                foreach (Binance.API.Csharp.Client.Models.Market.Balance balance in accountInfo.Balances)
                {
                    if (balance.Free > 0.00m)
                    {
                        if (balance.Asset.Contains("BTC"))
                            coins.Add(new Coin("BTC", "BTCUSDT", true, CoinIndikatoren.RSI));
                        else if (balance.Asset.Contains("USDT"))
                            coins.Add(new Coin("USDT", false));
                        else
                            coins.Add(new Coin(balance.Asset, balance.Asset + "BTC", true, CoinIndikatoren.RSI));
                    }
                }

                //Write Log
                Logger.WriteLog("InitiliazeAccount: finished", Logger.log.State);

                return;
            }
            catch(Exception e)
            {
                ;
            }
        }

        /// <summary>
        /// Get a history of all assets of the account to initialize the indicator
        /// </summary>
        private void InitializeTraidingHistory()
        {
            double dateMinute = -49.0;
            foreach (Coin coin in coins)
            {
                List<string> TraidingPairList = coin.GetTraidingPairs();
                foreach (string pair in TraidingPairList)
                {
                    var candlestick = (List<Candlestick>)binanceClient.GetCandleSticks(pair, TimeInterval.Minutes_1, null, null, 50);
                    foreach (Candlestick stick in candlestick)
                    {
                        if (coin.Traiding)
                        {
                            coin.AddPrice(pair, stick.High, stick.Low, stick.Open, stick.Close, stick.Volume, DateTime.Now.AddMinutes(dateMinute));
                            dateMinute++;
                        }
                    }
                }
            }

            Logger.WriteLog("Initialize TraidingHistory: finished ", Logger.log.State);

        }

        /// <summary>
        /// Main Task -- got called frequently to receive data from the exchange and evaluate them to buy/sell
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void MainLoop(object source, System.Timers.ElapsedEventArgs e)
        {
            foreach (Coin coin in coins)
            {
                List<string> TraidingPairList = coin.GetTraidingPairs();
                foreach (string pair in TraidingPairList)
                {
                    var candlestick = (List<Candlestick>)binanceClient.GetCandleSticks(pair, TimeInterval.Minutes_1, null, null, 1);
                    foreach (Candlestick stick in candlestick)
                    {
                        //Receive Data
                        //Receive only data if traiding is activated for this coin
                        if (coin.Traiding)
                            //Add price and evaluate data
                            coin.AddPrice(pair,stick.High, stick.Low, stick.Open, stick.Close, stick.Volume, DateTime.Now);
                        else
                            continue;

                        //Buy/Sell Asset
                        TraidingFlags flags = coin.EvaluatePrice(pair);
                        if (flags.Buy)
                            Logger.WriteLog("Buy " + pair + "at: " + stick.Close, Logger.log.Data);
                        else if (flags.Sell)
                            Logger.WriteLog("Sell " + pair + "at: " + stick.Close, Logger.log.Data);
                            
                    }
                }
            }

        }

        #region General
        public void TestConnectivity()
        {
            var test = binanceClient.TestConnectivity();

        }


        public void GetServerTime()
        {
            var serverTime = binanceClient.GetServerTime();
        }
        #endregion

        #region Market Data

        public void GetOrderBook()
        {
            var orderBook = binanceClient.GetOrderBook("ethbtc");
        }


        public void GetCandleSticks()
        {
            var candlestick = binanceClient.GetCandleSticks("ethbtc", TimeInterval.Minutes_15, new System.DateTime(2017, 11, 24), new System.DateTime(2017, 11, 26));
        }


        public void GetAggregateTrades()
        {
            var aggregateTrades = binanceClient.GetAggregateTrades("ethbtc");
        }


        public void GetPriceChange24H()
        {
            var singleTickerInfo = binanceClient.GetPriceChange24H("ETHBTC");

            var allTickersInfo = binanceClient.GetPriceChange24H();
        }


        public void GetAllPrices()
        {
            var tickerPrices = binanceClient.GetAllPrices();
        }


        public void GetOrderBookTicker()
        {
            var orderBookTickers = binanceClient.GetOrderBookTicker();
        }
        #endregion

        #region Account Information

        public void PostLimitOrder()
        {
            var buyOrder = binanceClient.PostNewOrder("KNCETH", 100m, 0.005m, OrderSide.BUY);
            var sellOrder = binanceClient.PostNewOrder("KNCETH", 1000m, 1m, OrderSide.SELL);
        }


        public void PostMarketOrder()
        {
            var buyMarketOrder = binanceClient.PostNewOrder("ethbtc", 0.01m, 0m, OrderSide.BUY, OrderType.MARKET);
            var sellMarketOrder = binanceClient.PostNewOrder("ethbtc", 0.01m, 0m, OrderSide.SELL, OrderType.MARKET);
        }


        public void PostIcebergOrder()
        {
            var icebergOrder = binanceClient.PostNewOrder("ethbtc", 0.01m, 0m, OrderSide.BUY, OrderType.MARKET, icebergQty: 2m);
        }


        public void PostNewLimitOrderTest()
        {
            var testOrder = binanceClient.PostNewOrderTest("ethbtc", 1m, 0.1m, OrderSide.BUY);
        }


        public void CancelOrder()
        {
            var canceledOrder = binanceClient.CancelOrder("ethbtc", 9137796);
        }


        public void GetCurrentOpenOrders()
        {
            var openOrders = binanceClient.GetCurrentOpenOrders("ethbtc");
        }


        public void GetOrder()
        {
            var order = binanceClient.GetOrder("ethbtc", 8982811);
        }


        public void GetAllOrders()
        {
            var allOrders = binanceClient.GetAllOrders("ethbtc");
        }


        public void GetAccountInfo()
        {

            var accountInfo = binanceClient.GetAccountInfo();

            foreach (Binance.API.Csharp.Client.Models.Market.Balance balance in accountInfo.Balances)
            {
                if (balance.Free >= 0.00m)
                {; }
            }
        }


        public void GetTradeList()
        {
            var tradeList = binanceClient.GetTradeList("ethbtc");
        }


        public void Withdraw()
        {
            var withdrawResult = binanceClient.Withdraw("AST", 100m, "@YourDepositAddress");
        }


        public void GetDepositHistory()
        {
            var depositHistory = binanceClient.GetDepositHistory("neo", DepositStatus.Success);
        }


        public void GetWithdrawHistory()
        {
            var withdrawHistory = binanceClient.GetWithdrawHistory("neo");
        }
        #endregion

        #region User stream

        public void StartUserStream()
        {
            var listenKey = binanceClient.StartUserStream().ListenKey;
        }


        public void KeepAliveUserStream()
        {
            var ping = binanceClient.KeepAliveUserStream("@ListenKey");
        }


        public void CloseUserStream()
        {
            var resut = binanceClient.CloseUserStream("@ListenKey");
        }
        #endregion

        #region WebSocket

        #region Depth
        private void DepthHandler(DepthMessage messageData)
        {
            var depthData = messageData;
        }


        public void TestDepthEndpoint()
        {
            binanceClient.ListenDepthEndpoint("ethbtc", DepthHandler);
            Thread.Sleep(50000);
        }

        #endregion

        #region Kline
        private void KlineHandler(KlineMessage messageData)
        {
            var klineData = messageData;
        }


        public void TestKlineEndpoint()
        {
            binanceClient.ListenKlineEndpoint("ethbtc", TimeInterval.Minutes_1, KlineHandler);
            Thread.Sleep(50000);
        }
        #endregion

        #region AggregateTrade
        private void AggregateTradesHandler(AggregateTradeMessage messageData)
        {
            var aggregateTrades = messageData;
        }


        public void AggregateTestTradesEndpoint()
        {
            binanceClient.ListenTradeEndpoint("ethbtc", AggregateTradesHandler);
            Thread.Sleep(50000);
        }

        #endregion

        #region User Info
        private void AccountHandler(AccountUpdatedMessage messageData)
        {
            var accountData = messageData;
        }

        private void TradesHandler(OrderOrTradeUpdatedMessage messageData)
        {
            var tradesData = messageData;
        }

        private void OrdersHandler(OrderOrTradeUpdatedMessage messageData)
        {
            var ordersData = messageData;
        }


        public void TestUserDataEndpoint()
        {
            binanceClient.ListenUserDataEndpoint(AccountHandler, TradesHandler, OrdersHandler);
            Thread.Sleep(50000);
        }
        #endregion

        #endregion
    }
}
