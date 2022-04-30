// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.GraphElements;

namespace RVHonorAI.Content.AI.Scorers
{
    public class IsCurrentTarget : AiScorerParams<TargetInfo>
    {
        #region Fields

        private ITargetProvider targetProvider;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => targetProvider = ContextAs<ITargetProvider>();

        protected override float Score(TargetInfo _parameter)
        {
            if (targetProvider.CurrentTarget == _parameter) return score;
            return 0;
        }

        #endregion
    }
}