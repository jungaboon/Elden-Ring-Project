// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVHonorAI.Content.AI.DataProviders
{
    public class CurrentAttackPreferredRangeProvider : FloatProvider
    {
        #region Fields

        private IAttacker attacker;

        #endregion

        #region Public methods

        public override bool ValidateData() => attacker.CurrentAttack != null;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            attacker = ContextAs<Component>().GetComponent<IAttacker>();
        }

        protected override float ProvideData() => attacker.CurrentAttack.PreferredEngageRange;

        #endregion
    }
}