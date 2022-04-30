// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Contexts
{
    public interface IWaypointsProvider
    {
        #region Properties

        /// <summary>
        /// Returns total count of waypoints
        /// </summary>
        int WaypointsCount { get; }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns absolute position of waypoint with <paramref name="_id"/>
        /// </summary>
        /// <param name="_id">Index of waypoint</param>
        /// <returns>Absolute position of waypoint</returns>
        Vector3 GetWaypointPosition(int _id);

        #endregion
    }
}