// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class IsCloserOrFurtherThan : AiAgentScorer
    {
        #region Fields

        public Vector3Provider firstPosition;

        public Vector3Provider secondPosition;

        #endregion

        #region Fields

        public bool closerThan = true;

        public bool furtherThan;
        
        [Header("Replaced by distance provider. Set to 0 to avoid errors")]
        public float distance;
        
        public FloatProvider distanceProvider;

        #endregion

        #region Public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            if(distance != 0) this.ObsoleteGraphElementFieldStillUsedError(nameof(distance));
        }

        public override float Score(float _deltaTime)
        {
            var dist = MeasureDistance();

            if (closerThan)
            {
                if (dist < distanceProvider) return score;
                return 0;
            }

            if (furtherThan)
            {
                if (dist > distanceProvider) return score;
                return 0;
            }

            return 0;
        }

        #endregion

        #region Not public methods

        /// <summary>
        /// Override this is you want other distance measurement method
        /// </summary>
        protected virtual float MeasureDistance() => Vector3.Distance(firstPosition, secondPosition);

        #endregion
    }
}