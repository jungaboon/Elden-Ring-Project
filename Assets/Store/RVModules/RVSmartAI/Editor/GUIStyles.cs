// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using UnityEngine;

namespace RVModules.RVSmartAI.Editor
{
    public class GUIStyles : ScriptableObject
    {
        #region Fields

        public static GUIStyle[] cachedStyles;
        public static GUIStyle[] cachedDebugStyles;
        public GUIStyle[] styles;
        public GUIStyle[] debugStyles;

        #endregion

        #region Public methods

        public static GUIStyle GetGUIStyle(int _id)
        {
            if (cachedStyles == null)
                cachedStyles = Resources.Load<GUIStyles>("GUIStyles").styles;
            return cachedStyles[_id];
        }

        public static GUIStyle GetGUIDebugStyle(int _id)
        {
            if (cachedDebugStyles == null)
                cachedDebugStyles = Resources.Load<GUIStyles>("GUIDebugStyles").styles;
            return cachedDebugStyles[_id];
        }

        #endregion
    }
}