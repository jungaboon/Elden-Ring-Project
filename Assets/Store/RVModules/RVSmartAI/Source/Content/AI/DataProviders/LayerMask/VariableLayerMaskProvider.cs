// Created by Ronis Vision. All rights reserved
// 26.01.2021.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class VariableLayerMaskProvider : LayerMaskProvider
    {
        [SerializeField]
        private string layerMaskName;

        protected override LayerMask ProvideData() => aiGraph.GraphAiVariables.GetLayerMask(layerMaskName);
    }
}