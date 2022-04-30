// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using UnityEngine;

namespace RVHonorAI
{
    [Serializable] public class Waypoint
    {
        #region Fields

        public float radius;
        public Vector3 position;

        #endregion

        public Waypoint()
        {
        }

        /// <summary>
        /// Create copy from other wp
        /// </summary>
        /// <param name="_waypoint"></param>
        public Waypoint(Waypoint _waypoint)
        {
            radius = _waypoint.radius;
            position = _waypoint.position;
        }
    }
}