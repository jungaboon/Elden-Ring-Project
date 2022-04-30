// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVLoadBalancer.Tasks;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class IsAiJobRunning : AiAgentScorer
    {
        #region Fields

        [SerializeField]
        protected string jobName;


        [SerializeField]
        protected float scoreNotRunning;


        [SerializeField]
        protected TaskHandler jobHandler;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => jobHandler.IsTaskRunning(jobName) ? score : scoreNotRunning;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            jobHandler = GetComponentFromContext<IJobHandlerProvider>()?.AiJobHandler;
        }

        #endregion
    }
}