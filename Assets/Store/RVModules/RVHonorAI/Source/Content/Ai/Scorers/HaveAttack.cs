// Created by Ronis Vision. All rights reserved
// 11.04.2021.

using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    public class HaveAttack : AiScorer
    {
        #region Fields

        private IAttacker attacker;

        [SerializeField]
        private float not;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => attacker?.CurrentAttack != null ? score : not;

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