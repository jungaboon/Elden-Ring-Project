// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class SetVariableFloat : AiTask
    {
        #region Fields

        [SerializeField]
        public FloatProvider floatProvider;

        [SerializeField]
        private string floatName;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => AiGraph.GraphAiVariables.AssureFloatExist(floatName);

        protected override void Execute(float _deltaTime) => AiGraph.GraphAiVariables.SetFloat(floatName, floatProvider);

        #endregion
    }
}