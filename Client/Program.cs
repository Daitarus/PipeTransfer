using CommandsKit;
using PipeProtocolTransport;
using System.IO;
using ScanFileSystem;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileInfo[] files = ScanFiles.GetFiles(new DirectoryInfo("C:\\Users\\User"), 2).ToArray();
            DirectoryInfo[] dirs = ScanFiles.GetDirectories(new DirectoryInfo("C:\\Users\\User"), 2).ToArray();

            //Console.Write("Enter server name: ");
            //string serverName = Console.ReadLine();

            //PptClient client = new PptClient("MyPipe", serverName, 10000);

            //if (client.Start())
            //{
            //    Console.WriteLine("Connect!");
            //    Console.Write("Enter file name: ");
            //    string fileName = Console.ReadLine();
            //    FileInfo fileInfo = new FileInfo(fileName);

            //    FileTransport.SendFile(fileInfo, client);
            //}

            //client.Close();

            //Console.WriteLine("End!");
            //Console.ReadKey();
        }        
    }
}