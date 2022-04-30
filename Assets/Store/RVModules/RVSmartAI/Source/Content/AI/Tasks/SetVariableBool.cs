// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class SetVariableBool : AiTask
    {
        #region Fields

        [SerializeField]
        public BoolProvider boolProvider;

        [SerializeField]
        private string boolName;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => AiGraph.GraphAiVariables.AssureBoolExist(boolName);

        protected override void Execute(float _deltaTime) => AiGraph.GraphAiVariables.SetBool(boolName, boolProvider.GetData());

        #endregion
    }
}