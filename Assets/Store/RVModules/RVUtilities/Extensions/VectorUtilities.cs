// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;

namespace RVModules.RVUtilities.Extensions
{
    public static class VectorUtilities
    {
        #region Fields

        // cache
        private static Vector2 v2;
        private static Vector3 v3;

        #endregion

        #region Public methods

        /// <summary>
        /// For fast distance comparisons, for example checking closest object wiihtout need for actual distance
        /// </summary>
        public static float ManhattanDistance(this Vector2 _v1, Vector2 _v2)
        {
            float dist = 0;
            dist = Mathf.Abs(_v1.x - _v2.x);
            dist += Mathf.Abs(_v1.y - _v2.y);
            return dist;
        }

        /// <summary>
        /// Takes x and z components and calculate manhattan distance
        /// </summary>
        public static float ManhattanDistance2d(this Vector3 _v1, Vector3 _v2)
        {
            float dist = 0;
            dist = Mathf.Abs(_v1.x - _v2.x);
            dist += Mathf.Abs(_v1.z - _v2.z);
            return dist;
        }

        /// <summary>
        /// Just like casting, but takes v3.z element instead of y like default implementation
        /// </summary>
        /// <param name="_v1"></param>
        /// <param name="_vector3"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this Vector3 _v1)
        {
            v2.x = _v1.x;
            v2.y = _v1.z;
            return v2;
        }


        /// <summary>
        /// Just like casting, but takes v3.z element instead of y like default implementation
        /// </summary>
        /// <param name="_v1"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this Vector2 _v1)
        {
            v3.x = _v1.x;
            v3.z = _v1.y;
            return v3;
        }

        #endregion
    }
}