// Created by Ronis Vision. All rights reserved
// 07.08.2020.

using RVModules.RVLoadBalancer;
using RVModules.RVUtilities;
using RVModules.RVUtilities.Extensions;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Animation
{
    /// <summary>
    /// Controls movement and stationary rotation animations based on transform position and rotation changes - it can be moved in any way
    /// Animator should have parameters: float x, y to control movement
    /// bool rotating to enter and exit rotating state (rotating when stationary)
    /// float rotation for rotating when stationary (>0 for rotating right, <0 for left) 
    /// </summary>
    public class CharacterMovementAnimation : LoadBalancedBehaviour
    {
        #region Fields

        private static readonly int y = Animator.StringToHash("y");
        private static readonly int x = Animator.StringToHash("x");
        private static readonly int rotation = Animator.StringToHash("rotation");
        private static readonly int rotating = Animator.StringToHash("rotating");

        private Vector3 lastPos;
        private Vector3 lastDir;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Transform transformToCalcVelocityFrom;

        [SerializeField]
        private float velocityDeltaSpeed = 10f;

        [SerializeField]
        private float movementMul = 1;

        [SerializeField]
        private Vector2 currentVelocity;

        [SerializeField]
        private float rotationDeltaSpeed = 5f;

        [SerializeField]
        private float currentRotation;

        [SerializeField]
        private float rotationMul = 1;

        #endregion

        #region Properties

        public Vector2 CurrentVelocity => currentVelocity;

        public Animator Animator
        {
            get => animator;
            protected set => animator = value;
        }

        #endregion

        #region Not public methods

        private void Awake()
        {
            if (!Animator) Animator = GetComponent<Animator>();
            if (!transformToCalcVelocityFrom) transformToCalcVelocityFrom = transform;
        }

        protected override void LoadBalancedUpdate(float _deltaTime)
        {
            var position = transformToCalcVelocityFrom.position;
            var dir = transformToCalcVelocityFrom.forward;

            var deltaTime = UnityTime.DeltaTime;
            var posDelta = (position - lastPos) / deltaTime * movementMul;
            var rotDelta = Vector3.SignedAngle(dir, lastDir, transformToCalcVelocityFrom.up) / deltaTime * rotationMul;
            currentRotation = Mathf.Lerp(currentRotation, rotDelta, deltaTime * rotationDeltaSpeed);

            lastPos = position;
            lastDir = dir;

            var movementVelocity = posDelta.ToVector2();
            if (movementVelocity.sqrMagnitude > .1f || currentRotation == 0)
            {
                SetAnimVelocities(movementVelocity);
                Animator.SetBool(rotating, false);
            }
            else
            {
                Animator.SetBool(rotating, true);
                Animator.SetFloat(rotation, -currentRotation);
            }
        }

        private void SetAnimVelocities(Vector2 v2)
        {
            var dx = Vector2.Dot(transformToCalcVelocityFrom.right.ToVector2(), v2);
            var dy = Vector2.Dot(transformToCalcVelocityFrom.forward.ToVector2(), v2);
            v2.x = dx;
            v2.y = dy;
            currentVelocity = Vector2.Lerp(CurrentVelocity, v2, UnityTime.DeltaTime * velocityDeltaSpeed);
            Animator.SetFloat(x, CurrentVelocity.x);
            Animator.SetFloat(y, CurrentVelocity.y);
        }

        #endregion

        protected override LoadBalancerConfig LoadBalancerConfig => new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0);
    }
}