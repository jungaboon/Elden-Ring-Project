// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using RVHonorAI.Combat;

namespace RVHonorAI
{
    /// <summary>
    /// Callbacks for target-related events
    /// </summary>
    public interface ITargetsDetectionCallbacks
    {
        #region Properties

        /// <summary>
        /// Called when new ITarget was detected - one that wasn't detected in earlier scans
        /// </summary>
        Action<ITarget> OnNewTargetDetected { get; set; }

        /// <summary>
        /// Called when ITarget was visible in earlier scans, but is not anymore
        /// Note that passed in argument ITarget can be null - eg when it was destroyed/removed
        /// </summary>
        Action<ITarget> OnTargetNotSeenAnymore { get; set; }

        /// <summary>
        /// Called when ITarget was not visible(but not forgotten) and is visible again
        /// </summary>
        Action<ITarget> OnTargetVisibleAgain { get; set; }

        /// <summary>
        /// Called when ITarget hasn't been seen long enough to be removed from targets list
        /// /// Note that passed in argument ITarget can be null - eg when it was destroyed/removed
        /// </summary>
        Action<ITarget> OnTargetForget { get; set; }

        #endregion
    }
}