using PipeProtocolTransport;
using CommandsKit;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PptServer server = new PptServer("MyPipe", new ComDeterminant());
            server.Start();
            server.Close();
        }
    }
}