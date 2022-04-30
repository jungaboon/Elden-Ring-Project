// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using System.Collections.Generic;
using RVModules.RVLoadBalancer.Tasks;
using RVModules.RVSmartAI.Content.Scanners;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Contexts
{
    /// <summary>
    /// Generic context providing the most common/useful members for ai
    /// </summary>
    public class AiAgentGenericContext : MonoBehaviour, IContext, IContextProvider, IMovementProvider, IMovementScannerProvider, IEnvironmentScannerProvider,
        IMoveTargetProvider, IWaypointsProvider, INearbyObjectsProvider, IJobHandlerProvider
    {
        #region Fields

        [SerializeField]
        protected Transform moveTarget;

        [SerializeField]
        protected List<Transform> waypoints;

        [SerializeField]
        protected List<Object> nearbyObjects = new List<Object>();

        [SerializeField]
        protected TaskHandler aiJobHandler;

        #endregion

        #region Properties

        public IMovement Movement { get; private set; }

        public IMovementScanner MovementScanner { get; private set; }

        public IEnvironmentScanner EnvironmentScanner { get; private set; }

        public Transform FollowTarget
        {
            get => moveTarget;
            set => moveTarget = value;
        }

        public List<Object> NearbyObjects => nearbyObjects;
        public int WaypointsCount => waypoints.Count;
        public TaskHandler AiJobHandler => aiJobHandler;

        #endregion

        #region Public methods

        /// <summary>
        /// Finds references and creates TaskHandler
        /// </summary>
        public virtual void Awake()
        {
            Movement = GetComponent<IMovement>();
            EnvironmentScanner = GetComponent<IEnvironmentScanner>();
            MovementScanner = GetComponent<IMovementScanner>();
            CreateAiJobHandler();
        }

        // IContextProvider implementation
        public virtual IContext GetContext() => this;

        public Vector3 GetWaypointPosition(int _id) => waypoints[_id].position;

        #endregion

        #region Not public methods

        /// <summary>
        /// Override for custom initial task handler settings
        /// </summary>
        protected virtual void CreateAiJobHandler() => aiJobHandler = new TaskHandler();

        #endregion
    }
}