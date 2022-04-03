using System;
using System.Collections.Generic;
using DigitalAsset.Coins.Indikatoren;
using Binance.API.Csharp.Client.Models.Enums;
using System.Text;

namespace DigitalAsset.Coins
{
    class TraidingPair
    {
        public string Pair { get { return mPair; } }

        //Version 1.1
        public List<decimal> Close;
        public List<decimal> High;
        public List<decimal> Open;
        public List<decimal> Low;
        public List<DateTime> TimeStamp;
        //Version 1.0
        public List<decimal> Prices;

        public List<decimal> Volume;
        public List<decimal> PriceChange;

        public List<IIndikatoren> mIndikatoren = new List<IIndikatoren>();

        public Boolean Selected { get => mSelected; set { mSelected = value; } }

        private TimeInterval Interval;
        private string mPair;
        private short mPriceChangeCounter;

        private bool mBuyOrder;
        private bool mSellOrder;

        private bool mSelected;
        private bool mPump;
        private bool mDump;
        private bool mBuyOrderIndicator;
        private bool mSellOrderIndicator;
        private bool mBuyOrderMarketData;
        private bool mSellOrderMarketData;
        private bool mBuyOrderPriceChange;
        private bool mSellOrderPriceChange;

        public TraidingPair(string pair, TimeInterval time, int capacity = 60, params CoinIndikatoren[] indikatoren)
        {
            this.mPair = pair;
            this.Interval = time;
            Low = new List<decimal>(capacity);
            High = new List<decimal>(capacity);
            Open = new List<decimal>(capacity);
            Close = new List<decimal>(capacity);
            TimeStamp = new List<DateTime>(capacity);
            Prices = new List<decimal>(capacity);
            Volume = new List<decimal>(capacity);
            PriceChange = new List<decimal>(capacity);

            foreach (CoinIndikatoren indicator in indikatoren)
            {
                AddIndicator(indicator);
            }
        }

        public void AddIndicator(CoinIndikatoren indikatoren)
        {
            switch (indikatoren)
            {
                case CoinIndikatoren.RSI:
                    mIndikatoren.Add(new RSI(6, 95.0f, 5.0f, 10.0f, 90.0f));
                    mIndikatoren.Add(new RSI(12, 90.0f, 10.0f, 83.0f, 17.0f));
                    mIndikatoren.Add(new RSI(24, 85.0f, 15.0f, 78.0f, 12.0f));
                    break;
                case CoinIndikatoren.SMA:
                    mIndikatoren.Add(new SMA());
                    break;
                default:
                    throw (new ArgumentException("Couldn't find the indicator"));
            }
        }

        public decimal GetIndicatorValue(CoinIndikatoren indikatoren)
        {
            switch (indikatoren)
            {
                case CoinIndikatoren.RSI:
                    return mIndikatoren[0].IndicatorValue;
                case CoinIndikatoren.SMA:
                    return mIndikatoren[0].IndicatorValue;
                default:
                    throw (new ArgumentException("Couldn't find the indicator"));
            }
        }

        /// <summary>
        /// Evaluate Indicator values, price Change, Market Data and defines a trend line of the traiding pair
        /// </summary>
        public void EvaluatePrices(TraidingFlags flags)
        {
            //Evaluate Indicator
            EvaluateIndicator();

            //Evaluate PriceChange
            //Pump ein balken abwarten
            //BuyOrder zwei doer sogar drei balken abwarten
            EvaluatePriceChange();

            //Evaluate MarketData
            //--

            //Evaluate trendline
            //----

            //Set Flags for Buying/Selling
            if (mBuyOrderPriceChange)
                flags.Buy = true;
            else if (mSellOrderPriceChange)
                flags.Sell = true;
        }

        private void EvaluateIndicator()
        {
            //RSI 
            EvaluateRSI(this.mIndikatoren[0], this.mIndikatoren[1], this.mIndikatoren[2]);

            //Other indicators
            foreach (IIndikatoren indicator in this.mIndikatoren)
            {
                if (indicator.IndicatorName.Contains("SMA"))
                    EvaluateSMA(indicator);
            }

        }

        private void EvaluateRSI(IIndikatoren RSI6, IIndikatoren RSI12, IIndikatoren RSI24)
        {
            //Set BuyOrder if all RSI indicates an oversold market
            if (RSI6.BuyOrder && RSI12.BuyOrder && RSI24.BuyOrder)
                this.mBuyOrderIndicator = true;
            //Set SellOrder if all RSI indicates an overbought market
            else if (RSI6.SellOrder && RSI12.SellOrder && RSI24.SellOrder)
                this.mSellOrderIndicator = true;
            //indicates a huge market buy
            else if (RSI6.Pump && RSI12.Pump && RSI24.Pump)
                this.mPump = true;
            //indicates a huge market sell
            else if (RSI6.Dump && RSI12.Dump && RSI24.Dump)
                this.mDump = true;
            //reset all flags
            else
            {
                this.mDump = false;
                this.mPump = false;
                this.mBuyOrderIndicator = false;
                this.mSellOrderIndicator = false;
            }
        }

        private void EvaluateSMA(IIndikatoren indicator)
        {
            ;
        }

        private void EvaluatePriceChange(short PumpDumpParameter = 1, short BuySellParameter = 3)
        {
            if (this.mPump || this.mBuyOrderIndicator)
            {
                if (PriceChange[PriceChange.Count - 1] < 0.0m)
                {
                    this.mPriceChangeCounter++;
                    if (mPump && (mPriceChangeCounter >= PumpDumpParameter))
                    {
                        mSellOrderPriceChange = true;
                        mPriceChangeCounter = 0;
                    }
                    else if (mBuyOrderIndicator && (mPriceChangeCounter >= BuySellParameter))
                    {
                        mSellOrderPriceChange = true;
                        mPriceChangeCounter = 0;
                    }
                    else
                        mSellOrderPriceChange = false;
                    
                }
            }

            if (this.mDump || this.mSellOrderIndicator)
            {
                if (PriceChange[PriceChange.Count - 1] > 0.0m)
                {
                    this.mPriceChangeCounter++;
                    if (mDump && (mPriceChangeCounter >= PumpDumpParameter))
                    {
                        mBuyOrderPriceChange = true;
                        mPriceChangeCounter = 0;
                    }
                    else if (mBuyOrderIndicator && (mPriceChangeCounter >= BuySellParameter))
                    {
                        mBuyOrderPriceChange = true;
                        mPriceChangeCounter = 0;
                    }
                    else
                        mBuyOrderPriceChange = false;
                    

                }
            }


        }
    }
}
