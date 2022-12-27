using CommandsKit;
using PipeProtocolTransport;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PipeProtocolTransport.Client client = new PipeProtocolTransport.Client("MyPipe", ".", 10000);
            client.Start();
            Command com = new UnknowCom(new byte[] { 0, 1, 2, 3 });
            client.SendCommand(com);
            client.Close();
        }
    }
}