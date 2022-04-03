using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalAsset.Coins.Indikatoren
{
    //Inherits all possible indicator
    public enum CoinIndikatoren { RSI = 0, SMA }

    interface IIndikatoren
    {
        /// <summary>
        /// Adds a new Value to the list and removes the first one 
        /// </summary>
        /// <param name="value"></param>
        void AddValue(decimal value);

        /// <summary>
        /// Result of the indicator
        /// </summary>
        decimal IndicatorValue { get; }

        /// <summary>
        /// Represents the Name of the indicator E.G.: RSI14
        /// </summary>
        string IndicatorName { get;  }

        bool BuyOrder { get; }

        bool SellOrder { get; }

        bool Dump { get; }

        bool Pump { get; }



    }
}
