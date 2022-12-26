using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace PipeProtocolTransport
{
    public class Server
    {       
        NamedPipeServerStream pipeStream;

        public Server(string pipeName)
        {
            if (pipeName == null)
                throw new ArgumentNullException(nameof(pipeName));

            PipeSecurity pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
            pipeStream = NamedPipeServerStreamAcl.Create(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, pipeSecurity);
        }

        public void Start()
        {
            Transport transport = new Transport(pipeStream);
            try
            {
                pipeStream.WaitForConnection();

                while (pipeStream.IsConnected)
                {
                    byte[] data = transport.GetData();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void Close()
        {
            pipeStream.Close();
        }
    }
}