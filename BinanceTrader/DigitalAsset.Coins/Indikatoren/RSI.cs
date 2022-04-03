using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DigitalAsset.Coins.Indikatoren
{
    /// <summary>
    /// Formel RSI = 100 - 1 / 1 + RS
    /// RS = (Average Gain / Counts) / (Average Loose / Counts)
    /// Furhter Information: https://stockcharts.com/school/doku.php?id=chart_school:technical_indicators:relative_strength_index_rsi
    /// </summary>
    class RSI : Indikatoren
    {
        private decimal RS;

        private List<decimal> Gain = new List<decimal>();
        private List<decimal> Loose = new List<decimal>();

        private decimal AverageGain = 0;
        private decimal AverageLoose = 0;

        private readonly float DumpLevel;
        private readonly float PumpLevel;
        private readonly float BuyLevel;
        private readonly float SellLevel;

        #region Constructor

        /// <summary>
        /// Assign standard values to parameters
        /// </summary>
        public RSI() : base(14)
        {
            this.mIndicatorName = "RSI14";
            for (int i = 1; i < mValues.Count; i++)
                CalcValueChange(mValues[i - 1], mValues[i]);

            this.DumpLevel = 90.0f;
            this.PumpLevel = 10.0f;
            this.BuyLevel = 20.0f;
            this.SellLevel = 80.0f;
        }
        
        public RSI(int capacity, float dumpLevel, float pumpLevel, float sellLevel, float buyLevel) : base(capacity)
        {
            this.mIndicatorName = "RSI" + capacity.ToString();
            for (int i = 1; i < mValues.Count; i++)
                CalcValueChange(mValues[i - 1], mValues[i]);

            this.DumpLevel = dumpLevel;
            this.PumpLevel = pumpLevel;
            this.BuyLevel = buyLevel;
            this.SellLevel = sellLevel;
        }

        public RSI(List<decimal> list, float dumpLevel, float pumpLevel, float sellLevel, float buyLevel) : base(list)
        {
            this.mIndicatorName = "RSI" + list.Count.ToString();
            for (int i = 1; i < mValues.Count; i++)
                CalcValueChange(mValues[i - 1], mValues[i]);

            this.DumpLevel = dumpLevel;
            this.PumpLevel = pumpLevel;
            this.BuyLevel = buyLevel;
            this.SellLevel = sellLevel;
        }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void AddValue(decimal value)
        {
            base.AddValue(value);

            
            if (mValues.Count >= mValues.Capacity)
            {
                CalcIndicator();
            }
        }

        private decimal CalcRSFirst()
        {
            if (Gain.Count > 0)
                AverageGain = Gain[0];
            else
                return 0;
            if(Loose.Count > 0)
                AverageLoose = Loose[0];
            else
                return 0;
            

            //Calculate the sum of all previous values
            for (int i = 1; i < Gain.Count; i++)
                AverageGain += Gain[i];
            for (int i = 1; i < Loose.Count; i++)
                AverageLoose += Loose[i];

            //Calculate the average
            AverageGain = AverageGain / mValues.Count;
            AverageLoose = AverageLoose / mValues.Count;

            return (AverageGain/AverageLoose);
        }

        private decimal CalcRS()
        {
            decimal CurrentGain = 0.0m;
            decimal CurrentLoose = 0.0m;

            int result = CalcValueChange(mValues[mValues.Count - 2], mValues[mValues.Count - 1]);

            if (result == 1)
                CurrentGain = Gain[Gain.Count - 1];
            else if (result == -1)
                CurrentLoose = Loose[Loose.Count - 1];

            AverageGain = ((mValues.Count - 1) * AverageGain + CurrentGain) / mValues.Count;
            AverageLoose = ((mValues.Count - 1) * AverageLoose + CurrentLoose) / mValues.Count;

            return (AverageGain / AverageLoose);
        }

        private int CalcValueChange(decimal prevValue, decimal nextValue)
        {
            decimal result;

            if (prevValue < nextValue)
            {
                result = nextValue - prevValue;
                Gain.Add(result);
                return 1;
            }
            else if (prevValue > nextValue)
            {
                result = prevValue - nextValue;
                Loose.Add(result);
                return -1;
            }
            else
            {
                return 0;
            }

        }
       
        protected override void CalcIndicator()
        {
            if (AverageGain == 0)
            {
                for (int i = 1; i < mValues.Count; i++)
                {
                    CalcValueChange(mValues[i - 1], mValues[i]);
                }
                RS = CalcRSFirst();
            }
            else
            {
                CalcValueChange(mValues[mValues.Count - 2], mValues[mValues.Count - 1]);
                RS = CalcRS();
            }

            //Calculate RSI-Value
            mIndicatorValue = 100 - 100 / (1 + RS);

            //Determine, if the Value has a potential increase or not, depending on the RSI Value
            RSIAlgorythmBuy();
            RSIAlgorythmSell();

        }


        private void RSIAlgorythmSell()
        {
            //----------------------------
            // Selling
            //----------------------------
            //Sell immediatly
            if (mIndicatorValue >= (decimal)DumpLevel)
                mDump = true;
            else if (mIndicatorValue >= (decimal)SellLevel)
            {
                mSellOrder = true;
            }
            else
            {
                mSellOrder = false;
                mDump = false;
            }
        }

        private void RSIAlgorythmBuy()
        {
            //----------------------------
            // Buying
            //----------------------------

            if (mIndicatorValue <= (decimal)PumpLevel)
                mPump = true;
            else if (mIndicatorValue <= (decimal)BuyLevel)
            {
                mBuyOrder = true;
            }
            else
            {
                mBuyOrder = false;
                mPump = false;
            }
        }
    }
}
