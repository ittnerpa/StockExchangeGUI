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
using DigitalAsset.Coins;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TradingBotGUI.Pages
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Binance : Page
    {
        BinanceAPI BinanceClient = new BinanceAPI();
        List<Coin> coins = new List<Coin>();


        public Binance()
        {
            this.InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            BinanceClient.StartServer();

            coins = BinanceClient.GetAllCoins();

        }
    }
}
