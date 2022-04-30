// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.DataProviders;
using RVModules.RVSmartAI.Content.AI.Tasks;
using RVModules.RVUtilities;
using UnityEngine;

namespace RVHonorAI.Content.AI.Tasks
{
    /// <summary>
    /// Sets CharacterState to flee for set amount of time and set it to normal after
    /// </summary>
    public class FleeJob : AiJob
    {
        #region Fields

        [SerializeField]
        protected float currentFleeTime;

        [SerializeField]
        protected FloatProvider fleeTime;

        private ICharacterState characterStateProvider;

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            characterStateProvider = ContextAs<ICharacterState>();
        }

        protected override void OnJobUpdate(float _dt)
        {
            currentFleeTime += UnityTime.DeltaTime;
            if (currentFleeTime > fleeTime) FinishJob();
        }

        protected override void OnJobStart() => characterStateProvider.CharacterState = CharacterState.Flee;

        protected override void OnJobFinish()
        {
            characterStateProvider.CharacterState = CharacterState.Normal;
            currentFleeTime = 0;
        }

        #endregion
    }
}