// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using System.Collections.Generic;
using RVModules.RVCommonGameLibrary.Audio;
using UnityEngine;
using UnityEngine.Serialization;

namespace RVHonorAI.Combat
{
    [Serializable] public class Weapon : MonoBehaviour, IWeapon
    {
        #region Fields

        [FormerlySerializedAs("attack")]
        [SerializeField]
        private float damage = 40;

        [SerializeField]
        private float range = .5f;

        [FormerlySerializedAs("attackInterval")]
        [SerializeField]
        private float attackDuration = 2;

        [SerializeField]
        private float attackAngle = 20;

        [SerializeField]
        private SoundConfig attackSound;

        [SerializeField]
        private SoundConfig hitSound;

        [SerializeField]
        protected DamageType damageType;

        private List<IAttack> attacks = new List<IAttack>();
        private IAttack maxRangeAtk;

        #endregion

        #region Properties

        public Transform Transform => transform;
        public DamageType DamageType => damageType;

        public float Damage => damage;

        public float Range => range;

        public float MaxEngageRange
        {
            get
            {
                if (maxRangeAtk == null) return 0;
                return maxRangeAtk.EngageRange;
            }
        }

        public float AttackDuration => attackDuration;

        public float AttackAngle => attackAngle;

        public virtual AttackType AttackType => AttackType.Melee;

        public virtual List<IAttack> Attacks => attacks;

        public virtual SoundConfig AttackSound => attackSound;

        public virtual SoundConfig HitSound => hitSound;

        #endregion

        #region Not public methods

        protected virtual void Awake()
        {
            attacks.Clear();
            attacks.AddRange(GetComponents<IAttack>());
            if (attacks.Count == 0)
            {
                var defAttack = gameObject.AddComponent<Attack>();
            }
        }
        protected virtual void Start()
        {
            GetAttacks();
        }

        private void GetAttacks()
        {
            attacks.Clear();
            attacks.AddRange(GetComponents<IAttack>());

            if (attacks.Count == 0) return;
            // find attack with the highest range
            float maxRange = 0;
            maxRangeAtk = attacks[0];
            foreach (var attack in attacks)
            {
                if (attack.EngageRange > maxRange)
                {
                    maxRange = attack.EngageRange;
                    maxRangeAtk = attack;
                }
            }
        }

        #endregion
    }

    public enum AttackType
    {
        Melee,
        Shooting
    }
}