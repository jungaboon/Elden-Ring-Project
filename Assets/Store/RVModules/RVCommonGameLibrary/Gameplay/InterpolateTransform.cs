// Created by Ronis Vision. All rights reserved
// 16.10.2020.

using RVModules.RVLoadBalancer;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
    /// <summary>
    /// 
    /// </summary>
    public class InterpolateTransform : LoadBalancedBehaviour
    {
        #region Fields

        [SerializeField]
        [Tooltip("Transform that will follow target transform")]
        private Transform interpolatedTransform;

        [SerializeField]
        private Transform targetTransform;

        [SerializeField]
        private bool lerpPosition = true, lerpRotation = true;

        [SerializeField]
        private float positionLerpSpeed = 10;

        [SerializeField]
        private float rotationLerpSpeed = 10;

        #endregion

        #region Properties

        protected override LoadBalancerConfig LoadBalancerConfig => new LoadBalancerConfig("Transforms interpolation", LoadBalancerType.EveryXFrames, 0);

        public Transform InterpolatedTransform
        {
            get => interpolatedTransform;
            set => interpolatedTransform = value;
        }

        public Transform TargetTransform
        {
            get => targetTransform;
            set => targetTransform = value;
        }

        public float PositionLerpSpeed
        {
            get => positionLerpSpeed;
            set => positionLerpSpeed = value;
        }

        public float RotationLerpSpeed
        {
            get => rotationLerpSpeed;
            set => rotationLerpSpeed = value;
        }

        #endregion

        #region Not public methods

        protected virtual void SetViewTransformPosition(float deltaTime) => interpolatedTransform.position =
            Vector3.Lerp(interpolatedTransform.position, TargetTransform.position, deltaTime * PositionLerpSpeed);

        protected virtual void SetViewTransformRotation(float deltaTime) => interpolatedTransform.rotation =
            Quaternion.Lerp(interpolatedTransform.rotation, TargetTransform.rotation, deltaTime * RotationLerpSpeed);

        protected override void LoadBalancedUpdate(float _deltaTime)
        {
            var deltaTime = UnityTime.DeltaTime;
            if (lerpPosition) SetViewTransformPosition(deltaTime);
            if (lerpRotation) SetViewTransformRotation(deltaTime);
        }

        #endregion
    }
}