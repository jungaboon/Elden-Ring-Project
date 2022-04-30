// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// Check distance in 2d using X and Z axis of vectors, and use this distance to return score based on configuration
    /// </summary>
    public abstract class Obsolete_ProximityToMeAiScorer : AiAgentScorerCurve
    {
        #region Fields

        [Header("Distance at time of 1 on curve")]
        public float distance = 10;

        #endregion

        #region Properties

        public abstract Vector3 PositionToMeasure { get; }

        #endregion

        #region Public methods

        protected override void OnContextUpdated()
        {
            this.ObsoleteGraphElementUsedError(typeof(DistanceBetweenTwoPositions));
        }

        //public override float Score(float _deltaTime) => GetScoreFromCurve(movement.Position.ManhattanDistance2d(PositionToMeasure) / distance);
        public override float Score(float _deltaTime)
        {
            this.ObsoleteGraphElementUsedError(typeof(DistanceBetweenTwoPositions));
            return 0;
        }

        #endregion
    }
}