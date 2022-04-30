// Created by Ronis Vision. All rights reserved
// 06.08.2020.

namespace RVModules.RVUtilities.RVInterfaceSolver
{
    public interface IHaveInterface<T> where T : class
    {
        #region Public methods

        T GetInterfaceInstance();

        #endregion
    }
}