// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class VariableFloatProvider : FloatProvider
    {
        #region Fields

        [SerializeField]
        private string floatName;

        #endregion

        #region Not public methods

        protected override float ProvideData() => AiGraph.GraphAiVariables.GetFloat(floatName);

        #endregion
    }
}