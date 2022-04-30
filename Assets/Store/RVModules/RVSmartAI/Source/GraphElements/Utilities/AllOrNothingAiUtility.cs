// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using RVModules.RVSmartAI.Content;

namespace RVModules.RVSmartAI.GraphElements.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class AllOrNothingAiUtility : AiUtility
    {
        #region Fields

        public float threshold;

        #endregion

        #region Public methods

        public override string ToString() => "All or nothing utility";

        public override float Score(float _deltaTime)
        {
            float sum = 0;
            foreach (var scorer in scorers)
            {
                if (!scorer.Enabled) continue;
                var ls = scorer.Score(_deltaTime);
                scorer.lastScore = ls;
                if (ls <= 0)
                {
                    lastScore = 0;
                    return 0;
                }

                switch (scorer.scorerType)
                {
                    case ScorerType.Add:
                        sum += ls;
                        break;
                    case ScorerType.Subtract:
                        sum -= ls;
                        break;
                    case ScorerType.Multiply:
                        sum *= ls;
                        break;
                    case ScorerType.Divide:
                        sum /= ls;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            lastScore = sum;
            return sum;
        }

        #endregion
    }
}