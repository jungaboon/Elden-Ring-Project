// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVLoadBalancer;
using RVModules.RVUtilities;

namespace RVHonorAI.Utilities
{
    /// <summary>
    /// Destroys game object after set amount of time
    /// </summary>
    public class DestroyGameObjectAfter : LoadBalancedBehaviour
    {
        #region Fields

        public float destroyAfter = float.MaxValue;
        private float creationTime;

        #endregion

        #region Not public methods

        private void Awake() => creationTime = UnityTime.Time;

        protected override void LoadBalancedUpdate(float _deltaTime)
        {
            if (UnityTime.Time > creationTime + destroyAfter) Destroy(gameObject);
        }

        #endregion
    }
}