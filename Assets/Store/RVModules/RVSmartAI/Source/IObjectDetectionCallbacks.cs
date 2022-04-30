// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using UnityEngine;

namespace RVModules.RVSmartAI
{
    /// <summary>
    /// Callbacks for object scanning
    /// </summary>
    public interface IObjectDetectionCallbacks
    {
        #region Properties

        /// <summary>
        /// Called when new Object was detected - one that wasn't detected in earlier scans
        /// </summary>
        System.Action<Object> OnNewObjectDetected { get; set; }

        /// <summary>
        /// Called when Object was detected in earlier scans, but is not anymore
        /// Note that passed in argument Object can be null - eg when it was destroyed/removed
        /// </summary>
        System.Action<Object> OnObjectNotDetectedAnymore { get; set; }

        #endregion
    }
}