// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public abstract class AiAgentScorerCurveParams<T> : AiAgentScorerParams<T>
    {
        #region Fields

        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        #endregion

        #region Not public methods

        /// <summary>
        /// Input value should be normalized (0-1), returned value is multiplied by score.
        /// If you want normalized result just divide returned value by score. 
        /// </summary>
        protected float GetScoreFromCurve(float _score) => curve.Evaluate(_score) * score;

        #endregion
    }
}