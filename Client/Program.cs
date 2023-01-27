using PipeProtocolTransport;
using System.Configuration;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string serverName = GetServerName(ConfigurationManager.AppSettings["server"]);

            PptClient client = new PptClient("MyPipe", serverName, 10000);

            //client.Start

            //request to server ...
            //EXAMPLE: FileTransport.SendFile(fileInfo, client);

            client.Close();
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