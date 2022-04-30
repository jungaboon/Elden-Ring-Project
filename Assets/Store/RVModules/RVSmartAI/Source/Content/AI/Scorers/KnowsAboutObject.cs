// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// Returns score if provided object is in NearbyObjects array
    /// </summary>
    public class KnowsAboutObject : AiAgentScorer
    {
        [SerializeField]
        private ObjectDataProvider objectProvider;
        
        #region Public methods

        public override float Score(float _deltaTime)
        {
            foreach (var nearbyObject in NearbyObjects)
            {
                if (nearbyObject == null) continue;
                if (!objectProvider.ValidateData()) continue;
                if (objectProvider.GetData() == nearbyObject) return score; 
            }

            return 0;
        }

        #endregion
    }
}