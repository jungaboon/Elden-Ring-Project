// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class Obsolete_SimpleProximityToMoveTargetAiScorerParams : Obsolete_SimpleProximityAiScorerParams
    {
        #region Properties

        protected override Vector3 PositionToMeasure => MoveTarget.position;

        #endregion
    }
}