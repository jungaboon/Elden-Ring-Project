// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using RVModules.RVLoadBalancer.Tasks;

namespace RVModules.RVSmartAI
{
    public interface IJobHandlerProvider
    {
        #region Properties

        TaskHandler AiJobHandler { get; }

        #endregion
    }
}