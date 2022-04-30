// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.Tasks;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVHonorAI.Content.AI.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class SetCharacterSpeed : AiAgentTask, ITaskWithScorers
    {
        #region Fields

        [SerializeField]
        protected float speed;

        #endregion

        #region Not public methods

        protected override void Execute(float _deltaTime)
        {
            var speedLocal = speed;
            speedLocal += Score(_deltaTime);
            movement.MovementSpeed = speedLocal;
            //characterAi.SetSpeed(speedLocal);
        }

        #endregion
    }
}