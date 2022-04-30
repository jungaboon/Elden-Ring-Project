// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;

namespace RVHonorAI
{
    /// <summary>
    /// Provides position from which to check field of view (head, eyes etc.)
    /// todo maybe this should provide transform, to be consistent with other position related providers
    /// </summary>
    public interface IFovPositionProvider
    {
        #region Properties

        /// <summary>
        /// Fov 'root' position - usually chacter's eyes/head
        /// </summary>
        Vector3 FovPosition { get; }

        #endregion
    }
}