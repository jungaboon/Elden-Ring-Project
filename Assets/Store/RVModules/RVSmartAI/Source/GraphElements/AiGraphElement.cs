// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#pragma warning disable 252,253

namespace RVModules.RVSmartAI.GraphElements
{
    /// <summary>
    /// Base class for all elements in AiGraph
    /// </summary>
    public abstract class AiGraphElement : MonoBehaviour, IAiGraphElement
    {
        #region Fields

        public new bool enabled = true;

        private IContext context;

        [SerializeField]
        [SmartAiHideInInspector]
        private string graphElementName;

        [SerializeField]
        [SmartAiHideInInspector]
        protected string description;

        [SerializeField]
        [HideInInspector]
        private AiGraph aiGraph;

//        [SerializeField]
//        private IAiGraphElement parentGraphElement;

        #endregion

        #region Properties

        protected virtual string DefaultDescription => "";

        public AiGraph AiGraph
        {
            get
            {
                if (aiGraph == null) aiGraph = GetComponentInParent<AiGraph>();
                return aiGraph;
            }
            set => aiGraph = value;
        }

        /// <summary>
        /// Updates context for this graph element and all it's children
        /// </summary>
        public IContext Context
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
                    //foreach (var dataProviderBase in DataProvidersContextUpdate) dataProviderBase.Context = value;
                    allGraphElement.Context = value;
                }

                OnContextUpdated();
            }
        }

        public virtual string Name
        {
            get => string.IsNullOrEmpty(graphElementName) ? ToString() : graphElementName;
            set
            {
                graphElementName = value;
                UpdateGameObjectName();
            }
        }

        public virtual string Description
        {
            get => description;
            set => description = value;
        }

        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public virtual IList ChildGraphElements => null;

        #endregion

        #region Public methods

        /// <summary>
        /// Shortcut for casting context to desired type
        /// </summary>
        protected T ContextAs<T>() where T : class => Context as T;

        /// <summary>
        /// Shortcut useful to get some other component living on the same GameObject as context
        /// </summary>
        /// <typeparam name="T">Type of component to get, can be interface</typeparam>
        /// <returns>Component of type T or null if there is no such component</returns>
        protected T GetComponentFromContext<T>() where T : class
        {
            var comp = Context as Component;
            return comp == null ? null : comp.GetComponent<T>();
        }

        public void UpdateGameObjectName()
        {
            if (AiGraph == null) return;
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfAnyPrefab(this)) return;
#endif

            var n = "";
//            var parent = GetParentGraphElement();
//            if (parent as Object != null)
//            {
//                n = parent.Name;
//                if (string.IsNullOrEmpty(parent.Name)) n = parent.GetType().Name;
//                n += "_";
//            }

            n += graphElementName;
            n += $" ({GetType().Name})";

            gameObject.name = n;
        }

        public virtual void RemoveNulls()
        {
        }

        // if graph element is destroyed, so is his go, so we dont need to explicitly destroy his dataProbviders, as they are always on the same go
//        /// <summary>
//        /// Call if overriden
//        /// </summary>
//        protected virtual void OnDestroy()
//        {
//            DataProviderBase.DestroyReferencedDataProviders(this);
//        }

        public void UpdateReferences()
        {
            var assignedChildren = GetChildGraphElements();

            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var childGraphElement = child.GetComponent<IAiGraphElement>();
                if (childGraphElement == null)
                {
                    Debug.LogError("Detected graph element child GO with no IAiGraphElement!", this);
                    continue;
                }

                if (!assignedChildren.Contains(childGraphElement)) AssignSubSelement(childGraphElement);
            }
        }

        /// <summary>
        /// Returns children graph elements including this
        /// </summary>
        /// <returns></returns>
        public virtual IAiGraphElement[] GetChildGraphElements() => new IAiGraphElement[] {this};

