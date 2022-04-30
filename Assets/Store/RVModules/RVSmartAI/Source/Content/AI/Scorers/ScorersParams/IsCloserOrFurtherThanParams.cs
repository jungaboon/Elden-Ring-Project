// Created by Ronis Vision. All rights reserved
// 27.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class IsCloserOrFurtherThanParams : AiAgentScorerParams<Vector3>
    {
        #region Fields

        [SerializeField]
        private bool closerThan = true;

        [SerializeField]
        private bool furtherThan;

        [SerializeField]
        private FloatProvider distance;

        [SerializeField]
        private Vector3Provider position;

        #endregion

        #region Not public methods

        protected override float Score(Vector3 _pos)
        {
            var dist = Vector3.Distance(position, _pos);

            if (closerThan)
            {
                if (dist < distance) return score;
                return 0;
            }

            if (furtherThan)
            {
                if (dist > distance) return score;
                return 0;
            }

            return 0;
        }

        #endregion
    }
}