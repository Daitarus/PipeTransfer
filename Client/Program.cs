using CommandsKit;
using PipeProtocolTransport;
using System.IO;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        { 
            Console.Write("Enter server name: ");
            string serverName = Console.ReadLine();

            PptClient client = new PptClient("MyPipe", serverName, 10000);

            if (client.Start())
            {
                Console.WriteLine("Connect!");
                Console.Write("Enter file name: ");
                string fileName = Console.ReadLine();
                FileInfo fileInfo = new FileInfo(fileName);

                FileWorker.SendFile(fileInfo, client);
            }

            client.Close();

            Console.WriteLine("End!");
            Console.ReadKey();
        }        
    }
}