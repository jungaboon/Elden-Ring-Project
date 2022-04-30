// Created by Ronis Vision. All rights reserved
// 30.04.2021.

using System;
using System.Collections.Generic;
using RVHonorAI.CharacterInspector;
using RVHonorAI.Combat;
using RVHonorAI.Content.AI.Tasks;
using RVHonorAI.Systems;
using RVModules.RVLoadBalancer;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content;
using RVModules.RVSmartAI.Content.AI.Tasks;
using RVModules.RVSmartAI.Content.Scanners;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVUtilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RVHonorAI
{
    /// <summary>
    /// Character AI, implements most of interfaces
    /// Handles variables related to AI spatial awareness, combat and movement
    /// </summary>
    public class CharacterAi : MonoBehaviour, IContext, IContextProvider, IDangerPositionProvider, ICharacterAi, IExposeCharInspectorFields
    {
        #region Fields

        private static readonly string playerTag = "Player";

        private Ai ai;

        private Action<CharacterState, CharacterState> stateChanged;

        private ICharacter character;

        [SerializeField]
        [HideInInspector]
        private List<Waypoint> waypoints;

        [SerializeField]
        [HideInInspector]
        private Transform moveTarget;

        private Dictionary<ITarget, TargetInfo> targetsDict = new Dictionary<ITarget, TargetInfo>();

        [SerializeField]
        [Tooltip("Drag here target you want this ai to attack")]
        private GameObject setTarget;

        [SerializeField]
        [CharacterInspectorField(drawWhenNotPlaying = false)]
        private TargetInfo currentTarget;

        [SerializeField]
        [CharacterInspectorField(drawWhenNotPlaying = false)]
        private List<TargetInfo> targets;

        [SerializeField]
        [HideInInspector]
        private bool useLocalPositionForWaypoints = true;

        [SerializeField]
        [CharacterInspectorField(drawWhenNotPlaying = false)]
        private List<Object> nearbyObjects = new List<Object>(10);

        [SerializeField]
        [HideInInspector]
        private AiGroup aiGroup;

        [SerializeField]
        [CharacterInspectorField(drawWhenNotPlaying = false)]
        protected CharacterState state = CharacterState.None;

        [SerializeField]
        private ICharacterAnimation characterAnimation;

        [SerializeField]
        [HideInInspector]
        private float courage = 50;

        [SerializeField]
        [HideInInspector]
        private LayerMask fovMask;

        [SerializeField]
        [HideInInspector]
        private bool treatNeutralCharactersAsEnemies;

        [SerializeField]
        [HideInInspector]
        private bool useFieldOfView;

        [SerializeField]
        [HideInInspector]
        private float fovAngle = 90;

        [SerializeField]
        [HideInInspector]
        private bool useRaycastsForFov;

        [SerializeField]
        [HideInInspector]
        private Transform headTransform;

        [SerializeField]
        [HideInInspector]
        private float detectionRange = 15;

        [SerializeField]
        [HideInInspector]
        [Tooltip("Range in which fov angle will be ignored")]
        private float alwaysVisibleRange = 2;

        [SerializeField]
        [HideInInspector]
        private bool waypointsLoop = true;

        [SerializeField]
        [HideInInspector]
        private bool randomWaypoints;

        [SerializeField]
        [HideInInspector]
        private Vector3 dangerPosition;

        [SerializeField]
        [HideInInspector]
        private bool lookAtPlayerAndTarget;

        [SerializeField]
        [HideInInspector]
        private bool neverFlee;

        private LookAt lookAt;

        [SerializeField]
        [HideInInspector]
        [Tooltip("How long targets will stay in targets list after target is not visible anymore")]
        private float targetMemory = 20;

        /// <summary>
        /// Called once, when changing State from Normal to Combat
        /// </summary>
        [Tooltip("Called once, when changing State from Normal to Combat")]
        [SerializeField]
        [HideInInspector]
        private UnityEvent onEnteredCombat;

        /// <summary>
        /// Called when character starts to flee
        /// </summary>
        [Tooltip("Called when character starts to flee")]
        [SerializeField]
        [HideInInspector]
        private UnityEvent onFlee;

        /// <summary>
        /// Called when in combat mode and there are no visible enemies around anymore 
        /// </summary>
        [Tooltip("Called when in combat mode and there are no visible enemies around anymore")]
        [SerializeField]
        [HideInInspector]
        private UnityEvent onNoMoreVisibleEnemies;

        protected int currentMovementSpeed;

        private AiSystems systems;

        [SerializeField]
        [Tooltip("For how long character will flee before evaluating situation again")]
//        [CharacterInspectorField(CharacterInspectorTab.Combat, drawWhenPlaying = false)]
        private float fleeTime = 10;

        [SerializeField]
        [Tooltip("Disables manually setting target from inspector to improve performance.")]
        private bool disableSetTarget;

        #endregion

        #region Properties

        /// <summary>
        /// Ai component
        /// </summary>
        public Ai Ai
        {
            get => ai;
            protected set => ai = value;
        }

        /// <summary>
        /// Courage. Lower value means character will flee instead of fighting with lower enemie's advantage
        /// Should be in 0-100 range
        /// </summary>
        public float Courage
        {
            get => courage;
            set => courage = value;
        }

        /// <summary>
        /// CharacterAnimation component reference 
        /// </summary>
        public virtual ICharacterAnimation CharacterAnimation
        {
            get => characterAnimation;
            protected set => characterAnimation = value;
        }

        /// <summary>
        /// Character's current state
        /// </summary>
        public virtual CharacterState CharacterState
        {
            get => state;
            set
            {
                if (value == state) return;

                OnStateChanged?.Invoke(state, value);

                // did we just entered combat?
                if (state == CharacterState.Normal && value == CharacterState.Combat)
                {
                    onEnteredCombat?.Invoke();
                    Movement.Destination = Vector3.zero;
                }

                if (value == CharacterState.Flee) OnFlee?.Invoke();
                if (state == CharacterState.Combat && value == CharacterState.Normal) OnNoMoreVisibleEnemies?.Invoke();

                state = value;

                if (state == CharacterState.Normal) MovementSpeed = MovementSpeed.Walking;

                if (state == CharacterState.Combat || state == CharacterState.Flee) MovementSpeed = MovementSpeed.Running;

                CharacterAnimation.SetState((int) state);
            }
        }

        /// <summary>
        /// Sets movement speed to walking or running speed
        /// </summary>
        public virtual MovementSpeed MovementSpeed
        {
            get => (MovementSpeed) currentMovementSpeed;
            set
            {
                currentMovementSpeed = Mathf.Clamp((int) value, 0, 1);
                Movement.MovementSpeed = currentMovementSpeed == 0 ? character.WalkingSpeed : character.RunningSpeed;
//                if (MeshAgent == null) return;
//                MeshAgent.speed = movementSpeed == 0 ? character.WalkingSpeed : character.RunningSpeed;
            }
        }

        /// <summary>
        /// System used for character movement
        /// </summary>
        public IMovement Movement { get; protected set; }

        /// <summary>
        /// IMovementScanner
        /// </summary>
        public IMovementScanner MovementScanner { get; protected set; }

        /// <summary>
        /// IEnvironmentScanner
        /// </summary>
        public IEnvironmentScanner EnvironmentScanner { get; protected set; }

        /// <summary>
        /// Follow target, can be used for summon/follower type behaviour, or just to move ai to that transform's position
        /// </summary>
        public Transform FollowTarget
        {
            get => moveTarget;
            set => moveTarget = value;
        }

        /// <summary>
        /// Character's waypoints
        /// </summary>
        public List<Waypoint> Waypoints
        {
            get => waypoints;
            set => waypoints = value;
        }

        /// <summary>
        /// Stores nearby Unity Objects
        /// </summary>
        public List<Object> NearbyObjects => nearbyObjects;

        /// <summary>
        /// Character reference 
        /// </summary>
        public ICharacter Character => character;

        public virtual ITarget Target => currentTarget?.Target;

        public TargetInfo CurrentTarget
        {
            get => currentTarget;
            set => currentTarget = value;
        }

        /// <summary>
        /// Character's AiGroup. Used for detecting relationship to other characters
        /// </summary>
        public AiGroup AiGroup
        {
            get => aiGroup;
            set => aiGroup = value;
        }

        /// <summary>
        /// IWaypointsProvider implementation
        /// </summary>
        public int WaypointsCount => waypoints.Count;

        /// <summary>
        /// LayerMask used for raycasting visible objects
        /// </summary>
        public LayerMask FovMask => fovMask;

        /// <summary>
        /// If true, waypoints will move with character, otherwise waypoints will keep their world position
        /// </summary>
        public bool UseLocalPositionForWaypoints => useLocalPositionForWaypoints;

        /// <summary>
        /// If other character is not ally to this, should it be treated as enemy
        /// </summary>
        public virtual bool TreatNeutralCharactersAsEnemies
        {
            get => treatNeutralCharactersAsEnemies;
            protected set => treatNeutralCharactersAsEnemies = value;
        }

        /// <summary>
        /// Head transform. Used for checking fov, look at etc...
        /// </summary>
        public virtual Transform HeadTransform
        {
            get => headTransform;
            set => headTransform = value;
        }

        /// <summary>
        /// Unity's enabled wrapper
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        /// <summary>
        /// Danger position - weighter average position of known hostile targets (weighted by their danger)
        /// </summary>
        public virtual Vector3 DangerPosition
        {
            get => dangerPosition;
            set => dangerPosition = value;
        }

        /// <summary>
        /// Targets we know about
        /// </summary>
        public Dictionary<ITarget, TargetInfo> TargetInfosDict => targetsDict;

        /// <summary>
        /// Targets we know about
        /// </summary>
        public List<TargetInfo> TargetInfos => targets;

//        /// <summary>
//        /// personal one on one enemy relationship
//        /// characters that became enemies, but are not enemies by group relationship
//        /// attacked me, violated territory etc; 
//        /// </summary>
//        public List<ICharacter> DynamicEnemies => dynamicEnemies;

        /// <summary>
        /// Never flee, if true, courage setting is ignored, and appropriate graph elements are turned off in graph
        /// </summary>
        public bool NeverFlee => neverFlee;

        /// <summary>
        /// Should this ai have limited field of view
        /// </summary>
        public bool UseFieldOfView => useFieldOfView;

        /// <summary>
        /// Field of view angle
        /// </summary>
        public float FovAngle => fovAngle;

        /// <summary>
        /// Should raycasts be used to check if there is obstruction between other character and this
        /// </summary>
        public bool UseRaycastsForFov => useRaycastsForFov;

        /// <summary>
        /// Detection range in meters
        /// </summary>
        public float DetectionRange => detectionRange;

        /// <summary>
        /// Range in which fov angle will be ignored
        /// </summary>
        public float AlwaysVisibleRange => alwaysVisibleRange;

        /// <summary>
        /// If false, character will follow waypoints to last one, and then go in opposite direction,
        /// otherwise it will move to first waypoint after reaching last one
        /// </summary>
        public bool WaypointsLoop => waypointsLoop;

        /// <summary>
        /// Should character select waypoints randomly
        /// </summary>
        public bool RandomWaypoints => randomWaypoints;

        /// <summary>
        /// Should character use Unity's LookAt feature to look at player and enemies  
        /// </summary>
        public bool LookAtPlayerAndTarget => lookAtPlayerAndTarget;

        /// <summary>
        /// Old state, new state
        /// </summary>
        public Action<CharacterState, CharacterState> OnStateChanged
        {
            get => stateChanged;
            set => stateChanged = value;
        }

        /// <summary>
        /// Called when character starts to flee
        /// </summary>
        public UnityEvent OnFlee => onFlee;

        /// <summary>
        /// Called when in combat mode and there are no visible enemies around anymore 
        /// </summary>
        public UnityEvent OnNoMoreVisibleEnemies => onNoMoreVisibleEnemies;

        public Vector3 FovPosition => HeadTransform.position;

        public virtual bool ExposeAllFieldsToCharInspector => false;
        public virtual CharacterInspectorTab DefaultCharInspectorTab => CharacterInspectorTab.Ai;

        public float FleeTime => fleeTime;

        public float TargetMemory => targetMemory;

        #endregion

        #region Public methods

        public void AddTarget(ITarget _target, bool _visible = true, Vector3 _lastSeenPosition = default)
        {
            if (_target.Object() == null) return;
            if (targetsDict.ContainsKey(_target)) return;

            // dont allow to target allies
            if (_target is IRelationship r && IsAlly(r)) return;

            var target = UpdateTargetList.targetInfoPool.GetObject();
//            var target = new TargetInfo();

            target.Target = _target;
            target.LastSeenTime = UnityTime.Time;

            if (_visible)
            {
                target.LastSeenPosition = _target.Transform.position;
                target.Visible = true;
            }
            else
            {
                target.LastSeenPosition = character.Transform.position;
            }

            if (_lastSeenPosition != default) target.LastSeenPosition = _lastSeenPosition;

//            targets.Add(target);
            targetsDict.Add(_target, target);
        }

        public void SetTarget(ITarget _target, bool _visible = true, Vector3 _lastSeenPosition = default)
        {
            if (_target.Object() == null) return;
            if (targetsDict.ContainsKey(_target))
            {
                CurrentTarget = targetsDict[_target];
                return;
            }

            AddTarget(_target, _visible, _lastSeenPosition);

            if (!targetsDict.ContainsKey(_target)) return;
            CurrentTarget = targetsDict[_target];
        }

        /// <summary>
        /// Check's relationship to other character
        /// </summary>
        public virtual bool IsEnemy(IRelationship _other, bool _contraCheck = false) => systems.RelationshipSystem.IsEnemy(this, _other, _contraCheck);

        /// <summary>
        /// Check's relationship to other character
        /// </summary>
        public virtual bool IsAlly(IRelationship _other) => systems.RelationshipSystem.IsAlly(this, _other);

        public virtual void FindReferences()
        {
            FindIMovement();
            FindScanners();

            FindCharacter();
            FindCharacterAnimation();
            //FindNavmeshAgent();
            FindAi();

            systems = GetComponentInChildren<AiSystems>();
        }

        /// <summary>
        /// IContextProvider implementation
        /// </summary>
        public virtual IContext GetContext() => this;

        public virtual Vector3 GetWaypointPosition(int _id)
        {
            var waypoint = waypoints[_id];

            var p = waypoint.position;
            if (waypoint.radius == 0) return p;
            var r = Random.insideUnitSphere * waypoint.radius;
            r.y = 0;
            return p + r;
        }

        /// <summary>
        /// Enable or disable environment scanning graph
        /// It assumes that scanning graph is first graph in secondary graphs array
        /// </summary>
        public virtual void EnableEnvironmentScanning(bool _enable)
        {
            if (ai.SecondaryGraphs.Length == 0) return;
            if (_enable) ai.SecondaryGraphs[0].RegisterToLoadBalancingSystem();
            else ai.SecondaryGraphs[0].UnregisterFromLoadBalancingSystem();
        }

        #endregion

        #region Not public methods

        /// <summary>
        /// Uses GetComponentInChildren to get reference to Ai
        /// </summary>
        protected virtual void FindAi() => ai = GetComponentInChildren<Ai>();

        /// <summary>
        /// Uses GetComponentInChildren to get reference to ICharacterAnimation
        /// </summary>
        protected virtual void FindCharacterAnimation() => CharacterAnimation = GetComponentInChildren<ICharacterAnimation>();

        /// <summary>
        /// Uses GetComponentInChildren to get reference to Character
        /// </summary>
        protected virtual void FindCharacter() => character = GetComponentInChildren<ICharacter>();

        /// <summary>
        /// Uses GetComponentInChildren to get reference to IEnvironmentScanner and IMovementScanner
        /// </summary>
        protected virtual void FindScanners()
        {
            EnvironmentScanner = GetComponentInChildren<IEnvironmentScanner>();
            MovementScanner = GetComponentInChildren<IMovementScanner>();
        }

        /// <summary>
        /// Uses GetComponentInChildren to get reference to IMovement
        /// </summary>
        protected virtual void FindIMovement() => Movement = GetComponentInChildren<IMovement>();

        protected virtual void Awake()
        {
            FindReferences();

            if (useLocalPositionForWaypoints)
            {
                foreach (var waypoint in waypoints) waypoint.position += transform.position;
                useLocalPositionForWaypoints = false;
            }

//            // awake external jobs
//            //externalAiJobs
//            var jobs = GetComponents<AiJob>();
//
//            foreach (var characterJob in jobs) characterJob.Context = this;
//
//            externalAiJobs = jobs.ToArray();
        }

        protected virtual void Start()
        {
            SetupGraphVariables();

            MovementSpeed = 0;
            CharacterState = CharacterState.Normal;

            if (LookAtPlayerAndTarget)
            {
                lookAt = CharacterAnimation.Animator.gameObject.AddComponent<LookAt>();
                lookAt.head = headTransform;
            }
        }

        protected virtual void LookAtLoop(float _dt)
        {
            // dont stare at player while in combat
            if (state == CharacterState.Combat)
            {
                if (Target as Object == null)
                {
                    lookAt.look = false;
                    return;
                }

                //lookAt.look = false;
                LookAt(Target.AimTransform, 180, 15);
                return;
            }

            // is player nearby?
            foreach (var o in nearbyObjects)
            {
                var otherChar = o as ICharacter;
                if (otherChar as Object == null) continue;
                var go = otherChar.Transform.gameObject;
                if (go.CompareTag(playerTag))
                {
                    var lookTargetTransform = otherChar.VisibilityCheckTransform;
                    LookAt(lookTargetTransform, 85, 6);
                    return;
                }
            }

            lookAt.look = false;
            lookAt.LookAtTransform = null;
        }

        private void LookAt(Transform _lookTargetTransform, int _maxAngle, int _maxDistance, float _bodyWeight = .1f)
        {
            var lookAtPos = _lookTargetTransform.position;
            var angle = Vector3.Angle(character.Transform.forward, (lookAtPos - headTransform.position).normalized);
            lookAt.look = Vector3.Distance(transform.position, lookAtPos) < _maxDistance && angle < _maxAngle;
            lookAt.LookAtTransform = lookAt.look ? _lookTargetTransform : null;
            lookAt.bodyWeight = _bodyWeight;
        }

        protected virtual void OnDisable()
        {
            LB.Unregister(this);
            ai.enabled = false;
            if (lookAt != null) lookAt.enabled = false;
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (!disableSetTarget)
                LB.Register(this, _f =>
                {
                    if (setTarget == null) return;
                    var target = setTarget.GetComponent<ITarget>();
                    if (target == null)
                    {
                        setTarget = null;
                        Debug.LogError("Provided object doesnt have any ITarget component!", setTarget);
                        return;
                    }

                    if (target is IRelationship r && r.IsAlly(this))
                    {
                        Debug.Log("Cant target allies!");
                        setTarget = null;
                        return;
                    }

                    SetTarget(target, true, target.Transform.position);
                    //AddTarget(target);
                    setTarget = null;
                }, 5);
#endif

            if (LookAtPlayerAndTarget)
            {
                LB.Register(this, LookAtLoop, 2);
                if (lookAt != null) lookAt.enabled = true;
            }

            ai.enabled = true;
        }

        /// <summary>
        /// Destroys ai components game object and movement
        /// </summary>
        protected virtual void OnDestroy()
        {
            Destroy(Ai.gameObject);
            if (Movement as Object != null) Destroy(Movement as Object);
        }

        /// <summary>
        /// Here you can customize graph and graph variables accordingly to ai agent instance configuration
        /// </summary>
        protected virtual void SetupGraphVariables()
        {
            // set all properties for graph element configuration
            var mainGraphElements = ai.MainAiGraph.GetAllGraphElements(true);
            IAiGraphElement[] scannerGraphElements = null;

            if (ai.SecondaryGraphs == null || ai.SecondaryGraphs.Length == 0)
            {
                Debug.LogWarning("There is no secondary AI graph responsible for scanning attached to this Character!", character as Object);
            }
            else
            {
                scannerGraphElements = ai.SecondaryGraphs[0].GetAllGraphElements();

                var scanningGraphVars = ai.SecondaryGraphs[0].GraphAiVariables;

                scanningGraphVars.AssureFloatExist(nameof(FovAngle));
                scanningGraphVars.SetFloat(nameof(FovAngle), FovAngle);
                scanningGraphVars.AssureFloatExist(nameof(AlwaysVisibleRange));
                scanningGraphVars.SetFloat(nameof(AlwaysVisibleRange), AlwaysVisibleRange);
                scanningGraphVars.AssureFloatExist(nameof(TargetMemory));
                scanningGraphVars.SetFloat(nameof(TargetMemory), TargetMemory);
            }

            var mainGraphVars = ai.MainGraphAiVariables;

            mainGraphVars.AssureBoolExist(nameof(RandomWaypoints));
            mainGraphVars.SetBool(nameof(RandomWaypoints), RandomWaypoints);

            mainGraphVars.AssureBoolExist(nameof(WaypointsLoop));
            mainGraphVars.SetBool(nameof(WaypointsLoop), WaypointsLoop);

            mainGraphVars.AssureFloatExist(nameof(FleeTime));
            mainGraphVars.SetFloat(nameof(FleeTime), FleeTime);

            if (scannerGraphElements != null)
                foreach (var graphElement in scannerGraphElements)
                    if (graphElement is CheckVisibleObjects checkVisibleObjects)
                    {
                        // todo some kinda logic that would enable/disable GE based on bool graph variable at startup?? maybe even remove it
                        checkVisibleObjects.enabled = UseFieldOfView;
                        // todo replace by data provider
                        checkVisibleObjects.checkVisibilityWithRaycast = UseRaycastsForFov;
                    }
                    else if (graphElement is AiScanSurrounding aiScanSurrounding)
                    {
                        aiScanSurrounding.enabled = !UseFieldOfView;
                    }

            foreach (var graphElement in mainGraphElements)
                if (NeverFlee)
                {
                    var aiGraphElement = graphElement as AiGraphElement;
                    // todo maybe its better to remove it instead of disabling?
                    if (graphElement.Name == "Too dangerous") aiGraphElement.enabled = false; //aiGraphElement.Remove(true);

                    //if (graphElement.Name == "Fleeing?") aiGraphElement.enabled = false;
                }
        }

        #endregion
    }
}