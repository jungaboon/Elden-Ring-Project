// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;

namespace RVHonorAI
{
    /// <summary>
    /// Provides transform used for checking field of view (for other entities see if they can see this entity)
    /// </summary>
    public interface IVisibilityCheckTransformProvider
    {
        #region Properties

        /// <summary>
        /// Transform used for checking field of view (for other entities see if they can see this entity)
        /// </summary>
        Transform VisibilityCheckTransform { get; }

        #endregion
    }
}