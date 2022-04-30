// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.Tasks;

namespace RVHonorAI.Content.AI.Tasks
{
    public abstract class CharacterTaskParams<T> : AiAgentTaskParams<T>
    {
        #region Fields

        protected ICharacter character;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            character = (Context as ICharacterProvider).Character;
        }

        #endregion
    }
}