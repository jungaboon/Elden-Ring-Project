// Created by Ronis Vision. All rights reserved
// 02.10.2020.

using System;
using System.Collections;
using UnityEngine;

namespace RVModules.RVSmartAI.GraphElements
{
    public interface IAiGraphElement
    {
        #region Properties

        string Name { get; set; }
        string Description { get; set; }
        AiGraph AiGraph { get; set; }
        IContext Context { get; set; }
        GameObject gameObject { get; }

        bool Enabled { get; }

        /// <summary>
        /// Returns only child graph elements, without self 
        /// </summary>
        IList ChildGraphElements { get; }

        #endregion

        #region Public methods

        /// <summary>
        /// Remove all nulls from own children, NOT recursive, so children of children wont get nulls removed
        /// </summary>
        void RemoveNulls();

        /// <summary>
        /// Find and add all child graph elements that are not referenced by this graph element
        /// </summary>
        void UpdateReferences();

        /// <summary>
        /// Updates game object accordingly to graph element name
        /// </summary>
        void UpdateGameObjectName();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_aiGraphElement"></param>
        void AssignSubSelement(IAiGraphElement _aiGraphElement);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_aiGraphElement"></param>
        /// <param name="_destroyGameObject"></param>
        void RemoveSubElement(IAiGraphElement _aiGraphElement, bool _destroyGameObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_destroyGameObjects"></param>
        void RemoveAllSubElements(bool _destroyGameObjects);

        /// <summary>
        /// all child recursive + self
        /// </summary>
        IAiGraphElement[] GetAllGraphElements();

        /// <summary>
        /// all child + self
        /// </summary>
        IAiGraphElement[] GetChildGraphElements();

        /// <summary>
        /// Can be null 
        /// </summary>
        IAiGraphElement GetParentGraphElement();

        /// <summary>
        /// Returns array of graph elements types that can be added to this graph element as child 
        /// </summary>
        Type[] GetAssignableSubElementTypes();

        #endregion
    }
}