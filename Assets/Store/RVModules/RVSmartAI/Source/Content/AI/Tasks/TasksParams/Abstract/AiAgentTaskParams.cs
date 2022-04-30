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
    /// 
    /// </summary>
    public abstract class AiAgentTaskParams<T> : AiTaskParams<T>
    {
        #region Fields

        protected IMovement movement;
        protected IMovementScanner movementScanner;
        protected IEnvironmentScanner environmentScanner;

        private INearbyObjectsProvider nearbyObjectsProvider;
        private IMoveTargetProvider moveTargetProvider;
        private IWaypointsProvider waypointsProvider;

        #endregion

        #region Properties

        protected Transform MoveTarget => moveTargetProvider.FollowTarget;
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
    }
}