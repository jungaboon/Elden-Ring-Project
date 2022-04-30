// Created by Ronis Vision. All rights reserved
// 05.11.2019.

using System;

namespace RVModules.RVLoadBalancer
{
    public interface ITickableMethodCaller : ITickable
    {
        #region Properties

        Action TickableAction { get; }

        #endregion
    }
}