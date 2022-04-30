// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class DebugBreakTask : AiTask
    {
        #region Not public methods

        protected override void Execute(float _deltaTime) => Debug.Break();

        #endregion
    }
}