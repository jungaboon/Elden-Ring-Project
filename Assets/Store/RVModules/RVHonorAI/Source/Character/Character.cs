// Created by Ronis Vision. All rights reserved
// 14.05.2021.

using System;
using System.Collections.Generic;
using RVHonorAI.Animation;
using RVHonorAI.CharacterInspector;
using RVHonorAI.Combat;
using RVHonorAI.Systems;
using RVHonorAI.Utilities;
using RVModules.RVCommonGameLibrary.Audio;
using RVModules.RVLoadBalancer;
using RVModules.RVLoadBalancer.Tasks;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content;
using RVModules.RVUtilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace RVHonorAI
{
    /// <summary>
    /// Character description
    /// todo unity events with parameters
    /// </summary>
    [DefaultExecutionOrder(-100)] public class Character : MonoBehaviour, ICharacter, IAttackAngle, IAttackRange, IAttackSoundPlayer, IAttacker,
        IJobHandlerProvider, IObjectDetectionCallbacks, ITargetsDetectionCallbacks, IExposeCharInspectorFields
    {
        #region Fields

        public RagdollCreator ragdollCreator;

        [SerializeField]
        public CharacterSounds characterSounds;

        [SerializeField]
        private GotHitUnityEvent onReceivedDamage;

        [SerializeField]
        private GotHitUnityEvent onGotHit;

        [SerializeField]
        private UnityEvent onAttack;

        [SerializeField]
        private float walkingSpeed = 1f;

        [SerializeField]
        private float runningSpeed = 2f;

        [SerializeField]
        private float armor = 20;

        [SerializeField]
        private ICharacterAi characterAi;

        [SerializeField]
        private float health = 100;

        [SerializeField]
        private float maxHealth = 100;

        [SerializeField]
        private bool healthRegeneration;

        [SerializeField]
        [Tooltip("Health regeneration per second")]
        private float healthRegenerationSpeed = 1;

        [Tooltip("Should ragdoll be created when character is killed instaed of playing dying animation")]
        [SerializeField]
        private bool useRagdoll;

        private List<IAttack> availableAttacks = new List<IAttack>();

//        [CharacterInspectorField(CharacterInspectorTab.Combat)]
        private List<IWeapon> availableWeapons = new List<IWeapon>();

        private IAttack currentAttack;

        // [SerializeField]
        // [CharacterInspectorField(drawWhenNotPlaying = false)]
        // private string currentAttackObject;
        //
        // [SerializeField]
        // [CharacterInspectorField(drawWhenNotPlaying = false)]
        // private string currentWeapon;

        [SerializeField]
        protected bool reserveDestinationPosition;

        [SerializeField]
        private bool showGuiInfo;

        [Tooltip("Optional transform that will be aimed for by other characters for shooting etc")]
        [SerializeField]
        private Transform aimTransform;

        [SerializeField]
        private UnityEvent onKilled;

        [SerializeField]
        protected TaskHandler aiJobHandler = new TaskHandler { MaxRunningTasks = 20 };

        [SerializeField]
        private bool removeDead = true;

        [Tooltip("Will remove dead after this many seconds. Applies to ragdoll also")]
        [SerializeField]
        private float removeDeadAfter = 30;

        [SerializeField]
        private bool useSoundPreset;

        [SerializeField]
        private SoundsPreset soundsPreset;

        private AudioSource audioSource;

        private float lastTimeAttack = float.MinValue;

        private IMovementRootMotionHandler movementRootMotionHandler;

        [SerializeField]
        private bool debugObjectDetecionEvents, debutTargetDetectionEvents;

        private AiSystems aiSystems;

        [SerializeField]
        private float radius = .5f;

        [SerializeField]
        [CharacterInspectorField(drawWhenNotPlaying = false)]
        [Tooltip("General strength of enemies divided by allies strength. Value over 1 means enemies are probably stronger and will likely win." +
                 " This value won't be calculated for AI with set 'Never flee'")]
        private float danger;

        #endregion

        #region Properties

        public Transform AimTransform
        {
            get
            {
                if (aimTransform != null) return aimTransform;
                return HeadTransform != null ? HeadTransform : transform;
            }
            set => aimTransform = value;
        }

        public TaskHandler AiJobHandler => aiJobHandler;

        public float MaxHitPoints => maxHealth;

        public UnityEvent OnKilled
        {
            get => onKilled;
            set => onKilled = value;
        }

        /// <summary>
        /// Should ragdoll be created when character is killed instaed of playing dying animation
        /// </summary>
        public bool UseRagdoll => useRagdoll;

        /// <summary>
        /// Value for determining generally how strong and dangerous this char is
        /// </summary>
        public virtual float Danger => HitPoints * .1f * DamagePerSecond * .1f;

        public virtual float Radius => radius;

        public virtual Transform Transform => transform;

        public virtual Transform HeadTransform
        {
            get => CharacterAi.HeadTransform;
            protected set => CharacterAi.HeadTransform = value;
        }

        /// <summary>
        /// Character's health
        /// </summary>
        public virtual float HitPoints
        {
            get => health;
            set => health = Mathf.Clamp(value, 0, maxHealth);
        }

        /// <summary>
        /// Character's AiGroup. Used for detecting relationship to other characters
        /// </summary>
        public virtual AiGroup AiGroup
        {
            get => CharacterAi.AiGroup;
            set => CharacterAi.AiGroup = value;
        }

        public bool TreatNeutralCharactersAsEnemies => CharacterAi.TreatNeutralCharactersAsEnemies;

        public virtual float DamagePerSecond => CurrentAttack == null ? 0 : CurrentAttack.Damage / CurrentAttack.AttackDuration;

        public virtual float Damage => CurrentAttack == null ? 0 : CurrentAttack.Damage;

        public virtual float Armor
        {
            get => armor;
            protected set => armor = value;
        }

        public virtual ICharacterAi CharacterAi
        {
            get => characterAi;
            private set => characterAi = value;
        }

        public virtual float DamageRange => CurrentAttack == null ? 0f : CurrentAttack.DamageRange;

        public virtual float EngageRange => CurrentAttack == null ? 0f : CurrentAttack.EngageRange;

        public virtual float AttackAngle => CurrentAttack == null ? 0 : CurrentAttack.AttackAngle;

//        public Object GetObject => this;

        public virtual bool HealthRegeneration
        {
            get => healthRegeneration;
            protected set => healthRegeneration = value;
        }

        public virtual Transform VisibilityCheckTransform => CharacterAi.HeadTransform == null ? transform : CharacterAi.HeadTransform;

        /// <summary>
        /// Action called when character attacks his target
        /// Second argument is dealth damage
        /// </summary>
        public Action<IDamageable, float> OnAttackAction { get; set; }

        /// <summary>
        /// Used weapon
        /// </summary>
        public IWeapon CurrentWeapon => CurrentAttack?.Weapon;

        /// <summary>
        /// 
        /// </summary>
        public virtual bool UseSoundPreset
        {
            get => useSoundPreset;
            protected set => useSoundPreset = value;
        }

        /// <summary>
        /// Sounds preset
        /// </summary>
        public SoundsPreset SoundsPreset
        {
            get => soundsPreset;
            protected set => soundsPreset = value;
        }

        /// <summary>
        /// Running speed in m/s
        /// </summary>
        public float RunningSpeed
        {
            get => runningSpeed;
            set => runningSpeed = value;
        }

        /// <summary>
        /// Walking speed in m/s
        /// </summary>
        public float WalkingSpeed
        {
            get => walkingSpeed;
            set => walkingSpeed = value;
        }

        /// <summary>
        /// Unity event called when Character deals damage
        /// </summary>
        public UnityEvent OnAttack
        {
            get => onAttack;
            set => onAttack = value;
        }

        /// <summary>
        /// Unity event called when Character gets damage
        /// First argument is damage source, second is suffered damage
        /// </summary>
        public GotHitUnityEvent OnReceivedDamage
        {
            get => onReceivedDamage;
            set => onReceivedDamage = value;
        }

        /// <summary>
        /// Unity event called when Character gets hit
        /// First argument is damage source, second is suffered damage
        /// </summary>
        public GotHitUnityEvent OnGotHit
        {
            get => onGotHit;
            set => onGotHit = value;
        }

        ITarget ITargetProvider.Target => CharacterAi.Target;

        TargetInfo ITargetProvider.CurrentTarget
        {
            get => CharacterAi.CurrentTarget;
            set => CharacterAi.CurrentTarget = value;
        }

        public Action<Object> OnNewObjectDetected { get; set; }
        public Action<Object> OnObjectNotDetectedAnymore { get; set; }
        public Action<ITarget> OnNewTargetDetected { get; set; }
        public Action<ITarget> OnTargetNotSeenAnymore { get; set; }
        public Action<ITarget> OnTargetVisibleAgain { get; set; }
        public Action<ITarget> OnTargetForget { get; set; }

        public virtual IAttack CurrentAttack
        {
            get => currentAttack;
            protected set
            {
                currentAttack = value;
                // currentAttackObject = currentAttack?.Name;
                // currentWeapon = currentAttack?.Weapon.Transform.gameObject.name;
            }
        }

        public virtual ITarget CurrentTarget => characterAi.Target;

        public virtual bool ExposeAllFieldsToCharInspector => false;
        public virtual CharacterInspectorTab DefaultCharInspectorTab => CharacterInspectorTab.General;

        public virtual List<IWeapon> AvailableWeapons => availableWeapons;

        public virtual AudioSource AudioSource
        {
            get => audioSource;
            protected set => audioSource = value;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Distance between two targets, taking into accound radius of both
        /// </summary>
        /// <param name="_first"></param>
        /// <param name="_second"></param>
        /// <returns></returns>
        public static float Distance(ITarget _first, ITarget _second) =>
            Vector3.Distance(_first.Transform.position, _second.Transform.position) - _first.Radius - _second.Radius;

        /// <summary>
        /// Receive set damage. Returned value is actual dealt damage  
        /// </summary>
        public virtual float ReceiveDamage(float _damage, Object _damageSource, DamageType _damageType, bool _damageEnemyOnly, Vector3 _hitPoint = default,
            Vector3 _hitForce = default, float _forceRadius = default)
        {
            if (RandomChance.Get(characterSounds.chanceToPlayGotHitSound))
                AudioManager.Instance.PlaySound(transform.position, characterSounds.gotHitSound, AudioSource, true);

            var receivedDmg = aiSystems.DamageSystem.ReceiveDamage(this, _damageSource, _damage, Armor, _damageType, _damageEnemyOnly, _hitPoint, _hitForce,
                _forceRadius);

            OnGotHit.Invoke(_damageSource, receivedDmg);
            if (receivedDmg > 0) OnReceivedDamage?.Invoke(_damageSource, receivedDmg);

            if (health <= 0)
            {
                Kill(_hitPoint, _hitForce, _forceRadius);
            }
            else
            {
                if (_damageSource is ITarget t && !characterAi.TargetInfosDict.ContainsKey(t)) characterAi.AddTarget(t);
            }

            return receivedDmg;
        }

        /// <summary>
        /// Selects attack using AttackSelectionModule
        /// </summary>
        public virtual void SelectAttack()
        {
            var target = CharacterAi.Target;
            if (availableAttacks.Count == 0)
            {
                CurrentAttack = null;
                return;
            }

            if (target == null || availableAttacks.Count == 1)
            {
                CurrentAttack = availableAttacks[0];
                return;
            }

            var selectedAtk = availableAttacks[0];
            var highestScore = float.MinValue;
            foreach (var atk in availableAttacks)
            {
                var score = aiSystems.AttackSelectionSystem.ScoreAttack(this, CurrentWeapon, atk, target);

                if (score > highestScore)
                {
                    selectedAtk = atk;
                    highestScore = score;
                }
            }

            CurrentAttack = selectedAtk;
//            Debug.Log($"Selected atk: {selectedAtk.Name}");
        }

        /// <summary>
        /// Instantly kills this character, plays die sound and creates ragdoll or play killed animation depending on configuration 
        /// </summary>
        public virtual void Kill() => Kill(Vector3.zero);

        /// <summary>
        /// Instantly kills this character, plays die sound and creates ragdoll or play killed animation depending on configuration 
        /// </summary>
        public virtual void Kill(Vector3 _hitPoint, Vector3 _hitForce = default, float _forceRadius = default)
        {
            onKilled?.Invoke();
            AudioSource.Stop();
            AudioManager.Instance.PlaySound(transform.position, characterSounds.dieSound);

            if (UseRagdoll) KilledRagdoll(_hitPoint, _hitForce, _forceRadius);
            else KilledAnimation();
        }

//        /// <summary>
//        /// Add passed characters to dynamic enemies list, and make him enemy despite group relationship
//        /// todo not implemented!
//        for now theres not dyna enemies. They reast to being attacked by neutral just by adding it to their targets list
//        /// </summary>
//        public void MakeEnemyDynamic(ICharacter _source) => throw new NotImplementedException();

        /// <summary>
        /// Heals set amount of health
        /// </summary>
        /// <param name="_amount">Amount of hp to add</param>
        public virtual void Heal(float _amount) => HitPoints += Mathf.Clamp(_amount, 0, float.MaxValue);

        public virtual bool IsEnemy(IRelationship _other, bool _contraCheck = false) => CharacterAi.IsEnemy(_other, _contraCheck);

        public virtual bool IsAlly(IRelationship _other) => CharacterAi.IsAlly(_other);

        /// <summary>
        /// Finds references using GetComponentInChildren
        /// </summary>
        public virtual void FindReferences()
        {
            CharacterAi = GetComponentInChildren<CharacterAi>();
            AudioSource = GetComponentInChildren<AudioSource>();
            if (AudioSource == null) AudioSource = gameObject.AddComponent<AudioSource>();
            aiSystems = GetComponentInChildren<AiSystems>();
        }

        /// <summary>
        /// Plays foot step sound
        /// </summary>
        public virtual void PlayFootstepSound() => AudioManager.Instance.PlaySound(transform.position, characterSounds.footstepSounds);

        /// <summary>
        /// Plays attack sound with random chance
        /// </summary>
        public virtual void PlayAttackSound()
        {
            if (!RandomChance.Get(characterSounds.chanceToPlayAttackSound)) return;
            AudioManager.Instance.PlaySound(transform.position, characterSounds.attackSound);
        }

        /// <summary>
        /// Use aweapon, will call MeleeAttack if have melee aweapon,
        /// or Shoot if have shooting type weapon
        /// </summary>
        public virtual void Attack()
        {
            // should we log somethig here?
            if (CurrentAttack == null) return;

            if (UnityTime.Time - lastTimeAttack < CurrentAttack.AttackDuration) return;
            lastTimeAttack = UnityTime.Time;

            switch (CurrentAttack.AttackType)
            {
                case AttackType.Melee:
                    MeleeAttack();
                    return;
                case AttackType.Shooting:
                    Shoot();
                    break;
            }
        }

        public virtual void PlayAttackHitSound() => AudioManager.Instance.PlaySound(transform.position, CurrentAttack.HitSound);

        public virtual void PlayWeaponAttackSound() => AudioManager.Instance.PlaySound(transform.position, CurrentAttack.AttackSound);

        /// <summary>
        /// Refreshes available weapons and attacks using GetWeapons and GetAttacks methods
        /// </summary>
        public virtual void GetAvailableWeaponsAndAttacks()
        {
            AvailableWeapons.Clear();
            foreach (var weapon in GetWeapons()) AvailableWeapons.Add(weapon);
            availableAttacks.Clear();
            availableAttacks.AddRange(GetAttacks());


            // var anims = GetComponentInChildren<ICharacterAnimationContainer>();
            // if (anims.SingleAnimations.attackingAnimations.Length > 0)
            //     foreach (var availableAttack in availableAttacks)
            //         if (availableAttack.AnimationClip == null)
            //             availableAttack.AnimationClip = anims.SingleAnimations.attackingAnimations[0].clip;
        }

        #endregion

        #region Not public methods

        /// <summary>
        /// This deals damage to current target using current attack or returns if theres no current target
        /// </summary>
        protected virtual void MeleeAttack() => aiSystems.AttackSystem.MeleeAttack(this);

        protected virtual void OnEnable()
        {
            CharacterAi.Enabled = true;
            LB.Register(this, CharacterUpdate, 1);
            HonorAIManager.Instance.activeCharactersCount++;
        }

        protected virtual void OnDisable()
        {
            CharacterAi.Enabled = false;
            LB.Unregister(this);
            HonorAIManager.Instance.activeCharactersCount--;
        }

        /// <summary>
        /// Shoots current weapon at target,
        /// animation independent
        /// </summary>
        protected virtual void Shoot()
        {
            var target = CharacterAi.Target;
            if (target.IsObjectNull()) return;

            if (CurrentWeapon != null && CurrentWeapon.AttackType == AttackType.Shooting)
            {
                var sw = CurrentWeapon as ShootingWeapon;
                if (sw != null) sw.Shoot(this, target, CurrentAttack.Damage, CurrentAttack.DamageOnlyEnemies);
            }
        }

        protected virtual void KilledAnimation()
        {
            if (removeDead) RemoveDeadBody(gameObject);

            // todo instaed of referencing animation system, make anim system hook into killed event of character (to consider?)
            CharacterAi.CharacterAnimation.PlayDeathAnimation();
            if (movementRootMotionHandler != null) Destroy(movementRootMotionHandler as Object);
            if (CharacterAi.CharacterAnimation != null) (CharacterAi.CharacterAnimation as MonoBehaviour).enabled = false;
            Destroy(this);
            Destroy(CharacterAi as Object);
            Destroy(GetComponent<LookAt>());
            Destroy(GetComponent<Collider>());
        }

        protected virtual void KilledRagdoll(Vector3 _hitPoint = default, Vector3 _hitForce = default, float _forceRadius = default)
        {
            var ragdoll = ragdollCreator.Create(CharacterAi.Movement.Velocity, transform.localScale);
            ragdollCreator.ApplyPointForce(_hitForce, _hitPoint, _forceRadius);
            if (removeDead && ragdoll != null) RemoveDeadBody(ragdoll);

            Destroy(gameObject);
        }

        protected virtual void RemoveDeadBody(GameObject _gameObject)
        {
            var destro = _gameObject.AddComponent<DestroyGameObjectAfter>();
            destro.destroyAfter = removeDeadAfter;
        }

        protected virtual void Awake()
        {
            if (debugObjectDetecionEvents)
            {
                OnNewObjectDetected += _o => Debug.Log($"{name} detected new object: {_o}", _o);
                OnObjectNotDetectedAnymore += _o => Debug.Log($"{name} not detected object anymore: {_o}", _o);
            }

            if (debutTargetDetectionEvents)
            {
                OnNewTargetDetected += _target => Debug.Log($"{name} saw new target: {_target}", _target as Object);
                OnTargetNotSeenAnymore += _target => Debug.Log($"{name} lost sight of target: {_target}", _target as Object);
                OnTargetVisibleAgain += _target => Debug.Log($"{name} saw target again: {_target}", _target as Object);
                OnTargetForget += _target => Debug.Log($"{name} forgot about target: {_target}", _target as Object);
            }

            SetupRagdoll();
            FindReferences();
            SetupMovement();

            if (UseSoundPreset) characterSounds = SoundsPreset.characterSounds;
            HonorAIManager.Instance.totalCharactersCount++;
        }

        /// <summary>
        /// Get reference to all available attacks of all weapons
        /// </summary>
        protected virtual IEnumerable<IAttack> GetAttacks() => GetComponentsInChildren<IAttack>();

        /// <summary>
        /// Get reference to all available weapons
        /// </summary>
        protected virtual IEnumerable<IWeapon> GetWeapons() => GetComponentsInChildren<IWeapon>();

        protected virtual void SetupRagdoll()
        {
            ragdollCreator.sourceRoot = transform;
            ragdollCreator.Initialize();
        }

        protected virtual void Start()
        {
            if (showGuiInfo)
            {
                Transform t;
                Instantiate(Resources.Load<GameObject>("CharCanvas"), (t = transform).position + Vector3.up * 2.1f, t.rotation, t);
            }

            SetupRMHandler();
            GetAvailableWeaponsAndAttacks();
            SelectAttack();
            GetRadius();
        }

        protected virtual void GetRadius()
        {
            var coll = GetComponent<CapsuleCollider>();
            if (coll == null) return;
            radius = coll.radius * transform.localScale.x;
        }

        protected virtual void SetupMovement()
        {
            var movement = gameObject.GetComponent<IMovement>();
            if (movement != null) movement.ReserveDestinationPosition = reserveDestinationPosition;
        }

        protected virtual void SetupRMHandler()
        {
            if (!CharacterAi.CharacterAnimation.UseRootMotion) return;
            if (movementRootMotionHandler == null) movementRootMotionHandler = gameObject.AddComponent<NavMeshAgentRootMotionHandler>();
        }

        /// <summary>
        /// Health regen and fight sounds logic
        /// </summary>
        protected virtual void CharacterUpdate(float _dt)
        {
#if UNITY_EDITOR
            danger = characterAi.Ai.MainGraphAiVariables.GetFloat("Danger");
#endif
            if (healthRegeneration) HealthRegenerationLogic();
            if (CharacterAi.CharacterState != CharacterState.Combat) return;
            if (characterSounds.fightSound == null) return;
            if (!RandomChance.Get(characterSounds.chanceToPlayFightSound)) return;

            AudioManager.Instance.PlaySound(transform.position, characterSounds.fightSound, AudioSource, false);
        }

        /// <summary>
        /// Adds healthRegenerationSpeed to Health
        /// </summary>
        protected virtual void HealthRegenerationLogic() => HitPoints += healthRegenerationSpeed;

        protected virtual void OnDestroy() => HonorAIManager.Instance.totalCharactersCount--;

        #endregion

        [Serializable] public class GotHitUnityEvent : UnityEvent<Object, float>
        {
        }

        // changed to attributes
//        /// <summary>
//        /// Allows to expose additional serialized fields under relevant tabs in Character inspector
//        /// </summary>
//        public virtual List<InspectorExposedFieldInfo> InspectorExposedFields
//        {
//            get
//            {
//                // example usage 
//                var list = new List<InspectorExposedFieldInfo>();
////                list.Add(new InspectorExposedFieldInfo("General", "armor"));
//                return list;
//            }
//        }
    }
}