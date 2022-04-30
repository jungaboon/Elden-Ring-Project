// Created by Ronis Vision. All rights reserved
// 05.03.2021.

using System;
using RVModules.RVLoadBalancer;
using RVModules.RVUtilities;
using UnityEngine;
using UnityEngine.Events;

namespace RVModules.RVCommonGameLibrary.Pooling
{
    /// <summary>
    /// 
    /// </summary>
    public class PoolableGameObject : MonoBehaviour, IPoolable
    {
        #region Fields

        public UnityEvent unityOnSpawn;
        public UnityEvent unityOnDespawn;

        public Transform parentOnDespawn;

        [Tooltip("Will automatically fill below component references using GetComponent")]
        public bool automaticallySetComponents = true;

        [Tooltip("Clears trail renderer on despawn to avoid artefacts")]
        public TrailRenderer trailRenderer;

        [Tooltip("Sets kinematic to true on despawn, and false on despawn, to reset velocities")]
        public new Rigidbody rigidbody;

        [Tooltip("Stops playing on despawn")]
        public AudioSource audioSource;

        [Tooltip("Stops and clears particle system on despawn")]
        public new ParticleSystem particleSystem;

        [SerializeField]
        private bool disableGameObjectOnDespawn = true;

        [SerializeField]
        private bool despawnWhenNotPlayingAudio;

        [SerializeField]
        private bool setParentOnDespawn;

        private Action<float> waitForAudioSourceToStoPlaying;

        #endregion

        #region Properties

        public Action OnSpawn { get; set; }
        public Action OnDespawn { get; set; }

        #endregion

        #region Not public methods

        protected virtual void Awake()
        {
            if (automaticallySetComponents)
            {
                trailRenderer = GetComponent<TrailRenderer>();
                rigidbody = GetComponent<Rigidbody>();
                audioSource = GetComponent<AudioSource>();
                particleSystem = GetComponent<ParticleSystem>();
            }

            OnSpawn += () =>
            {
                unityOnSpawn.Invoke();
                if (disableGameObjectOnDespawn) gameObject.SetActive(true);
            };

            if (setParentOnDespawn) OnDespawn += () => transform.SetParent(parentOnDespawn);

            if (trailRenderer) OnDespawn += () => trailRenderer.Clear();
            if (rigidbody)
            {
                OnDespawn += () => rigidbody.isKinematic = true;
                OnSpawn += () => rigidbody.isKinematic = false;
            }

            if (audioSource)
            {
                waitForAudioSourceToStoPlaying = WaitForAudioSourceToStoPlaying;
                OnSpawn += () => { LB.Register(this, waitForAudioSourceToStoPlaying, 10); };
                OnDespawn += () => { audioSource.Stop(); };
            }

            if (particleSystem)
                OnDespawn += () =>
                {
                    particleSystem.Stop(true);
                    particleSystem.Clear(true);
                };

            OnDespawn += () =>
            {
                unityOnDespawn.Invoke();
                if (disableGameObjectOnDespawn) gameObject.SetActive(false);
            };
        }

        private void WaitForAudioSourceToStoPlaying(float _dt)
        {
            if (audioSource.isPlaying) return;
            LB.Unregister(this, waitForAudioSourceToStoPlaying);
            OnDespawn();
        }

        protected virtual void OnDestroy()
        {
            LB.Unregister(this);
        }

        #endregion
    }
}