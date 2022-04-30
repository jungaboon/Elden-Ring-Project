// Created by Ronis Vision. All rights reserved
// 22.09.2020.

namespace RVModules.RVSmartAI
{
    /// <summary>
    /// Used for reflection-based graph elements, where user needs to provide property name at runtime
    /// </summary>
    public interface IForceUpdate
    {
        #region Public methods

        void ForceUpdate();

        #endregion
    }
}