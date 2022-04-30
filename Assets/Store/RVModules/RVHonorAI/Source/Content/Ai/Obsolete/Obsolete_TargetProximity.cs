// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.Content.AI.Scorers;
using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVHonorAI.Content.AI.Obsolete
{
    /// <summary>
    /// Check distance in 2d using X and Z axis of vectors, and use this distance to return score based on configuration
    /// </summary>
    public class Obsolete_TargetProximity : AiAgentScorerCurveParams<TargetInfo>
    {
        #region Fields

        [Header("Distance at time of 1 on curve")]
        [SerializeField]
        private FloatProvider distance;

        [SerializeField]
        private Vector3Provider positionToMeasure;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => this.ObsoleteGraphElementUsedError(typeof(DistanceToPositionCurve));

        protected override float Score(TargetInfo _parameter) =>
            GetScoreFromCurve(_parameter.Target.Transform.position.ManhattanDistance2d(positionToMeasure) / distance);

        #endregion
    }
}