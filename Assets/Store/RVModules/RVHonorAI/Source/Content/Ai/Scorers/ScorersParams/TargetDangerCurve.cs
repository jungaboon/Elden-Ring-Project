// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.Content.AI.Scorers;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    public class TargetDangerCurve : AiScorerCurveParams<TargetInfo>
    {
        #region Fields

        [SerializeField]
        [Header("Danger value at time of 1 on curve")]
        private float danger;

        #endregion

        #region Not public methods

        protected override float Score(TargetInfo _parameter) => GetScoreFromCurve(_parameter.Target.Danger / danger);

        #endregion
    }
}