// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;
using System.Collections.Generic;
using System.IO;
using RVModules.RVUtilities.FilesManagement;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace RVModules.RVUtilities.Editor
{
    public static class CopyFoldersTool
    {
        #region Public methods

        public static void CopyModulesSourceFilesToTargets(FoldersCopyConfig selectedCM)
        {
            if (selectedCM == null) return;

            foreach (var targetPath in selectedCM.targetPaths)
            {
                if (string.IsNullOrEmpty(targetPath)) continue;

                foreach (var sourceFile in selectedCM.files)
                {
                    if (sourceFile == null) continue;
                    var p = AssetDatabase.GetAssetPath(sourceFile);

                    FileOperations.CopyFolderContents(new FileInfo(p).Directory.FullName, Path.Combine(targetPath, sourceFile.name), true,
                        selectedCM.matchPattern, selectedCM.fileExcludes, selectedCM.folderExcludes);
                }
            }
        }

        #endregion

        #region Not public methods

        //[MenuItem("Tools/CopyModulesAssembliesToTargets")]
//        public static void CopyModulesAssembliesToTargets_()
//        {
//            FoldersCopyConfig selectedCM = Selection.activeObject as FoldersCopyConfig;
//            if (selectedCM == null)
//                return;
//
//            foreach (var selectedCmTargetPath in selectedCM.targetPaths)
//            {
//                if (selectedCmTargetPath == null) continue;
//                CopyAssemblies(selectedCM.files.Select(a => a.name).ToList(), selectedCmTargetPath, selectedCM.putAssembliesIntoOwnFolders);
//            }
//        }

        private static void CopyAssemblies(List<string> moduleNamesToCopy, string _targetPath, bool modulesToOwnFolders)
        {
            var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);

            foreach (var assembly in assemblies)
            {
                if (!moduleNamesToCopy.Contains(assembly.name))
                    continue;

                var assemblyPath = Application.dataPath.Replace("/Assets", String.Empty) + "/" + assembly.outputPath;

                var targetPath = _targetPath;
                if (modulesToOwnFolders)
                {
                    if (!Directory.Exists(Path.Combine(targetPath, assembly.name)))
                        Directory.CreateDirectory(Path.Combine(targetPath, assembly.name));
                    targetPath = Path.Combine(targetPath, assembly.name, assembly.name + ".dll");
                }
                else
                {
                    targetPath = Path.Combine(targetPath, assembly.name + ".dll");
                }

                FileOperations.CopyFile(assemblyPath, targetPath);
            }
        }

        #endregion

//    private static void CopyFiles(string[] modulesToCopy, string _target)
//    {
//        DirectoryInfo[] dirs = new DirectoryInfo(modulesPath).GetDirectories();
//        foreach (var directoryInfo in dirs)
//        {
//            if (modulesToCopy.Contains(directoryInfo.Name))
//                FileOperations.CopyFolder(directoryInfo.FullName, Path.Combine(_target, directoryInfo.Name), containsExclude: new[] {".meta", "copy.txt"});
//        }
//    }
    }
}