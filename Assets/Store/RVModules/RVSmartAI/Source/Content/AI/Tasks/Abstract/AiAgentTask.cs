// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using System.Collections.Generic;
using RVModules.RVSmartAI.Content.AI.Contexts;
using RVModules.RVSmartAI.Content.Scanners;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    /// <summary>
    /// Convenience base class for most commonly used interfaces related to spatial awareness and movement
    /// </summary>
    public abstract class AiAgentTask : AiTask
    {
        #region Fields

        private INearbyObjectsProvider nearbyObjectsProvider;
        private IMoveTargetProvider moveTargetProvider;
        protected IWaypointsProvider waypointsProvider;

        protected IMovement movement;
        protected IMovementScanner movementScanner;
        protected IEnvironmentScanner environmentScanner;

        #endregion

        #region Properties

        protected Transform MoveTarget
        {
            get => moveTargetProvider.FollowTarget;
            set => moveTargetProvider.FollowTarget = value;
        }

        protected List<Object> NearbyObjects => nearbyObjectsProvider.NearbyObjects;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            movement = (Context as IMovementProvider)?.Movement;
            movementScanner = (Context as IMovementScannerProvider)?.MovementScanner;
            environmentScanner = (Context as IEnvironmentScannerProvider)?.EnvironmentScanner;
            moveTargetProvider = Context as IMoveTargetProvider;
            nearbyObjectsProvider = Context as INearbyObjectsProvider;
            waypointsProvider = Context as IWaypointsProvider;
        }

        #endregion

        //protected List<Transform> Waypoints => waypointsProvider.Waypoints;
    }
}