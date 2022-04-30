// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVLoadBalancer;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    /// <summary>
    /// Scans provided position surroundings to find walkable positions using IMovementScanner, then scores them and sets IMovement destination
    /// to position with highest score 
    /// </summary>
    public class ScanAndMoveToBestPosition : AiAgentTaskParams<Vector3>
    {
        #region Fields

        [Tooltip("Distance(radius) in which positions will be scanned and scored")]
        [SerializeField]
        private FloatProvider scanDistance;

        [SerializeField]
        private Vector3Provider position;

        [Tooltip("If true, scanning and scoring will be spread across multiple frames using below settings, and " +
                 "graph execution will be stopped until it finishes")]
        [SerializeField]
        private bool loadBalancedTask;

        [ConditionalHide(nameof(loadBalancedTask), hideInInspector = true)]
        [SerializeField]
        [Tooltip("How many positions will be scored per tick")]
        private int parametersPerTick = 10;

        [ConditionalHide(nameof(loadBalancedTask), hideInInspector = true)]
        [SerializeField]
        [Tooltip("How many positions will be scanned per tick")]
        private int positionScansPerTick = 5;

        [ConditionalHide(nameof(loadBalancedTask), hideInInspector = true)]
        [SerializeField]
        private LoadBalancerConfig loadBalancerConfig;

        private bool scanningDone;

        #endregion

        #region Properties

        protected override string DefaultDescription =>
            "Scans provided position surroundings to find \n" +
            "walkable positions using IMovementScanner, \n" +
            "then scores them and sets IMovement destination \n" +
            "to position with highest score";

        protected override LoadBalancerConfig RunningTaskLbc => loadBalancerConfig;

        public override bool IsRunningTask => loadBalancedTask;

        #endregion

        #region Not public methods

        protected override bool StartExecuting()
        {
            scanningDone = false;
            movementScanner.SetPositionForLoadBalancedScanning(position, scanDistance);
            return true;
        }

        protected override void Execute(float _deltaTime)
        {
            // get walkable positions near position
            var positions = movementScanner.FindWalkablePositions(position, scanDistance);

            // if we cant find any walkable position just exit, to avoid exception in GetBest method
            if (positions.Count == 0) return;

            // set our agent destination to this pos
            movement.Destination = GetBest(positions);
        }

        protected override void Executing(float deltaTime)
        {
            if (!scanningDone)
            {
                scanningDone = movementScanner.FindWalkablePositionsLoadBalanced(out var walkablePositions, positionScansPerTick);
                if (scanningDone) SetParametersForExecuting(walkablePositions);
                return;
            }

            for (var i = 0; i < parametersPerTick; i++)
                if (GetBestDelayed(out var best))
                {
                    movement.Destination = best;
                    StopExecuting();
                    return;
                }
        }

        #endregion
    }
}