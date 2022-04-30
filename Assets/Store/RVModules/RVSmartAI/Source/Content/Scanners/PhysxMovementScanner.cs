// Created by Ronis Vision. All rights reserved
// 27.10.2019.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RVModules.RVSmartAI.Content.Scanners
{
    /// <summary>
    /// Physx movement scanner. Scans for walkable and non accesible spots
    /// </summary>
    public class PhysxMovementScanner : MonoBehaviour, IMovementScanner
    {
        #region Fields

        /// <summary>
        /// Mask used for getting walkable positions
        /// </summary>
        [FormerlySerializedAs("walkableLayerMask")]
        [SerializeField]
        private LayerMask obstaclesLayerMask;

        /// <summary>
        /// Mask used for checking ground height
        /// </summary>
        [SerializeField]
        private LayerMask groundLayerMask;

        /// <summary>
        /// At which height shoot raycasts to scan positions for FindWalkablePositions
        /// </summary>
        [SerializeField]
        private float rayHeight = 3f;

        /// <summary>
        /// Scanner resolution; distance between each scan and how big radius of checkSphere will be used for scanning for obstacles
        /// Bigger resolution - less checks, better performance, worse precision (less positions will be scanned)
        /// To approx how many casts will called for scan:
        /// square(_range * 2 / walkablePositionsResolution)
        /// </summary>
        [FormerlySerializedAs("walkablePositionsResolution")]
        [SerializeField]
        private float scannerResolution = 2f;

        [SerializeField]
        [Tooltip("Will render lines in scene view - red when scanner hit terrain and hit obstacle(non walkable)," +
                 "and green when it hit terrain but didn't hit obstacle(walkable)")]
        private bool drawDebugRays;

        [SerializeField]
        private List<Vector3> nonBlockedPositions = new List<Vector3>();

        //private RaycastHit[] groundHitsResultBuffer = {new RaycastHit()};

        #endregion

        #region Public methods

        public List<Vector3> FindWalkablePositions(Vector3 _position, float _range)
        {
            var pos = Vector3.zero;
            nonBlockedPositions.Clear();
            var scansCount = (int) (_range / scannerResolution);
            var scanStart = -scansCount;

            for (var y = scanStart; y < scansCount + 1; y++)
            for (var x = scanStart; x < scansCount + 1; x++)
            {
                ScanPosition(_position, x, y);
            }

            return nonBlockedPositions;
        }

        private void ScanPosition(Vector3 _scanCenter, int x, int y)
        {
            Vector3 pos;
            //if (x == 0 && y == 0) return;
            pos.x = _scanCenter.x + x * scannerResolution;
            pos.y = _scanCenter.y + rayHeight;
            pos.z = _scanCenter.z + y * scannerResolution;

            // assure we're above walkable ground, and get height for scan
            if (!Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 1000, groundLayerMask)) return;
            pos.y = hit.point.y;

            if (Physics.CheckSphere(pos, scannerResolution * .5f, obstaclesLayerMask))
            {
#if UNITY_EDITOR
                if (drawDebugRays)
                    Debug.DrawRay(pos, Vector3.up * scannerResolution, Color.red, 1);
#endif
                return;
            }
#if UNITY_EDITOR
            if (drawDebugRays) Debug.DrawRay(pos, Vector3.up * scannerResolution, Color.green, 1);
#endif

            nonBlockedPositions.Add(pos);
        }

        /// <summary>
        /// Set position for load balanced scanning (x times per call instead of all at once) 
        /// </summary>
        public void SetPositionForLoadBalancedScanning(Vector3 _position, float _range)
        {
            nonBlockedPositions.Clear();
            centerPosLb = _position;
            scansCountLb = (int) (_range / scannerResolution);
            xLb = yLb = -scansCountLb;
        }

        private Vector3 centerPosLb;
        private int scansCountLb;
        private int xLb, yLb;

        public bool FindWalkablePositionsLoadBalanced(out List<Vector3> walkablePositions, int _scans = 1)
        {
            var scanStart = -scansCountLb;

            for (int i = 0; i < _scans; i++)
            {
                ScanPosition(centerPosLb, xLb, yLb);
                xLb++;

                if (xLb >= scansCountLb + 1)
                {
                    xLb = scanStart;
                    yLb++;

                    if (yLb >= scansCountLb + 1)
                    {
                        walkablePositions = nonBlockedPositions;
                        return true;
                    }
                }
            }

            walkablePositions = nonBlockedPositions;
            return false;
        }

        #endregion
    }
}