// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class SetVariableAnimationCurve : AiTask
    {
        #region Fields

        [SerializeField]
        public CurveProvider curveProvider;

        [SerializeField]
        private string curveName;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => AiGraph.GraphAiVariables.AssureBoolExist(curveName);

        protected override void Execute(float _deltaTime) => AiGraph.GraphAiVariables.SetAnimationCurve(curveName, curveProvider);

        #endregion
    }
}