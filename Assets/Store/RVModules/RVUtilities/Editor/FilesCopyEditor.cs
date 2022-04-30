// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using RVModules.RVUtilities.FilesManagement;
using UnityEditor;
using UnityEngine;

namespace RVModules.RVUtilities.Editor
{
    [CustomEditor(typeof(FoldersCopyConfig))]
    public class FilesCopyEditor : UnityEditor.Editor
    {
        #region Public methods

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Copy"))
            {
                var foldersCopyConfig = target as FoldersCopyConfig;
                CopyFoldersTool.CopyModulesSourceFilesToTargets(foldersCopyConfig);
            }
        }

        #endregion
    }
}