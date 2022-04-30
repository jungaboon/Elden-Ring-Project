// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using System;
using System.Collections;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;
using XNode;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI.Nodes
{
    /// <summary>
    /// Base type for all SmartAi nodes used in AiGraph
    /// </summary>
    public abstract class SmartAiNode : MonoNode, IAiGraphElement
    {
        #region Fields

        public bool expanded = true;

        public AiGraphElement selectedElement;

        [SerializeField]
        private string description;

        [SerializeField]
        private AiGraph aiGraph;

        private IContext context;

        #endregion

        #region Properties

        public virtual IContext Context
        {
            get => context;
            set
            {
                context = value;
                // update context for all children
                foreach (var allGraphElement in GetChildGraphElements())
                {
                    // avoid stack overflow
                    if (allGraphElement == this) continue;
                    allGraphElement.Context = value;
                }

                context = value;
            }
        }

        public bool Enabled => true;
        public abstract IList ChildGraphElements { get; }

        public abstract int OutputsCount { get; }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public AiGraph AiGraph
        {
            get
            {
                if (aiGraph == null) aiGraph = GetComponentInParent<AiGraph>();
                return aiGraph;
            }
            set => aiGraph = value;
        }

        public bool IsRoot => AiGraph?.RootNode == this;

        #endregion

        #region Public methods

        public void RemoveNulls()
        {
        }

        public void UpdateReferences()
        {
        }

        public void UpdateGameObjectName() => gameObject.name = $"<<<NODE>>> {ToString().ToUpper()} <<<NODE>>>";
        public abstract void AssignSubSelement(IAiGraphElement _aiGraphElement);
        public abstract void RemoveSubElement(IAiGraphElement _aiGraphElement, bool _destroyGameObject);

        public virtual void RemoveAllSubElements(bool _destroyGameObjects)
        {
            foreach (var childGraphElement in ChildGraphElements)
            {
                var ge = childGraphElement as IAiGraphElement;
                if (ge as Object == null) continue;
                RemoveSubElement(ge, _destroyGameObjects);
            }
        }

        public override object GetValue(NodePort port) => null;

        public bool Destroy()
        {
            if (Application.isPlaying)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
            return true;
        }

        public abstract IAiGraphElement[] GetAllGraphElements();
        public abstract IAiGraphElement[] GetChildGraphElements();

        public IAiGraphElement GetParentGraphElement() => null;

        public void Remove(bool _destroyGameObject)
        {
            if (!_destroyGameObject) return;
            Destroy(gameObject);
        }

        public abstract Type[] GetAssignableSubElementTypes();

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name)) return GetType().Name;
            return Name;
        }


#if UNITY_EDITOR
        [ContextMenu("Set as root node")]
        public void SetRoot() => AiGraph.RootNode = this;
#endif

        public virtual SmartAiNode GetConnectedNode(int portIndex)
        {
            // combination to avoid gc
            var i = 0;
            foreach (var p in ports.Values)
            {
                // loop on outputs
                if (!p.IsOutput) continue;
                if (i == portIndex) return p.Connection?.node as SmartAiNode;
                i++;
            }

            return null;
        }

        #endregion

        #region Not public methods

        protected override void Init() => AiGraph = graph as AiGraph;

        protected void RemoveElement(IAiGraphElement _aiGraphElement, bool _destroyGameObject)
        {
            if (_destroyGameObject) DestroyImmediate(_aiGraphElement.gameObject);
            else DestroyImmediate(_aiGraphElement as Object);
        }

        #endregion
    }
}