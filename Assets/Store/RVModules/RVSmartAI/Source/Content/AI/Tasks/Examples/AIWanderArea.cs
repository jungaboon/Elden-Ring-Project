// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    /// <summary>
    /// Finds walkable position around MoveTarget within set range and goes there. Range is measured in straight line from agent position(like radius)
    /// Requires context providing IMovement, IMovementScanner and MoveTarget
    /// </summary>
    public class AIWanderArea : AiAgentTask
    {
        #region Fields

        [Header("Radius in which agent will go next, measuring from area position")]
        public int areaRadius = 10;

        [Header("Radius in which scanner will try to find walkable position around random pos in defined area")]
        public int scanRadius = 3;

        #endregion

        #region Not public methods

        protected override void Execute(float _deltaTime)
        {
            var pos = MoveTarget == null ? movement.Position : MoveTarget.position;
            var walkablePositions = movementScanner.FindWalkablePositions(pos + Random.insideUnitCircle.ToVector3() * areaRadius, scanRadius);
            if (walkablePositions.Count == 0) return;

            var dest = walkablePositions[Random.Range(0, walkablePositions.Count)];
            movement.Destination = dest;
        }

        #endregion
    }
}