// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.GraphElements;

namespace RVHonorAI.Content.AI.Scorers
{
    public class Courage : AiScorer
    {
        #region Public methods

        private ICourageProvider courageProvider;

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            courageProvider = ContextAs<ICourageProvider>();
        }

        public override float Score(float _deltaTime) => courageProvider.Courage * score;

        #endregion
    }
}