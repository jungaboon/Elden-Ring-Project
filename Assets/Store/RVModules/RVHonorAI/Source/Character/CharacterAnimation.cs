// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System.Collections.Generic;
using RVHonorAI.Animation;
using RVModules.RVLoadBalancer;
using RVModules.RVSmartAI.Content;
using RVModules.RVUtilities;
using UnityEngine;
using UnityEngine.AI;

namespace RVHonorAI
{
    /// <summary>
    /// Simple character animation component
    /// </summary>
    public class CharacterAnimation : MonoBehaviour, ICharacterAnimation, ICharacterAnimationContainer
    {
        #region Fields

        private readonly int y = Animator.StringToHash("y");
        private readonly int x = Animator.StringToHash("x");

        private readonly int state1 = Animator.StringToHash("State");
        private readonly int rotating = Animator.StringToHash("rotating");
        private readonly int rotation = Animator.StringToHash("rotation");
        private readonly int moving = Animator.StringToHash("moving");

        [SerializeField]
        private MovementAnimations movementAnimations;

        [SerializeField]
        private MovementAnimations combatMovementAnimations;

        [SerializeField]
        private SingleAnimations singleAnimations;

        protected IMovement movement;

        protected Vector2 velocity = Vector2.zero;

        protected float velocityMul = 1;

        protected new Transform transform;

        protected Vector3 movementVelocity;

        [Tooltip("How quickly character transition between different animations")]
        [SerializeField]
        private float velocityDeltaSpeed = 3f;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private bool useRootMotion = true;

        [SerializeField]
        [HideInInspector]
        private bool autoUpdateAnimatorController = true;

        private int[] attackAnimations;
        private static readonly int randomIdleId = Animator.StringToHash("randomIdleId");

        [SerializeField]
        [Range(0, 100)]
        [Tooltip("Chance to play random idle animation per second")]
        private float chanceForRandomIdleAnimation = 20f;

        #endregion

        #region Properties

        public bool UseRootMotion => useRootMotion;

        public virtual bool Rotating
        {
            set
            {
                if (Animator == null) return;
                //isRotating = value;
                Animator.SetBool(rotating, value);
            }
        }

        public virtual float RotatingSpeed
        {
            set => Animator?.SetFloat(rotation, value);
        }

        public virtual bool Moving
        {
            set => Animator.SetBool(moving, value);
        }

        public Animator Animator
        {
            get => animator;
            protected set => animator = value;
        }

        public MovementAnimations MovementAnimations
        {
            get => movementAnimations;
            set => movementAnimations = value;
        }

        public MovementAnimations CombatMovementAnimations
        {
            get => combatMovementAnimations;
            set => combatMovementAnimations = value;
        }

        public SingleAnimations SingleAnimations
        {
            get => singleAnimations;
            set => singleAnimations = value;
        }

        #endregion

        #region Public methods

        public virtual void FindReferences()
        {
            if(animator == null) animator = GetComponentInChildren<Animator>();
            movement = GetComponentInChildren<IMovement>();
            
            // left for backward compatibility! :s now animator should be on root/same as character
            if (animator == null) animator = GetComponentInParent<Animator>();
            if (movement == null) movement = GetComponentInParent<IMovement>();
        }

        public virtual void SetState(int _state)
        {
            if (Animator == null) return;
            Animator.SetInteger(state1, _state);
        }

        /// <summary>
        /// If animationClip is not provided, attack animation will be selected randomly
        /// </summary>
        /// <param name="_animationClip">Clip to play</param>
        public virtual void PlayAttackAnimation(AnimationClip _animationClip = null)
        {
            if (SingleAnimations.attackingAnimations.Length == 0) return;

            if (_animationClip == null)
            {
                PlayAttackAnimation(Random.Range(0, SingleAnimations.attackingAnimations.Length));
            }
            else
            {
                for (var i = 0; i < singleAnimations.attackingAnimations.Length; i++)
                {
                    var singleAnimationsAttackingAnimation = singleAnimations.attackingAnimations[i];
                    if (singleAnimationsAttackingAnimation.clip == _animationClip)
                    {
                        PlayAttackAnimation(i);
                        return;
                    }
                }

                // if not found
                PlayAttackAnimation(0);
            }
        }

        public virtual void PlayAttackAnimation(int attackAnimationId)
        {
            if (SingleAnimations.attackingAnimations.Length == 0 || attackAnimationId >= singleAnimations.attackingAnimations.Length) return;

            if (!Animator.isHuman) Animator.CrossFade(attackAnimations[attackAnimationId], .25f);
            else
            {
                Animator.SetTrigger(attackAnimations[attackAnimationId]);
                //Animator.CrossFade( attackAnimations[attackAnimationId], .25f, 1);
            }
        }

        public virtual void PlayDeathAnimation() => PlayDeathAnimation(Random.Range(0, SingleAnimations.dyingAnimations.Length));

        public void PlayDeathAnimation(int deathAnimationId)
        {
            if (SingleAnimations.dyingAnimations.Length == 0 || deathAnimationId >= singleAnimations.dyingAnimations.Length) return;
            Animator.applyRootMotion = true;
            Animator.SetTrigger("dying" + deathAnimationId);
        }

        public void PlayCustomAnimation(string _animationName) => PlayCustomAnimation(_animationName, .25f);
        
