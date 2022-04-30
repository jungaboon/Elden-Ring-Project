// Created by Ronis Vision. All rights reserved
// 10.08.2020.

using System;
using RVModules.RVLoadBalancer;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
    /// <summary>
    /// Calculates velocity based on transform position changes
    /// </summary>
    public class TransformVelocity : MonoBehaviour
    {
        #region Fields

        [Header("If not set, will automatically set this to own transform")]
        [SerializeField]
        private Transform transformToCalcVelocityFrom;

        [SerializeField]
        private Vector3 velocity, smoothedVelocity, lerpedVelocity;

        [SerializeField]
        private bool calculateVelocity = true, calculateSmoothVelocity, calculateLerpedVelocity;

        [SerializeField]
        private float velocitySmoothingSpeed = 10, velocityLerpSpeed = 10;

        [SerializeField]
        [Header("LBCs need to have deltaTime calculated to work properly!")]
        private LoadBalancerConfig velocityLbc = new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0, true);

        [SerializeField]
        private LoadBalancerConfig smoothVelocityLbc = new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0, true);

        [SerializeField]
        private LoadBalancerConfig lerpVelocityLbc = new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0, true);

        [SerializeField]
        [Tooltip("For performance reasons this can't be changed at runtime")]
        private VelocityType selectedVelocity;

        private Vector3 currVelocity;

        private Vector3 lastPos;
        private Vector3 lastPosSmooth;
        private Vector3 lastPosLerp;

        private Func<Vector3> selectedVelocityProvider;

        private new Transform transform;

        #endregion

        #region Properties

        public Vector3 Velocity => velocity;
        public Vector3 SmoothedVelocity => smoothedVelocity;
        public Vector3 LerpedVelocity => lerpedVelocity;

        /// <summary>
        /// Returns velocity type selected in inspector
        /// </summary>
        public Vector3 SelectedVelocity => selectedVelocityProvider();

        #endregion

        #region Not public methods

        private void Awake()
        {
            transform = base.transform;
            if (transformToCalcVelocityFrom == null) transformToCalcVelocityFrom = transform;

            switch (selectedVelocity)
            {
                case VelocityType.Direct:
                    selectedVelocityProvider = () => velocity;
                    break;
                case VelocityType.Smoothed:
                    selectedVelocityProvider = () => smoothedVelocity;
                    break;
                case VelocityType.Lerped:
                    selectedVelocityProvider = () => lerpedVelocity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnEnable()
        {
            if (calculateVelocity) LoadBalancerSingleton.Instance.Register(this, CalculateVelocity, velocityLbc);
            if (calculateSmoothVelocity) LoadBalancerSingleton.Instance.Register(this, CalculateSmoothedVelocity, smoothVelocityLbc);
            if (calculateLerpedVelocity) LoadBalancerSingleton.Instance.Register(this, CalculateLerpedVelocity, lerpVelocityLbc);
        }

        private void OnDisable()
        {
            if (LoadBalancerSingleton.Instance == null) return;
            LoadBalancerSingleton.Instance.Unregister(this);
        }

        private void CalculateVelocity(float dt)
        {
            CalcVel(out var vel, ref lastPos, dt);
            velocity = vel;
        }

        private void CalculateSmoothedVelocity(float dt)
        {
            CalcVel(out var vel, ref lastPosSmooth, dt);
            if (vel == lastPosSmooth) return;
            smoothedVelocity = Vector3.MoveTowards(smoothedVelocity, vel, dt * velocitySmoothingSpeed);
        }

        private void CalculateLerpedVelocity(float dt)
        {
            CalcVel(out var vel, ref lastPosLerp, dt);
            if (vel == lastPosLerp) return;
            lerpedVelocity = Vector3.Lerp(lerpedVelocity, vel, dt * velocityLerpSpeed);
        }

        private bool CalcVel(out Vector3 vel, ref Vector3 _lastPos, float deltaTime)
        {
            var position = transformToCalcVelocityFrom.position;
            var posDiff = (position - _lastPos);
            if (posDiff == Vector3.zero)
            {
                vel = Vector3.zero;
                return false;
            }

            vel = posDiff / deltaTime;
            _lastPos = position;
            return true;
        }

        #endregion

        public enum VelocityType
        {
            Direct,
            Smoothed,
            Lerped
        }
    }
}