// Created by Ronis Vision. All rights reserved
// 08.08.2020.

using System;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
    /// <summary>
    /// Provides better interpolation and extrapolation based on (externally) provided velocity,
    /// usefull for networked objects
    ///
    /// todo wip
    /// </summary>
    public class InterpolatedTransformVelocityWIP : MonoBehaviour
    {
        [SerializeField]
        private Transform transformToFollow;

        /// <summary>
        /// Cached transform for fast access
        /// </summary>
        private new Transform transform;

        [SerializeField]
        private bool lerpPosition = true, lerpRotation = true;

        [SerializeField]
        protected int positionLerpSpeed = 10;

        [SerializeField]
        protected int rotationLerpSpeed = 10;

        private void Awake()
        {
            transform = base.transform;
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            if (lerpPosition) SetViewTransformPosition(transformToFollow, deltaTime);
            if (lerpRotation) SetViewTransformRotation(transformToFollow, deltaTime);
        }

        protected virtual void SetViewTransformPosition(Transform followTarget, float deltaTime) => transform.position =
            Vector3.Lerp(transform.position, followTarget.position, deltaTime * positionLerpSpeed);

        protected virtual void SetViewTransformRotation(Transform followTarget, float deltaTime) => transform.rotation =
            Quaternion.Lerp(transform.rotation, followTarget.rotation, deltaTime * rotationLerpSpeed);
        
        // stuff copied from authoritative movement blablabla
//                #region Fields
//
//        [SerializeField]
//        [Tooltip("dont assign it, it's created automatically, it's there just for reference")]
//        private GameObject viewGameObject;
//
//        [SerializeField]
//        [Tooltip("Game objects you want reparented to view game object")]
//        private GameObject[] viewGameObjects;
//
//        [SerializeField]
//        private int posInterpolationSpeed = 15;
//
//        [SerializeField]
//        private int rotInterpolationSpeed = 20;
//
//        private Vector3 lastPos;
//        private float lastPosChangeTime;
//
//        [SerializeField]
//        private Vector3 movementExtrapolation;
//
//        private new Transform transform;
//
//        #endregion
//
//        #region Properties
//
//        public GameObject ViewGameObject => viewGameObject;
//
//        public int PosInterpolationSpeed
//        {
//            get => posInterpolationSpeed;
//            set => posInterpolationSpeed = value;
//        }
//
//        public int RotInterpolationSpeed
//        {
//            get => rotInterpolationSpeed;
//            set => rotInterpolationSpeed = value;
//        }
//
//        #endregion
//
//        #region Not public methods
//
//        private void Awake()
//        {
//            transform = base.transform;
//        }
//
//        private void Start()
//        {
//            CreateViewObject();
//        }
//
//        /// <summary>
//        /// Reparents all viewGameObjects to newly game object responsible for interpolating visuals 
//        /// </summary>
//        protected virtual void CreateViewObject()
//        {
//            viewGameObject = new GameObject($"{gameObject.name} view");
//            ViewGameObject.transform.position = transform.position;
//            ViewGameObject.transform.rotation = transform.rotation;
//            foreach (var o in viewGameObjects)
//            {
//                if (o == null) continue;
//                o.transform.SetParent(ViewGameObject.transform, false);
//            }
//        }
//
//        /// <summary>
//        /// Interpolates position and rotation
//        /// </summary>  
//        protected override void LoadBalancedUpdate(float _deltaTime)
//        {
//            var deltaTime = Time.deltaTime;
//            var viewTransform = ViewGameObject.transform;
//
//            SetViewTransformPosition(viewTransform, deltaTime);
//            SetViewTransformRotation(viewTransform, deltaTime);
//
////            bool posChanged = transform.position != lastPos;
////
////            lastPos = transform.position;
////
////            if (posChanged)
////            {
////                movementExtrapolation = Vector3.zero;
////            }
////
////            if (!posChanged && movementExtrapolation != Vector3.zero)
////                movementExtrapolation = Vector3.Lerp(movementExtrapolation, Vector3.zero, Time.deltaTime * posInterpolationSpeed * 5);
////
////            movementExtrapolation += deltaTime * velocity;
//        }
//
////        public Vector3 velocity;
//
//        protected virtual void SetViewTransformRotation(Transform viewTransform, float deltaTime) => viewTransform.rotation =
//            Quaternion.Lerp(viewTransform.rotation, transform.rotation, deltaTime * RotInterpolationSpeed);
//
//        protected virtual void SetViewTransformPosition(Transform viewTransform, float deltaTime) => viewTransform.position =
//            Vector3.Lerp(viewTransform.position, transform.position, deltaTime * PosInterpolationSpeed);
//        //Vector3.MoveTowards(viewTransform.position, transform.position + movementExtrapolation, deltaTime * velocity.magnitude);
//
//        protected override LoadBalancerConfig LoadBalancerConfig => new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0);
//
//        /// <summary>
//        /// Sets ViewGameObject active to true
//        /// </summary>
//        protected override void OnEnable()
//        {
//            base.OnEnable();
//            if (ViewGameObject != null) ViewGameObject.SetActive(true);
//        }
//
//        /// <summary>
//        /// Sets ViewGameObject active to false
//        /// </summary>
//        protected override void OnDisable()
//        {
//            base.OnDisable();
//            if (ViewGameObject != null) ViewGameObject.SetActive(false);
//        }
//
//        /// <summary>
//        /// Destroys ViewGameObject
//        /// </summary>
//        protected virtual void OnDestroy()
//        {
//            Destroy(ViewGameObject);
//        }
//
//
//        /// <summary>
//        /// Calculates velocity of other client character controller from last two received states
//        /// </summary>
////        private void LogicChangedPosition(Vector3 pos)
////        {
////            // 
////            movementExtrapolation = Vector3.zero;
////
////            var dt = Time.time;
////            if (lastReceivedState != null)
////            {
////                var timeDelta = dt - lastReceivedStateTime;
////                if (timeDelta > 0) calculatedVelocityFromLastTwoPositions = (pos.Position - lastReceivedState.Position) / timeDelta;
////            }
////
////            lastReceivedState = pos;
////            lastReceivedStateTime = dt;
////
////            SetState(pos);
////        }
//
//        #endregion
    }
}