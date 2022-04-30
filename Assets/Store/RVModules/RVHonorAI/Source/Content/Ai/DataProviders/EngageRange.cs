// Created by Ronis Vision. All rights reserved
// 20.06.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVHonorAI.Content.AI.DataProviders
{
    public class EngageRange : FloatProvider
    {
        #region Fields

        private IAttacker attacker;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            attacker = ContextAs<Component>().GetComponent<IAttacker>();
        }

        protected override float ProvideData() => attacker.EngageRange;

        #endregion
    }
}