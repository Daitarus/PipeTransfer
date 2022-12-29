using CommandsKit;
using PipeProtocolTransport;
using System.IO;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo dir = new DirectoryInfo(@"C:\test\");
            FileInfo[] fileInfo = FileSystemInfo.CheckSubFiles(dir, 2).ToArray();

            PptClient client = new PptClient("MyPipe", ".", 10000);
            client.Start();

            FileWorker.SendFile(fileInfo[0], client);

            client.Close();
        }        
    }
}