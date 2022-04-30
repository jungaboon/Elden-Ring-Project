// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public abstract class Obsolete_SimpleProximityAiScorerParams : AiAgentScorerParams<Vector3>
    {
        #region Properties

        protected abstract Vector3 PositionToMeasure { get; }

        protected override string DefaultDescription =>
            "Returned score is distance multiplied by score. Useful for creating simple logic like moving away or toward something";

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => this.ObsoleteGraphElementUsedError(typeof(DistanceToPosition));

        protected override float Score(Vector3 _parameter) => PositionToMeasure.ManhattanDistance2d(_parameter) * score;

        #endregion
    }
}