// Created by Ronis Vision. All rights reserved
// 29.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;
using UnityEngine.Serialization;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class DistanceToPosition : AiAgentScorerParams<Vector3>
    {
        #region Fields

        [SerializeField]
        private Vector3Provider positionToMeasure;

        protected override string DefaultDescription =>
            "Returned score is distance multiplied by score. Useful for creating simple logic like moving away or toward something and a little cheaper than curve counterpart";

        #endregion

        #region Not public methods

        protected override float Score(Vector3 _parameter)
        {
            if (!positionToMeasure.ValidateData()) return 0;
            return Vector3.Distance(_parameter, positionToMeasure) * score;
        }

        #endregion
    }
}