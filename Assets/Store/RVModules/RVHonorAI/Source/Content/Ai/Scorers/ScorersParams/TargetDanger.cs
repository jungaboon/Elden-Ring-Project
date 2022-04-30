// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.GraphElements;

namespace RVHonorAI.Content.AI.Scorers
{
    public class TargetDanger : AiScorerParams<TargetInfo>
    {
        #region Not public methods

        protected override float Score(TargetInfo _parameter) => _parameter.Target.Danger * score;

        #endregion
    }
}