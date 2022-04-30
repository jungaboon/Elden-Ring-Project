// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.Content.AI.Scorers;
using UnityEngine;

namespace RVHonorAI.Content.AI.Obsolete
{
    public class Obsolete_DistToPositionVsRangeScorerParams : AiAgentScorerCurveParams<Vector3>
    {
        #region Fields

        private IAttacker attacker;

        [SerializeField]
        private Vector3Provider positionProvider;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            this.ObsoleteGraphElementUsedError(typeof(Obsolete_DistanceToPositionDividedByRange));
            base.OnContextUpdated();
            attacker = ContextAs<Component>().GetComponent<IAttacker>();
        }

        protected override float Score(Vector3 _parameter) => 0;

        #endregion
    }
}