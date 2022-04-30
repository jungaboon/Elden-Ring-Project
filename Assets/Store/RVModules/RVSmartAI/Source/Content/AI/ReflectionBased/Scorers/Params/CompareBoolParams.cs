// Created by Ronis Vision. All rights reserved
// 01.01.2020.

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Scorers.Params
{
    public class CompareBoolParams : GetPropertyParams<object>
    {

        public bool value;


        public float scoreFalse;

        protected override float Score(object _parameter)
        {
            var v = GetPropertyValue(_parameter);
            if (v == null) return 0;
            return (bool)v == value ? score : scoreFalse;
        }
    }
}