// Created by Ronis Vision. All rights reserved
// 02.04.2021.

namespace RVHonorAI
{
    /// <summary>
    /// Provides HitPoints and MaxHitPoints float properties
    /// </summary>
    public interface IHitPoints
    {
        #region Properties

        /// <summary>
        /// HP
        /// </summary>
        float HitPoints { get; set; }

        /// <summary>
        /// Max HP
        /// </summary>
        float MaxHitPoints { get; }

        #endregion
    }
}