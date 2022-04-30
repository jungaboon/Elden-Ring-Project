// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class Obsolete_MoveToBestPosNearMe : AiAgentTaskParams<Vector3>
    {
        #region Fields

        public float distToTarget = 3;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated() => this.ObsoleteGraphElementUsedError(typeof(ScanAndMoveToBestPosition));

        protected override void Execute(float _deltaTime)
        {
            // get walkable positions close to our follow target
            var positions = movementScanner.FindWalkablePositions(movement.Position, distToTarget);

            // if we cant find any walkable position just exit, to avoid exception in GetBest method
            if (positions.Count == 0) return;

            // set our agent destination to this pos
            movement.Destination = GetBest(positions);
        }

        #endregion
    }
}