using DigitalAsset.Coins.Events;
using DigitalAsset.Coins.NewsTicker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace TraidingBotGUI.Pages
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MyContentPage : Page
    {
        //Receive all current news from CoinMarketCap
        private MarketEventsAPI _marketEvents = new MarketEventsAPI();
        private CryptoPanicAPI _cryptoPanic = new CryptoPanicAPI();

        //Contains all Data received from the APIs
        private News _cryptoNews = new News();
        private List<DigitalAsset.Coins.Events.JSON.Event> _cryptoEvents = new List<DigitalAsset.Coins.Events.JSON.Event>();

        private ObservableCollection<string> _itemsNews = new ObservableCollection<string>();
        private ObservableCollection<string> _itemsEvents = new ObservableCollection<string>();

        public ObservableCollection<string> ItemsNews
        {
            get { return this._itemsNews; }
        }
        public ObservableCollection<string> ItemsEvents
        {
            get { return this._itemsEvents; }
        }

        public MyContentPage()
        {
            this.InitializeComponent();

            this.InitializeNews();
            this.InitiliazeEvents();
        }

        private void InitializeNews()
        {
            _cryptoNews = _cryptoPanic.GetCryptoNews().Result;

            int counter = 0;

            foreach (var _news in _cryptoNews.results)
            {
                ItemsNews.Add(counter.ToString() + ". " + _news.title);
                counter++;
            }

            listViewNews.ItemsSource = ItemsNews;

        }
        private void InitiliazeEvents()
        {
            _cryptoEvents = _marketEvents.GetCryptoEvents(1, 16, "08%2F10%2F2018", "24%2F12%2F2018", "", "", "", "").Result;

            int counter = 0;

            foreach(var cEvent in _cryptoEvents)
            {
                ItemsEvents.Add(counter.ToString() + ". " + cEvent.title);
                counter++;
            }

            listViewEvents.ItemsSource = ItemsEvents;
        }

        private void listViewNews_ItemClick(object sender, ItemClickEventArgs e)
        {
            string url;
            string clickedItem = e.ClickedItem.ToString();
            url = _cryptoNews.results[int.Parse(clickedItem[0].ToString())].url;

            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
