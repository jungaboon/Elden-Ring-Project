// Created by Ronis Vision. All rights reserved
// 21.03.2021.

using UnityEngine;

namespace RVModules.RVSmartAI.Content
{
    /// <summary>
    /// Renders score results numbers in scene view for Vector3 tasks/scorers. Label is rendered at Vector3 world space position
    /// and is it's float value(sum for this Vector3 from all results)
    /// </summary>
    [DefaultExecutionOrder(200)] public class Vector3AiDebugger : VisualDebugger<Vector3>
    {
        #region Not public methods

        protected override Vector3 DataPosition(Vector3 _data) => _data;

        #endregion
    }
}