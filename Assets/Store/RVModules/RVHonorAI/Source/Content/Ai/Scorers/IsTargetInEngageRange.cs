// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.Content.AI.Scorers;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    /// <summary>
    /// Required context: IMovement, ITargetProvider, IAttackRange
    /// </summary>
    public class IsTargetInEngageRange : AiAgentScorer
    {
        #region Fields

        public float scoreNotInRange;

        private IAttacker attacker;

        // we
        private ITarget target;

        #endregion

        #region Properties

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) =>
//            Vector3.Distance(movement.Position, targetProvider.Target.Transform.position) < attacker.CurrentAttack.PreferredEngageRange
            Character.Distance(target, attacker.CurrentTarget) < attacker.CurrentAttack.PreferredEngageRange
                ? score
                : scoreNotInRange;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            attacker = GetComponentFromContext<IAttacker>();
            target = GetComponentFromContext<ITarget>();
        }

        #endregion
    }
}