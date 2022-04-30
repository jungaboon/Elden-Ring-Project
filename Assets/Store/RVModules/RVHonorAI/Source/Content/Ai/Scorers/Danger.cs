// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.Contexts;
using RVModules.RVSmartAI.GraphElements;

namespace RVHonorAI.Content.AI.Scorers
{
    /// <summary>
    /// todo store allies list to remove all those wasteful isAlly checks 
    /// </summary>
    public class Danger : AiScorer
    {
        #region Fields

        private INearbyObjectsProvider nearbyObjectsProvider;
        private IRelationship relationship;
        private ITargetInfosProvider targetInfosProvider;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Calculated danger value using IDangerProvider from nearby objects \n" +
                                                        "Required context: INearbyObjectsProvider, IRelationship";

        #endregion

        #region Public methods

        public override float Score(float _deltaTime)
        {
            float allyStrength = 0;
            float enemyStrength = 0;

            foreach (var targetInfo in targetInfosProvider.TargetInfos) enemyStrength += targetInfo.Target.Danger;

            foreach (var nearbyObject in nearbyObjectsProvider.NearbyObjects)
            {
                if (nearbyObject == null) continue;
                var otherNpc = nearbyObject as IRelationship;
                var otherDanger = nearbyObject as IDangerProvider;
                if (otherNpc == null || otherDanger == null) continue;

                if (otherNpc.IsAlly(relationship)) allyStrength += otherDanger.Danger;
            }

            if (enemyStrength < 1) enemyStrength = 1;
            if (allyStrength < 1) allyStrength = 1;
            var danger = enemyStrength / allyStrength * score;
            AiGraph.GraphAiVariables.SetFloat("Danger", danger);
            return danger;
        }

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            nearbyObjectsProvider = ContextAs<INearbyObjectsProvider>();
            relationship = ContextAs<IRelationship>();
            targetInfosProvider = ContextAs<ITargetInfosProvider>();
            AiGraph.GraphAiVariables.AssureFloatExist("Danger");
        }

        #endregion
    }
}