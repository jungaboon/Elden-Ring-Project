// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using UnityEngine;
using UnityEngine.Serialization;

namespace RVModules.RVUtilities.FilesManagement
{
    [CreateAssetMenu] public class FoldersCopyConfig : ScriptableObject
    {
        #region Fields

        public Object[] files;
        public string[] targetPaths;
        public string matchPattern = "*.*";

        [FormerlySerializedAs("excludes")]
        public string[] fileExcludes;

        public string[] folderExcludes = {"test", "tests", "Test", "Tests"};
        // public bool putAssembliesIntoOwnFolders;

        #endregion
    }
}