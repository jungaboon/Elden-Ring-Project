// Created by Ronis Vision. All rights reserved
// 13.02.2021.

using RVModules.RVLoadBalancer;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
    public class TransformTranslate : LoadBalancedBehaviour
    {
        #region Fields

        public Vector3 translation;

        public Space space;

        [SerializeField]
        private LoadBalancerConfig loadBalancerConfig = new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0, true);

        #endregion

        #region Properties

        protected override LoadBalancerConfig LoadBalancerConfig => loadBalancerConfig;

        #endregion

        #region Not public methods

        protected override void LoadBalancedUpdate(float _deltaTime) => transform.Translate(translation * _deltaTime, space);

        #endregion
    }
}