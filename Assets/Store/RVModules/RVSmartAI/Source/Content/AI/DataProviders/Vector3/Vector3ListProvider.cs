// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using System.Collections.Generic;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public abstract class Vector3ListProvider : DataProvider<List<Vector3>>
    {
        #region Public methods

        public static implicit operator List<Vector3>(Vector3ListProvider _provider) => _provider.GetData();

        #endregion
    }
}