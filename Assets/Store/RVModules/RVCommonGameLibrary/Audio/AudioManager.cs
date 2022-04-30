// Created by Ronis Vision. All rights reserved
// 08.08.2020.

using RVModules.RVCommonGameLibrary.Gameplay;
using RVModules.RVCommonGameLibrary.Pooling;
using RVModules.RVLoadBalancer;
using RVModules.RVUtilities;
using UnityEngine;
using UnityEngine.Events;

namespace RVModules.RVCommonGameLibrary.Audio
{
    /// <summary>
    /// todo class description and api comments
    /// todo add overloads with array of audioclips
    /// not to self: if there will ever again be sound artifacts: make sure it's not doppler effect messing them because of 'teleporting' of audio source
    /// objects
    /// </summary>
    public class AudioManager : MonoSingleton<AudioManager>
    {
        #region Fields

//        [SerializeField]
//        private PoolManager poolManager;

        public AudioSourceUnityEvent onAudioPlay;

        [SerializeField]
        [Tooltip("Is set to true pitch will be multiplied by current timescale to simulate faster/slower audio playback")]
        private bool timescaleAffectsPitch = true;

        [SerializeField]
        [Tooltip("Renaming can cause some garbage since its operation on string")]
        private bool renameGameObjectsToClipName = false;

        [SerializeField]
        private GameObject audioSourcePrefab;

        /// <summary>
        /// Is set to true pitch will be multiplied by current timescale to simulate faster/slower audio playback
        /// </summary>
        public bool TimescaleAffectsPitch
        {
            get => timescaleAffectsPitch;
            set => timescaleAffectsPitch = value;
        }

        public GameObject AudioSourcePrefab => audioSourcePrefab;

        protected override void SingletonInitialization()
        {
            //if (poolManager == null) poolManager = PoolManager.Instance;
            if (audioSourcePrefab == null) audioSourcePrefab = Resources.Load<GameObject>("AudioSource");
            audioSourcePrefab.CreatePoolIfDoesntExist();
            if(onAudioPlay == null) onAudioPlay = new AudioSourceUnityEvent();
        }

        #endregion

        #region Public methods

        // position overloads
        public AudioSource PlaySound(Vector3 position, SoundConfig soundConfig) =>
            PlaySoundInt(position, soundConfig);

        public AudioSource PlaySound(Vector3 position, SoundConfig soundConfig, AudioSource audioSource, bool overrideCurrentlyPlaying) =>
            PlaySoundInt(position, soundConfig, audioSource, overrideCurrentlyPlaying);

        public AudioSource PlaySound(Vector3 position, AudioClip clip, float range, float volume = 1, float pitch = 1) =>
            PlaySoundInt(position, clip, range, volume: volume, pitch: pitch);

        public AudioSource PlaySound(Vector3 position, AudioClip clip, float range, AudioSource audioSource,
            bool overrideCurrentlyPlaying, float volume = 1, float pitch = 1) =>
            PlaySoundInt(position, clip, range, audioSource, overrideCurrentlyPlaying, volume, pitch);


        // transform overloads
        public AudioSource PlaySound(Transform parent, SoundConfig soundConfig) => PlaySoundInt(parent, soundConfig);

        public AudioSource PlaySound(Transform parent, SoundConfig soundConfig, AudioSource audioSource, bool overrideCurrentlyPlaying) =>
            PlaySoundInt(parent, soundConfig, audioSource, overrideCurrentlyPlaying);

        public AudioSource PlaySound(Transform parent, AudioClip clip, float range, float volume = 1, float pitch = 1) =>
            PlaySoundInt(parent, clip, range, volume, pitch);

        public AudioSource PlaySound(Transform parent, AudioClip clip, float range, AudioSource audioSource,
            bool overrideCurrentlyPlaying, float volume = 1, float pitch = 1) =>
            PlaySoundInt(parent, clip, range, volume, pitch, audioSource, overrideCurrentlyPlaying);

        // non spatial overloads
        public AudioSource PlayNonSpatialSound(AudioClip clip, float volume = 1, float pitch = 1) =>
            PlayNonSpatialSoundInt(clip, volume, pitch);

        public AudioSource PlayNonSpatialSound(AudioClip clip, AudioSource audioSource, bool overrideCurrentlyPlaying, float volume = 1, float pitch = 1) =>
            PlayNonSpatialSoundInt(clip, volume, pitch, audioSource, overrideCurrentlyPlaying);

        #endregion

        #region Not public methods

        private AudioSource PlaySoundInt(Vector3 position, SoundConfig soundConfig, AudioSource audioSource = null, bool overrideCurrentlyPlaying = false) =>
            PlaySoundInt(null, position, soundConfig, audioSource, overrideCurrentlyPlaying);

