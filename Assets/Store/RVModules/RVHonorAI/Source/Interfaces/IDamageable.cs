// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;

namespace RVHonorAI
{
    public interface IDamageable
    {
        #region Public methods

        /// <summary>
        /// Returned value should be actual dealt damage (in after armor, etc.)
        /// </summary>
        /// <param name="_damage">Damage amount</param>
        /// <param name="_damageSource">Damage source - who is dealing damage?</param>
        /// <param name="_damageType">Damage type - can be physical or magic</param>
        /// <param name="_damageEnemyOnly"></param>
        /// <param name="hitPoint">Point of impact</param>
        /// <param name="_hitForce">Physical hit force. Can be used for ragdoll deaths etc.</param>
        /// <param name="forceRadius">Physical hit radius. Like hit force, can be used for ragdolls - smaller radius will affect ragdoll more
        /// locally</param>
        /// <returns></returns>
        float ReceiveDamage(float _damage, Object _damageSource, DamageType _damageType, bool _damageEnemyOnly = false, Vector3 hitPoint = default, Vector3 _hitForce = default,
            float forceRadius = default);

        #endregion
    }
}