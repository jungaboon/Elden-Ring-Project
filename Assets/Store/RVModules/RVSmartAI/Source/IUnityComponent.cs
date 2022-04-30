// Created by Ronis Vision. All rights reserved
// 14.02.2021.

using UnityEngine;

namespace RVModules.RVSmartAI
{
    /// <summary>
    /// Can be used on AiTaskParam generic parameter to show relevant game object info in inspector for debugging
    /// </summary>
    public interface IUnityComponent
    {
        Component ToUnityComponent();
    }
}