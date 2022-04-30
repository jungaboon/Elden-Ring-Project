// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// Check distance in 2d using X and Z axis of vectors, and use this distance to return score based on configuration
    /// </summary>
    public abstract class Obsolete_ProximityAiScorerParams : AiAgentScorerCurveParams<Vector3>
    {
        #region Fields

        [Header("Distance at time of 1 on curve")]
        public float distance = 10;

        #endregion

        #region Properties

        protected override void OnContextUpdated() => this.ObsoleteGraphElementUsedError(typeof(DistanceToPositionCurve));

        public abstract Vector3 PositionToMeasure { get; }

        #endregion

        #region Not public methods

        protected override float Score(Vector3 _parameter) => GetScoreFromCurve(_parameter.ManhattanDistance2d(PositionToMeasure) / distance);

        #endregion
    }
}