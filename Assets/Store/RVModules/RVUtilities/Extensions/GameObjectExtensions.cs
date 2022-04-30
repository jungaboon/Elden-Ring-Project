// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities.Extensions
{
    public static class GameObjectExtensions
    {
        #region Public methods

        public static T AddOrGetComponent<T>(this GameObject _gameObject) where T : Component
        {
            var c = _gameObject.GetComponent<T>();
            if (c == null)
                c = _gameObject.AddComponent<T>();
            return c;
        }

        #endregion
    }
}