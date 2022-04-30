// Created by Ronis Vision. All rights reserved
// 01.01.2020.

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Scorers
{
    public class CompareBool : GetProperty
    {

        public bool value;


        public float scoreFalse;

        public override float Score(float _deltaTime)
        {
            bool s = (bool)GetPropertyValue;
            return s == value ? score : scoreFalse;
        }
    }
}