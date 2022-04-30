// Created by Ronis Vision. All rights reserved
// 26.01.2021.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class VariableStringProvider : StringProvider
    {
        [SerializeField]
        private string stringName;

        protected override string ProvideData() => aiGraph.GraphAiVariables.GetString(stringName);
    }
}