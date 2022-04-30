// Created by Ronis Vision. All rights reserved
// 06.08.2020.

namespace RVModules.RVUtilities
{
    public interface IObserver
    {
        #region Public methods

        void OnNotify(object _event);

        #endregion

        // destroyed observer must inform main observerHandler that it's no more
        //void OnDestroy();
    }
}