// Created by Ronis Vision. All rights reserved
// 07.02.2021.

using System.Collections.Generic;
using RVModules.RVLoadBalancer;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
    /// <summary>
    /// Set transform position to average position of provided transforms 
    /// </summary>
    public class SetPositionToAverage : LoadBalancedBehaviour
    {
        [SerializeField]
        private Transform transformToSetPosition;

        [SerializeField]
        private List<Transform> transforms;

        [SerializeField]
        private LoadBalancerConfig loadBalancerConfig;

        protected override LoadBalancerConfig LoadBalancerConfig => loadBalancerConfig;

        public List<Transform> Transforms
        {
            get => transforms;
            set => transforms = value;
        }

        protected override void LoadBalancedUpdate(float _deltaTime)
        {
            Vector3 avgPos = Vector3.zero;
            int nonNulls = 0;

            foreach (var trn in Transforms)
            {
                if (trn == null) continue;
                nonNulls++;
                avgPos += trn.position;
            }

            if (nonNulls == 0) return;

            avgPos /= nonNulls;

            transformToSetPosition.position = avgPos;
        }
    }
}