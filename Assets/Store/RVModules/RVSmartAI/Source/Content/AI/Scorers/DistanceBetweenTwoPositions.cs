// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using System;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// Returns score based on distance between two positions divided by distance(optional), remapped by curve and multiplied by score
    /// </summary>
    public class DistanceBetweenTwoPositions : AiScorer
    {
        #region Fields

        [Header("If distance should be divided by below value to get normalized input for animation curve")]
        [SerializeField]
        private bool normalizeCurveInput = true;

        [Header("Distance at time of 1 on curve, if normalizeInput is true")]
        [SerializeField]
        private FloatProvider maxDistance;

        [SerializeField]
        private Vector3Provider firstPosition, secondPosition;

        [SerializeField]
        private CurveProvider curve;

        [Header("For debugging only")]
        [SerializeField]
        private float lastDistance;

        private Func<float> scoreAction;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => scoreAction();

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            if (normalizeCurveInput)
                scoreAction = () =>
                {
                    if (!firstPosition.ValidateData() || !secondPosition.ValidateData()) return 0;
                    lastDistance = Vector3.Distance(firstPosition, secondPosition);
                    return StaticMethods.EvaluateCurve(curve, lastDistance / maxDistance, score);
                };
            else
                scoreAction = () =>
                {
                    if (!firstPosition.ValidateData() || !secondPosition.ValidateData()) return 0;
                    lastDistance = Vector3.Distance(firstPosition, secondPosition);
                    return StaticMethods.EvaluateCurve(curve, lastDistance, score);
                };
        }

        #endregion
    }
}