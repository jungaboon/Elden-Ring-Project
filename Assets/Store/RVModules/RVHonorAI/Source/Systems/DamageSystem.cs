// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;

namespace RVHonorAI.Systems
{
    [CreateAssetMenu] public class DamageSystem : ScriptableObject
    {
        #region Fields

        [SerializeField]
        protected float armorMultiplier = 1;

        [SerializeField]
        protected float damageMultiplier = 1;

        #endregion

        #region Public methods

        /// <summary>
        /// Deals damage to <paramref name="_damageReceiver"/>
        /// Will reduce <paramref name="_damageReceiver"/> health if it implements IHealth
        /// </summary>
        /// <param name="_damageReceiver">Damage receiving party</param>
        /// <param name="_damageSource">Damage source</param>
        /// <param name="_damage">Attack value</param>
        /// <param name="_armor">Receiving party armor</param>
        /// <param name="_damageType">Damage type</param>
        /// <param name="_hitPoint">Point of hit in world space</param>
        /// <param name="_hitForce">Hit force, can be used for physx etc</param>
        /// <param name="forceRadius">Hit radius, can be used for physx etc</param>
        /// <param name="_damageEnemyOnly">Will check for IRelationship on both dmg source and receiver and check if their enemy. If not, will not deal dmg</param>
        /// <returns>Actual dealth damage value</returns>
        public virtual float ReceiveDamage(IHitPoints _damageReceiver, Object _damageSource, float _damage, float _armor, DamageType _damageType,
            bool _damageEnemyOnly = false, Vector3 _hitPoint = default, Vector3 _hitForce = default, float forceRadius = default)
        {
            if (_damageEnemyOnly)
            {
                var receiverRel = (_damageReceiver as Component).GetComponent<IRelationship>();
                var dmgSourceRel = (_damageSource as Component).GetComponent<IRelationship>();
                if (receiverRel != null && dmgSourceRel != null)
                {
                    if (!receiverRel.IsEnemy(dmgSourceRel)) return 0;
                }
            }

            var dmg = CalculateReceivedDamage(_damage, _armor, _damageType);
            if (dmg > 0) _damageReceiver.HitPoints -= _damage;
            return dmg;
        }

        #endregion

        #region Not public methods

        protected virtual float CalculateReceivedDamage(float _damage, float _armor, DamageType _damageType)
        {
            var dmg = Random.Range(_damage * .5f, _damage) * damageMultiplier;
            if (_damageType == DamageType.Physical) dmg -= Random.Range(0, _armor * armorMultiplier);
            return dmg;
        }

        #endregion
    }
}