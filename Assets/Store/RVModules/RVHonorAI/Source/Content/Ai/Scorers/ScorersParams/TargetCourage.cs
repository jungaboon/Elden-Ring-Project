// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.Content.AI.Scorers;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    public class TargetCourage : AiAgentScorerCurveParams<TargetInfo>
    {
        #region Fields

        [Header("Courage value at time of 1 on curve")]
        public FloatProvider courage;

        #endregion

        #region Not public methods

        protected override float Score(TargetInfo _parameter)
        {
            if (_parameter.Target.Object() == null) return 0;
            var courageProvider = _parameter.Target.Component().GetComponent<ICourageProvider>();
            if (courageProvider == null) return 0;

            return GetScoreFromCurve(courageProvider.Courage / courage);
        }

        #endregion
    }
}