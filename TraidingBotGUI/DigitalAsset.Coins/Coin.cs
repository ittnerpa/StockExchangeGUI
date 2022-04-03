using System;
using DigitalAsset.Coins.Indikatoren;
using System.Collections.Generic;
using Binance.API.Csharp.Client.Models.Enums;



namespace DigitalAsset.Coins
{
    /// <summary>
    /// Contains all necessary information for a crypto currency
    /// </summary>
    public partial class Coin
    {
        private string mName;
        private bool mTraiding = true;
        private List<TraidingPair> mPair = new List<TraidingPair>();

        public string Name { get { return mName; } }

        public bool Traiding { get { return mTraiding; } private set { mTraiding = value; } }

        #region Constructor
        public Coin(string name, bool traiding = true)
        {
            this.mName = name;
            this.mTraiding = true;
        }

        public Coin(string name, string TraidingPairs, bool traiding = true)
        {
            this.mName = name;
            this.mTraiding = traiding;
            this.mPair.Add(new TraidingPair(TraidingPairs, TimeInterval.Minutes_1));

        }

        /// <summary>
        /// Add multiple Indicator to the coin 
        /// </summary>
        /// <param name="indikatoren"></param>
        public Coin(string name, string TraidingPairs, bool traiding = true, params CoinIndikatoren[] indikatoren)
        {
            this.mName = name;
            this.mTraiding = traiding;
            this.mPair.Add(new TraidingPair(TraidingPairs, TimeInterval.Minutes_1, 60, indikatoren));
        }
        #endregion Constructor

        
        #region Private-Methods

        /// <summary>
        /// Returns the corresponding traiding pair of the name. If not match was found, null will be returned
        /// </summary>
        /// <param name="pair">Name of the traiding pair</param>
        /// <returns></returns>
        private TraidingPair FindTraidingPair(string pair)
        {
            foreach (TraidingPair Pair in mPair)
            {
                if (Pair.Pair.Contains(pair))
                {
                    return Pair;
                }
            }
            throw new ArgumentException("Couldn't find the corresponding pair");
        }

        private decimal CandleChange(decimal preValue, decimal nextValue)
        {
            return decimal.Round((((nextValue - preValue) / preValue) * 100), 4);
        }

        #endregion 

        /// <summary>
        /// Removes a TraidingPair from the list
        /// </summary>
        /// <param name="pair">Name of the traiding pair</param>
        /// <returns></returns>
        public bool RemoveTraidingPair(string pair)
        {
            var traidingpair = FindTraidingPair(pair);

            if (traidingpair != null)
            {
                mPair.Remove(traidingpair);
                return true;
            }
            return false;
        }

        /// <summary>
        /// TODO:
        /// </summary>
        public void AddVolume()
        {
            ;
        }

        public void AddTraidingPair(string pair, TimeInterval interval)
        {
            this.mPair.Add(new TraidingPair(pair, interval));
        }

        public void AddPrice(string pair, decimal price)
        {
            var traidingpair = FindTraidingPair(pair);

            if (traidingpair != null)
            {
                traidingpair.Prices.Add(price);
                foreach (IIndikatoren indicator in traidingpair.mIndikatoren)
                {
                    indicator.AddValue(price);
                }
                //EvaluatePrices();
                return;
            }
                  
            throw (new ArgumentException("Couldnt find the pair"));
        }

        public void AddPrice(string pair, decimal high, decimal low, decimal open, decimal close, decimal Volume, DateTime TimeStamp )
        {
            var traidingpair = FindTraidingPair(pair);

            if (traidingpair != null)
            {   
                traidingpair.High.Add(high);
                traidingpair.Low.Add(low);
                traidingpair.Open.Add(open);
                traidingpair.Close.Add(close);
                traidingpair.Volume.Add(Volume);
                traidingpair.TimeStamp.Add(TimeStamp);

                if (traidingpair.PriceChange.Count > 1)
                    traidingpair.PriceChange.Add(CandleChange(traidingpair.Close[traidingpair.Close.Count - 2], traidingpair.Close[traidingpair.Close.Count - 1]));
                foreach (IIndikatoren indicator in traidingpair.mIndikatoren)
                {
                    indicator.AddValue(close);
                }

                return;
            }

            throw (new ArgumentException("Couldnt find the pair"));
        }

        public void AddPrice(string pair, decimal price, decimal Volume)
        {
            var traidingpair = FindTraidingPair(pair);

            if (traidingpair != null)
            {
                traidingpair.Prices.Add(price);
                traidingpair.Volume.Add(Volume);
                if (traidingpair.PriceChange.Count > 1)
                    traidingpair.PriceChange.Add(CandleChange(traidingpair.Prices[traidingpair.Prices.Count - 2], traidingpair.Prices[traidingpair.Prices.Count - 1]));
                foreach (IIndikatoren indicator in traidingpair.mIndikatoren)
                {
                    indicator.AddValue(price);
                }

                return;
            }

            throw (new ArgumentException("Couldnt find the pair"));
        }

        /// <summary>
        /// Returns a certain amount of 
        /// </summary>
        /// <param name="pair">Traiding pair</param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public List<decimal> GetPriceList(string pair, int amount = 50)
        {
            var traidingpair = FindTraidingPair(pair);
            List<decimal> PriceList = traidingpair.Close;

            return PriceList;
        }

        /// <summary>
        /// Returns a certain amount of
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public List<DateTime> GetTimeStampList(string pair, int amount = 50)
        {
            var traidingPair = FindTraidingPair(pair);
            List<DateTime> List = traidingPair.TimeStamp;

            return List;
        }

        public List<string> GetTraidingPairs()
        {
            List<string> strList = new List<string>();
            foreach(TraidingPair pair in mPair)
            {
                strList.Add(pair.Pair);
            }

            return strList;
        }

        public string FindSelectedPair()
        {
            foreach(TraidingPair pairs in mPair)
            {
                if (pairs.Selected)
                    return pairs.Pair;
            }
            return null;
        }

        public void SetSelected(string pair)
        {
            try
            {
                FindTraidingPair(pair).Selected = true;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public void Unselect(string pair)
        {
            try
            {
                FindTraidingPair(pair).Selected = false;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public void ShowAllTraidingPairs()
        {
            foreach (TraidingPair pair in mPair)
            {
                Console.WriteLine(Name + ": ");

            }
        }

        public void ShowAllPrices()
        {
            foreach (TraidingPair pair in mPair)
            {
                    Console.WriteLine("Pair: " + pair.Pair);
                    foreach (decimal price in pair.Prices)
                    {
                        Console.WriteLine(price);
                    }
                
            }
        }

        public void ShowIndicator(string pair)
        {
            var traidingpair = FindTraidingPair(pair);

            if (traidingpair != null)
            {
                foreach (IIndikatoren indicator in traidingpair.mIndikatoren)
                    Console.WriteLine(indicator.IndicatorName + ": "  + indicator.IndicatorValue);
            }
        }
        
        public void ShowPrices(string pair)
        {
            var traidingpair = FindTraidingPair(pair);

            if (traidingpair != null)
            {
                Console.WriteLine("Pair: " + traidingpair.Pair);
                foreach (decimal price in traidingpair.Prices)
                {
                    Console.WriteLine(price);
                }

            }
        }

        public TraidingFlags EvaluatePrice(string pair)
        {
            //Add Errorhandling
            var traidingPair = FindTraidingPair(pair);

            TraidingFlags flags = new TraidingFlags(); 

            traidingPair.EvaluatePrices(flags);

            return flags;
        }

    }
}
