// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI
{
    public static class StaticMethods
    {
        #region Public methods

        /// <summary>
        /// Input value should be normalized (0-1), returned value is multiplied by score.
        /// If you want normalized result just divide returned value by score.
        /// </summary>
        /// <param name="curve">Curve to evaluate from</param>
        /// <param name="inputValue">Input value for curve evaluation</param>
        /// <param name="score">Value returned from curve evaluation will be multiplied by this</param>
        public static float EvaluateCurve(AnimationCurve curve, float inputValue, float score) => curve.Evaluate(inputValue) * score;

        /// <summary>
        /// Returned value is direct value from curve
        /// </summary>
        /// <param name="curve">Curve to evaluate from</param>
        /// <param name="inputValue">Input value for curve evaluation</param>
        /// <param name="score">Value returned from curve evaluation will be multiplied by this</param>
        public static float EvaluateCurveNormalized(AnimationCurve curve, float inputValue) => curve.Evaluate(inputValue);

        public static void ObsoleteGraphElementUsedError(this object obsoleteGe, Type _newElementToUseInstead)
        {
            Debug.LogError($"{obsoleteGe.GetType().Name} is obsolete! Please replace it with {_newElementToUseInstead.Name}", obsoleteGe as Object);
        }

        public static void ObsoleteGraphElementFieldStillUsedError(this Object ge, string obsoleteField)
        {
            Debug.LogError(
                $"Data providers upgrade reminder! please update this graph elements field '{obsoleteField}' to default value and make sure you use DataProvider instead!",
                ge);
        }

        #endregion
    }
}