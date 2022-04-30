// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVLoadBalancer;
using UnityEngine;
using UnityEngine.Events;

namespace RVHonorAI.Utilities
{
    public class DisableRigidbodyOnSleep : LoadBalancedBehaviour
    {
        #region Fields

        public UnityEvent onRigidbodySleep;
        private Rigidbody rb;

        [SerializeField]
        [Tooltip("Rigidbody will be disabled after x seconds")]
        private int forceDisableAfter = 20;

        [SerializeField]
        [Tooltip("If false, rigidbody will be set to kinematic, if true it will be removed")]
        private bool removeRigidbody;

        [SerializeField]
        [Tooltip("Should colliders on this game objects be removed")]
        private bool removeColliders;

        [SerializeField]
        private float elapsedTime;

        #endregion

        #region Properties

        protected override LoadBalancerConfig LoadBalancerConfig => new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0, true);

        #endregion

        #region Not public methods

        private void Awake() => rb = GetComponent<Rigidbody>();

        protected override void LoadBalancedUpdate(float _deltaTime)
        {
            elapsedTime += _deltaTime;
            if (rb.IsSleeping() || elapsedTime > forceDisableAfter && forceDisableAfter >= 0)
            {
                onRigidbodySleep.Invoke();
                if (removeColliders)
                    foreach (var component in GetComponents<Collider>())
                        Destroy(component);

                if (removeRigidbody)
                    Destroy(rb);
                else
                    rb.isKinematic = true;
                Destroy(this);
            }
        }

        #endregion
    }
}