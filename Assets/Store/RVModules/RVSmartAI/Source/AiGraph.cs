// Created by Ronis Vision. All rights reserved
// 06.04.2021.

using System;
using System.Collections.Generic;
using System.Linq;
using RVModules.RVLoadBalancer;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.GraphElements.Stages;
using RVModules.RVSmartAI.Nodes;
using RVModules.RVUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI
{
    /// <summary>
    /// Main Ai graph component
    /// </summary>
    [Serializable] public class AiGraph : MonoNodeGraph
    {
        #region Fields

        public string description;

        public Object owner;

        [NonSerialized]
        public SmartAiNode lastNode;

        private List<AiTask> tasks = new List<AiTask>(5);

        private bool isRuntimeDebugGraph;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        private int portIndexOfWinner;

        [NonSerialized]
        private AiTask lastTask;

        [SerializeField]
        private AiGraph parentGraph;

        [SerializeField]
        private int graphStepsPerUpdate = 9999;

        private bool hasParentGraph;

        [NonSerialized]
        private Stage currentStage;

        [NonSerialized]
        private SmartAiNode currentNode;

        [SerializeField]
        private SmartAiNode rootNode;

        private IContext context;

        private bool blockingTask;

        // if subgraph entered running task
        [SerializeField]
        private bool subGraphBlockingTask;

        // instanced subGraphs
        private List<AiGraph> subGraphs = new List<AiGraph>();

        private bool dontHideInHierarchy;

        [FormerlySerializedAs("graphVariables")]
        [SerializeField]
        private AiVariables graphAiVariables;

        private Action gettingBackToBlockingTask;
        private float blockingTaskStartTime;

        [SerializeField]
        private AiGraphConfig aiGraphConfig;

        private Action<float> updateGraphAction;

        private List<AiGraph> subGraphsRecursive = new List<AiGraph>(5);

        private bool isRunning;

        #endregion

        #region Properties

        /// <summary>
        /// GraphVariables
        /// </summary>
        public AiVariables GraphAiVariables
        {
            get => graphAiVariables;
            set => graphAiVariables = value;
        }

//        private void OnDisable()
//        {
//            UnregisterFromLoadBalancingSystem();
//        }

        /// <summary>
        /// Root node
        /// </summary>
        public SmartAiNode RootNode
        {
            get => rootNode;
            set => rootNode = value;
        }

        /// <summary>
        /// If this graph instance was created as nested graph(reference in other graph) it will have it's parent graph reference
        /// </summary>
        public AiGraph ParentGraph
        {
            get => parentGraph;
            private set
            {
                parentGraph = value;
                hasParentGraph = value != null;
            }
        }

        /// <summary>
        /// How many steps graph should do each update
        /// One step is processing of one node
        /// </summary>
        public int GraphStepsPerUpdate
        {
            get => graphStepsPerUpdate;
            set => graphStepsPerUpdate = value;
        }

        /// <summary>
        /// Returns currently assigned context. To set it use UpdateContext method
        /// </summary>
        public IContext Context => context;

        /// <summary>
        /// Load balancer config used currently by this graph. Use UpdateLoadBalancerConfig method to change it
        /// </summary>
        public LoadBalancerConfig CurrentLoadBalancerConfig => aiGraphConfig.loadBalancerConfig;

        /// <summary>
        /// Iterates on all subgraphs recursively
        /// </summary>
        public IEnumerable<AiGraph> SubGraphs
        {
            get
            {
                if (subGraphsRecursive.Count == 0)
                    foreach (var subGraph in subGraphs)
                    {
                        subGraphsRecursive.Add(subGraph);
                        subGraphsRecursive.AddRange(subGraph.SubGraphs);
                    }

                foreach (var aiGraph in subGraphsRecursive) yield return aiGraph;
            }
        }

        public bool IsRuntimeDebugGraph
        {
            get => isRuntimeDebugGraph;
            private set => isRuntimeDebugGraph = value;
        }

        private bool SubGraphBlockingTask
        {
            get => subGraphBlockingTask;
            set
            {
                subGraphBlockingTask = value;
                var parent = parentGraph;
                if (parent != null) parent.SubGraphBlockingTask = value;
            }
        }

        /// <summary>
        /// Is this graph registered to load balancing system and running?
        /// </summary>
        public bool IsRunning => isRunning;

        #endregion

        #region Public methods

        /// <summary>
        /// Changes load balancer config, and updates it in load balancing system
        /// </summary>
        public void UpdateLoadBalancerConfig(LoadBalancerConfig _loadBalancerConfig)
        {
            aiGraphConfig.loadBalancerConfig = _loadBalancerConfig;
            RegisterToLoadBalancingSystem();
        }

        /// <summary>
        /// Registers this ai graph loop to load balancing system.
        /// </summary>
        /// <param name="_newGraphConfig">If null, graph config from inspector will be used</param>
        public void RegisterToLoadBalancingSystem(AiGraphConfig _newGraphConfig = null)
        {
            //UnregisterFromLoadBalancingSystem();

            if (_newGraphConfig == null) _newGraphConfig = aiGraphConfig;

            aiGraphConfig = _newGraphConfig;

            if (aiGraphConfig.overrideGraphVariablesForNestedGraphs)
                foreach (var subGraph in SubGraphs)
                    subGraph.GraphAiVariables = GraphAiVariables;

            if (!aiGraphConfig.useCustomLoadBalancerConfig)
            {
                if (aiGraphConfig.scalableLoadBalancing)
                    LB.Register(this, updateGraphAction,
                        new LoadBalancerConfig(LoadBalancerType.XtimesPerSecondWithYLimit, aiGraphConfig.updateFrequency,
                            _valueY: aiGraphConfig.maxAllowedUpdateFrequency));
                else LB.Register(this, updateGraphAction, aiGraphConfig.updateFrequency);
            }
            else
            {
                LB.Register(this, updateGraphAction, aiGraphConfig.loadBalancerConfig);
            }

            isRunning = true;
        }

        public void UnregisterFromLoadBalancingSystem()
        {
            LB.Unregister(this);
            isRunning = false;
        }

        /// <summary>
        /// Updates context for all graph elements, including subgraphs
        /// </summary>
        public void UpdateContext(IContext _newContext)
        {
            context = _newContext;
            foreach (var node in nodes)
            {
                var sn = node as SmartAiNode;
                if (sn == null) continue;
                sn.Context = _newContext;
            }

            onContextUpdated?.Invoke();
        }

        /// <summary>
        /// For internal use only
        /// </summary>
        public void UpdateAiGraphForAllElements()
        {
            foreach (var monoNode in nodes)
            {
                var sn = monoNode as SmartAiNode;
                if (sn == null) continue;
                sn.AiGraph = this;
                sn.graph = this;
                foreach (var allGraphElement in sn.GetAllGraphElements())
                    allGraphElement.AiGraph = this;
            }
        }

        /// <summary>
        /// Returns all IAiGraphElement from this AiGrap and optionally also subgraphs
        /// </summary>
        /// <param name="includeSubgraphs">Should nested(subgraphs) graphs be included in search? Recursive</param>
        /// <returns></returns>
        public IAiGraphElement[] GetAllGraphElements(bool includeSubgraphs = false)
        {
            var list = new List<IAiGraphElement>();
            foreach (var node in nodes)
            {
                var aiNode = node as SmartAiNode;
                if (aiNode == null) continue;
                list.AddRange(aiNode.GetAllGraphElements());
            }

            if (includeSubgraphs)
                foreach (var subGraph in SubGraphs)
                    list.AddRange(subGraph.GetAllGraphElements(true));

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateGameObjectNames()
        {
            foreach (var allGraphElement in GetAllGraphElements()) allGraphElement.UpdateGameObjectName();
        }

        #endregion

        #region Not public methods

        protected override void OnDestroy()
        {
            for (var i = 0; i < subGraphsRecursive.Count; i++)
            {
                var aiGraph = subGraphsRecursive[i];
                if (aiGraph == null) continue;
                Destroy(aiGraph.gameObject);
            }
        }

        private void UpdateGraph(float _deltaTime, bool gettingBack = false)
        {
            if ((blockingTask || SubGraphBlockingTask) && !gettingBack) return;
            //Debug.LogError($"Running task {lastTask} didn't finished before next graph update!");

            onGraphUpdate?.Invoke();

            //bool hasTask = false;
            // this loop controls how many utilities without task we can 'skip' in one go to not slow down ai decision making process
            // for debugging purposes its best to set it at 1 so it wont skip any utilities
            for (var i = 0; i < GraphStepsPerUpdate; i++)
            {
                // just all null checks and shit to find first stage
                if (nodes.Length == 0) return;

                if (currentNode == null)
                {
                    currentNode = RootNode;
#if UNITY_EDITOR
                    debugValues.Clear();
                    winners.Clear();
                    winNodes.Clear();
                    lastNode = null;

                    tasks.Clear();

                    for (var index = 0; index < subGraphsRecursive.Count; index++)
                    {
                        var subGraph = subGraphsRecursive[index];
                        subGraph.debugValues.Clear();
                        subGraph.winners.Clear();
                        subGraph.winNodes.Clear();
                        subGraph.lastNode = null;
                    }
#endif
                }

                if (currentNode == null) return;

                if (blockingTask) goto tasks;

                lastNode = currentNode;

                // we're in stage node?
                if (currentNode is StageNode stageNode)
                {
                    currentStage = stageNode.stage;
                    // we have first stage, evaluate it! 
                    var utility = currentStage.Select(_deltaTime);
                    if (utility == null)
                    {
                        portIndexOfWinner = -1;
                        currentNode = null;
                        break;
                    }

                    portIndexOfWinner = stageNode.GetIndexOfUtility(utility);
                    tasks.AddRange(utility.tasks);

#if UNITY_EDITOR
                    winners.Add(utility);
                    winNodes.Add(currentNode);
#endif
                }
                // we're not in stage node, we still have to add it to win nodes so itll get highlighted in debug
                else if (currentNode is GraphNode gn)
                {
#if UNITY_EDITOR
                    winNodes.Add(currentNode);
#endif
                    if (gn.graphReference != null)
                    {
                        //reset current stuff because we cant continue after going to other graph node
                        currentNode = null;
                        portIndexOfWinner = -1;
                        gn.graphReference.UpdateGraph(_deltaTime);
                    }

                    break;
                }
                else if (currentNode is ConditionNode conditionNode)
                {
#if UNITY_EDITOR
                    winNodes.Add(currentNode);
#endif
                    // if no scorer, always true
                    if (conditionNode.scorer == null)
                    {
                        portIndexOfWinner = 0;
                    }
                    else
                    {
                        var score = conditionNode.scorer.Score(_deltaTime);
                        portIndexOfWinner = score > 0 ? 0 : 1;
                    }

                    conditionNode.lastWinner = portIndexOfWinner;
#if UNITY_EDITOR
                    winners.Add(currentNode);
#endif
                    tasks.AddRange(portIndexOfWinner == 0 ? conditionNode.trueTasks : conditionNode.falseTasks);
                }

                tasks:
                foreach (var task in tasks)
                {
                    if (blockingTask)
                    {
                        if (task == lastTask)
                        {
                            if (task.IsExecuting) return;
                            blockingTask = false;
                            if (hasParentGraph) parentGraph.SubGraphBlockingTask = false;
                        }

                        continue;
                    }

                    if (!task.Enabled) continue;

                    if (task.IsRunningTask)
                        // this caused corner case, if there is only one task and its blocking this block would never execute, causing complete
                        // block of graph execution
                        //if (task != lastTask)
                    {
                        if (task.StartExecutingInternal())
                        {
                            lastTask = task;
                            blockingTask = true;
                            if (hasParentGraph) parentGraph.SubGraphBlockingTask = true;
                            task.onStoppedExecuting = null;
                            task.onStoppedExecuting = gettingBackToBlockingTask;
                            blockingTaskStartTime = Time.time;
                            return;
                        }

                        continue;
                    }

                    task.Exec(_deltaTime);
                    lastTask = task;
                }

                tasks.Clear();

                // go to next connected node, can be null
                currentNode = currentNode.GetConnectedNode(portIndexOfWinner);

                // only one graph walkthrough/reset permitted per update
                if (currentNode == null) break;
            }
        }

        internal static AiGraph CreateAndInitializeGraph(bool _dontHideInHierarchy, int _instantiatedGraphGraphStepsPerUpdate, Object _owner,
            AiGraph _prefab, AiGraph _parentGraph = null)
        {
            var olName = _prefab.name;
            var instantiatedGraph = Instantiate(_prefab);
            instantiatedGraph.dontHideInHierarchy = _dontHideInHierarchy;
            instantiatedGraph.name = olName;
            instantiatedGraph.IsRuntimeDebugGraph = true;
            instantiatedGraph.owner = _owner;
            instantiatedGraph.GraphStepsPerUpdate = _instantiatedGraphGraphStepsPerUpdate;
            instantiatedGraph.ParentGraph = _parentGraph;
            if (!_dontHideInHierarchy) instantiatedGraph.gameObject.hideFlags = HideFlags.HideInHierarchy;

            // ! this causes deep in callstack on context update before context is set !
            //instantiatedGraph.RemoveNullsAndUpdateReferences();

            instantiatedGraph.gettingBackToBlockingTask = () => instantiatedGraph.UpdateGraph(UnityTime.Time - instantiatedGraph.blockingTaskStartTime, true);
            instantiatedGraph.updateGraphAction = _f => instantiatedGraph.UpdateGraph(_f);

            // instance all subgraphs
            foreach (var monoNode in instantiatedGraph.nodes)
            {
                var graphNode = monoNode as GraphNode;
                if (graphNode == null) continue;
                if (graphNode.graphReference == null) continue;
                var graphInstance = CreateAndInitializeGraph(_dontHideInHierarchy, _instantiatedGraphGraphStepsPerUpdate, _owner,
                    graphNode.graphReference, instantiatedGraph);
                graphNode.graphReference = graphInstance;
                instantiatedGraph.subGraphs.Add(graphInstance);
            }

            return instantiatedGraph;
        }

        // called after adding this component, only once 
        private void Reset() => name = gameObject.name;

        #endregion

        /// <summary>
        /// Called when graph context is updated
        /// </summary>
        public event Action onContextUpdated;

        /// <summary>
        /// Called before execution of graph logic
        /// </summary>
        public event Action onGraphUpdate;

#if UNITY_EDITOR
        // this is needed only for debugging

        /// <summary>
        /// For internal use only
        /// </summary>
        public List<Object> winners = new List<Object>();

        /// <summary>
        /// For internal use only
        /// </summary>
        public List<MonoNode> winNodes = new List<MonoNode>();

        /// <summary>
        /// Editor-time only!
        /// </summary>
        public Dictionary<object, float> debugValues = new Dictionary<object, float>();

        internal void AddDebugValue(object _value, float _score)
        {
            if (ParentGraph != null) ParentGraph.AddDebugValue(_value, _score);
            if (debugValues.ContainsKey(_value))
                debugValues[_value] += _score;
            else
                debugValues.Add(_value, _score);
        }

        /// <summary>
        /// Editor-time method only!
        /// </summary>
        public object CreateNewElement(Type _type, IAiGraphElement _graphElementParent)
        {
            var newGo = new GameObject();
            Undo.RegisterCreatedObjectUndo(newGo, "create new graph element");
            if (_graphElementParent == null)
                newGo.transform.SetParent(transform);
            else
                newGo.transform.SetParent(_graphElementParent.gameObject.transform);


            var c = Undo.AddComponent(newGo, _type);
            var graphElement = c as IAiGraphElement;
            graphElement?.UpdateGameObjectName();
            return c;
        }

        /// <summary>
        /// Editor-time method only!
        /// </summary>
        public void AssignRootNode() => RootNode = nodes.FirstOrDefault(n => n as SmartAiNode != null) as SmartAiNode;

        /// <summary>
        /// Adds node. Editor-time method only!
        /// </summary>
        public override INode AddNode(Type type)
        {
            MonoNode.graphHotfix = this;
            var node = CreateNewElement(type, null) as MonoNode;
            node.OnEnable();
            node.graph = this;
            var nodesList = new List<MonoNode>(nodes);
            nodesList.Add(node);
            nodes = nodesList.ToArray();
            Undo.RegisterCreatedObjectUndo(node.gameObject, "create node");
            var stageNode = node as SmartAiNode;
            // update context for when adding nodes at runtime
            if (stageNode != null && Application.isPlaying) stageNode.Context = Context;

            if (stageNode != null && RootNode == null) RootNode = stageNode;
            return node;
        }

        /// <summary>
        /// Removes node. Editor-time method only!
        /// </summary>
        public override void RemoveNode(INode node)
        {
            Undo.DestroyObjectImmediate((node as Component).gameObject);
            base.RemoveNode(node);
        }

        /// <summary>
        /// Returns null of no graph element is selected
        /// Editor-time method only!
        /// </summary>
        /// <returns></returns>
        public IAiGraphElement GetSelectedGraphElement()
        {
            var sn = Selection.activeObject as SmartAiNode;
            if (sn == null) return null;

            if (sn.selectedElement == null) return sn;
            return sn.selectedElement;
        }
#endif
    }
}