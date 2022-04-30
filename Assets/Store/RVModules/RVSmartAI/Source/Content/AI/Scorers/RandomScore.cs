// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class RandomScore : AiScorer
    {
        #region Fields

        public float range;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => Random.Range(0, range);

        #endregion
    }
}