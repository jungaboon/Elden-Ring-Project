// Created by Ronis Vision. All rights reserved
// 16.10.2020.

using RVModules.RVLoadBalancer;
using UnityEngine;
using UnityEngine.Events;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
    /// <summary>
    /// Calls UnityEvent after set time span and destroy itself after
    /// </summary>
    public class TimedUnityEvent : LoadBalancedBehaviour
    {
        private float elapsed;

        [SerializeField]
        private float invokeAfter = 1;

        public UnityEvent unityEvent;

        protected override LoadBalancerConfig LoadBalancerConfig => new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0, true);

        protected override void LoadBalancedUpdate(float _deltaTime)
        {
            elapsed += _deltaTime;
            if (elapsed > invokeAfter)
            {
                unityEvent.Invoke();
                Destroy(this);
            }
        }
    }
}