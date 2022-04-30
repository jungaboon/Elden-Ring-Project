// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using System.Collections.Generic;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using UnityEngine;
using UnityEngine.Serialization;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    /// <summary>
    /// Fills AiAgentGenericContext.nearbyObjects using IEnvironmentScanner in defined scanRange.
    /// Uses IObjectDetectionCallbacks if any object on Context GameObject implements it
    /// </summary>
    public class AiScanSurrounding : AiAgentTask
    {
        #region Fields

        [FormerlySerializedAs("scanRangeProvider")]
        [Tooltip("Scan radius, in meters")]
        public FloatProvider scanRange;

//        /// <summary>
//        /// How much time must pass before this AiTask can be executed
//        /// </summary>
//        [Header("Time interval between AiTask execution, in seconds")]
//        public float callInterval = 2f;
//
//        public DateTime lastCallTime;

        protected IObjectDetectionCallbacks detectionCallbacks;
        protected bool hasDetectionCallbacks;

//        protected bool called;

        protected List<Object> toRemove = new List<Object>();
        protected HashSet<Object> lastDetectedObjects = new HashSet<Object>();
        protected HashSet<Object> hash = new HashSet<Object>();

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            detectionCallbacks = GetComponentFromContext<IObjectDetectionCallbacks>();
            hasDetectionCallbacks = detectionCallbacks != null;
        }

        protected override void Execute(float _deltaTime) => Scan(hasDetectionCallbacks);

        protected void Scan(bool withCallbacks)
        {
            // we cant scan if we dont have any environmentScanner
            if (environmentScanner == null) return;

//            called = false;
//
//            // exit early if we called it too soon
//            if ((DateTime.Now - lastCallTime).TotalSeconds < callInterval) return;
//            lastCallTime = DateTime.Now;
//
//            called = true;

            var scannedObjects = environmentScanner.ScanEnvironment(movement.Position, scanRange);

            if (!withCallbacks)
                FillNearbyObjects(scannedObjects);
            else
                FillNearbyObjectsWithCallbacks(scannedObjects);
        }

        protected void FillNearbyObjects(List<Object> objects)
        {
            NearbyObjects.Clear();

            for (var i = 0; i < objects.Count; i++)
            {
                var entity = objects[i];
                AddToNearbyObjects(entity);
            }
        }

        protected void FillNearbyObjectsWithCallbacks(List<Object> objects)
        {
            NearbyObjects.Clear();
            hash.Clear();

            for (var i = 0; i < objects.Count; i++)
            {
                var entity = objects[i];
                AddToNearbyObjectsDetectionCallbacks(entity);
            }

            HandleDetectionCallbacks();
        }

        protected void HandleDetectionCallbacks()
        {
            // remove objects that are no longer detected by scanner
            foreach (var lastDetectedObject in lastDetectedObjects)
                if (!hash.Contains(lastDetectedObject))
                {
                    toRemove.Add(lastDetectedObject);
                    // removed from detected range callback
                    detectionCallbacks.OnObjectNotDetectedAnymore?.Invoke(lastDetectedObject);
                }

            foreach (var o in toRemove) lastDetectedObjects.Remove(o);
            toRemove.Clear();
        }

        protected virtual void AddToNearbyObjects(Object _object) => NearbyObjects.Add(_object);

        protected virtual void AddToNearbyObjectsDetectionCallbacks(Object _object)
        {
            NearbyObjects.Add(_object);

            hash.Add(_object);
            // is this new object?
            if (!lastDetectedObjects.Contains(_object))
            {
                lastDetectedObjects.Add(_object);
                // new detected object callback
                detectionCallbacks.OnNewObjectDetected?.Invoke(_object);
            }
        }

        #endregion
    }
}