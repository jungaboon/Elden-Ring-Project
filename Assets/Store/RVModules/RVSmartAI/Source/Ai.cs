// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System.Collections.Generic;
using System.Linq;
using RVModules.RVLoadBalancer;
using UnityEngine;
using UnityEngine.Serialization;

namespace RVModules.RVSmartAI
{
    /// <summary>
    /// AI component that is added to gameObject that needs AI, provides reference to AiGraph
    /// </summary>
    public class Ai : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private AiGraph aiGraph;

        [SerializeField]
        private AiGraphConfig[] secondaryGraphs;

        [SerializeField]
        private bool useCustomLoadBalancerConfig;

        [Tooltip("Limits total AI updates per second and lowers frequency after reaching it, slowing down AI updates frequency instead of taking more CPU time")]
        [SerializeField]
        private bool scalableLoadBalancing;

        [SerializeField]
        private bool expandPerfInfo;

        [SerializeField]
        [Tooltip("How many updates per second is allowed. After reaching this number AI update frequency will automatically lower to match this value")]
        private int maxAllowedUpdateFrequency = 120;

        [SerializeField]
        private bool overrideGraphVariablesForNestedGraphs = true;

        [SerializeField]
        private LoadBalancerConfig mainGraphLbc;

        [SerializeField]
        private AiGraph[] instantiatedSecondaryGraphs;

        /// <summary>
        /// Assign any component that implements IContextProvider
        /// </summary>
        [SerializeField]
        private Object contextProvider;

        // debug info
        [SerializeField]
        private int graphStepsPerUpdate = 9999;

        [SerializeField]
        private bool dontHideInHierarchy;

        [Tooltip("How many times per second update this AI's graph")]
        [SerializeField]
        private int updateFrequency = 2;

        [FormerlySerializedAs("sceneReferences")]
        [SerializeField]
        private AiVariables.GraphVariableUnityObject[] references;

        #endregion

        #region Properties

        /// <summary>
        /// Variables of main ai graph
        /// </summary>
        public AiVariables MainGraphAiVariables => MainAiGraph.GraphAiVariables;

        public AiGraph MainAiGraph
        {
            get
            {
                if (Application.isPlaying && !IsInitialized)
                {
                    Debug.LogError("Accessing AiGraph before it's instantation is not allowed to avoid accidental change of AiGraph prefab data!");
                    return null;
                }

                return aiGraph;
            }
        }

        /// <summary>
        /// Returns copy of instantiated secondary graphs
        /// </summary>
        public AiGraph[] SecondaryGraphs => instantiatedSecondaryGraphs.ToArray();

//        public string CurrentNode { get; private set; }
//
//        public string LastUtility { get; private set; }
//
//        public string LastTask { get; private set; }

        public bool IsInitialized { get; protected set; }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public void UpdateGraphsContextUsingContextProvider()
        {
            var newContext = (contextProvider as IContextProvider)?.GetContext();
            UpdateGraphsContext(newContext);
        }

        /// <summary>
        /// Updates main and secondary graphs context
        /// </summary>
        /// <param name="_context">Context to set for AiGraphs</param>
        public void UpdateGraphsContext(IContext _context)
        {
            aiGraph.UpdateContext(_context);
            foreach (var secondaryGraph in instantiatedSecondaryGraphs) secondaryGraph.UpdateContext(_context);
        }

        #endregion

        #region Not public methods

//        /// <summary>
//        /// Can be safely changed at runtime
//        /// </summary>
//        public int UpdateFrequency
//        {
//            get => updateFrequency;
//            set
//            {
//                if (!Application.isPlaying)
//                {
//                    Debug.LogError("Can't change update frequency in edit mode!");
//                    return;
//                }
//
//                updateFrequency = value;
//                if (!enabled || !IsInitialized) return;
//                if (LoadBalancerSingleton.Instance == null) return;
//                // new lb version handes re-registering automatically
//                //LoadBalancerSingleton.Instance.Unregister(this, Tick);
//                RegisterGraphUpdateLoop();
//            }
//        }

        private void RegisterGraphsLb()
        {
            if (!Application.isPlaying) return;

            // done like that for backward-compatibility reason, replacing AiGraph with AiGraphConfig would brake all already existing ai prefabs
            var mainGraphConfig = new AiGraphConfig
            {
                aiGraph = aiGraph,
                loadBalancerConfig = mainGraphLbc,
                updateFrequency = updateFrequency,
                useCustomLoadBalancerConfig = useCustomLoadBalancerConfig,
                overrideGraphVariablesForNestedGraphs = overrideGraphVariablesForNestedGraphs,
                scalableLoadBalancing = scalableLoadBalancing
            };

            MainAiGraph.RegisterToLoadBalancingSystem(mainGraphConfig);

            int secondaryGraphId = 0;
            foreach (var aiGraphConfig in secondaryGraphs)
            {
                if (aiGraphConfig?.aiGraph == null) continue;
                instantiatedSecondaryGraphs[secondaryGraphId].RegisterToLoadBalancingSystem(aiGraphConfig);
                secondaryGraphId++;
            }
        }

