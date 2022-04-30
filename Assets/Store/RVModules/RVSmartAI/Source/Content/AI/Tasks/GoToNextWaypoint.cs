// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class GoToNextWaypoint : AiAgentTask
    {
        #region Fields

        [Header("Loop means after reaching last WP it move to 0.")]
        public BoolProvider loop;

        [Header("Select random waypoint")]
        public BoolProvider selectRandomWaypoint;

        [Header("Just for info")]
        public int currentWaypoint;

        private bool goingUp = true;

        #endregion

        #region Not public methods

        protected override void Execute(float _deltaTime)
        {
            if (waypointsProvider.WaypointsCount == 0) return;

            if (selectRandomWaypoint)
            {
                currentWaypoint = Random.Range(0, waypointsProvider.WaypointsCount);
                movement.Destination = waypointsProvider.GetWaypointPosition(currentWaypoint);
                return;
            }

            if (loop)
            {
                if (currentWaypoint >= waypointsProvider.WaypointsCount)
                    currentWaypoint = 0;
            }
            else
            {
                if (currentWaypoint >= waypointsProvider.WaypointsCount)
                {
                    goingUp = false;
                    currentWaypoint = waypointsProvider.WaypointsCount - 2;
                }

                if (currentWaypoint <= 0 && !goingUp)
                {
                    goingUp = true;
                    currentWaypoint = 0;
                }
            }

            movement.Destination = waypointsProvider.GetWaypointPosition(currentWaypoint);

            if (loop)
            {
                currentWaypoint++;
            }
            else
            {
                if (goingUp) currentWaypoint++;
                else currentWaypoint--;
            }
        }

        #endregion
    }
}