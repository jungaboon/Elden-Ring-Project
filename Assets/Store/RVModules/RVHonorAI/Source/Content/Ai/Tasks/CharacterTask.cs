// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.Tasks;

namespace RVHonorAI.Content.AI.Tasks
{
    public abstract class CharacterTask : AiAgentTask
    {
        #region Fields

        private ICharacter character;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            character = ContextAs<CharacterAi>().Character;
        }

        #endregion
    }
}