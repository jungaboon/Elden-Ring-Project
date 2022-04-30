// Created by Ronis Vision. All rights reserved
// 17.02.2021.

using System;
using RVModules.RVLoadBalancer;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVCommonGameLibrary.Effects
{
    /// <summary>
    /// Allows pooling particle effects
    /// Usage: add this component at root of effect prefab and use pooling API to create pool and spawn it, it'll despawn automatically after finished playing
    /// </summary>
    public class ParticleEffect : MonoBehaviour, IPoolable
    {
        #region Fields

        private ParticleSystem[] particleSystems;

        #endregion

        #region Properties

        public Action OnSpawn { get; set; }
        public Action OnDespawn { get; set; }

        public ParticleSystem[] ParticleSystems => particleSystems;

        #endregion

        #region Not public methods

        private void Awake()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            OnSpawn += () =>
            {
                gameObject.SetActive(true);
                LoadBalancerSingleton.Instance.Register(this, CheckIfStillAlive, 10);
                foreach (var system in ParticleSystems) system.Play(true);
            };
            OnDespawn += () =>
            {
                LoadBalancerSingleton.Instance.Unregister(this);

                foreach (var system in ParticleSystems)
                {
                    system.Stop(true);
                    system.Clear(true);
                }

                gameObject.SetActive(false);
            };
        }

        private void OnDestroy()
        {
            if (LoadBalancerSingleton.Instance == null) return;
            LoadBalancerSingleton.Instance.Unregister(this);
        }

        private void CheckIfStillAlive(float _dt)
        {
            foreach (var system in ParticleSystems)
                if (system.IsAlive(true))
                    return;

            OnDespawn.Invoke();
        }

        #endregion
    }
}