// Created by Ronis Vision. All rights reserved
// 01.01.2020.

using System;

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Scorers
{
    public class CompareNumber : GetProperty
    {

        public ValueComparison valueComparison;


        public float value;


        public float scoreFalse;

        public override float Score(float _deltaTime)
        {
            var v = GetPropertyValue;

            float usedValue = 0;
            if (GetPropertyValue is int)
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