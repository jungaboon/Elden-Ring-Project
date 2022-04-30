// Created by Ronis Vision. All rights reserved
// 07.08.2020.

using System;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Audio
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "CommonGameLibrary/Sound config")] [Serializable]
    public class SoundConfig : ScriptableObject
    {
        #region Fields

        public AudioClip[] audioClips;
        public float range = 40;
        public float volume = 1;
        public float pitchMin = 0.8f;
        public float pitchMax = 1.2f;

        #endregion
    }
}