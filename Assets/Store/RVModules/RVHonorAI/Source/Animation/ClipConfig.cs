// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVHonorAI.Animation
{
    /// <summary>
    /// Unity's AnimationClip configuration class
    /// </summary>
    [Serializable] public class ClipConfig
    {
        #region Fields

        public AnimationClip clip;
        public float animSpeed = 1;
        public bool mirror;

        #endregion

        public ClipConfig() => animSpeed = 1;

        public ClipConfig(AnimationClip _clip)
        {
            clip = _clip;
            animSpeed = 1;
        }

        public ClipConfig(AnimationClip _clip, float _animSpeed, bool _mirror)
        {
            clip = _clip;
            animSpeed = _animSpeed;
            mirror = _mirror;
        }

        #region Public methods

        public ClipConfig Copy() => new ClipConfig(clip, animSpeed, mirror);

        public static implicit operator Motion(ClipConfig cc) => cc.clip;

        public static implicit operator AnimationClip(ClipConfig cc) => cc.clip;

        #endregion
    }

    // todo (non)interruptable single animations
//    [Serializable]
//    public class AdvancedClipConfig : ClipConfig
//    {
//        public bool canBeInterrupted;
//    }
}