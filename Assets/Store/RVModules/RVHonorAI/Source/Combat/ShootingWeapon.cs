// Created by Ronis Vision. All rights reserved
// 03.07.2021.

using System;
using RVModules.RVCommonGameLibrary.Audio;
using RVModules.RVCommonGameLibrary.Effects;
using RVModules.RVCommonGameLibrary.Pooling;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RVHonorAI.Combat
{
    public class ShootingWeapon : Weapon
    {
        #region Fields

        public UnityEvent onShoot;
        public GameObject projectilePrefab;

        [SerializeField]
        protected ShootingWeaponType shootingWeaponType;

        [SerializeField]
        protected LayerMask raycastShootingHitMask;

        [Tooltip("Relevant only for prefab shooting type, in meters")]
        [SerializeField]
        protected float dispersionAt1km;

        /// <summary>
        /// Applicable for raycast shooting type only
        /// </summary>
        [SerializeField]
        protected float baseHitChance = 100;

        [Header("Effects")]
        [SerializeField]
        private GameObject hitEffect;

//        [Tooltip("If true, hit effect will show on target's aim point(usually head), otherwise at target's root transform")]
//        [SerializeField]
//        private bool hitEffectOnAimPoint;

        [SerializeField]
        private HitEffectPlacement hitEffectPlacement = HitEffectPlacement.AimPosition;

        [SerializeField]
        private Vector3 hitEffectPositionOffset;

        [SerializeField]
        private GameObject shootEffect;

        [SerializeField]
        private Transform shootEffectPositionTransform;

        [Tooltip("If true, shoot effect will be parented under shootEffectPositionTransform")]
        [SerializeField]
        private bool parentShootEffect;

        private Action<Transform, Collision> onHit;

        [SerializeField]
        private Transform projectileSpawnParent;

        #endregion

        #region Properties

        public ShootingWeaponType ShootingWeaponType => shootingWeaponType;

        public override AttackType AttackType => AttackType.Shooting;
        public Transform ProjectileSpawnParent => projectileSpawnParent;

        #endregion

        #region Public methods

        private bool dmgEnemyOnly;

        public void Shoot(Component _shooter, ITarget target, float _damage, bool _dmgEnemyOnly)
        {
            dmgEnemyOnly = _dmgEnemyOnly;
            var targetPos = target.AimTransform.position;

            switch (shootingWeaponType)
            {
                case ShootingWeaponType.Raycast:
                    ShootRaycast(_shooter, target, _damage);
                    break;
                case ShootingWeaponType.ImmediateHit:
                    ShootAlwaysHit(_shooter, target, _damage);
                    break;
                case ShootingWeaponType.Prefab:
                    ShootPrefab(_shooter, targetPos, target, _damage);
                    break;
            }

            if (AttackSound != null) AudioManager.Instance.PlaySound(transform.position, AttackSound);
            if (shootEffect != null && shootEffect.TryGetFromPool(out ParticleEffect shootFx))
            {
                shootFx.transform.SetPositionAndRotation(shootEffectPositionTransform.position, shootEffectPositionTransform.rotation);
                if (parentShootEffect) shootFx.transform.SetParent(shootEffectPositionTransform, true);
            }

            onShoot?.Invoke();
        }

        #endregion

        #region Not public methods

        protected override void Start()
        {
            base.Start();
            onHit = (_transform, _collision) =>
            {
                if (_collision != null)
                {
                    if (_collision.contactCount > 0)
                    {
                        var contactPoint = _collision.GetContact(0);
                        CreateHitFx(_collision.transform, contactPoint.point, contactPoint.normal);
                    }
                    else
                    {
                        CreateHitFx(_transform, _transform.position, _transform.forward);
                    }
                }
                else
                {
                    CreateHitFx(_transform, _transform.position, _transform.forward);
                }
            };

            // make sure spawned crap is added to the pooling system
            if (hitEffect != null) hitEffect.CreatePoolIfDoesntExist();
            if (shootEffect != null) shootEffect.CreatePoolIfDoesntExist();
            if (projectilePrefab != null) projectilePrefab.CreatePoolIfDoesntExist();
        }

        protected override void Awake()
        {
            base.Awake();
            if (projectileSpawnParent == null) projectileSpawnParent = transform;
        }

        protected virtual void ShootRaycast(Component _shooter, ITarget target, float _damage)
        {
            var rayOrigin = ProjectileSpawnParent.transform.position;
            var targetPos = target.AimTransform.position;
            var dir = targetPos - rayOrigin;

            if (Random.Range(0f, 100f) > baseHitChance) return;
            if (Physics.Raycast(rayOrigin, dir, out var hit, Range, raycastShootingHitMask))
            {
                if (hitEffectPlacement == HitEffectPlacement.HitPosition) CreateHitFx(hit.transform, hit.point, hit.normal);

                var hitTarget = hit.transform.GetComponent<ITarget>();
                if (hitTarget == null || hitTarget != target) return;

                if (hitEffectPlacement != HitEffectPlacement.HitPosition) CreateHitFx(target.Transform, hit.point, hit.normal);

                if (!(target is IDamageable damageable)) return;

                damageable.ReceiveDamage(_damage, _shooter, damageType);
            }
        }

        protected virtual void ShootAlwaysHit(Component _shooter, ITarget target, float _damage)
        {
            var damageable = target as IDamageable;

            if (Random.Range(0f, 100f) > baseHitChance) return;

            var rayOrigin = ProjectileSpawnParent.transform.position;

            if (hitEffectPlacement == HitEffectPlacement.HitPosition)
            {
                var targetPos = target.AimTransform.position;
                var dir = targetPos - rayOrigin;
                if (Physics.Raycast(rayOrigin, dir, out var hit, Range, raycastShootingHitMask))
                    CreateHitFx(target.Transform, hit.point, hit.normal);
                else
                    CreateHitFx(target.Transform, target.Transform.position, target.Transform.position - rayOrigin);
            }
            else
            {
                CreateHitFx(target.Transform, target.Transform.position, target.Transform.position - rayOrigin);
            }

            if (damageable == null) return;
            damageable.ReceiveDamage(_damage, _shooter, damageType);
        }

        protected virtual void ShootPrefab(Component _shooter, Vector3 targetPos, ITarget _target, float _damage)
        {
            var projectile = projectilePrefab.GetFromPool<IProjectile>();

            if (projectile == null)
            {
                Debug.LogError($"There's no projectile '{projectilePrefab.name}' added to pool manager!", gameObject);
                return;
            }

            projectile.OnHit = onHit;
            var position = ProjectileSpawnParent.transform.position;
            var dir = (targetPos - position).normalized * 1000 + Random.insideUnitSphere * dispersionAt1km;
            dir = dir.normalized;
            position += dir.normalized * .2f;

            projectile.Shoot(_shooter, _damage, position, dir, _target, dmgEnemyOnly);
        }

        protected virtual void CreateHitFx(Transform _transform, Vector3 hitPoint, Vector3 hitNormal, ITarget _target = null)
        {
            if (hitEffect == null) return;

            var effectPos = _transform.position;
            var effectRot = _transform.forward;

            if (_target == null) _target = _transform.GetComponent<ITarget>();

            switch (hitEffectPlacement)
            {
                case HitEffectPlacement.AimPosition:
                    if (_target as Object != null) effectPos = _target.AimTransform.position;
                    break;
                case HitEffectPlacement.HitPosition:
                    effectPos = hitPoint;
                    effectRot = hitNormal;
                    break;
            }

            effectPos += hitEffectPositionOffset;

            PoolManager.Instance.TryGetObject(hitEffect.name, out ParticleEffect hitFx);

            hitFx.transform.position = effectPos;
            hitFx.transform.rotation = Quaternion.LookRotation(effectRot);
        }

        #endregion
    }

    public enum ShootingWeaponType
    {
        Prefab,
        Raycast,
        ImmediateHit
    }

    public enum HitEffectPlacement
    {
        RootTransform,
        AimPosition,
        HitPosition
    }
}