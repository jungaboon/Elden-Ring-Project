// Created by Ronis Vision. All rights reserved
// 14.04.2021.

using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Animation
{
    /// <summary>
    /// Simple API to control IK targets and weights of hands and feet of humanoid animation
    /// </summary>
    public class HumanoidIkHandler : MonoBehaviour
    {
        #region Fields

        private Animator animator;

        [SerializeField]
        private float leftHandPositionWeight;

        [SerializeField]
        private float leftHandRotationWeight;

        [SerializeField]
        private Transform leftHandTarget;

        [SerializeField]
        private float rightHandPositionWeight;

        [SerializeField]
        private float rightHandRotationWeight;

        [SerializeField]
        private Transform rightHandTarget;

        [SerializeField]
        private float leftFootPositionWeight;

        [SerializeField]
        private float leftFootRotationWeight;

        [SerializeField]
        private Transform leftFootTarget;

        [SerializeField]
        private float rightFootPositionWeight;

        [SerializeField]
        private float rightFootRotationWeight;

        [SerializeField]
        private Transform rightFootTarget;

        #endregion

        #region Properties

        public float LeftHandPositionWeight
        {
            get => leftHandPositionWeight;
            set => leftHandPositionWeight = value;
        }

        public float RightHandPositionWeight
        {
            get => rightHandPositionWeight;
            set => rightHandPositionWeight = value;
        }

        public float LeftFootPositionWeight
        {
            get => leftFootPositionWeight;
            set => leftFootPositionWeight = value;
        }

        public float RightFootPositionWeight
        {
            get => rightFootPositionWeight;
            set => rightFootPositionWeight = value;
        }

        public float LeftHandRotationWeight
        {
            get => leftHandRotationWeight;
            set => leftHandRotationWeight = value;
        }

        public float RightHandRotationWeight
        {
            get => rightHandRotationWeight;
            set => rightHandRotationWeight = value;
        }

        public float LeftFootRotationWeight
        {
            get => leftFootRotationWeight;
            set => leftFootRotationWeight = value;
        }

        public float RightFootRotationWeight
        {
            get => rightFootRotationWeight;
            set => rightFootRotationWeight = value;
        }
        
        public Transform LeftHandTarget
        {
            get => leftHandTarget;
            set => leftHandTarget = value;
        }

        public Transform RightHandTarget
        {
            get => rightHandTarget;
            set => rightHandTarget = value;
        }

        public Transform LeftFootTarget
        {
            get => leftFootTarget;
            set => leftFootTarget = value;
        }

        public Transform RightFootTarget
        {
            get => rightFootTarget;
            set => rightFootTarget = value;
        }

        #endregion

        #region Not public methods

        private void OnEnable()
        {
            // Its here only to show enable tick in inspector
        }

        private void Awake() => animator = GetComponent<Animator>();

        private void OnAnimatorIK(int layerIndex)
        {
            if (!enabled) return;

            if (LeftHandTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, LeftHandPositionWeight);

                animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, LeftHandRotationWeight);
            }

            if (RightHandTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, RightHandPositionWeight);

                animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, RightHandRotationWeight);
            }

            if (LeftFootTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootTarget.position);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, LeftFootPositionWeight);

                animator.SetIKRotation(AvatarIKGoal.LeftFoot, LeftFootTarget.rotation);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, LeftFootRotationWeight);
            }

            if (RightFootTarget != null)
            {
                animator.SetIKPosition(AvatarIKGoal.RightFoot, RightFootTarget.position);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, RightFootPositionWeight);

                animator.SetIKRotation(AvatarIKGoal.RightFoot, RightFootTarget.rotation);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, RightFootRotationWeight);
            }
        }

        #endregion
    }
}