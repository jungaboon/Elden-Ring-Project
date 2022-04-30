// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using RVModules.RVSmartAI.GraphElements.Utilities;

namespace RVModules.RVSmartAI.GraphElements.Stages
{
    [Serializable] public class HighestUtilityWinsStage : Stage
    {
        #region Public methods

        public override AiUtility Select(float _deltaTime)
        {
            float highestScore = -1;
            if (utilities.Count == 0) return null;
            AiUtility highestAiUtility = null;

            foreach (var utility in utilities)
            {
                if (!utility.Enabled) continue;
                var score = utility.Score(_deltaTime);
                if (score > highestScore)
                {
                    highestAiUtility = utility;
                    highestScore = score;
                }
            }

            return highestAiUtility;
        }

        #endregion
    }
}