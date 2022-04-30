// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public abstract class SimpleProximityToMeAiScorer : AiAgentScorer
    {
        #region Properties

        protected abstract Vector3 PositionToMeasure { get; }

        #endregion

        #region Public methods

        protected override void OnContextUpdated()
        {
            this.ObsoleteGraphElementUsedError(typeof(DistanceBetweenTwoPositions));
        }

        public override float Score(float _deltaTime)
        {
            this.ObsoleteGraphElementUsedError(typeof(DistanceBetweenTwoPositions));
            return PositionToMeasure.ManhattanDistance2d(movement.Position) * score;
        }

        #endregion
    }
}