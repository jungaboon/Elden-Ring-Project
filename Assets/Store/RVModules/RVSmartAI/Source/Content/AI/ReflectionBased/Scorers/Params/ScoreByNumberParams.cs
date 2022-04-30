// Created by Ronis Vision. All rights reserved
// 01.01.2020.

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Scorers.Params
{
    public class ScoreByNumberParams: GetPropertyParams<object>
    {
        protected override float Score(object _parameter) 
        {
            var v = GetPropertyValue(_parameter);
            if (v == null) return 0;

            float usedValue = 0;
            if (v is int)
                usedValue = (int) v;
            else
                usedValue = (float) v;

            return usedValue * score;
        }
    }
}