// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.Content.AI.Tasks;
using RVModules.RVUtilities;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace RVHonorAI.Content.AI.Tasks
{
    /// <summary>
    /// Allows to rotate ai agent toward target using simple transform manipulation and by using root motion
    /// todo configurable allow aiming while moving
    /// </summary>
    public class AimTowardTarget : AiJob
    {
        #region Fields

        private float currentRotationSpeed;

        // does it make sense to have smth like this? if ai cant aim while moving it should be implemented in graph logic, not aiming logic
//        [SerializeField]
//        private BoolProvider canAimWhileMoving;

        [FormerlySerializedAs("rotationSpeed")]
        [SerializeField]
        private float _obsolete_rotationSpeed = 160;

        private ITargetProvider targetProvider;
        private ICharacterAnimation characterAnimation;

        private float timeInRightAngle;
        private float timeInRightAngleToFinishAiming = 2;
        
        private bool charMoving;
        private float lastTimeCharMovingChanged;
        
        #endregion

        #region Properties

        protected override string DefaultDescription => "";

        #endregion

        #region Not public methods

//        private float smoothedCharVel;

        private void OnEnable() => name = "rotateTowardTarget";

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            targetProvider = ContextAs<ITargetProvider>();
            characterAnimation = ContextAs<ICharacterAnimationProvider>().CharacterAnimation;
        }

        protected override void OnJobUpdate(float _dt)
        {
            var Target = targetProvider.Target;

            if (Target as Object == null)
            {
                FinishJob();
                return;
            }

            var myTransform = movement.Transform;
            var transformPosition = myTransform.position;
            transformPosition.y = 0;
            var targetPosition = Target.Transform.position;
            targetPosition.y = 0;

            var angle = Vector3.SignedAngle(myTransform.forward, targetPosition - transformPosition, Vector3.up);

            float deadZone = 6;
            float targetRotationSpeed = 0;

            targetRotationSpeed = Mathf.Lerp(0, 1, Math.Abs(angle) * .035f);

            if (angle < 0) targetRotationSpeed *= -1;

            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetRotationSpeed, UnityTime.DeltaTime * 5);

//            var rotSpeed = currentRotationSpeed * _obsolete_rotationSpeed * UnityTime.DeltaTime;
            var rotSpeed = currentRotationSpeed * movement.RotationSpeed * UnityTime.DeltaTime;

            //var vel = characterAnimation.UseRootMotion ? characterAnimation.Animator.velocity : movement.Velocity;

//            smoothedCharVel = Mathf.MoveTowards(smoothedCharVel, movement.Velocity.sqrMagnitude, UnityTime.DeltaTime * 5);
            var moving = movement.Velocity.sqrMagnitude > 1;

            // allow to change 'charMoving' only one time threshold to avoid agressive constant animation transistions that interfere with attacks animations  
            if (moving != charMoving && UnityTime.Time - lastTimeCharMovingChanged > .5f)
            {
                charMoving = moving;
                lastTimeCharMovingChanged = UnityTime.Time;
            }

//            if (charMoving && !canAimWhileMoving)
//            {
//                FinishJob();
//                return;
//            }

            characterAnimation.Moving = charMoving;
            var inRightAngle = Math.Abs(angle) < deadZone;
            characterAnimation.Rotating = !charMoving; //&& !inRightAngle;

            // for animation we want normalized rotation speed value!
            characterAnimation.RotatingSpeed = currentRotationSpeed;

            // we take control over rotation of our character to allow facing other direction for aiming when moving
            if (!characterAnimation.UseRootMotion || charMoving) myTransform.Rotate(Vector3.up, rotSpeed, Space.Self);

            if (inRightAngle)
            {
                timeInRightAngle += UnityTime.DeltaTime;
                if (timeInRightAngle > timeInRightAngleToFinishAiming) FinishJob();
            }
            else
            {
                timeInRightAngle = 0;
            }
        }

        protected override void OnJobStart()
        {
            timeInRightAngle = 0;
            movement.UpdateRotation = false;
            charMoving = false;
            lastTimeCharMovingChanged = 0;
        }

        protected override void OnJobFinish()
        {
            characterAnimation.Rotating = false;
            characterAnimation.Moving = false;
            characterAnimation.RotatingSpeed = 0;
            
            if (Context as Object == null) return;
            movement.UpdateRotation = true;
        }

        #endregion
    }
}