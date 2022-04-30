// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;

namespace RVHonorAI.Systems
{
    public class MeleeAttackSystem : ScriptableObject
    {
        #region Public methods

        public virtual void MeleeAttack(Character _character)
        {
            var target = _character.CharacterAi.Target;

            _character.PlayWeaponAttackSound();

            if (_character.transform == null || _character == null || target as Object == null) return;

            var currAtk = _character.CurrentAttack;
            if (currAtk == null) return;

            if (Character.Distance(_character, target) > currAtk.DamageRange) return;
            //if (_character.IsTargetInAttackRange(target)) return;

            var damageable = target as IDamageable;
            float dmg = 0;
            if (damageable != null)
            {
                var damageType = DamageType.Physical;
                if (_character.CurrentWeapon != null) damageType = _character.CurrentWeapon.DamageType;

                Vector3 hitForce = default;
                if (currAtk.HitForce != 0)
                {
                    var hitDir = target.AimTransform.position - _character.transform.position;
                    hitDir.y = 0;
                    hitForce = hitDir.normalized * currAtk.HitForce;
                }

                dmg = damageable.ReceiveDamage(_character.CurrentAttack.Damage, _character, damageType, currAtk.DamageOnlyEnemies,
                    currAtk.Weapon.Transform.position, hitForce, 1);
            }

            // for some reason sometimes this (Character component) doesnt exist, but this code still executes
            if (_character == null) return;
            if (dmg > 0) _character.PlayAttackHitSound();

            _character.OnAttackAction?.Invoke(damageable, dmg);
            _character.OnAttack?.Invoke();
        }

        #endregion
    }
}