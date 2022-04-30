// Created by Ronis Vision. All rights reserved
// 28.03.2021.

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVModules.RVSmartAI
{
    /// <summary>
    /// Defines interface that will always be implemented on Unity Component
    /// Allows for convenient usage of extension methods to get Unity Object directly from interface, without casting
    /// </summary>
    public interface IComponent
    {
    }

    public static class IComponentInterfaceExt
    {
        #region Public methods

        /// <summary>
        /// Cast to unity Object and checks if its null
        /// </summary>
        /// <returns></returns>
        public static bool IsObjectNull(this IComponent _component) => _component.Object() == null;

        /// <summary>
        /// Returns this object casted to unity Object
        /// </summary>
        public static Object Object(this IComponent _object) => _object as Object;

        /// <summary>
        /// Returns this object casted to unity Component
        /// </summary>
        public static Component Component(this IComponent _component)
        {
            var component = _component as Component;
            return component == null ? null : component;
        }

        /// <summary>
        /// Returns GameObject this component is added on
        /// </summary>
        public static GameObject GameObject(this IComponent _component)
        {
            var component = _component as Component;
            return component == null ? null : component.gameObject;
        }

        /// <summary>
        /// Returns Transform this component is added on
        /// </summary>
        public static Transform Transform(this IComponent _component)
        {
            var component = _component as Component;
            return component == null ? null : component.transform;
        }

        #endregion
    }
}