// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    /// <summary>
    /// 
    /// </summary>
    public class HasTarget : AiScorer
    {
        #region Fields

        [SerializeField]
        private float not;

        private ITargetProvider targetProvider;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => targetProvider.CurrentTarget.Target.Object() != null ? score : not;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            targetProvider = ContextAs<ITargetProvider>();
        }

        #endregion
    }
}