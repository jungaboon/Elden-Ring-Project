// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class DefaultBoolProvider : BoolProvider
    {
        #region Fields

        [SerializeField]
        private bool value;

        #endregion

        #region Not public methods

        protected override bool ProvideData() => value;

        #endregion
    }
}