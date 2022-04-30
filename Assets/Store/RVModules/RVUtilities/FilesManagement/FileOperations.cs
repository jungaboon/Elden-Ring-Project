// Created by Ronis Vision. All rights reserved
// 06.08.2020.

using System;
using System.IO;
using UnityEngine;

namespace RVModules.RVUtilities.FilesManagement
{
    public static class FileOperations
    {
        #region Public methods

        /// <summary>
        /// Compares two files. Returns true if they are exactly the same.
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
                // Return true to indicate that the files are the same.
                return true;

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            } while (file1byte == file2byte && file1byte != -1);

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return file1byte - file2byte == 0;
        }

        public static void CopyFile(string from, string to)
        {
            if (File.Exists(to) && FileCompare(from, to)) return;

            File.Copy(from, to, true);
            Debug.Log("Copying ''" + from + "'' to ''" + to + "''");
        }

        public static void CopyFolderContents(string sourceDirectory, string targetDirectory, bool recursive = true, string matchPattern = "*.*",
            string[] containsExclude = default, string[] containsExcludeFolders = default)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);
            //Debug.Log($"Copying folder {diSource} to {diTarget}");

            CopyAll(diSource, diTarget, recursive, matchPattern, containsExclude, containsExcludeFolders);
        }

        #endregion

        #region Not public methods

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target, bool recursive = true, string matchPattern = "*.*",
            string[] containsExclude = default, string[] containsExcludeFolders = default)
        {
            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles(matchPattern))
                try
                {
                    var containExclude = false;

                    if (containsExclude != null)
                        foreach (var s in containsExclude)
                        {
                            if (!fi.Name.Contains(s)) continue;
                            containExclude = true;
                            break;
                        }

                    if (containExclude) continue;

                    if (!Directory.Exists(target.FullName)) Directory.CreateDirectory(target.FullName);

                    CopyFile(fi.FullName, Path.Combine(target.FullName, fi.Name));
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

            if (!recursive) return;

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var containExclude = false;

                if (containsExcludeFolders != null)
                    foreach (var containsExcludeFolder in containsExcludeFolders)
                    {
                        if (!diSourceSubDir.Name.Contains(containsExcludeFolder)) continue;
                        containExclude = true;
                        break;
                    }

                if (containExclude) continue;

                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, true, matchPattern, containsExclude, containsExcludeFolders);
            }
        }

        #endregion
    }
}