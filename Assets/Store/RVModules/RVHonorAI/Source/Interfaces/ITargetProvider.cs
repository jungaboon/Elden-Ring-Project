// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI;

namespace RVHonorAI
{
    /// <summary>
    /// This represents agent that can have target (to aim at, attack etc.)
    /// </summary>
    public interface ITargetProvider : IComponent
    {
        #region Properties

        /// <summary>
        /// Current target
        /// </summary>
        ITarget Target { get; }

        /// <summary>
        /// Current target info
        /// </summary>
        TargetInfo CurrentTarget { get; set; }

        #endregion
    }
}