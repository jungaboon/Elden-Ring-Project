// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVHonorAI.Combat;
using RVModules.RVSmartAI.Content;
using UnityEngine;

namespace RVHonorAI.Content.AI
{
    /// <summary>
    /// Shows scores for selecting targets in 3d in scene view
    /// </summary>
    [DefaultExecutionOrder(200)] public class TargetInfoAiDebugger : VisualDebugger<TargetInfo>
    {
        #region Not public methods

        protected override bool ValidateData(TargetInfo _data) => _data.Target as Object != null;

        protected override Vector3 DataPosition(TargetInfo _data) => _data.Target.Transform.position;

        #endregion
    }
}