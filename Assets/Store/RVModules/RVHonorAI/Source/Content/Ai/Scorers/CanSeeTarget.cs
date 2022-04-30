// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    public class CanSeeTarget : AiScorer
    {
        #region Fields

        private ITargetProvider targetProvider;

        [SerializeField]
        protected float not;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Returns set score if we can see current target";

        #endregion

        #region Public methods

        public override float Score(float _deltaTime)
        {
            if (targetProvider.Target.Object() == null) return not;
            return targetProvider.CurrentTarget.Visible ? score : not;
        }

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => targetProvider = ContextAs<ITargetProvider>();

        #endregion
    }
}