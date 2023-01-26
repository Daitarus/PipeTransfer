using PipeProtocolTransport;
using CommandsKit;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                PptServer server = new PptServer("MyPipe", new ComDeterminant());
                server.Start();
                server.Close();
            }
        }
    }
}