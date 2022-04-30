// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class VariableVector3Provider : Vector3Provider
    {
        #region Fields

        [SerializeField]
        private string variableName;

        #endregion

        #region Not public methods

        protected override Vector3 ProvideData() => AiGraph.GraphAiVariables.GetVector3(variableName);

        #endregion
    }
}