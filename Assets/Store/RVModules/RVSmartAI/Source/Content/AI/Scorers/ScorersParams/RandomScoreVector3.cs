// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class RandomScoreVector3 : AiScorerParams<Vector3>
    {
        #region Fields

        public float range;

        #endregion

        #region Not public methods

        protected override float Score(Vector3 _parameter) => Random.Range(0, range);

        #endregion
    }
}