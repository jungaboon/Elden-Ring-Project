// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using UnityEngine;

namespace RVModules.RVSmartAI
{
    /// <summary>
    /// Override this to use MonoBehaviour style context. Make sure context field is set before AI needs it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ComponentContextProvider<T> : MonoBehaviour, IContextProvider where T : class, IContext
    {
        #region Public methods

        public IContext GetContext() => this as T;

        #endregion
    }
}