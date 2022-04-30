// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class MoveToPosition : AiAgentTask
    {
        #region Fields

        [SerializeField]
        private Vector3Provider position;

        #endregion

        #region Not public methods

        protected override void Execute(float _deltaTime) => movement.Destination = position;

        #endregion
    }
}