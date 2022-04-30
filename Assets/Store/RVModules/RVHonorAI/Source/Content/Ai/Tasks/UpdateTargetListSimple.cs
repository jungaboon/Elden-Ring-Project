// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.Content.AI.Contexts;
using RVModules.RVSmartAI.GraphElements;

namespace RVHonorAI.Content.AI.Tasks
{
    /// <summary>
    /// Compared to UpdateTargetList this one dont update memory stuff (TargetInfo)
    /// and works on ITargetListProvider insted of ITargetInfosProvider
    /// </summary>
    public class UpdateTargetListSimple : AiTask
    {
        #region Fields

        private ITargetListProvider targetListProvider;
        private INearbyObjectsProvider nearbyObjectsProvider;
        private IRelationship ourCharacter;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            targetListProvider = Context as ITargetListProvider;
            nearbyObjectsProvider = Context as INearbyObjectsProvider;
            ourCharacter = ContextAs<IRelationship>();
        }

        protected override void Execute(float _deltaTime)
        {
            var targets = targetListProvider.Targets;
            targets.Clear();

            foreach (var o in nearbyObjectsProvider.NearbyObjects)
            {
                var target = o as ITarget;
                var relationship = target as IRelationship;
                if (relationship == null) continue;
                if (relationship.IsEnemy(ourCharacter)) targets.Add(target);
            }
        }

        #endregion
    }
}