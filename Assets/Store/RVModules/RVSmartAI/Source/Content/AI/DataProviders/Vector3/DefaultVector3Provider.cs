// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class DefaultVector3Provider : Vector3Provider
    {
        #region Fields

        [SerializeField]
        private Vector3 value;

        #endregion

        #region Not public methods

        protected override Vector3 ProvideData() => value;

        #endregion
    }
}