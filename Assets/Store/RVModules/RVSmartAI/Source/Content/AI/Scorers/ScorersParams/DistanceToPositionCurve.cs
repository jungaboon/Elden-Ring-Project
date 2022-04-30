// Created by Ronis Vision. All rights reserved
// 16.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;
using UnityEngine.Serialization;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class DistanceToPositionCurve : AiAgentScorerCurveParams<Vector3>
    {
        #region Fields

        [FormerlySerializedAs("distance")]
        [Header("Distance at time of 1 on curve")]
        public FloatProvider range;

        [SerializeField]
        private Vector3Provider positionToMeasure;

        #endregion

        #region Not public methods

        protected override float Score(Vector3 _parameter)
        {
            if (!positionToMeasure.ValidateData()) return 0;
            return GetScoreFromCurve(Vector3.Distance(_parameter, positionToMeasure) / range);
        }

        #endregion
    }
}