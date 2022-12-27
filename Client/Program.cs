using CommandsKit;
using PipeProtocolTransport;
using System.IO;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PipeProtocolTransport.Client client = new PipeProtocolTransport.Client("MyPipe", ".", 10000);
            byte[] fileBytes = ReadFile("test.txt");
            client.Start();
            Command com = new FileRequest(0, "test.txt", fileBytes);
            client.SendCommand(com);
            client.Close();
        }

        static byte[] ReadFile(string fileName)
        {
            byte[] fileBytes = new byte[0];
            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
            {
                fileBytes = new byte[fileInfo.Length];
                using (FileStream fstream = System.IO.File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    int read = fstream.Read(fileBytes);
                }
            }

            return fileBytes;
        }
    }
}