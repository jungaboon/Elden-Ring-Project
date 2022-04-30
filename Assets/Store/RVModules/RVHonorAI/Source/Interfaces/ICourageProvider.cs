// Created by Ronis Vision. All rights reserved
// 02.04.2021.

namespace RVHonorAI
{
    public interface ICourageProvider
    {
        #region Properties

        /// <summary>
        /// Courage. Lower value means character will flee instead of fighting with lower enemie's advantage
        /// </summary>
        float Courage { get; set; }

        #endregion
    }
}