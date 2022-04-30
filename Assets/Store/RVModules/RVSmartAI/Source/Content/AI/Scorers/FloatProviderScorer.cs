// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class FloatProviderScorer : AiScorer
    {
        #region Fields

        public FloatProvider scoreProvider;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Returns value from float provider multiplied by score";

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => scoreProvider * score;

        #endregion
    }
}