        public void PlayCustomAnimation(string _animationName, float _transitionDuration)
        {
            Animator.CrossFadeInFixedTime(_animationName, _transitionDuration);
//            
//            for (var i = 0; i < singleAnimations.customAnimations.Length; i++)
//            {
//                var customAnimation = singleAnimations.customAnimations[i];
//                if (customAnimation.clip == null) continue;
//                if (_animationName != customAnimation.clip.name) continue;
//                PlayCustomAnimation(i);
//                return true;
//            }
//
//            return false;
        }

        public void PlayCustomAnimation(int animationId) => animator.SetTrigger($"custom{animationId}");

        #endregion

        #region Not public methods

        /// <summary>
        /// Register this component logic to LB system
        /// </summary>
        protected virtual void Register()
        {
            if (UseRootMotion)
                LB.Register(this, RootMotionAnimationLogic, new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0));
            else
                LB.Register(this, SetAnimVelocitiesFromAgentVelocity, new LoadBalancerConfig(LoadBalancerType.EveryXFrames, 0));
            if (singleAnimations.randomIdleAnimations.Length > 0) LB.Register(this, RandomIdle, 1);
        }

        private void RandomIdle(float dt)
        {
            if (!RandomChance.Get(chanceForRandomIdleAnimation) || movement.Velocity.sqrMagnitude > .2f)
            {
                animator.SetInteger(randomIdleId, 0);
                return;
            }

            animator.SetInteger(randomIdleId, Random.Range(0, singleAnimations.randomIdleAnimations.Length + 1));
        }

        /// <summary>
        /// Unregister from LB system
        /// </summary>
        private void Unregister() => LB.Unregister(this);

        protected virtual void RootMotionAnimationLogic(float _dt)
        {
            //var worldDeltaPosition = (agent.nextPosition - transform.position);
//        var dx = Vector3.Dot(transform.right, worldDeltaPosition);
//        var dy = Vector3.Dot(transform.forward, worldDeltaPosition);

            SetAnimVelocitiesFromAgentVelocity(_dt);

            // decoupling from navmesh agent...
            movement.Position = transform.position;
//            agent.nextPosition = transform.position;

//        if (worldDeltaPosition.magnitude > agent.radius * .1f)
//        {
//            var deltaTime = UnityTime.DeltaTime;
//            transform.position = Vector3.MoveTowards(transform.position, agent.nextPosition,
//                (1 - animVsNavagentPullingWeight) * pullingVelocity * deltaTime);
//            agent.nextPosition =
//                Vector3.MoveTowards(agent.nextPosition, transform.position, animVsNavagentPullingWeight * pullingVelocity * deltaTime);
//        }
        }

        protected virtual void SetAnimVelocitiesFromAgentVelocity(float _dt)
        {
            GetCurrentMovementVelocity();

            var rot = transform.rotation;
            var right = rot * Vector3.right;
            var forward = rot * Vector3.forward;

            var dx = Vector3.Dot(right, movementVelocity);
            var dy = Vector3.Dot(forward, movementVelocity);

            SetAnimValues(dx, dy, velocityDeltaSpeed);
        }

        protected virtual void GetCurrentMovementVelocity() => movementVelocity = movement.Velocity;

        protected virtual void SetAnimValues(float _dx, float _dy, float _velocityDeltaSpeed)
        {
            var deltaPosition = new Vector2(_dx, _dy);

            // never change this to move towards! it causes jittery animation blending and movement!
            velocity = Vector2.Lerp(velocity, deltaPosition, UnityTime.DeltaTime * _velocityDeltaSpeed);
            //velocity = Vector2.MoveTowards(velocity, deltaPosition, UnityTime.DeltaTime * velocityDeltaSpeed);

            animator.SetFloat(x, velocity.x * velocityMul);
            animator.SetFloat(y, velocity.y * velocityMul);
        }

        protected virtual void Awake()
        {
            // cache transform access
            transform = base.transform;
            FindReferences();

            // create attack animation hashes
            var atkAnimCount = SingleAnimations.attackingAnimations.Length;
            var atkAnimations = new List<int>(atkAnimCount);
            for (var i = 0; i < atkAnimCount; i++) atkAnimations.Add(Animator.StringToHash($"attacking{i}"));
            attackAnimations = atkAnimations.ToArray();
        }

        protected virtual void Start()
        {
            Animator.applyRootMotion = UseRootMotion;
            movement.UpdatePosition = !UseRootMotion;

            var character = GetComponentInParent<ICharacter>();
            // turn off animation events to avoid stupid Unity "AnimationEvent 'NewEvent' has no receiver" logs
            character.OnKilled.AddListener(() =>
            {
                if (this == null) return;
                if (Animator == null) return;
                Animator.fireEvents = false;
            });
        }

        protected virtual void OnEnable() => Register();

        protected virtual void OnDisable() => Unregister();

        #endregion

//    [SerializeField]
//    [HelpAttribute("How much animation transform will be pulled toward navigation agent vs vice-versa; it's trade-off between animation quality " +
//                   "vs navmesh accuracy, at 1 only animations will move ai, at 0 only navmesh agent.")]
//    [Range(0, 1)]
//    private float animVsNavagentPullingWeight = .5f;
//
//    [SerializeField]
//    [Range(0, 4)]
//    private float pullingVelocity = 2;

        //private bool isRotating;
    }
}