// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVLoadBalancer.Tasks;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class FinishAiJob : AiAgentTask
    {
        #region Fields

        [SerializeField]
        protected string jobName;

        [SerializeField]
        protected TaskHandler jobHandler;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            jobHandler = GetComponentFromContext<IJobHandlerProvider>()?.AiJobHandler;
        }

        protected override void Execute(float _deltaTime) => jobHandler.FinishTask(jobName);

        #endregion
    }
}