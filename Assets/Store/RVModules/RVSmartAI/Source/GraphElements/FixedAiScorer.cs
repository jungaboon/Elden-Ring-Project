// Created by Ronis Vision. All rights reserved
// 22.09.2020.

namespace RVModules.RVSmartAI.GraphElements
{
    /// <summary>
    /// Always returns preset score
    /// </summary>
    public class FixedAiScorer : AiScorer
    {
        #region Public methods

        public override float Score(float _deltaTime) => score;

        #endregion
    }
}