// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    public class TargetVisible : AiScorerParams<TargetInfo>
    {
        #region Fields

        [SerializeField]
        protected float not;

        #endregion

        #region Not public methods

        protected override float Score(TargetInfo _parameter) => _parameter.Visible ? score : not;

        #endregion
    }
}