        private void UnregisterGraphsLb()
        {
            MainAiGraph.UnregisterFromLoadBalancingSystem();

            foreach (var secondaryGraph in instantiatedSecondaryGraphs) secondaryGraph.UnregisterFromLoadBalancingSystem();
        }

        protected virtual void Awake()
        {
            if (aiGraph == null)
            {
                Debug.LogError($"You need to assign {typeof(AiGraph)}!", this);
                return;
            }

            if (contextProvider as IContextProvider == null)
            {
                Debug.LogError("Coudln't get IContextProvider from assigned contexProvider object!", this);
                return;
            }

            // Create own copy of Ai graphs
            aiGraph = AiGraph.CreateAndInitializeGraph(dontHideInHierarchy, graphStepsPerUpdate,
                transform.parent != null ? transform.parent.gameObject : gameObject, aiGraph);
//#if UNITY_EDITOR
//            aiGraph.onGraphUpdate += MainGraphDebugInfos;
//#endif
            var secGraphsTemp = new List<AiGraph>();
            for (var i = 0; i < secondaryGraphs.Length; i++)
            {
                var secondaryGraph = secondaryGraphs[i];
                if (secondaryGraph?.aiGraph == null) continue;
                secGraphsTemp.Add(
                    AiGraph.CreateAndInitializeGraph(dontHideInHierarchy, graphStepsPerUpdate,
                        transform.parent != null ? transform.parent.gameObject : gameObject, secondaryGraph.aiGraph));
            }

            instantiatedSecondaryGraphs = secGraphsTemp.ToArray();

            Initialized();
        }

        protected virtual void Start()
        {
            UpdateGraphsContextUsingContextProvider();
            AddReferencesToGraphVariables();
        }

        private void AddReferencesToGraphVariables()
        {
            AddReferencesToGraphAndSubGraphs(MainAiGraph, references);
            foreach (var secondaryGraph in instantiatedSecondaryGraphs) AddReferencesToGraphAndSubGraphs(secondaryGraph, references);
        }

        private void AddReferencesToGraphAndSubGraphs(AiGraph _graph, AiVariables.GraphVariableUnityObject[] references)
        {
            var vars = _graph.GraphAiVariables;

            if (vars != null)
            {
                foreach (var graphVariableUnityObject in references)
                {
                    _graph.GraphAiVariables.AddUnityObjectVariable(graphVariableUnityObject.name, graphVariableUnityObject.value);
                }
            }

            foreach (var graphSubGraph in _graph.SubGraphs)
            {
                // if subgraph's variables are same as his parent vars dont add refs to them and this will be duplicate
                if (graphSubGraph.GraphAiVariables == null || graphSubGraph.GraphAiVariables == vars) continue;

                foreach (var graphVariableUnityObject in references)
                {
                    graphSubGraph.GraphAiVariables.AddUnityObjectVariable(graphVariableUnityObject.name, graphVariableUnityObject.value);
                }
            }
        }

//        private void MainGraphDebugInfos()
//        {
//            //if (aiGraph != null) aiGraph.UpdateGraph(_deltaTime);
//#if UNITY_EDITOR
//            if (aiGraph == null) return;
//            // debug info
//            if (aiGraph.lastNode != null) CurrentNode = aiGraph.lastNode.Name;
//            if (aiGraph.lastAiUtility != null) LastUtility = aiGraph.lastAiUtility.Name;
//            if (aiGraph.lastTask != null) LastTask = aiGraph.lastTask.Name;
//#endif
//        }

        protected void Initialized() => IsInitialized = true;

        protected virtual void OnEnable()
        {
            if (!IsInitialized) return;
            RegisterGraphsLb();
        }

        protected virtual void OnDisable()
        {
            if (!IsInitialized) return;
            UnregisterGraphsLb();
        }

        protected virtual void OnDestroy()
        {
            if (!Application.isPlaying || !IsInitialized) return;

            if (aiGraph != null) Destroy(aiGraph.gameObject);
            foreach (var instantiatedSecondaryGraph in instantiatedSecondaryGraphs)
                if (instantiatedSecondaryGraph != null)
                    Destroy(instantiatedSecondaryGraph.gameObject);
        }

        #endregion
    }
}