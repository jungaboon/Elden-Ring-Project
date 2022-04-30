// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using System;
using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.GraphElements;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    /// <summary>
    /// Blocks execution of graph for set duration 
    /// </summary>
    public class BlockGraph : AiTask
    {
        #region Fields

        private float startTime;

        [Tooltip("In seconds")]
        [SerializeField]
        private FloatProvider waitTime;

        #endregion

        #region Properties

        public override bool IsRunningTask => true;

        #endregion

        #region Not public methods

        protected override void Execute(float _deltaTime) => throw new NotImplementedException();

        protected override bool StartExecuting()
        {
            startTime = UnityTime.Time;
            return true;
        }

        protected override void Executing(float _deltaTime)
        {
            if (UnityTime.Time - startTime > waitTime) StopExecuting();
        }

        #endregion
    }
}