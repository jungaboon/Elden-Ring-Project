// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class VariableCurveProvider : CurveProvider
    {
        #region Fields

        [SerializeField]
        private string curveName;

        #endregion

        #region Not public methods

        protected override AnimationCurve ProvideData() => AiGraph.GraphAiVariables.GetAnimationCurve(curveName);

        #endregion
    }
}