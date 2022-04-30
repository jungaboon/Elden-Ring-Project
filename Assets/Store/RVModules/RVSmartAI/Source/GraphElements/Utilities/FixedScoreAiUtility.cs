// Created by Ronis Vision. All rights reserved
// 22.09.2020.

namespace RVModules.RVSmartAI.GraphElements.Utilities
{
    public class FixedScoreAiUtility : AiUtility
    {
        #region Fields

        public float score = 1;

        #endregion

        #region Public methods

        public override string ToString() => "Fixed score utility";

        public override float Score(float _score)
        {
            lastScore = score;
            return score;
        }

        #endregion
    }
}