using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace DigitalAsset.Coins.Indikatoren
{
    
    public class Indikatoren : IIndikatoren
    {
        protected List<decimal> mValues;
        protected List<decimal> mIndicatorHistory;
        protected decimal mIndicatorValue = 0.0m;
        protected string mIndicatorName;
        protected bool mBuyOrder;
        protected bool mSellOrder;
        protected bool mPump;
        protected bool mDump;

        public string IndicatorName { get { return mIndicatorName; } }
        public decimal IndicatorValue { get { return mIndicatorValue; } }
        public bool BuyOrder { get { return mBuyOrder; } }
        public bool SellOrder { get { return mSellOrder; } }
        public bool Pump { get { return mPump;  } }
        public bool Dump { get { return mDump;  } }

        /// <summary>
        /// Add a value to the end of the list
        /// </summary>
        /// <param name="value"></param>
        public virtual void AddValue(decimal value)
        {
            if (mValues.Count >= mValues.Capacity)
            {
                mValues.RemoveAt(0);
            }

            mValues.Add(value);
        }

        virtual protected void CalcIndicator()
        {
        }

        virtual protected void EvaluateIndicatorData()
        {

        }

        /// <summary>
        /// Generates a new List with a determine capacity
        /// </summary>
        /// <param name="capacity"></param>
        public Indikatoren(int capacity)
        {
            mValues = new List<decimal>(capacity);
        }

        /// <summary>
        /// Initialize the indicator with a previous list of values
        /// </summary>
        /// <param name="valueList"></param>
        public Indikatoren(List<decimal> valueList)
        {
            this.mValues = valueList;
        }

        public Indikatoren() 
        {
            mValues = new List<decimal>(14);
        }


    }
}
