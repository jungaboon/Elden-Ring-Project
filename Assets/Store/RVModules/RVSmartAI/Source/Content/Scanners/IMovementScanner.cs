// Created by Ronis Vision. All rights reserved
// 23.08.2019.

using System.Collections.Generic;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.Scanners
{
    public interface IMovementScanner
    {
        #region Public methods

        List<Vector3> FindWalkablePositions(Vector3 _position, float _range);

        #endregion

        /// <summary>
        /// Set position for load balanced scanning 
        /// </summary>
        void SetPositionForLoadBalancedScanning(Vector3 _position, float _range);

        /// <summary>
        /// Load balanced scanning. Will returnw true when all desired positions are scanned
        /// </summary>
        bool FindWalkablePositionsLoadBalanced(out List<Vector3> walkablePositions, int scans = 1);
    }
}