// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class DebugLogTask : AiTask
    {
        #region Fields

        [SerializeField]
        private StringProvider stringToLog;

        #endregion

        #region Not public methods

        protected override void Execute(float _deltaTime) => Debug.Log(stringToLog.ToString());

        #endregion
    }
}