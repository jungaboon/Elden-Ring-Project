// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System.Collections.Generic;
using UnityEngine;

namespace RVModules.RVUtilities.Extensions
{
    public static class TransformExtensions
    {
        #region Public methods

        public static List<Transform> GetTransformsRecursive(this Transform _transform, List<Transform> _list = null)
        {
            if (_list == null) _list = new List<Transform>();

            _list.Add(_transform);

            for (var i = 0; i < _transform.childCount; i++)
            {
                var t = _transform.GetChild(i);
                GetTransformsRecursive(t, _list);
            }

            return _list;
        }

        #endregion
    }
}