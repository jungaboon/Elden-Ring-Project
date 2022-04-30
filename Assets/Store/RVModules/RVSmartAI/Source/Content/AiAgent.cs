// Created by Ronis Vision. All rights reserved
// 22.03.2021.

using RVModules.RVSmartAI.Content.Scanners;
using UnityEngine;

namespace RVModules.RVSmartAI.Content
{
    /// <summary>
    /// Simple generic ai character representation
    /// This is for example purpose only, but you can use it as base class for your NPCs or create your own
    /// </summary>
    public class AiAgent : MonoBehaviour, IScannable, IObjectDetectionCallbacks
    {
        #region Properties

        // IScannable implementation
        //public Object GetObject => this;

        public System.Action<Object> OnNewObjectDetected { get; set; }
        public System.Action<Object> OnObjectNotDetectedAnymore { get; set; }

        #endregion
    }
}