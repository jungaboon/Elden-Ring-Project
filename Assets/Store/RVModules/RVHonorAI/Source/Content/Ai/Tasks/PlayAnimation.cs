// Created by Ronis Vision. All rights reserved
// 23.04.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVHonorAI.Content.AI.Tasks
{
    public class PlayAnimation : AiTask
    {
        #region Fields

        private Animator animator;

        [SerializeField]
        [Tooltip("Name of state in animator. This is NOT your clip name")]
        private StringProvider animationName;

        [SerializeField]
        [Tooltip("How long transition to this animation should take in seconds")]
        private FloatProvider crossFadeTime;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => animator = GetComponentFromContext<Animator>();

        protected override void Execute(float _deltaTime)
        {
            animator.CrossFadeInFixedTime(animationName.ToString(), crossFadeTime);
        }

        #endregion
    }
}