// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.GraphElements;

namespace RVHonorAI.Content.AI.Scorers
{
    public class CharacterState : AiScorer
    {
        #region Fields

        public RVHonorAI.CharacterState state;

        public float falseScore;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => ContextAs<ICharacterState>().CharacterState == state ? score : falseScore;

        #endregion
    }
}