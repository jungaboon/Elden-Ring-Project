// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.Tasks;
using UnityEngine;

namespace RVHonorAI.Content.AI.Tasks
{
    public class SetMovementSpeed : AiAgentTask
    {
        #region Fields

        [SerializeField]
        private MovementSpeed movementSpeed;

        #endregion

        #region Not public methods

        protected override void Execute(float _deltaTime) => ContextAs<IMovementSpeedProvider>().MovementSpeed = movementSpeed;

        #endregion
    }
}