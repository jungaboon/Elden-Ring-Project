// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.Scorers;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    /// <summary>
    /// Make sure ITargetProvider.Target won't be null before using this scorer
    /// Required context: ITargetProvider, IAttackAngle, IMovement
    /// </summary>
    public class IsTargetInAttackAngle : AiAgentScorer
    {
        #region Fields

        [SerializeField]
        private float notInAttackAngleScore;

        private ITargetProvider targetProvider;
        private IAttackAngle attackAngle;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Make sure ITargetProvider.Target won't be null before using this scorer" +
                                                        "\n Required context: ITargetProvider, IAttackAngle, IMovement";

        #endregion

        #region Public methods

        public override float Score(float _deltaTime)
        {
            var transformPosition = movement.Position;
            transformPosition.y = 0;
            var targetPosition = targetProvider.Target.Transform.position;
            targetPosition.y = 0;

            var forwardVector = movement.Rotation * Vector3.forward;
            var angle = Vector3.Angle(forwardVector, targetPosition - transformPosition);

            return angle < attackAngle.AttackAngle ? score : notInAttackAngleScore;
        }

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            targetProvider = ContextAs<ITargetProvider>();
            attackAngle = GetComponentFromContext<IAttackAngle>();
        }

        #endregion
    }
}