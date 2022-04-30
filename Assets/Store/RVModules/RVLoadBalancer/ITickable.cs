// Created by Ronis Vision. All rights reserved
// 05.11.2019.

namespace RVModules.RVLoadBalancer
{
    public interface ITickable
    {
        #region Public methods

        void Tick(float _deltaTime);

        #endregion
    }
}