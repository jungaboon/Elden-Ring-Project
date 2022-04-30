// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    /// <summary>
    /// 
    /// </summary>
    public class IsMovingAiScorer : AiAgentScorer
    {
        #region Fields

        public float scoreNotMoving;
        
        [Tooltip("Velocity needed to return score")]
        [SerializeField]
        private float movementVelocity = .15f;

        #endregion

        #region Public methods

        public override float Score(float _deltaTime) => movement.Velocity.magnitude > movementVelocity ? score : scoreNotMoving;

        #endregion
    }
}