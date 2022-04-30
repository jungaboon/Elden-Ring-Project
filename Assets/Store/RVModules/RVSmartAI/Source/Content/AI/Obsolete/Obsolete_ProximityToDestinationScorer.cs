// Created by Ronis Vision. All rights reserved
// 13.10.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Scorers
{
    public class Obsolete_ProximityToDestinationScorer : Obsolete_ProximityToMeAiScorer
    {
        #region Properties

        public override Vector3 PositionToMeasure
        {
            get
            {
                if (movement.AtDestination) return movement.Position;
                return movement.Destination;
            }
        }

        #endregion
    }
}