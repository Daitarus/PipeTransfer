using PipeProtocolTransport;
using CommandsKit;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PipeProtocolTransport.Server server = new PipeProtocolTransport.Server("MyPipe", new ComDeterminant());
            server.Start();
            server.Close();
        }
    }
}