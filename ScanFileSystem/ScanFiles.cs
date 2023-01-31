using System.Collections.Generic;
using System.IO;
using System;

namespace ScanFileSystem
{
    public static class ScanFiles
    {
        public static List<FileInfo> GetFiles(DirectoryInfo homeDirectory, int numRecursion = -1)
        {
            if (numRecursion < -1)
                throw new ArgumentException("Argument must be more -1", nameof(numRecursion));

            List<FileInfo> resultList = new List<FileInfo>();
            if (homeDirectory != null && homeDirectory.Exists)
            {
                try
                {
                    resultList.AddRange(homeDirectory.GetFiles());
                    DirectoryInfo[] subDirs = homeDirectory.GetDirectories();

                    if (numRecursion > 0)
                        numRecursion--;

                    if (numRecursion != 0)
                    {
                        foreach (DirectoryInfo subDir in subDirs)
                        {
                            resultList.AddRange(GetFiles(subDir, numRecursion));
                        }
                    }
                }
                catch { }
            }

            return resultList;
        }

        public static List<DirectoryInfo> GetDirectories(DirectoryInfo homeDirectory, int numRecursion = -1)
        {
            if (numRecursion < -1)
                throw new ArgumentException("Argument must be more -1", nameof(numRecursion));

            List<DirectoryInfo> resultList = new List<DirectoryInfo>();
            if (homeDirectory != null && homeDirectory.Exists)
            {
                try
                {
                    DirectoryInfo[] subDirs = homeDirectory.GetDirectories();
                    resultList.AddRange(subDirs);

                    if (numRecursion > 0)
                        numRecursion--;

                    if (numRecursion != 0)
                    {
                        foreach (DirectoryInfo subDir in subDirs)
                        {
                            resultList.AddRange(GetDirectories(subDir, numRecursion));
                        }
                    }
                }
                catch { }
            }

            return resultList;
        }
    }
}