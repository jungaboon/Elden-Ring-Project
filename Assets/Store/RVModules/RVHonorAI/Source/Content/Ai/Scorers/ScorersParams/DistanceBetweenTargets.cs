// Created by Ronis Vision. All rights reserved
// 16.03.2021.

using RVHonorAI;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;
using UnityEngine.Serialization;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// Scores based on distance between our current target and us, taking into account radius of both (substracting both radiuses from distance)
    /// this is useful for attack ranges which have to account for targets size/radius
    /// </summary>
    public class DistanceBetweenTargets : AiAgentScorerCurveParams<Vector3>
    {
        #region Fields

        [FormerlySerializedAs("distance")]
        [Header("Distance at time of 1 on curve")]
        public FloatProvider range;

        [SerializeField]
        private Vector3Provider positionToMeasure;

        private IAttacker attacker;

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            attacker = GetComponentFromContext<IAttacker>();
        }

        #endregion

        #region Not public methods

        protected override float Score(Vector3 _parameter)
        {
            if (!positionToMeasure.ValidateData()) return 0;
            return GetScoreFromCurve((Vector3.Distance(_parameter, attacker.CurrentTarget.Transform.position) - attacker.Radius -
                                     attacker.CurrentTarget.Radius) / range);
        }

        #endregion
    }
}