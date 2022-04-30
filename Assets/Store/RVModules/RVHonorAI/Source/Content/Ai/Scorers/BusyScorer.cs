// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVLoadBalancer.Tasks;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.GraphElements;

namespace RVHonorAI.Content.AI.Scorers
{
    public class BusyScorer : AiScorer
    {
        #region Fields

        private TaskHandler taskHandler;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Returns IJobHandlerProvider.AiJobHandler.BusyPriority multiplied by score";

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => taskHandler.BusyPriority * score;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            taskHandler = GetComponentFromContext<IJobHandlerProvider>()?.AiJobHandler;
        }

        #endregion
    }
}