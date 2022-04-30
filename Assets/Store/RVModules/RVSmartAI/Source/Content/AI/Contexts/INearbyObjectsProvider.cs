// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using System.Collections.Generic;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Contexts
{
    public interface INearbyObjectsProvider
    {
        #region Properties

        List<Object> NearbyObjects { get; }

        #endregion
    }
}