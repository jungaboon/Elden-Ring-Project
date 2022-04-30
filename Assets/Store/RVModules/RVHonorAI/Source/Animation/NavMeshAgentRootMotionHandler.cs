// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using UnityEngine;
using UnityEngine.AI;

namespace RVHonorAI.Animation
{
    /// <summary>
    /// Changes navmeshAgent settings so that it's movements and rotation can be handled by root motion ainmation
    /// </summary>
    public class NavMeshAgentRootMotionHandler : MonoBehaviour, IMovementRootMotionHandler
    {
        #region Fields

        private NavMeshAgent navMeshAgent;

        [SerializeField]
        private Animator animator;

        private new Transform transform;

        #endregion

        #region Not public methods

        private void Awake() => transform = base.transform;

        private void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Sets transform y position to y pos from navmesh agent if using root motion
        /// </summary>
        protected virtual void OnAnimatorMove()
        {
            var position = animator.rootPosition;
            position.y = navMeshAgent.nextPosition.y;
            transform.SetPositionAndRotation(position, animator.rootRotation);
//
//            var agentVelocity = characterAi.navMeshAgent.desiredVelocity;
//            if (agentVelocity.sqrMagnitude > .5f)
//            {
//                transform1.rotation = Quaternion.RotateTowards(
//                    Quaternion.LookRotation(transform1.forward),
//                    Quaternion.LookRotation(agentVelocity),
//                    120 * UnityTime.DeltaTime);
//            }
        }

        #endregion
    }
}