// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class DefaultFloatProvider : FloatProvider
    {
        #region Fields

        [SerializeField]
        private float value;

        #endregion

        #region Not public methods

        protected override float ProvideData() => value;

        #endregion
    }
}