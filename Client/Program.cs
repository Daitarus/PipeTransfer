using CommandsKit;
using PipeProtocolTransport;
using System.IO;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo dir = new DirectoryInfo(@"C:\My_Job\");
            FileInfo[] fileInfo = FileWorker.CheckSubFiles(dir, 2).ToArray();

        }        
    }
}