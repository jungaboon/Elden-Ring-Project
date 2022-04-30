// Created by Ronis Vision. All rights reserved
// 23.08.2019.

using System;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.GraphElements.Stages;
using XNode;

namespace RVModules.RVSmartAI.Nodes
{
    [CreateNodeMenu("Create highest utility wins stage node")] [Serializable]
    public class HighestUtilityWinsStageNode : StageNode
    {
        #region Not public methods

#if UNITY_EDITOR
        protected override Stage CreateStage(AiGraph _aiGraph)
        {
            // make sure stage object exist
            if (stage != null) return stage;
            var highestScoreWinsstage = _aiGraph.CreateNewElement(typeof(HighestUtilityWinsStage), this);
            AssignSubSelement(highestScoreWinsstage as IAiGraphElement);
            return stage;
        }
#endif

        #endregion
    }
}