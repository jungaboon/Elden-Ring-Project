// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class Obsolete_ProximityToMoveTargetScorer : Obsolete_ProximityToMeAiScorer
    {
        #region Properties

        public override Vector3 PositionToMeasure => MoveTarget.position;

        #endregion
    }
}