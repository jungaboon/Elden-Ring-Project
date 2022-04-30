// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;

namespace RVHonorAI
{
    /// <summary>
    /// Used to communicate character animations handling with other systems
    /// </summary>
    public interface ICharacterAnimation
    {
        #region Properties

        /// <summary>
        /// Should Unity's root motion be used
        /// </summary>
        bool UseRootMotion { get; }

        /// <summary>
        /// Returns Animator component
        /// </summary>
        Animator Animator { get; }

        /// <summary>
        /// Inform character animation component if character is currently moving
        /// </summary>
        bool Moving { set; }

        /// <summary>
        /// Inform character animation component if character is currently rotating in place
        /// </summary>
        bool Rotating { set; }

        /// <summary>
        /// To animate rotation, this should be in -1 to 1 range, and set relevant property in animator that controlls rotation animation
        /// (check CharacterAnimation for example implementation)
        /// </summary>
        float RotatingSpeed { set; }

        #endregion

        #region Public methods

        /// <summary>
        /// 0 - normal, 1 - combat state
        /// </summary>
        void SetState(int _state);

        /// <summary>
        /// Plays attack animation. 
        /// </summary>
        /// <param name="_animationClip">Optional clip to play</param>
        void PlayAttackAnimation(AnimationClip _animationClip = null);

        /// <summary>
        /// Plays attack animation
        /// </summary>
        /// <param name="attackAnimationId">Id of attack animation to play</param>
        void PlayAttackAnimation(int attackAnimationId);

        /// <summary>
        /// Plays random death animtaion 
        /// </summary>
        void PlayDeathAnimation();

        /// <summary>
        /// Plays death animtaion 
        /// </summary>
        void PlayDeathAnimation(int deathAnimationId);

        /// <summary>
        /// Plays custom animation
        /// </summary>
        void PlayCustomAnimation(string _animationName, float _fixedTransitionDuration);

        /// <summary>
        /// Plays custom animation
        /// </summary>
        /// <param name="animationId">Id of custom animation to play</param>
        void PlayCustomAnimation(int animationId);

        #endregion
    }
}