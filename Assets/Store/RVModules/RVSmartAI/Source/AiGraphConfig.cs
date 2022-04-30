// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using RVModules.RVLoadBalancer;
using UnityEngine;

namespace RVModules.RVSmartAI
{
    [Serializable] public class AiGraphConfig
    {
        #region Fields

        public AiGraph aiGraph;
        public int updateFrequency = 2;
        public bool useCustomLoadBalancerConfig;
        public bool expandPerfInfo;
        [Tooltip("Limits total AI updates per second and lowers frequency after reaching it, slowing down AI updates frequency instead of taking more CPU time")]
        public bool scalableLoadBalancing;
        [Tooltip("How many updates per second is allowed. After reaching this number AI update frequency will automatically lower to match this value")]
        public int maxAllowedUpdateFrequency = 120;
        public LoadBalancerConfig loadBalancerConfig;
        public bool overrideGraphVariablesForNestedGraphs = true;

        #endregion
    }
}