        private AudioSource PlaySoundInt(Transform parent, SoundConfig soundConfig, AudioSource audioSource = null,
            bool overrideCurrentlyPlaying = false) =>
            PlaySoundInt(parent, Vector3.zero, soundConfig, audioSource, overrideCurrentlyPlaying);

        private AudioSource PlaySoundInt(Transform parent, Vector3 position, SoundConfig soundConfig, AudioSource audioSource = null,
            bool overrideCurrentlyPlaying = false)
        {
            if (soundConfig == null) return audioSource;
            if (soundConfig.audioClips.Length == 0) return audioSource;

            var audioClip = GetAudioClipConfig(soundConfig, out var pitchMax, out var pitchMin, out var volume, out var range);
            if (audioClip == null) return null;

            var pitch = pitchMin + Random.Range(0, pitchMax - pitchMin);

            var aS = PlaySoundInt(parent, position, audioClip, range, volume, pitch, audioSource, overrideCurrentlyPlaying);

            return aS;
        }

        private AudioSource PlaySoundInt(Vector3 position, AudioClip clip, float range, AudioSource audioSource = null,
            bool overrideCurrentlyPlaying = false, float volume = 1, float pitch = 1) =>
            PlaySoundInt(null, position, clip, range, volume, pitch, audioSource, overrideCurrentlyPlaying);

        private AudioSource PlaySoundInt(Transform parent, AudioClip clip, float range, float volume = 1, float pitch = 1, AudioSource audioSource = null,
            bool overrideCurrentlyPlaying = false) => PlaySoundInt(parent, Vector3.zero, clip, range, volume, pitch, audioSource, overrideCurrentlyPlaying);

        private AudioSource PlaySoundInt(Transform parent, Vector3 pos, AudioClip clip, float range, float volume = 1, float pitch = 1,
            AudioSource audioSource = null,
            bool overrideCurrentlyPlaying = false)
        {
            if (clip == null) return null;

            var newSoundGo = false;

            PoolableGameObject poolableAudioSource = null;
            if (audioSource == null)
            {
                // create new GO with audio source component and config it
                poolableAudioSource = GetAudioSourceGo(parent, clip, out newSoundGo);
                audioSource = poolableAudioSource.audioSource;
                if (parent == null) poolableAudioSource.transform.position = pos;
            }
            else if (audioSource.isPlaying && !overrideCurrentlyPlaying)
            {
                return audioSource;
            }

            audioSource.clip = clip;
            audioSource.maxDistance = range;
            audioSource.volume = volume;
            if (TimescaleAffectsPitch) pitch *= Time.timeScale;
            audioSource.pitch = pitch;
            audioSource.Play();

            onAudioPlay.Invoke(audioSource);
            
            return audioSource;
        }

        private AudioSource PlayNonSpatialSoundInt(AudioClip clip, float volume = 1, float pitch = 1, AudioSource audioSource = null,
            bool overrideCurrentlyPlaying = false)
        {
            var aS = PlaySoundInt(null, clip, 1, volume, pitch, audioSource, overrideCurrentlyPlaying);
            aS.spatialize = false;
            return aS;
        }

        private PoolableGameObject GetAudioSourceGo(Transform parent, AudioClip audioClip, out bool newSoundGo)
        {
            //poolManager.TryGetObject("AudioSource", out PoolableGameObject poolableAudioSource);
            audioSourcePrefab.TryGetFromPool(out PoolableGameObject poolableAudioSource);
            var soundGo = poolableAudioSource.gameObject;

            if (renameGameObjectsToClipName) soundGo.name = $"sound - {audioClip.name}";

            if (parent != null)
            {
                soundGo.transform.SetParent(parent);
                soundGo.transform.localPosition = Vector3.zero;
            }

            // use audio source prefabs instead
            //var audioSourceLocal = poolableAudioSource.audioSource;
            //SetupNewAudioSource(audioSourceLocal);

            newSoundGo = true;
            return poolableAudioSource;
        }

//        protected virtual void SetupNewAudioSource(AudioSource audioSourceLocal)
//        {
//            //audioSourceLocal.rolloffMode = AudioRolloffMode.Linear;
//            //audioSourceLocal.spatialBlend = 1;
//        }

//        private void WaitForSoundEnd(PoolableGameObject audioSource)
//        {
//            if (audioSource.audioSource.isPlaying) return;
//            audioSource.OnDespawn();
//            LoadBalancerSingleton.Instance.Unregister(audioSource.audioSource);
//        }

        private AudioClip GetAudioClipConfig(SoundConfig soundConfig, out float pitchMax, out float pitchMin, out float volume, out float range)
        {
            var audioClips = soundConfig.audioClips;
            pitchMax = soundConfig.pitchMax;
            pitchMin = soundConfig.pitchMin;
            volume = soundConfig.volume;
            range = soundConfig.range;

            if (audioClips.Length == 0) return null;

            var audioClip = audioClips[Random.Range(0, audioClips.Length)];
            return audioClip;
        }

        #endregion
    }
}