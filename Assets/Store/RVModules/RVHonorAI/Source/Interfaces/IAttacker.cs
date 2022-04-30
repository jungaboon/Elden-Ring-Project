// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;

namespace RVHonorAI
{
    /// <summary>
    /// Entity that can attack
    /// </summary>
    public interface IAttacker : ITarget
    {
        #region Properties

        /// <summary>
        /// What is maximum engage range of attacker?
        /// </summary>
        float EngageRange { get; }

        /// <summary>
        /// Current attack
        /// </summary>
        IAttack CurrentAttack { get; }

        /// <summary>
        /// Current target
        /// </summary>
        ITarget CurrentTarget { get; }

        #endregion

        #region Public methods

        /// <summary>
        /// Attack current target
        /// This is immediate attack, damage should be dealt in this method, or weapon should be shot if using ranged/shooting weapon.
        /// Animation-independent.
        /// </summary>
        void Attack();

        /// <summary>
        /// Select current attack 
        /// </summary>
        void SelectAttack();

        #endregion
    }
}