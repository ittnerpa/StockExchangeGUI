using System.Collections.ObjectModel;

namespace TraidingBotGUI.Pages
{
    class TradingViewModel
    {    
        public ObservableCollection<TradingChartItems> CandlesSeries { get; set; }
        //public CandlestickSeries CandleSeries { get; set; }

        public TradingViewModel()
        {
            CandlesSeries = new ObservableCollection<TradingChartItems>();
        }


        public class TradingChartItems
        {
            public string Name { get; set; }
            public double Value { get; set; }
        }

    }
}
