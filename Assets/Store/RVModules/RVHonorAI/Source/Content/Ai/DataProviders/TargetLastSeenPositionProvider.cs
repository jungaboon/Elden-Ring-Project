// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVHonorAI.Content.AI.DataProviders
{
    public class TargetLastSeenPositionProvider : Vector3Provider
    {
        #region Fields

        private ITargetProvider targetProvider;

        #endregion

        #region Public methods

        public override bool ValidateData() => targetProvider.Target.Object() != null;

        #endregion

        #region Not public methods

        protected override Vector3 ProvideData() => targetProvider.CurrentTarget.LastSeenPosition;

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            targetProvider = ContextAs<ITargetProvider>();
        }

        #endregion
    }
}