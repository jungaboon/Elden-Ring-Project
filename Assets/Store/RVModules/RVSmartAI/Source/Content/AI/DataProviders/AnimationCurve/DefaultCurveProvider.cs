// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class DefaultCurveProvider : CurveProvider
    {
        #region Fields

        [SerializeField]
        private AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        #endregion

        #region Not public methods

        protected override AnimationCurve ProvideData() => animationCurve;

        #endregion
    }
}