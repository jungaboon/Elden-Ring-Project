// Created by Ronis Vision. All rights reserved
// 23.08.2019.

using System;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVSmartAI.GraphElements.Stages;
using XNode;

namespace RVModules.RVSmartAI.Nodes
{
    [CreateNodeMenu("Create first utility wins stage node")] [Serializable]
    public class FirstUtilityWinsStageNode : StageNode
    {
        #region Not public methods

#if UNITY_EDITOR
        protected override Stage CreateStage(AiGraph _aiGraph)
        {
            // make sure stage object exist
            if (stage != null) return stage;
            var firstScoreWinsstage = _aiGraph.CreateNewElement(typeof(FirstUtilityWinsStage), this);
            AssignSubSelement(firstScoreWinsstage as IAiGraphElement);
            return stage;
        }
#endif

        #endregion
    }
}