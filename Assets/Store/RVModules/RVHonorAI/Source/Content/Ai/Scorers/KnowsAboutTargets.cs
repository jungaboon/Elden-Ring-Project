// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    public class KnowsAboutTargets : AiScorer
    {
        #region Fields

        private ITargetInfosProvider targetInfosProvider;

        [SerializeField]
        protected float not;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Returns score when ITargetInfosProvider.TargetInfos has any entries" +
                                                        "\n Required context: ITargetInfosProvider";

        #endregion

        #region Public methods

        public override float Score(float _deltaTime)
        {
            foreach (var targetInfo in targetInfosProvider.TargetInfos)
            {
                // if you axe me.... yes, unfortunately this mess IS necessary... courtesy unity ofc :)
                if (targetInfo == null) continue;
                if (targetInfo.Target.Object() != null) return score;
            }

            return not;
        }

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => targetInfosProvider = ContextAs<ITargetInfosProvider>();

        #endregion
    }
}