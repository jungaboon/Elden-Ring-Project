// Created by Ronis Vision. All rights reserved
// 01.01.2020.

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Scorers.Params
{
    public class CompareStringParams : GetPropertyParams<object>
    {

        public string value;


        public float scoreFalse;

        protected override float Score(object _parameter)
        {
            var v = GetPropertyValue(_parameter)?.ToString();
            
            return v == value ? score : scoreFalse;
        }
    }
}