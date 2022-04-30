// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;

namespace RVHonorAI
{
    /// <summary>
    /// todo replace this by graph variable, as its used by graphs only
    /// </summary>
    public interface IDangerPositionProvider
    {
        #region Properties

        /// <summary>
        /// Danger direction - average of vectors from our position to all visible enemies, normalized
        /// This vector is actually reversed from danger (away from danger)
        /// </summary>
        Vector3 DangerPosition { get; set; }

        #endregion
    }
}