// Created by Ronis Vision. All rights reserved
// 22.09.2020.

using UnityEditor;

namespace RVModules.RVSmartAI
{
    public static class SmartAiSettings
    {
#if UNITY_EDITOR
        public static bool AutoFocusOnRoot => EditorPrefs.GetBool("autoFocusOnRoot");

        [SettingsProvider]
        private static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/RonisVision", SettingsScope.User)
            {
                label = "RVSmartAI",

                guiHandler = _searchContext =>
                {
                    EditorPrefs.SetBool("autoFocusOnRoot",
                        EditorGUILayout.Toggle("Focus on root node on graph opening", AutoFocusOnRoot));
                }
            };
            return provider;
        }
#endif
    }
}