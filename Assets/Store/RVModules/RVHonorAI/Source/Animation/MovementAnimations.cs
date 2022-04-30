// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;

namespace RVHonorAI.Animation
{
    [Serializable] public struct MovementAnimations
    {
        public ClipConfig idleAnimation;
        public ClipConfig walkingAnimation;
        public ClipConfig runningAnimation;
        public ClipConfig walkingRightAnimation;
        public ClipConfig walkingLeftAnimation;
        public ClipConfig walkingBackAnimation;

        public ClipConfig turningRightAnimation;
        public ClipConfig turningLeftAnimation;

        public ClipConfig[] GetAllAnimations() => new[]
        {
            idleAnimation, walkingAnimation, runningAnimation, walkingRightAnimation, walkingLeftAnimation, walkingBackAnimation, turningRightAnimation,
            turningLeftAnimation
        };

        private void AssignAnimations(ClipConfig[] animations)
        {
            idleAnimation = animations[0];
            walkingAnimation = animations[1];
            runningAnimation = animations[2];
            walkingRightAnimation = animations[3];
            walkingLeftAnimation = animations[4];
            walkingBackAnimation = animations[5];
            turningRightAnimation = animations[6];
            turningLeftAnimation = animations[7];
        }

        public MovementAnimations Copy()
        {
            var newMa = new MovementAnimations();

            var allMy = GetAllAnimations();
            var allNew = newMa.GetAllAnimations();

            for (var i = 0; i < allMy.Length; i++)
            {
                var clipConfig = allMy[i];
                if (clipConfig == null)
                    allNew[i] = new ClipConfig();
                else
                    allNew[i] = clipConfig.Copy();
            }

            newMa.AssignAnimations(allNew);
            return newMa;
        }

        public ClipConfig[] GetMovementAnimations()
        {
            var walkingRight = walkingRightAnimation;
            var walkingLeft = walkingLeftAnimation;
            if (walkingLeft.clip != null || walkingRight.clip != null)
            {
                if (walkingRight.clip == null)
                    walkingRight = new ClipConfig(walkingLeftAnimation.clip, walkingLeftAnimation.animSpeed, !walkingLeftAnimation.mirror);
                if (walkingLeft.clip == null)
                    walkingLeft = new ClipConfig(walkingRightAnimation.clip, walkingRightAnimation.animSpeed, !walkingRightAnimation.mirror);
            }

            var anims = new[]
            {
                idleAnimation,
                walkingAnimation,
                walkingBackAnimation,
                walkingRight, walkingLeft,
                runningAnimation
            };
            return anims;
        }

        public ClipConfig[] GetRotatingAnimations()
        {
            var turningLeft = turningLeftAnimation;
            var turningRight = turningRightAnimation;
            if (turningLeft.clip != null || turningRight.clip != null)
            {
                if (turningRight.clip == null)
                    turningRight = new ClipConfig(turningLeftAnimation.clip, turningLeftAnimation.animSpeed, !turningLeftAnimation.mirror);
                if (turningLeft.clip == null)
                    turningLeft = new ClipConfig(turningRightAnimation.clip, turningRightAnimation.animSpeed, !turningRightAnimation.mirror);
            }

            var anims = new[]
            {
                turningLeft,
                idleAnimation,
                turningRight
            };

            return anims;
        }
    }
}