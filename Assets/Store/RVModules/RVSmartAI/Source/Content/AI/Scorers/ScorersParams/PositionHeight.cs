// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;
using UnityEngine.Serialization;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class PositionHeight : AiScorerParams<Vector3>
    {
        #region Fields

        [Header("Returned score will be Y of scored position multiplied by this")]
        [SerializeField]
        [FormerlySerializedAs("scoreProvider")]
        private FloatProvider heightMultiplier;

        #endregion

        #region Not public methods

        protected override float Score(Vector3 _parameter) => _parameter.y * heightMultiplier;

        #endregion
    }
}