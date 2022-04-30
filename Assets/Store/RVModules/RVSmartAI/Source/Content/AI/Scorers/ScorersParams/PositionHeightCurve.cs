// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class PositionHeightCurve : AiScorerCurveParams<Vector3>
    {
        #region Fields

        [Header("Height at time of 1 on curve")]
        public FloatProvider height;

        #endregion

        #region Not public methods

        protected override float Score(Vector3 _parameter) => GetScoreFromCurve(_parameter.y / height);

        #endregion
    }
}