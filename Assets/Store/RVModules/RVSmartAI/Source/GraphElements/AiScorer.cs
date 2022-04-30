// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using RVModules.RVSmartAI.Content;

namespace RVModules.RVSmartAI.GraphElements
{
    [Serializable] public abstract class AiScorer : AiGraphElement
    {
        #region Fields

        public ScorerType scorerType;

        public float score = 1;

        /// <summary>
        /// for debugging only
        /// </summary>
        [SmartAiHideInInspector]
        public float lastScore;

        #endregion

        #region Public methods

        public abstract float Score(float _deltaTime);

        #endregion
    }
}