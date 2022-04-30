// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Contexts
{
    public interface IMoveTargetProvider
    {
        #region Properties

        Transform FollowTarget { get; set; }

        #endregion
    }
}