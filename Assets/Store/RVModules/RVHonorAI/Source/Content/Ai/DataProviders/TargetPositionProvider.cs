// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVHonorAI.Content.AI.DataProviders
{
    public class TargetPositionProvider : Vector3Provider
    {
        #region Fields

        private ITargetProvider targetProvider;

        #endregion

        #region Public methods

        public override bool ValidateData() => targetProvider.Target as Object != null;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            targetProvider = ContextAs<ITargetProvider>();
        }

        protected override Vector3 ProvideData() => targetProvider.Target.Transform.position;

        #endregion
    }
}