using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Binance.API.Trader;
using DigitalAsset.Coins.Events;
using DigitalAsset.Coins.NewsTicker;
using DigitalAsset.Coins;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace BinanceTraderGUI
{


    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<Coin> coins;
        //Initiliaze BinanceAPI 
        BinanceAPI mServer = new BinanceAPI();
        CryptoPanicAPI mCryptoPanic = new CryptoPanicAPI();
        MarketEventsAPI mEvents = new MarketEventsAPI();
        //mEvents.GetCryptoEvents(1, 16, "08%2F10%2F2018", "24%2F12%2F2018", "cardano", "", "", "hot_events");
        //mServer.StartServer();
        //var test = mCryptoPanic.GetCryptoNews();
        //mServer.GetAllPrices();
        //Save the the choice of the user
        //int choice;
        //Name of the Coin
        string CoinName, CoinPair;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //Start the Server to receive information, about the current assets
            mServer.StartServer();

            //Add all received coins to the List
            FillCoinList();

        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //Stop synchronizing with the binance server
            mServer.StopServer();
        }

        /// <summary>
        /// Got called, as soon as an item got clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoinList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void FillCoinList()
        {
            coins = mServer.GetAllCoins();

            //foreach (Coin coin in coins)
                //this.CoinList.Items.Add(coin.Name);
        }
    }
}
