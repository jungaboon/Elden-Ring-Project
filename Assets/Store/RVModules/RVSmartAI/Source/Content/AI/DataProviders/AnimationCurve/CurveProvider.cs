// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public abstract class CurveProvider : DataProvider<AnimationCurve>
    {
        #region Public methods

        public static implicit operator AnimationCurve(CurveProvider _provider) => _provider.GetData();

        #endregion
    }
}