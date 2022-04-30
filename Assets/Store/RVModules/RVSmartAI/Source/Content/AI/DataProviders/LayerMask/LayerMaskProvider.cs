// Created by Ronis Vision. All rights reserved
// 26.01.2021.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public abstract class LayerMaskProvider : DataProvider<LayerMask>
    {
        public static implicit operator LayerMask(LayerMaskProvider _layerMaskProvider) => _layerMaskProvider.GetData();
    }
}