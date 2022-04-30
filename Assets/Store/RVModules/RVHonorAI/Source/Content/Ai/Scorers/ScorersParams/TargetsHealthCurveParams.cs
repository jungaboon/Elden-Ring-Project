// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.Content.AI.Scorers;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    public class TargetsHealthCurveParams : AiAgentScorerCurveParams<TargetInfo>
    {
        #region Fields

        [Header("Obsolete, use healthValue")]
        public float health = 100;

        [SerializeField]
        [Header("Health value at time of 1 on curve")]
        private FloatProvider healthValue;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Target must have IHealth implemented";

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            if (health != 0) this.ObsoleteGraphElementFieldStillUsedError(nameof(health));
        }

        protected override float Score(TargetInfo _parameter)
        {
            var hitPointsProvider = _parameter.Target as IHitPoints;
            if (hitPointsProvider == null) return 0;
            return GetScoreFromCurve(hitPointsProvider.HitPoints / healthValue);
        }

        #endregion
    }
}