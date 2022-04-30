// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class VariableBoolProvider : BoolProvider
    {
        #region Fields

        [SerializeField]
        private string boolName;

        #endregion

        #region Not public methods

        protected override bool ProvideData() => aiGraph.GraphAiVariables.GetBool(boolName);

        #endregion
    }
}