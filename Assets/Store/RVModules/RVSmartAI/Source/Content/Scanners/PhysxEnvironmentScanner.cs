// Created by Ronis Vision. All rights reserved
// 23.08.2019.

using System.Collections.Generic;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.Scanners
{
    /// <summary>
    /// Use Physx for spatial queries
    /// For object to be added it has to have MonoBehaviour implementing IScannable on the same game object as it's collider
    /// </summary>
    public class PhysxEnvironmentScanner : MonoBehaviour, IEnvironmentScanner
    {
        #region Fields

        /// <summary>
        /// Mask used for scanning environment
        /// </summary>
        [SerializeField]
        private LayerMask scannerLayerMask;

        /// <summary>
        /// How many objects will scanner be able to get in one scan
        /// </summary>
        [Tooltip("How many objects will scanner be able to get in one scan")]
        [SerializeField]
        private int bufferSize = 100;

        // buffer, serialized to show in inspector
        [Tooltip("All colliders detected by scanner")]
        [SerializeField]
        private Collider[] scanResults;

        [Tooltip("All IScannable detected by scanner")]
        [SerializeField]
        private List<Object> objects = new List<Object>();

        #endregion

        #region Public methods

        public List<Object> ScanEnvironment(Vector3 _position, float _range)
        {
            objects.Clear();

            // clear buffer
            for (var i = 0; i < scanResults.Length; i++) scanResults[i] = null;

            Physics.OverlapSphereNonAlloc(_position, _range, scanResults, scannerLayerMask);
            for (var index = 0; index < scanResults.Length; index++)
            {
                var result = scanResults[index];
                if (result == null) continue;
                var scannable = result.GetComponent<IScannable>();
                if (scannable == null) continue;
//                objects.Add(scannable.GetObject);
                objects.Add(scannable.Component());
            }

            return objects;
        }

        #endregion

        #region Not public methods

        private void Awake()
        {
            scanResults = new Collider[bufferSize];
        }

        #endregion
    }
}