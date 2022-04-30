// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System.Collections.Generic;
using RVHonorAI.Combat;

namespace RVHonorAI
{
    public interface ITargetListProvider
    {
        #region Properties

        /// <summary>
        /// List of targets
        /// </summary>
        List<ITarget> Targets { get; }

        #endregion
    }
}