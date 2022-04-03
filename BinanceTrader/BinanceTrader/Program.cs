using System;
using System.Collections.Generic;
using Binance.API.Csharp.Client;
using DigitalAsset.Coins;
using DigitalAsset.Coins.Indikatoren;
using Binance.API.Csharp.Client.Models;
using DigitalAsset.Coins.NewsTicker;
using DigitalAsset.Coins.Events;

namespace Binance.API.Trader
{
    class Program
    {

        static void Main(string[] args)
        {
            //Initialize the Server
            BinanceAPI mServer = new BinanceAPI();
            CryptoPanicAPI mCryptoPanic = new CryptoPanicAPI();
            //MarketEventsAPI mEvents = new MarketEventsAPI();
            //mEvents.GetCryptoEvents(1, 16, "08%2F10%2F2018", "24%2F12%2F2018", "cardano", "", "", "hot_events");
            //mServer.StartServer();
            //var test = mCryptoPanic.GetCryptoNews();
            mServer.StartServer();
            //mServer.GetAllPrices();
            //Save the the choice of the user
            int choice;
            //Name of the Coin
            string CoinName, CoinPair;
            /*
            do
            {
                //Menu
                Console.WriteLine("1. Add Coin");
                Console.WriteLine("2. Show Coin");
                Console.WriteLine("3. Add Börse(not implemented yet)");
                Console.WriteLine("4. Start/Stop Server");
                Console.WriteLine("9. Exit");
           
                //Reads ASCII Code 
                choice = Console.Read() - 48;

                switch (choice)
                {
                    case 1:
                        Console.Write("Name of the coin: ");
                        Console.ReadLine();
                        CoinName = Console.ReadLine();
                        Console.Write("Name of the TraidingPair: ");
                        CoinPair = Console.ReadLine();
                        mServer.AddCoin(CoinName, CoinPair, Csharp.Client.Models.Enums.TimeInterval.Minutes_1);
                        break;
                    case 2:
                        try
                        {
                            mServer.ShowAllCoins();
                        } catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 9:
                        break;
                    default:
                        break;
                }


            } while (choice != 9);
            */
        }
    }
}
