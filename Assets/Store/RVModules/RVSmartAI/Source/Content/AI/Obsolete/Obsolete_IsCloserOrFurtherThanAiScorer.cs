// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using System;
using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// OBSOLETE
    /// </summary>
    public abstract class Obsolete_IsCloserOrFurtherThanAiScorer : AiAgentScorer
    {
        #region Fields

        public bool closerThan = true;


        public bool furtherThan;


        public float distance = 5;

        #endregion

        #region Properties

//        protected abstract Vector3 PositionToMeasure { get; }
//        protected virtual Vector3 SecondPositionToMeasure => movement.Position;

        #endregion

        #region Public methods

        protected override void OnContextUpdated() => this.ObsoleteGraphElementUsedError(typeof(IsCloserOrFurtherThan));


        public override float Score(float _deltaTime)
        {
            this.ObsoleteGraphElementUsedError(typeof(IsCloserOrFurtherThan));
//            var dist = MeasureDistance(SecondPositionToMeasure);
//
//            if (closerThan)
//            {
//                if (dist < distance) return score;
//                return 0;
//            }
//
//            if (furtherThan)
//            {
//                if (dist > distance) return score;
//                return 0;
//            }
//
            return 0;
        }

        #endregion

        #region Not public methods

//        /// <summary>
//        /// Override this is you want other distance measurement method
//        /// </summary>
//        protected virtual float MeasureDistance(Vector3 _pos) => PositionToMeasure.ManhattanDistance2d(_pos);

        #endregion
    }
}