// Created by Ronis Vision. All rights reserved
// 01.01.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.ReflectionBased.Scorers
{
    public class ScoreByNumberCurve : GetProperty
    {
        [Header("Value(vertical axis) is returned value multiplied by score, time(horizontal axis) is number from property divided by maxValue")]
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Your property value will be divided by this to evaluate curve")]
        public float maxValue = 1;

        public override float Score(float _deltaTime)
        {
            var v = GetPropertyValue;

            float usedValue = 0;
            if (GetPropertyValue is int)
                usedValue = (int) v;
            else
                usedValue = (float) v;

            return curve.Evaluate(usedValue / maxValue) * score;
        }
    }
}