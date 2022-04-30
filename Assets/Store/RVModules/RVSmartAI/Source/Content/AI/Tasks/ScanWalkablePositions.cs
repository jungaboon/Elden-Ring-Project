// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using System.Collections.Generic;
using RVModules.RVLoadBalancer;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    /// <summary>
    /// Stores walkable positions in list Vector3 variable for later use
    /// </summary>
    public class ScanWalkablePositions : AiAgentTask
    {
        #region Fields

        [Tooltip("Distance(radius) in which positions will be scanned")]
        [SerializeField]
        private FloatProvider scanDistance;

        [SerializeField]
        private Vector3Provider position;

        [SerializeField]
        [Tooltip("Name of graph variable where list of walkable positions will be stored")]
        private string variablePositions;
        
        [Tooltip("If true, scanning will be spread across multiple frames using below settings, and " +
                 "graph execution will be stopped until it finishes")]
        [SerializeField]
        private bool loadBalancedTask;
        
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
            "walkable positions using IMovementScanner, and stores resulted positions\n" +
            "in Vector3 list variable";

        protected override LoadBalancerConfig RunningTaskLbc => loadBalancerConfig;

        public override bool IsRunningTask => loadBalancedTask;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            // assure list with selected name exists
            if (!AiGraph.GraphAiVariables.AssureVector3ListExist(variablePositions))
                AiGraph.GraphAiVariables.SetVector3List(variablePositions, new List<Vector3>(20));
        }

        protected override bool StartExecuting()
        {
            scanningDone = false;
            AiGraph.GraphAiVariables.GetVector3List(variablePositions).Clear();
            movementScanner.SetPositionForLoadBalancedScanning(position, scanDistance);
            return true;
        }

        protected override void Execute(float _deltaTime)
        {
            var walkablePositions = movementScanner.FindWalkablePositions(position, scanDistance);
            AiGraph.GraphAiVariables.SetVector3List(variablePositions, walkablePositions);
        }

        protected override void Executing(float deltaTime)
        {
            scanningDone = movementScanner.FindWalkablePositionsLoadBalanced(out var walkablePositions, positionScansPerTick);

            if (scanningDone)
            {
                AiGraph.GraphAiVariables.SetVector3List(variablePositions, walkablePositions);
                StopExecuting();
            }
        }

        #endregion
    }
}