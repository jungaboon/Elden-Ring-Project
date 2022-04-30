// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using UnityEngine;

namespace RVHonorAI
{
    /// <summary>
    /// ITarget with Velocity
    /// </summary>
    public interface IMovingTarget : ITarget
    {
        #region Properties

        /// <summary>
        /// Current target velocity in m/s
        /// </summary>
        Vector3 Velocity { get; }

        #endregion
    }
}