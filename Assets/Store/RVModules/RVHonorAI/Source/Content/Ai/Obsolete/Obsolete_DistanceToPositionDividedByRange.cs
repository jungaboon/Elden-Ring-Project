// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.Content.AI.Scorers;
using UnityEngine;

namespace RVHonorAI.Content.AI.Obsolete
{
    public class Obsolete_DistanceToPositionDividedByRange : AiAgentScorerCurveParams<Vector3>
    {
        #region Fields

        [SerializeField]
        private Vector3Provider positionProvider;

        [SerializeField]
        private FloatProvider range;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Distance to position divided by range";

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => this.ObsoleteGraphElementUsedError(typeof(DistanceToPositionCurve));

        protected override float Score(Vector3 _parameter)
        {
            if (!positionProvider.ValidateData()) return 0;
            return GetScoreFromCurve(Vector3.Distance(positionProvider, _parameter) / range);
        }

        #endregion
    }
}