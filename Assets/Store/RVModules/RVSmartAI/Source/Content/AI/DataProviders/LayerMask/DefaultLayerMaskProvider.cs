// Created by Ronis Vision. All rights reserved
// 26.01.2021.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class DefaultLayerMaskProvider : LayerMaskProvider
    {
        [SerializeField]
        private LayerMask value;

        protected override LayerMask ProvideData() => value;
    }
}