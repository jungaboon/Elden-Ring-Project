// Created by Ronis Vision. All rights reserved
// 08.08.2020.

using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Audio
{
    /// <summary>
    /// Allows for convenient playing sounds via Unity events (no code)
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private float range;

        [SerializeField]
        private float volume;

        [SerializeField]
        private float pitch;

        #endregion

        #region Public methods

        public void PlaySound(AudioClip _clip) => AudioManager.Instance.PlaySound(transform, _clip, range, audioSource, true, volume, pitch);

        public void PlaySound(SoundConfig soundConfig) => AudioManager.Instance.PlaySound(transform, soundConfig, audioSource, true);

        #endregion

//        #region Fields
//
//        private PoolManager poolManager;
//
//
//
//        #endregion
//
//        #region Public methods
//
////        public void Play(SoundConfig _soundConfig, bool overrideIfCurrentlyPlaying = false, float chanceToPlay = 100)
////        {
////            Play(audioSource.transform.position, _soundConfig, overrideIfCurrentlyPlaying, chanceToPlay);
////        }
//
//        public void Play(Vector3 pos, SoundConfig _soundConfig, AudioSource audioSource = null, bool overrideIfCurrentlyPlaying = false,
//            float chanceToPlay = 100)
//        {
//            var aS = Play(null, _soundConfig, audioSource, overrideIfCurrentlyPlaying, chanceToPlay);
//            if (aS == null) return;
//            aS.transform.position = pos;
//        }
//
//        public GameObject Play(Transform parent, SoundConfig _soundConfig, AudioSource audioSource = null, bool overrideIfCurrentlyPlaying = false,
//            float chanceToPlay = 100)
//        {
//            if (_soundConfig == null) return null;
//
//            var audioClips = _soundConfig.audioClips;
//            var pitchMax = _soundConfig.pitchMax;
//            var pitchMin = _soundConfig.pitchMin;
//            var volume = _soundConfig.volume;
//            var range = _soundConfig.range;
//
//            if (audioClips.Length == 0) return null;
//            if (chanceToPlay < 100 && Random.Range(0, 100) > chanceToPlay) return null;
//            var audioClip = audioClips[Random.Range(0, audioClips.Length - 1)];
//            if (audioClip == null) return null;
//
//            var newSoundGo = false;
//
//            PoolableGameObject poolableAudioSource = null;
//            if (audioSource == null)
//            {
//                // create new GO with audio source component and config it
//                poolableAudioSource = GetAudioSourceGo(parent, audioClip, out newSoundGo);
//                audioSource = poolableAudioSource.audioSource;
//            }
//            else if (audioSource.isPlaying && !overrideIfCurrentlyPlaying)
//            {
//                return audioSource.gameObject;
//            }
//
//            audioSource.clip = audioClip;
//            audioSource.pitch = pitchMin + Random.Range(0, pitchMax - pitchMin);
//            audioSource.volume = volume;
//            audioSource.maxDistance = range;
//            audioSource.Play();
//            if (newSoundGo) LoadBalancerSingleton.Instance.Register(audioSource, _dt => { WaitForSoundEnd(poolableAudioSource); }, 10);
//            return audioSource.gameObject;
//        }
//
//        #endregion
//
//        #region Not public methods
//
//        private void Start() => poolManager = PoolManager.Instance;
//
//        #endregion
    }
}