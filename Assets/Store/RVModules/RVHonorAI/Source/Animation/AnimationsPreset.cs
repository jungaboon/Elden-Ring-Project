// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;

namespace RVHonorAI.Animation
{
    [CreateAssetMenu(menuName = "Honor AI/Animations preset")]
    public class AnimationsPreset : ScriptableObject, ICharacterAnimationContainer
    {
        #region Fields

        public MovementAnimations movementAnimations;
        public MovementAnimations combatMovementAnimations;
        public SingleAnimations singleAnimations;

        #endregion

        #region Properties

        public MovementAnimations MovementAnimations
        {
            get => movementAnimations;
            set => movementAnimations = value;
        }

        public SingleAnimations SingleAnimations
        {
            get => singleAnimations;
            set => singleAnimations = value;
        }

        public MovementAnimations CombatMovementAnimations
        {
            get => combatMovementAnimations;
            set => combatMovementAnimations = value;
        }

        #endregion
    }
}