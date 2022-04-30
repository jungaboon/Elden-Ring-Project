// Created by Ronis Vision. All rights reserved
// 03.07.2021.

using System;
using RVHonorAI.Combat;
using RVModules.RVCommonGameLibrary.Effects;
using RVModules.RVCommonGameLibrary.Pooling;
using RVModules.RVLoadBalancer;
using RVModules.RVUtilities;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace RVHonorAI
{
    public class Projectile : MonoBehaviour, IProjectile
    {
        #region Public methods

        public void Shoot(Component _shooter, float _damage, Vector3 pos, Vector3 dir, ITarget _target, bool _dmgEnemyOnly)
        {
            dmgEnemyOnly = _dmgEnemyOnly;
            target = _target;
            transform.position = pos;
            var transformRotation = Quaternion.LookRotation(dir);
            transform.rotation = transformRotation;
            if (!guidedMissile) rigidbody.velocity = dir.normalized * speed;
            shooter = _shooter;
            damage = _damage;
            if (guidedMissile) LB.Register(this, GuidedMissile, loadBalancerConfig);
            if (lifeTime > 0) LB.Register(this, LifeTimeTimer, loadBalancerConfig);
            if (trailEffect != null) LB.Register(this, SetParticlePos, loadBalancerConfig);
        }

        #endregion

        #region Fields

        private static LoadBalancerConfig loadBalancerConfig = new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0) { dontRemoveWhenEmpty = true };

        [SerializeField]
        private float speed = 20;

        [SerializeField]
        private float damage;

        [Tooltip("How long before destroying bullet in seconds")]
        [SerializeField]
        private float lifeTime = 10;

        [SerializeField]
        private DamageType damageType;

        [FormerlySerializedAs("particleEffect")]
        [Tooltip("Effect that will spawn and move with projectile")]
        [SerializeField]
        private ParticleEffect trailEffect;

        // spawned particle
        private ParticleEffect spawnedTrail;

        private new Rigidbody rigidbody;
        private TrailRenderer trailRenderer;

        [SerializeField]
        private Component shooter;

        [Tooltip("Will move toward target and always hit it")]
        [SerializeField]
        private bool guidedMissile;

        private ITarget target;

        private new Transform transform;

        /// <summary>
        /// Collision can be null !!
        /// </summary>
        internal Action<Transform, Collision> onHit;

        #endregion

        #region Properties

        Action IPoolable.OnSpawn { get; set; }

        Action IPoolable.OnDespawn { get; set; }

        /// <summary>
        /// Collision can be null !!
        /// </summary>
        public Action<Transform, Collision> OnHit
        {
            get => onHit;
            set => onHit = value;
        }

        #endregion

        #region Not public methods

        private void Awake()
        {
            transform = base.transform;
            rigidbody = GetComponent<Rigidbody>();
            trailRenderer = GetComponent<TrailRenderer>();
            if (trailEffect != null) trailEffect.gameObject.CreatePoolIfDoesntExist();

            if (rigidbody == null)
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.useGravity = false;
            }

            ((IPoolable)this).OnSpawn += () =>
            {
                if (trailEffect != null)
                    spawnedTrail = trailEffect.GetFromPool();
                // doesnt work/fix anything anyway...
//                    foreach (var particleParticleSystem in particle.ParticleSystems) particleParticleSystem.Clear(true);

                gameObject.SetActive(true);
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.isKinematic = guidedMissile;
                actualLifeTime = 0;
            };
            ((IPoolable)this).OnDespawn += () =>
            {
                LB.Unregister(this);

                if (trailRenderer) trailRenderer.Clear();
                if (spawnedTrail != null)
                {
                    foreach (var particleParticleSystem in spawnedTrail.ParticleSystems)
                        particleParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    spawnedTrail = null;
                }

                gameObject.SetActive(false);
                onHit = null;
            };
        }

        private float actualLifeTime;

        private void LifeTimeTimer(float _dt)
        {
            actualLifeTime += UnityTime.DeltaTime;
            if (actualLifeTime > lifeTime) ((IPoolable)this).OnDespawn?.Invoke();
        }

        protected virtual void OnDestroy() => LB.Unregister(this);

        private void OnCollisionEnter(Collision other) => Hit(other.transform);

        private bool dmgEnemyOnly;

        private void Hit(Transform other, Collision _collision = null)
        {
            onHit?.Invoke(other, _collision);

            if (dmgEnemyOnly && shooter != null)
            {
                var otherRel = other.transform.GetComponent<IRelationship>();
                var ourRel = shooter.GetComponent<IRelationship>();
                if (otherRel != null && ourRel != null)
                    if (otherRel.IsEnemy(ourRel))
                    {
                        ((IPoolable)this).OnDespawn?.Invoke();
                        return;
                    }
            }

            var damageable = other.transform.GetComponent<IDamageable>();
            if (damageable == null)
            {
                ((IPoolable)this).OnDespawn?.Invoke();
                return;
            }

            damageable.ReceiveDamage(damage, shooter, damageType);
            ((IPoolable)this).OnDespawn?.Invoke();
        }

        private void SetParticlePos(float dt) => spawnedTrail.transform.position = transform.position;

        private void GuidedMissile(float dt)
        {
            if (target as Object == null)
            {
                ((IPoolable)this).OnDespawn?.Invoke();
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, target.AimTransform.position, speed * UnityTime.DeltaTime);
            if (Vector3.Distance(transform.position, target.AimTransform.position) < .1f) Hit(target.Transform);
        }

        #endregion
    }
}