// Created by Ronis Vision. All rights reserved
// 01.01.2020.

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Scorers
{
    public class CompareString : GetProperty
    {

        public string value;


        public float scoreFalse;

        public override float Score(float _deltaTime)
        {
            string s = GetPropertyValue.ToString();

            return s == value ? score : scoreFalse;
        }
    }
}