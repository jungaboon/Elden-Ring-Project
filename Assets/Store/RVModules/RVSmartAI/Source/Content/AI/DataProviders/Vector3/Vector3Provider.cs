// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using System;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    [Serializable] public abstract class Vector3Provider : DataProvider<Vector3>
    {
        #region Public methods

        public static implicit operator Vector3(Vector3Provider _positionProvider) => _positionProvider.GetData();

        #endregion
    }
}