// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content.AI.Scorers;
using UnityEngine;

namespace RVHonorAI.Content.AI.Obsolete
{
    /// <summary>
    /// Returns percentage of used range to current target
    /// Required context: IAttackRange, IMovement, ITargetProvider
    /// </summary>
    public class Obsolete_DistanceToTargetVsWeaponRangeScorer : AiAgentScorerCurve
    {
        #region Fields

        //[Header("Distance at time of 1 on curve")]
        //public float distance = 10;

        private IAttackRange attackRange;
        private ITargetProvider targetProvider;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Returns percentage of used range to current target\n" +
                                                        " Required context: IAttackRange, IMovement, ITargetProvider";

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) =>
            GetScoreFromCurve(Vector3.Distance(movement.Position, targetProvider.CurrentTarget.Target.Transform.position) / attackRange.DamageRange);

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            this.ObsoleteGraphElementUsedError(typeof(DistanceBetweenTwoPositions));
            base.OnContextUpdated();
            targetProvider = ContextAs<ITargetProvider>();
            attackRange = GetComponentFromContext<IAttackRange>();
        }

        #endregion
    }
}