// Created by Ronis Vision. All rights reserved
// 01.01.2020.

using System;

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Scorers.Params
{
    public class CompareNumberParams : GetPropertyParams<object>
    {

        public ValueComparison valueComparison;


        public float value;


        public float scoreFalse;

        protected override float Score(object _parameter)
        {
            var v = GetPropertyValue(_parameter);
            if (v == null) return 0;
            
            float usedValue = 0;
            if (v is int)
                usedValue = (int) v;
            else
                usedValue = (float) v;

            float s = 0;
            switch (valueComparison)
            {
                case ValueComparison.Equals:
                    s = value == usedValue ? score : scoreFalse;
                    break;
                case ValueComparison.Lower:
                    s = value > usedValue ? score : scoreFalse;
                    break;
                case ValueComparison.Higher:
                    s = value < usedValue ? score : scoreFalse;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return s;
        }
    }
}