// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.GraphElements;

namespace RVHonorAI.Content.AI.Scorers
{
    public class HasRangedWeapon : AiScorer
    {
        #region Fields

        public float scoreNoRangedWeapon;
        private IAttacker attacker;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => attacker.CurrentAttack?.AttackType == AttackType.Shooting ? score : scoreNoRangedWeapon;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            attacker = GetComponentFromContext<IAttacker>();
        }

        #endregion
    }
}