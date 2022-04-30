// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using RVModules.RVUtilities;
using UnityEngine;

namespace RVModules.RVSmartAI.Content
{
    /// <summary>
    /// Debugger for AiTaskParams. Displays sum of scores for specific parameters in 3d positions making debugging easier.
    /// Add this component to your AI root game object and it will connect automatically.
    /// </summary>
    /// <typeparam name="T">Type of data to debug</typeparam>
    [DefaultExecutionOrder(200)] public abstract class VisualDebugger<T> : MonoBehaviour
    {
        #region Fields

        public Color textColor = Color.white;
        public Color backgroundColor = Color.black;
        public Vector3 labelsOffset = new Vector3(0, .5f, 0);

        [SerializeField]
        private AiGraph debuggedGraph;

        #endregion

        #region Not public methods

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !enabled || debuggedGraph == null) return;
            foreach (var graphDebugValue in debuggedGraph.debugValues)
            {
                if (!(graphDebugValue.Key is T data)) continue;
                if (!ValidateData(data)) continue;
                DebugUtilities.DrawString(graphDebugValue.Value.ToString("n2"), DataPosition(data) + labelsOffset, textColor, backgroundColor);
            }
        }
#endif

        /// <summary>
        /// World-space position at which label with score will be rendered
        /// </summary>
        protected abstract Vector3 DataPosition(T _data);

        /// <summary>
        /// Check if data should be displayed
        /// </summary>
        protected virtual bool ValidateData(T _data) => true;

        private void Awake() => debuggedGraph = GetComponentInChildren<Ai>().MainAiGraph;

        #endregion
    }
}