// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Animation;

namespace RVHonorAI
{
    public interface ICharacterAnimationContainer
    {
        #region Properties

        MovementAnimations MovementAnimations { get; set; }
        MovementAnimations CombatMovementAnimations { get; set; }
        SingleAnimations SingleAnimations { get; set; }

        #endregion
    }
}