// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVLoadBalancer;
using UnityEngine;

namespace RVHonorAI
{
    [RequireComponent(typeof(Animator))] public class LookAt : MonoBehaviour
    {
        #region Fields

        public Transform head;
        public Transform lookAtTransform;

        public bool look = true;

        public float bodyWeight = .2f;
        public float headWeight = .65f;
        public float eyesWeight = 1f;
        public float clampWeight = 0.8f;

        private Animator anim;

        [SerializeField]
        private float movementSpeed = 4;

        //[SerializeField]
        private float lookAtWeight;

        //[SerializeField]
        private float currentTargetWeight;

        //[SerializeField]
        private Vector3 lookAtPos;

        private bool targetChanged;

        #endregion

        #region Properties

        public Transform LookAtTransform
        {
            get => lookAtTransform;
            set
            {
                if (lookAtTransform == value) return;
                lookAtTransform = value;
                targetChanged = true;
            }
        }

        #endregion

        #region Not public methods

        private void Start()
        {
            anim = GetComponent<Animator>();
            lookAtPos = head.position + head.forward;
        }

        private void OnEnable() => LB.Register(this, LookAtLogic, new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0, true));

        private void OnDisable() => LB.Unregister(this);

        private void LookAtLogic(float dt)
        {
            if (LookAtTransform != null)
                if (!targetChanged)
                    lookAtPos = LookAtTransform.position;
            //lse if (lookAtWeight < .05f) return;

            var targetWeight = 0;
            if (look)
            {
                targetWeight = 1;
                if (targetChanged)
                {
                    targetWeight = 0;
                    if (lookAtWeight < .1f) targetChanged = false;
                }
            }
            else
            {
                targetWeight = 0;
            }

            if (Mathf.Abs(lookAtWeight - targetWeight) < .01f) return;
            currentTargetWeight = Mathf.MoveTowards(currentTargetWeight, targetWeight, dt * movementSpeed * .5f);
            lookAtWeight = Mathf.Lerp(lookAtWeight, currentTargetWeight, dt * movementSpeed);
        }

        private void OnAnimatorIK()
        {
            if (anim == null) return;
            anim.SetLookAtWeight(lookAtWeight, bodyWeight, headWeight, eyesWeight, clampWeight);
            anim.SetLookAtPosition(lookAtPos);
        }

        #endregion
    }
}