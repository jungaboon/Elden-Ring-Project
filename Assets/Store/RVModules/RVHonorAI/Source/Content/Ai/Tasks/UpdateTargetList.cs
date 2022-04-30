// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using System.Collections.Generic;
using RVHonorAI.Combat;
using RVModules.RVSmartAI.Content.AI.Contexts;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVHonorAI.Content.AI.Tasks
{
    /// <summary>
    /// Updates target and targetInfo lists from ITargetListProvider and ITargetInfosProvider
    /// </summary>
    public class UpdateTargetList : AiTask
    {
        #region Fields

        private ITargetInfosProvider targetInfoProvider;
        private INearbyObjectsProvider nearbyObjectsProvider;
        private IRelationship ourCharacter;
        public static ObjectPool<TargetInfo> targetInfoPool = new ObjectPool<TargetInfo>(() => new TargetInfo());
        private ITargetsDetectionCallbacks targetsDetectionCallbacks;

        private List<ITarget> newTargets = new List<ITarget>();

        [Tooltip("How long should TargetInfo be kept in ITargetInfosProvider.TargetInfos list after not being seen, in seconds")]
        [SerializeField]
        private FloatProvider targetNotSeenMemorySpan;

        private bool hasTargetDetectionCallbacks;

        private List<TargetInfo> tisToRemove = new List<TargetInfo>();

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            nearbyObjectsProvider = Context as INearbyObjectsProvider;
            targetInfoProvider = ContextAs<ITargetInfosProvider>();
            ourCharacter = ContextAs<IRelationship>();
            targetsDetectionCallbacks = GetComponentFromContext<ITargetsDetectionCallbacks>();
            hasTargetDetectionCallbacks = targetsDetectionCallbacks != null;
        }

        protected override void Execute(float _deltaTime)
        {
            var targetInfos = targetInfoProvider.TargetInfosDict;

            var time = UnityTime.Time;

            // get all targets not known earlier earlier
            newTargets.Clear();

            // find all targets that are enemies
            foreach (var o in nearbyObjectsProvider.NearbyObjects)
            {
                if (o == null) continue;

                var target = o as ITarget;
                if (target == null) continue;

                // already seen him
                if (targetInfos.ContainsKey(target))
                {
                    var targetInfo = targetInfos[target];
                    targetInfo.LastSeenTime = time;
                    targetInfo.LastSeenPosition = target.Transform.position;
                    if (targetInfo.Visible == false)
                    {
                        targetInfo.Visible = true;
                        if (hasTargetDetectionCallbacks) targetsDetectionCallbacks.OnTargetVisibleAgain?.Invoke(target);
                    }

                    continue;
                }

                var relationshipProvider = target as IRelationship;
                if (relationshipProvider == null) continue;

                if (ourCharacter.IsEnemy(relationshipProvider))
                {
                    newTargets.Add(target);
                    if (hasTargetDetectionCallbacks) targetsDetectionCallbacks.OnNewTargetDetected?.Invoke(target);
                }
            }

            foreach (var target in newTargets)
            {
                TargetInfo targetInfo = null;

                targetInfo = targetInfoPool.GetObject();
                targetInfo.Target = target;
                targetInfos.Add(targetInfo.Target, targetInfo);
                targetInfoProvider.TargetInfos.Add(targetInfo);

                targetInfo.LastSeenTime = time;
                targetInfo.LastSeenPosition = target.Transform.position;
                targetInfo.Visible = true;
            }

            foreach (var kvp in targetInfos)
            {
                var targetInfo = kvp.Value;

                if (!nearbyObjectsProvider.NearbyObjects.Contains(targetInfo.Target as Object) && targetInfo.Visible)
                {
                    targetInfo.Visible = false;
                    if (hasTargetDetectionCallbacks) targetsDetectionCallbacks.OnTargetNotSeenAnymore?.Invoke(kvp.Key);
                }

                if (time > targetInfo.LastSeenTime + targetNotSeenMemorySpan || targetInfo.Target as Object == null)
                {
                    tisToRemove.Add(targetInfo);
                    if (hasTargetDetectionCallbacks) targetsDetectionCallbacks.OnTargetForget?.Invoke(kvp.Key);
                }
            }

            foreach (var targetInfo in tisToRemove)
            {
                targetInfos.Remove(targetInfo.Target);
                targetInfoProvider.TargetInfos.Remove(targetInfo);
                targetInfo.OnDespawn();
            }

            tisToRemove.Clear();
        }

        #endregion
    }
}