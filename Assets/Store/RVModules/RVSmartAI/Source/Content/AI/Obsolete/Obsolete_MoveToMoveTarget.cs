// Created by Ronis Vision. All rights reserved
// 21.03.2021.

namespace RVModules.RVSmartAI.Content.AI.Tasks
{
    public class Obsolete_MoveToMoveTarget : AiAgentTask
    {
        #region Not public methods

        protected override void OnContextUpdated() => this.ObsoleteGraphElementUsedError(typeof(MoveToPosition));

        protected override void Execute(float _deltaTime)
        {
            this.ObsoleteGraphElementUsedError(typeof(MoveToPosition));
            movement.Destination = MoveTarget.position;
        }

        #endregion
    }
}