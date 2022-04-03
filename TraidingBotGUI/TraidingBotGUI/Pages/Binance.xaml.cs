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
using DigitalAsset.Coins;
using DigitalAsset.Coins.Events;
using TraidingBotGUI.Utilitys;
using Binance.API.Csharp.Client.Domain;
using Binance.API.Csharp.Client;
using DigitalAsset.Coins.NewsTicker;
using System.ComponentModel;
using Telerik.UI.Xaml.Controls.Chart;
using static TraidingBotGUI.Pages.TradingViewModel;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TraidingBotGUI.Pages
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Binance : Page
    {
        private string mSelectedPair;

        DispatcherTimer timer = new DispatcherTimer();

        TradingViewModel tViewModel = new TradingViewModel();

        BinanceAPI BinanceClient = new BinanceAPI();
        List<Coin> coins = new List<Coin>();      

        List<TradingChartItems> ChartItems = new List<TradingChartItems>();
        List<string> traidingpairs = new List<string>();

        public Binance()
        {
            this.InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BinanceClient.StartServer();

                coins = BinanceClient.GetAllCoins();

                InitiliazeTradingDropBox();

                CBoxTraidingPair.PlaceholderText = "BTCUSDT";
                InitiliazeTradingChart(coins[0], traidingpairs[0]);
            }
            catch(ArgumentOutOfRangeException exc)
            {
                BinanceClient.StopServer();

                throw;
            }
            catch(Exception)
            {
                throw;
            }
        }
        
        private void InitiliazeTradingDropBox()
        {
            foreach (Coin cCoin in coins)
            {
                foreach (string strPair in cCoin.GetTraidingPairs())
                {
                    CBoxTraidingPair.Items.Add(strPair);
                    traidingpairs.Add(strPair);
                }
            }
        }

        private void InitiliazeTradingChart(Coin coin, string pair)
        {
            if (tViewModel.CandlesSeries.Count > 0)
                tViewModel.CandlesSeries.Clear();

            {
                var price = coin.GetPriceList(pair);
                var time = coin.GetTimeStampList(pair);

                for (int i = 0; i < price.Count; i++)
                {
                    tViewModel.CandlesSeries.Add(new TradingChartItems
                    {
                        Name = time[i].Minute.ToString(),
                        Value = (double)price[i]
                    });
                }
            }

            coin.SetSelected(pair);
            ChartView.DataContext = tViewModel.CandlesSeries;
            this.DataContext = tViewModel;

            timer.Interval = TimeSpan.FromMinutes(1);

            //Refresh all incoming data
            timer.Tick += (sender, args) =>
            {
                string item;
                try
                {
                    item = CBoxTraidingPair.SelectedItem.ToString();
                } catch
                {
                    item = "BTCUSDT";
                }

                //TODO: Improve Performance
                foreach (Coin cCoin in coins)
                {
                    List<string> list;
                    list = cCoin.GetTraidingPairs();
                    foreach (string strPair in list)
                    {
                        if (item.Contains(strPair))
                        {
                            decimal price = cCoin.GetPriceList(strPair)[49];
                            int time = cCoin.GetTimeStampList(strPair)[49].Minute;
                            tViewModel.CandlesSeries.Add(new TradingChartItems { Name = time.ToString(), Value = (double)price});
                            tViewModel.CandlesSeries.RemoveAt(0);
                            break;
                        }
                    }
                }
            };

            timer.Start();
        }

        private void CBoxTraidingPair_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ((ComboBox)sender).SelectedItem.ToString();

            //TODO: Improve Performance
            foreach(Coin cCoin in coins)
            {
                List<string> list;
                list = cCoin.GetTraidingPairs();
                foreach(string strPair in list)
                {
                    if (item.Contains(strPair))
                        InitiliazeTradingChart(cCoin, strPair);
                }
            }
        }

        private string FindSelectedPair()
        {
            string str;
            foreach(Coin coin in coins)
            {
                str = coin.FindSelectedPair();
                if (str != null)
                    return str;
            }
            return null;
        }
    }
}
