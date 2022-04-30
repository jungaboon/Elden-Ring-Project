// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using System.Collections.Generic;

namespace RVHonorAI.Animation
{
    [Serializable] public struct SingleAnimations
    {
        public ClipConfig[] attackingAnimations;
        public ClipConfig[] dyingAnimations;
        public ClipConfig[] randomIdleAnimations;
        public ClipConfig[] customAnimations;

        public SingleAnimations Copy()
        {
            var newSa = new SingleAnimations
            {
                attackingAnimations = new ClipConfig[attackingAnimations.Length],
                dyingAnimations = new ClipConfig[dyingAnimations.Length],
                randomIdleAnimations = new ClipConfig[randomIdleAnimations.Length],
                customAnimations = new ClipConfig[customAnimations.Length]
            };

            for (var i = 0; i < attackingAnimations.Length; i++) newSa.attackingAnimations[i] = attackingAnimations[i].Copy();
            for (var i = 0; i < dyingAnimations.Length; i++) newSa.dyingAnimations[i] = dyingAnimations[i].Copy();
            for (var i = 0; i < randomIdleAnimations.Length; i++) newSa.randomIdleAnimations[i] = randomIdleAnimations[i].Copy();
            for (var i = 0; i < customAnimations.Length; i++) newSa.customAnimations[i] = customAnimations[i].Copy();

            return newSa;
        }

        public ClipConfig[] GetAllAnimations()
        {
            var l = new List<ClipConfig>();
            l.AddRange(attackingAnimations);
            l.AddRange(dyingAnimations);
            l.AddRange(randomIdleAnimations);
            l.AddRange(customAnimations);
            return l.ToArray();
        }
    }
}