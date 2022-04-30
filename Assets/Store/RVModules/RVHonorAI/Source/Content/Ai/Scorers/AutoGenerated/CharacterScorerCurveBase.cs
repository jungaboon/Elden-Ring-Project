// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.Scorers;
using UnityEngine;

namespace RVHonorAI.Content.AI.Scorers
{
    public abstract class CharacterScorerCurveBase : AiAgentScorer
    {
        #region Fields

// AiAgentBaseScorerCurve
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

// CharacterScorer
        protected ICharacter character;

        #endregion

// AiAgentBaseScorerCurve
// CharacterScorer

        #region Methods

// AiAgentBaseScorerCurve
        protected float GetScoreFromCurve(float _score) => curve.Evaluate(_score) * score;

// CharacterScorer
        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            character = (Context as ICharacterProvider)?.Character;
        }

        #endregion
    }
}