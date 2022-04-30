// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEditor;
using UnityEngine;

namespace RVModules.RVUtilities
{
    public class DebugUtilities
    {
        #region Public methods

#if UNITY_EDITOR
        public static void DrawString(string text, Vector3 worldPos, Color? textColor = null, Color? backColor = null)
        {
            Handles.BeginGUI();
            var restoreTextColor = GUI.color;
            var restoreBackColor = GUI.backgroundColor;

            GUI.color = textColor ?? Color.white;
            GUI.backgroundColor = backColor ?? Color.black;

            var view = SceneView.currentDrawingSceneView;
            if (view != null && view.camera != null)
            {
                var screenPos = view.camera.WorldToScreenPoint(worldPos);
                if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
                {
                    GUI.color = restoreTextColor;
                    Handles.EndGUI();
                    return;
                }

                var size = GUI.skin.label.CalcSize(new GUIContent(text));
                var r = new Rect(screenPos.x - size.x / 2, -screenPos.y + view.position.height + 4, size.x, size.y);
                GUI.Box(r, text, EditorStyles.numberField);
                GUI.Label(r, text);
                GUI.color = restoreTextColor;
                GUI.backgroundColor = restoreBackColor;
            }

            Handles.EndGUI();
        }
#endif

        #endregion
    }
}