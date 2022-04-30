// Created by Ronis Vision. All rights reserved
// 14.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// Allows for kinda-update-rate independent random chance of scoring.
    /// Go look at source to understand 
    /// </summary>
    public class RandomChanceForScore : AiScorer
    {
        [Header("In percent (0-100)")]
        [SerializeField]
        private FloatProvider chancePerSecond;

        private float lastTimeCall;

        public override float Score(float _deltaTime)
        {
            float timeInterval = Mathf.Clamp01(UnityTime.Time - lastTimeCall);
            lastTimeCall = UnityTime.Time;
            var scored = RandomChance.Get(chancePerSecond * timeInterval);
            if (scored) return score;
            return 0;
        }
    }
}