using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PipeProtocolTransport.Client client = new PipeProtocolTransport.Client("MyPipe", ".", 10000);
            client.Start();
            client.SendByte(new byte[524288000]);
            client.Close();
        }
    }
}