using System;
using System.ComponentModel;

namespace TraidingBotGUI.Pages
{
    public class ChartCandle : INotifyPropertyChanged
    {
        DateTime _openTime;
        decimal _highPrice;
        decimal _lowPrice;
        decimal _openPrice;
        decimal _closePrice;
        private int? _numberCandle;


        public ChartCandle(DateTime opentime, decimal open, decimal high, decimal low, decimal close)
        {
            _openTime = opentime;
            _openPrice = open;
            _highPrice = high;
            _lowPrice = low;
            _closePrice = close;
        }

        public ChartCandle() { }


        public DateTime OpenTime
        {
            get { return _openTime; }
            set
            {
                if (!Equals(_openTime, value))
                {
                    _openTime = value;
                    OnPropertyChanged("OpenTime");
                }
            }
        }

        public decimal HighPrice
        {
            get { return _highPrice; }
            set
            {
                if (!Equals(_highPrice, value))
                {
                    _highPrice = value;
                    OnPropertyChanged("HighPrice");
                }
            }
        }

        public decimal LowPrice
        {
            get { return _lowPrice; }
            set
            {
                if (!Equals(_lowPrice, value))
                {
                    _lowPrice = value;
                    OnPropertyChanged("LowPrice");
                }
            }
        }

        public decimal OpenPrice
        {
            get { return _openPrice; }
            set
            {
                if (!Equals(_openPrice, value))
                {
                    _openPrice = value;
                    OnPropertyChanged("OpenPrice");
                }
            }
        }

        public decimal ClosePrice
        {
            get { return _closePrice; }
            set
            {
                if (!Equals(_closePrice, value))
                {
                    _closePrice = value;
                    OnPropertyChanged("ClosePrice");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }


}