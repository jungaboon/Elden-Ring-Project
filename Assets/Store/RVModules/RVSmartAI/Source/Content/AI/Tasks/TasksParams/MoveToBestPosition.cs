// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVLoadBalancer;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class MoveToBestPosition : AiAgentTaskParams<Vector3>
    {
        #region Fields

        [SerializeField]
        private Vector3ListProvider positionsProvider;

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
        private LoadBalancerConfig loadBalancerConfig;

        #endregion

        #region Properties

        protected override string DefaultDescription =>
            "Scores provided positions and sets IMovement destination \n" +
            "to position with highest score.\n" +
            $"To be used with {nameof(ScanWalkablePositions)}";

        protected override LoadBalancerConfig RunningTaskLbc => loadBalancerConfig;

        public override bool IsRunningTask => loadBalancedTask;

        #endregion

        #region Not public methods

        protected override void Execute(float _deltaTime)
        {
            var positions = positionsProvider.GetData();

            // if we cant find any walkable position just exit, to avoid exception in GetBest method
            if (positions.Count == 0) return;

            // set our agent destination to this pos
            movement.Destination = GetBest(positions);
        }

        protected override bool StartExecuting()
        {
            SetParametersForExecuting(positionsProvider.GetData());
            return true;
        }

        protected override void Executing(float deltaTime)
        {
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