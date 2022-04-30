// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVCommonGameLibrary.Audio;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVHonorAI.Combat
{
    /// <summary>
    /// Works as kind of weapon modifier
    /// </summary>
    public class Attack : MonoBehaviour, IAttack
    {
        #region Fields

        [SerializeField]
        protected new string name;

        [SerializeField]
        protected AnimationClip animationClip;
        
        [SerializeField]
        private bool damageOnlyEnemies;

        [ConditionalHide(nameof(setDurationAndTimeFromAnimationClip), hideInInspector = true, inverse = true)]
        [Tooltip("Time at which this attack will deal damage." +
                 "Used only if not using animation attack events")]
        [SerializeField]
        protected float damageTime;

        [Tooltip("Damage time and attack duration will be set automatically from animation clip and its Attack event")]
        [SerializeField]
        protected bool setDurationAndTimeFromAnimationClip = true;

        [Header("If set to 0, will use weapon stat with multiplier")]
        [SerializeField]
        protected float damageOverride;

        [SerializeField]
        protected float rangeOverride;

        [ConditionalHide(nameof(setDurationAndTimeFromAnimationClip), hideInInspector = true, inverse = true)]
        [SerializeField]
        protected float attackDurationOverride;

        [SerializeField]
        protected float attackAngleOverride;

        //        [Range(0, 5)]
        [ConditionalHide(nameof(setDurationAndTimeFromAnimationClip), hideInInspector = true, inverse = true)]
        [SerializeField]
        protected float attackDurationMultiplier = 1;
        
        [SerializeField]
        [Range(0, 5)]
        protected float damageMultiplier = 1,
            rangeMultiplier = 1,
            attackAngleMultiplier = 1;

        [Tooltip("Overall multiplier for chance of selecting attack")]
        [SerializeField]
        [Range(0, 5)]
        protected float preference = 1;

        [Header("If null, will use weapon sounds")]
        [SerializeField]
        protected SoundConfig attackSound;

        [SerializeField]
        protected SoundConfig hitSound;

        protected IWeapon weapon;

        protected Vector3 attackTranslation;


        #endregion

        #region Properties

        public bool DamageOnlyEnemies => damageOnlyEnemies;

        public virtual string Name => string.IsNullOrEmpty(name) ? base.name : name;

        public virtual AnimationClip AnimationClip
        {
            get => animationClip;
            set
            {
                animationClip = value;
                UpdateAnimationBasedStats();
            }
        }

        public virtual IWeapon Weapon => weapon;
        public virtual float Damage => damageOverride == 0 ? weapon.Damage * damageMultiplier : damageOverride;
        public virtual float DamagePerSecond => Damage / AttackDuration;
        public virtual float DamageTime => damageTime;
        public virtual float Preference => preference;
        public virtual float DamageRange => rangeOverride == 0 ? weapon.Range * rangeMultiplier : rangeOverride;
        public virtual float EngageRange => DamageRange + AttackTranslation.z;
        public virtual float PreferredEngageRange => EngageRange;
        public virtual float AttackDuration => attackDurationOverride == 0 ? weapon.AttackDuration * attackDurationMultiplier : attackDurationOverride;
        public virtual float AttackAngle => attackAngleOverride == 0 ? weapon.AttackAngle * attackAngleMultiplier : attackAngleOverride;

        public virtual AttackType AttackType => weapon.AttackType;
        public virtual SoundConfig AttackSound => attackSound == null ? weapon.AttackSound : attackSound;
        public virtual SoundConfig HitSound => hitSound == null ? weapon.HitSound : hitSound;
        public virtual float HitForce => 0;

        public virtual DamageType DamageType => weapon.DamageType;

        /// <summary>
        /// How much translation (movement, in local space) is expected before dealing damage event is called
        /// Used to calculate engage range 
        /// </summary>
        public virtual Vector3 AttackTranslation => attackTranslation;

        #endregion

        #region Not public methods

        protected virtual void Awake()
        {
            weapon = GetComponent<IWeapon>();
            if (weapon == null) Debug.LogError("Attack with no weapon!", this);
            if (animationClip != null) UpdateAnimationBasedStats();
        }

        protected void UpdateAnimationBasedStats()
        {
            if (animationClip == null) return;
            
            var events = animationClip.events;
            AnimationEvent atkEvent = null;
            foreach (var animationEvent in events)
                if (animationEvent.functionName == "Attack" || animationEvent.functionName == "Shoot")
                    atkEvent = animationEvent;

            var atkTime = atkEvent?.time ?? animationClip.averageDuration;

            if (setDurationAndTimeFromAnimationClip)
            {
                if (atkEvent == null) damageTime = atkTime * .7f;
                else damageTime = atkTime;
                attackDurationOverride = animationClip.averageDuration;
            }

            attackTranslation = AnimationClip.averageSpeed * DamageTime;

//                Debug.Log($"atk time {atkTime}, duration {animationClip.averageDuration}, atkTranslation {attackTranslation}");
        }

        #endregion
    }
}