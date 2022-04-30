// Created by Ronis Vision. All rights reserved
// 02.04.2021.

namespace RVHonorAI
{
    public interface IMovementSpeedProvider
    {
        #region Properties

        /// <summary>
        /// Sets agent movement speed to walking or running speed
        /// </summary>
        MovementSpeed MovementSpeed { get; set; }

        #endregion
    }
}