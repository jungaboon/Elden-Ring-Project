// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using System.Reflection;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public class DataProviderBase : MonoBehaviour
    {
        #region Fields

        protected AiGraph aiGraph;

        #endregion

        #region Properties

        /// <summary>
        /// Data provider description. Not editable from inspector, shown as info help box
        /// </summary>
        public virtual string Description => "";

        /// <summary>
        /// Context from AiGraph
        /// </summary>
        protected IContext Context => aiGraph.Context;

        /// <summary>
        /// AiGraph this DataProvider is part of
        /// </summary>
        protected AiGraph AiGraph => aiGraph;

        /// <summary>
        /// Check if data can be safely provided
        /// </summary>
        public virtual bool ValidateData() => true;

        #endregion

        #region Public methods

        public void OnDestroy()
        {
#if UNITY_EDITOR
            DestroyReferencedDataProviders(this);
#endif
        }

        /// <summary>
        /// Since data providers don't have their own dedicated game objects like AiGraphElement, we need to explicitly destroy all referenced(created)
        /// by our data provider other data providers
        /// Note: super slow, creates lot of garbage, dont use at runtime !
        /// </summary>
        public static void DestroyReferencedDataProviders(Component _component)
        {
            if (Application.isPlaying || _component == null) return;

            // find all fields, cast on DataProviderBase, if not null, destroy 
            var fields = _component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields)
            {
                var dataProvider = fieldInfo.GetValue(_component) as DataProviderBase;
                if (dataProvider == null) continue;
                DestroyReferencedDataProviders(dataProvider);
                DestroyImmediate(dataProvider);
            }
        }

        #endregion

        #region Not public methods

        /// <summary>
        /// Called when graph context is updated
        /// </summary>
        protected virtual void OnContextUpdated()
        {
        }

        protected virtual void Start()
        {
            aiGraph = GetComponent<IAiGraphElement>().AiGraph;
            aiGraph.onContextUpdated += OnContextUpdated;

            if(Application.isPlaying) OnContextUpdated();
        }

        #endregion
    }
}