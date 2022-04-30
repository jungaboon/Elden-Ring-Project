// Created by Ronis Vision. All rights reserved
// 27.10.2019.

using RVModules.RVUtilities.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace RVModules.RVSmartAI.Content
{
    /// <summary>
    /// IMovement implementation using Unity's NavMeshAgent
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnityNavMeshMovement : MonoBehaviour, IMovement
    {
        #region Fields

        [SerializeField]
        private bool reserveDestinationPosition = true;

        [SerializeField]
        [HideInInspector]
        private NavMeshAgent agent;

        private GameObject destPosBlocker;

        [SerializeField]
        private int destinationBlockLayer = 9;

        // serialized for debugging only
        [SerializeField]
        private Vector3 destination;

        // cached trasform access
        private new Transform transform;

        [SerializeField]
        private bool randomAvoidancePriority = true;

        [Tooltip("If closer to destination than this AtDestination will return true")]
        [SerializeField]
        private float atDestinationDistance = .2f;

        public Transform Transform => transform;

        #endregion

        #region Properties

        public Vector3 Velocity => Agent.velocity;

        public virtual float MovementSpeed
        {
            get => Agent.speed;
            set => Agent.speed = value;
        }

        public virtual float RotationSpeed
        {
            get => Agent.angularSpeed;
            set => Agent.angularSpeed = value;
        }

        public virtual bool UpdatePosition
        {
            get => Agent.updatePosition;
            set => Agent.updatePosition = value;
        }

        public virtual bool UpdateRotation
        {
            get => Agent.updateRotation;
            set
            {
                if (Agent == null) return;
                Agent.updateRotation = value;
            }
        }

        public virtual Vector3 Position
        {
            get => transform.position;
            set => Agent.nextPosition = value;
        }

        public virtual Quaternion Rotation => transform.rotation;

        public virtual bool AtDestination => Destination == Vector3.zero || Agent.isStopped || !Agent.hasPath || IsInDestinationRange;

        public virtual Vector3 Destination
        {
            get => destination;
            set
            {
                Agent.destination = value;
                destination = value;
                //destination = agent.destination;

                if (destination == Vector3.zero ||  IsInDestinationRange)
                    Agent.isStopped = true;
                else
                    Agent.isStopped = false;

                if (ReserveDestinationPosition) destPosBlocker.transform.position = destination;
            }
        }

        protected virtual bool IsInDestinationRange => Vector2.Distance(transform.position.ToVector2(), Destination.ToVector2()) < atDestinationDistance;

        /// <summary>
        /// Create 'blocker' object with collider that is set to destination position
        /// to avoid many agents trying to go to the same position
        /// </summary>
        public virtual bool ReserveDestinationPosition
        {
            get => reserveDestinationPosition;
            set
            {
                if (value && destPosBlocker == null) CreateDestinationBlocker();
                if (!value && destPosBlocker != null) Destroy(destPosBlocker);
                reserveDestinationPosition = value;
            }
        }

        public virtual int DestinationBlockLayer
        {
            get => destinationBlockLayer;
            set
            {
                destinationBlockLayer = value;
                if (destPosBlocker != null) destPosBlocker.layer = value;
            }
        }

        public NavMeshAgent Agent => agent;

        #endregion

        #region Not public methods

        /// <summary>
        /// Removes destination position blocker
        /// </summary>
        protected virtual void OnDestroy()
        {
            Destroy(Agent);
            if (destPosBlocker == null) return;
            Destroy(destPosBlocker);
        }

        protected virtual void Awake()
        {
            transform = base.transform;
            agent = gameObject.AddOrGetComponent<NavMeshAgent>();

            if (randomAvoidancePriority) Agent.avoidancePriority = Random.Range(0, 100);

            //agent.updateRotation = false;
//            agent.velocity = destination;
//            agent.angularSpeed = destPosBlockerLayer;

            if (!ReserveDestinationPosition) return;
            CreateDestinationBlocker();
        }

        protected virtual void CreateDestinationBlocker()
        {
            if (destPosBlocker != null) return;
            destPosBlocker = new GameObject(name + " destination blocker");
            var coll = destPosBlocker.AddComponent<SphereCollider>();
            coll.isTrigger = true;
            DestinationBlockLayer = destinationBlockLayer;
        }

        #endregion
    }
}