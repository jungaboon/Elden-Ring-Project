// Created by Ronis Vision. All rights reserved
// 14.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class BoolScorer : AiScorer
    {
        [Header("Obsolete, use bool provider")]
        [SerializeField]
        private string variableName;

        [SerializeField]
        private bool expectedBoolValue;

        [SerializeField]
        private BoolProvider boolValue;

        protected override void OnContextUpdated()
        {
            if(!string.IsNullOrEmpty(variableName)) this.ObsoleteGraphElementFieldStillUsedError(nameof(variableName));
        }

        public override float Score(float _deltaTime)
        {
            if (boolValue.GetData() == expectedBoolValue) return score;
            return 0;
        }
    }
}