//        public IAiGraphElement GetParentGraphElement() => parentGraphElement ?? (parentGraphElement = transform.parent.GetComponent<IAiGraphElement>());
        public IAiGraphElement GetParentGraphElement() => transform.parent == null ? null : transform.parent.GetComponent<IAiGraphElement>();

        public void Remove(bool _destroyGameObject)
        {
            var parentGraphElement = GetParentGraphElement();
            if (parentGraphElement != null)
                parentGraphElement.RemoveSubElement(this, _destroyGameObject);
            else if (_destroyGameObject) Destroy(gameObject);
        }

        /// <summary>
        /// Returns all children graph elements recursively (children off children etc) including this
        /// </summary>
        /// <returns></returns>
        public virtual IAiGraphElement[] GetAllGraphElements() => new IAiGraphElement[] {this};

        /// <summary>
        /// Returns Types of graph elements that can be assigned as children
        /// </summary>
        /// <returns></returns>
        public virtual Type[] GetAssignableSubElementTypes() => new Type[0];

        /// <summary>
        /// Needs to be called when overriden
        /// </summary>
        /// <param name="_aiGraphElement"></param>
        public virtual void AssignSubSelement(IAiGraphElement _aiGraphElement)
        {
            // dont ever try to assign context at edit time!
            if (!Application.isPlaying) return;
            foreach (var graphElement in _aiGraphElement.GetAllGraphElements())
                graphElement.Context = Context;

//            var aiGraphElement = _aiGraphElement as AiGraphElement;
//            if (aiGraphElement != null) aiGraphElement.parentGraphElement = this;
        }

        public void RemoveSubElement(IAiGraphElement _aiGraphElement, bool _destroyGo)
        {
            if (_aiGraphElement == null) return;
            if (ChildGraphElements == null) return;
            if (!ChildGraphElements.Contains(_aiGraphElement)) return;
            ChildGraphElements.Remove(_aiGraphElement);
            OnSubGraphElementRemoved(_aiGraphElement);
            if (_aiGraphElement.gameObject != null && _destroyGo) Destroy(_aiGraphElement.gameObject);
        }

        public virtual void RemoveAllSubElements(bool _destroyGos)
        {
            if (ChildGraphElements == null) return;
            // needs to have copy so we wont modify collection we foreach on
            var children = ChildGraphElements.Cast<object>().ToArray();
            foreach (var childGraphElement in children) RemoveSubElement((IAiGraphElement) childGraphElement, _destroyGos);
        }

        /// <summary>
        /// Returns true if destroy was possible
        /// </summary>
        public bool Destroy()
        {
#if UNITY_EDITOR
            if (CanBeRemoved())
            {
                DestroyMethod(gameObject);
                return true;
            }

            return false;
#else
            Destroy(gameObject);
            return true;
#endif
        }

        public override string ToString()
        {
#if UNITY_EDITOR
            return ObjectNames.NicifyVariableName(GetType().Name);
#else
            return name;
#endif
        }

        #endregion

        #region Not public methods

        protected virtual void OnSubGraphElementRemoved(IAiGraphElement _aiGraphElement)
        {
        }

        protected virtual void OnContextUpdated()
        {
        }

        protected virtual void Reset()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(Name)) Name = ToString();
            if (string.IsNullOrEmpty(description)) description = DefaultDescription;
#endif
        }

        #endregion

#if UNITY_EDITOR
        public bool CanBeRemoved() =>
            AiGraph != null && AiGraph.IsRuntimeDebugGraph || !IsParentPartOfPrefab() ||
            PrefabUtility.IsAddedGameObjectOverride(gameObject);

        internal bool IsParentPartOfPrefab() => transform.parent != null && PrefabUtility.IsPartOfAnyPrefab(transform.parent.gameObject);

        private void DestroyMethod(Object _object)
        {
            if (Application.isPlaying)
                Destroy(_object);
            else
                Undo.DestroyObjectImmediate(_object);
        }
#endif
    }
}