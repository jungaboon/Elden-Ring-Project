// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVLoadBalancer.Tasks;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.GraphElements;

namespace RVHonorAI.Content.AI.Scorers
{
    public class BusyHigherThan : AiScorer
    {
        #region Fields

        public int higherThan;

        public int scoreIfLower;

        private TaskHandler taskHandler;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => taskHandler.BusyPriority > higherThan ? score : scoreIfLower;

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