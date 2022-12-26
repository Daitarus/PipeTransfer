using PipeProtocolTransport;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PipeProtocolTransport.Server server = new PipeProtocolTransport.Server("MyPipe");
            server.Start();
            server.Close();
        }
    }
}