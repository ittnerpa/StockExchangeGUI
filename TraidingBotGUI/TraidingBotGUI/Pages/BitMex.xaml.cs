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
using Bitmex.API;
using System.Threading.Tasks;
using DigitalAsset.Coins;
using Bitmex.API;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TraidingBotGUI.Pages
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class BitMex : Page
    {
        private string mSelectedPair;

        DispatcherTimer timer = new DispatcherTimer();

        TradingViewModel tViewModel = new TradingViewModel();

        BinanceAPI BinanceClient = new BinanceAPI();
        List<Coin> coins = new List<Coin>();
        List<double> test;


        public BitMex()
        {
            this.InitializeComponent();
            this.DataContext = CityManager.GetCities();

            timer.Interval = TimeSpan.FromMinutes(1);

            //Refresh all incoming data
            timer.Tick += (sender, args) =>
            {
                test = BitMexAPI.PriceList;
                
            };

            timer.Start();
        }

        public class City
        {
            public string Name { get; set; }
            public double Value { get; set; }
        }

        public class CityManager
        {
            private static Random random = new Random();
            private static string[] cityNames = new string[] { "Melbourne", "Sydney", "Brisbane", "Adelaide", "Perth" };

            public static List<City> GetCities()
            {
                List<City> cities = new List<City>();
                for (int i = 0; i < cityNames.Length; i++)
                {
                    cities.Add(new City() { Name = cityNames[i], Value = random.Next(50, 100) });
                }
                return cities;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => BitMexAPI.Start());
        }
    }
}
