// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using RVModules.RVSmartAI.Content;

namespace RVModules.RVSmartAI.GraphElements.Utilities
{
    [Serializable] public class SumAiUtility : AiUtility
    {
        #region Public methods

        public override string ToString() => "Sum utility";

        public override float Score(float _deltaTime)
        {
            float sum = 0;
            foreach (var scorer in scorers)
            {
                if (!scorer.Enabled) continue;
                var ls = scorer.lastScore = scorer.Score(_deltaTime);

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