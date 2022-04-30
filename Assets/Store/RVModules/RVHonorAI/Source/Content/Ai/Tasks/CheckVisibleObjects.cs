// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System;
using System.Collections.Generic;
using RVHonorAI.Combat;
using RVModules.RVLoadBalancer;
using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.Content.AI.Tasks;
using RVModules.RVSmartAI.Content.Scanners;
using RVModules.RVUtilities;
using RVModules.RVUtilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RVHonorAI.Content.AI.Tasks
{
    /// <summary>
    /// More advanced version of AiScanSurrounding that will fill NearbyObjects with only objects that are visible to our agent 
    /// </summary>
    public class CheckVisibleObjects : AiScanSurrounding
    {
        #region Fields

        public bool checkVisibilityWithRaycast;

        [SerializeField]
        private FloatProvider fovAngle;

        [SerializeField]
        private bool drawFovRaycastsDebugLines;

        [ConditionalHide(nameof(drawFovRaycastsDebugLines), hideInInspector = true)]
        [SerializeField]
        private float debugLinesDuration = .5f;

        private IFovMaskProvider fovMaskProvider;

        private List<Object> visibleObjects = new List<Object>();

        private GameObject ourChar;

        private IFovPositionProvider fovPositionProvider;

        [Tooltip("If true, scanning and scoring will be spread across multiple frames using below settings, and " +
                 "graph execution will be stopped until it finishes")]
        [SerializeField]
        private bool loadBalancedTask;

        [ConditionalHide(nameof(loadBalancedTask), hideInInspector = true)]
        [SerializeField]
        [Tooltip("How many fov checks should be done in one tick")]
        private int checksPerTick = 5;
        
        [ConditionalHide(nameof(loadBalancedTask), hideInInspector = true)]
        [SerializeField]
        private LoadBalancerConfig loadBalancerConfig;

        private int currentObjectId;
        [SerializeField]
        [Tooltip("Targets closer than this will automatically be visible")]
        private FloatProvider alwaysVisibleDistance;

        private List<Object> nearbyTemp = new List<Object>();

        #endregion

        #region Properties

        protected override LoadBalancerConfig RunningTaskLbc => loadBalancerConfig;

        public override bool IsRunningTask => loadBalancedTask;

        #endregion

        #region Not public methods

        protected virtual bool CheckIfIsInFovAngle(Vector3 myPos, Vector3 otherPos)
        {
            var angleFromUsToTarget = GetAngleBetweenTwoPoints(myPos.ToVector2(), otherPos.ToVector2());

            var ourAngle = GetAngleBetweenTwoPoints(Vector2.zero, (movement.Rotation * Vector3.forward).ToVector2());

            return !(Math.Abs(angleFromUsToTarget - ourAngle) > fovAngle);
        }

        private static float GetAngleBetweenTwoPoints(Vector2 v1, Vector2 v2)
        {
            var xDelta = v2.x - v1.x;
            var yDelta = v2.y - v1.y;

            return (float) Math.Atan2(yDelta, xDelta) * 180.0f / 3.1415f;
        }

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            fovMaskProvider = ContextAs<IFovMaskProvider>();
            fovPositionProvider = ContextAs<IFovPositionProvider>();
            ourChar = ContextAs<Component>().gameObject;
        }

        protected override bool StartExecuting()
        {
            Scan(false);

            if (NearbyObjects.Count == 0) return false;

            nearbyTemp.Clear();
            nearbyTemp.AddRange(NearbyObjects);

            // a little cheat to preserve visible objects from last scan for the time of scanning, 
            // thanks to this nearby objects array won't be cleared and filled again between runs
            NearbyObjects.Clear();
            NearbyObjects.AddRange(visibleObjects);

            visibleObjects.Clear();

            return true;
        }

        protected override void Executing(float _deltaTime)
        {
            for (var i = 0; i < checksPerTick; i++)
            {
                if (currentObjectId >= nearbyTemp.Count)
                {
                    currentObjectId = 0;
                    NearbyObjects.Clear();
                    if (hasDetectionCallbacks) FillNearbyObjectsWithCallbacks(visibleObjects);
                    else FillNearbyObjects(visibleObjects);
                    StopExecuting();
                    return;
                }

                CheckObjectVisible(nearbyTemp[currentObjectId]);
                currentObjectId++;
            }
        }

        protected override void Execute(float _deltaTime)
        {
            // do standard, AiScanSurrounding stuff
            Scan(false);

            visibleObjects.Clear();

            foreach (var nearbyObject in NearbyObjects) CheckObjectVisible(nearbyObject);

            if (hasDetectionCallbacks) FillNearbyObjectsWithCallbacks(visibleObjects);
            else FillNearbyObjects(visibleObjects);
        }

        protected virtual void CheckObjectVisible(Object nearbyObject)
        {
            var obj = nearbyObject as Component;
            if (obj == null) return;

            var objTransform = obj.transform;
            var targetPosition = objTransform.position;
            var ourPosition = movement.Position;
            var target = obj as ITarget;

            //add self to visible objs this is needed for danger calculations
            if (ourChar == objTransform.gameObject)
            {
                visibleObjects.Add(obj);
                return;
            }

            if (target != null) targetPosition = target.VisibilityCheckTransform.position;

            if (Vector3.Distance(targetPosition, ourPosition) < alwaysVisibleDistance)
            {
                visibleObjects.Add(obj);
                return;
            }

            var inAngle = CheckIfIsInFovAngle(ourPosition, targetPosition);
            if (!inAngle) return;

            if (checkVisibilityWithRaycast)
            {
                var rayStartPos = fovPositionProvider.FovPosition;
                var ray = new Ray(rayStartPos, targetPosition - rayStartPos);

                if (Physics.Raycast(ray, out var _hit, scanRange, fovMaskProvider.FovMask))
                {
                    var hitScannable = _hit.collider.GetComponent<IScannable>();
                    if (hitScannable != null && hitScannable.GameObject() == obj.gameObject)
                    {
#if UNITY_EDITOR
                        if (drawFovRaycastsDebugLines) Debug.DrawLine(rayStartPos, _hit.point, Color.green, debugLinesDuration);
#endif
                        visibleObjects.Add(obj);
                    }
#if UNITY_EDITOR
                    else if (drawFovRaycastsDebugLines)
                    {
                        Debug.DrawLine(rayStartPos, _hit.point, Color.red, debugLinesDuration);
                    }
#endif
                }
#if UNITY_EDITOR
                else if (drawFovRaycastsDebugLines)
                {
                    Debug.DrawLine(rayStartPos, targetPosition, Color.red, debugLinesDuration);
                }
#endif
            }
            else
            {
                visibleObjects.Add(obj);
            }
        }

        #endregion
    }
}