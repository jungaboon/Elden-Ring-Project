// Created by Ronis Vision. All rights reserved
// 02.06.2020.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// Distance to position minus referenceRange divided by range
    /// Useful to finding pos closest to one at certain distance from positionToMeasure
    /// </summary>
    public class DistanceToRange : AiAgentScorerCurveParams<Vector3>
    {
        protected override string DefaultDescription => "Distance to position minus referenceRange divided by range.\n " +
                                                        "Useful to finding pos closest to one at certain distance from positionToMeasure";

        [SerializeField]
        [Header("Obsolete, use range instead")]
        private float distance = 10;

        [SerializeField]
        private FloatProvider range;

        [SerializeField]
        private FloatProvider referenceRange;

        [SerializeField]
        private Vector3Provider positionToMeasure;

        protected override void OnContextUpdated()
        {
            if (distance != 0) this.ObsoleteGraphElementFieldStillUsedError(nameof(distance));
            base.OnContextUpdated();
        }

        protected override float Score(Vector3 _parameter) =>
            GetScoreFromCurve(Mathf.Abs(Vector3.Distance(positionToMeasure, _parameter) - referenceRange) / range);
        // todo remove after updating graph
        //  GetScoreFromCurve(Mathf.Abs(Vector3.Distance(targetProvider.Target.Transform.position, _parameter) - attackRange.DamageRange) / distance);
    }
}