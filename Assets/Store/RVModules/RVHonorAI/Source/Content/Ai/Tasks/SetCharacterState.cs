// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVHonorAI.Content.AI.Tasks
{
    public class SetCharacterState : AiTask
    {
        #region Fields

        [SerializeField]
        private CharacterState state;

        #endregion

        #region Not public methods

        protected override void Execute(float _deltaTime) => ContextAs<ICharacterState>().CharacterState = state;

        #endregion
    }
}