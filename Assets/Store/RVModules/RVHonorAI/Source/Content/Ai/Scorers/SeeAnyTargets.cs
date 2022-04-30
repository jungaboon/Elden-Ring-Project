// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    /// <summary>
    /// Returns score when ITargetInfosProvider.TargetsInfos has any entries with Visible set to true.
    /// Required context: ITargetListProvider 
    /// </summary>
    public class SeeAnyTargets : AiScorer
    {
        #region Fields

        public float falseScore;

        private ITargetInfosProvider targetInfosProvider;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Returns score if see any enemies";

        #endregion

        #region Public methods

        public override float Score(float _deltaTime)
        {
            var targets = targetInfosProvider.TargetInfos;

            foreach (var target in targets)
            {
                if (target.Target as Object == null) continue;
                if (target.Visible) return score;
            }

            return falseScore;
        }

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            targetInfosProvider = ContextAs<ITargetInfosProvider>();
        }

        #endregion
    }
}