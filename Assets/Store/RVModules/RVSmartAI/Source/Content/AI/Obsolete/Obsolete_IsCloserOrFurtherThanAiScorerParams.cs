// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// Measure distance from given point to PositionToMeasure and score accordingly to settings
    /// By default uses 2d manhattan distance calculation 
    /// </summary>
    public abstract class Obsolete_IsCloserOrFurtherThanAiScorerParams : AiAgentScorerParams<Vector3>
    {
        #region Fields

        public bool closerThan = true;


        public bool furtherThan;


        public float distance = 5;

        #endregion

        #region Properties

        protected abstract Vector3 PositionToMeasure { get; }

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => this.ObsoleteGraphElementUsedError(typeof(IsCloserOrFurtherThanParams));

        protected override float Score(Vector3 _pos)
        {
            var dist = MeasureDistance(_pos);

            if (closerThan)
            {
                if (dist < distance) return score;
                return 0;
            }

            if (furtherThan)
            {
                if (dist > distance) return score;
                return 0;
            }

            return 0;
        }

        /// <summary>
        /// Override this is you want other distance measurement method
        /// </summary>
        protected virtual float MeasureDistance(Vector3 _pos) => PositionToMeasure.ManhattanDistance2d(_pos);

        #endregion
    }
}