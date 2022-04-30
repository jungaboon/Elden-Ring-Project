// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVSmartAI.Content.AI.Tasks;

namespace RVHonorAI.Content.AI.Tasks
{
    /// <summary>
    /// Moves  to target position or it's last seen position if it is not visible
    /// </summary>
    public class MoveToTarget : AiAgentTask
    {
        #region Fields

        private ITargetProvider targetProvider;

        #endregion

        #region Properties

        protected override string DefaultDescription => "Moves  to target position or it's last seen position if it is not visible";

        #endregion

        #region Not public methods

        protected override void OnContextUpdated()
        {
            base.OnContextUpdated();
            targetProvider = ContextAs<ITargetProvider>();
        }

        protected override void Execute(float _deltaTime) => movement.Destination =
            targetProvider.CurrentTarget.Visible ? targetProvider.Target.Transform.position : targetProvider.CurrentTarget.LastSeenPosition;

        #endregion
    }
}