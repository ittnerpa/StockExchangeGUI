using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalAsset.Coins.Indikatoren
{
    /// <summary>
    /// Simple Moving Average
    ///
    /// </summary>
    class SMA : Indikatoren
    {
        public SMA() : base(14)
        {
            this.mIndicatorName = "SMA14";
        }

        public SMA(int capacity) : base(capacity)
        {
            this.mIndicatorName = "SMA" + capacity.ToString();
        }

        protected override void CalcIndicator()
        {
            decimal result = 0;

            foreach (int value in mValues)
            {
                result += value;
            }

            mIndicatorValue = result / mValues.Count;
        }

        public override void AddValue(decimal value)
        {
            base.AddValue(value);

            if (mValues.Count >= mValues.Capacity)
            {
                CalcIndicator();
            }
        }
    }
}
