namespace Client
{
    internal static class FileSystemInfo
    {
        public static List<FileInfo> CheckFiles(DirectoryInfo directoryInfo)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();

            if(directoryInfo.Exists)
            {
                try
                {
                    filesInfo.AddRange(directoryInfo.GetFiles());
                }
                catch { }
            }

            return filesInfo;
        }

        public static List<DirectoryInfo> CheckDirectories(DirectoryInfo directory, int numStep = -1)
        {
            List<DirectoryInfo> subDirectories = new List<DirectoryInfo>();

            if (numStep > 0) 
                numStep--;

            if(directory.Exists)
            {
                try
                {
                    List<DirectoryInfo> thisSubDirectories = new List<DirectoryInfo>();
                    thisSubDirectories.AddRange(directory.GetDirectories());

                    foreach(DirectoryInfo dir in thisSubDirectories)
                    {
                        subDirectories.Add(dir);

                        if(numStep!=0)
                            subDirectories.AddRange(CheckDirectories(dir, numStep));
                    }
                }
                catch { }
            }

            return subDirectories;
        }

        public static List<FileInfo> CheckSubFiles(DirectoryInfo directory, int numStep = -1)
        {
            List<FileInfo> allFileInfo = new List<FileInfo>();

            allFileInfo.AddRange(directory.GetFiles());

            List<DirectoryInfo> subDirectories = CheckDirectories(directory, numStep);

            foreach(DirectoryInfo dir in subDirectories)
            {
                allFileInfo.AddRange(CheckFiles(dir));
            }

            return allFileInfo;
        }
    }
}
