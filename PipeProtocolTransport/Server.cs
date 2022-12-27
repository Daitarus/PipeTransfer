using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace PipeProtocolTransport
{
    public sealed class Server
    {       
        NamedPipeServerStream pipeStream;
        IDeterminant determinant;

        public Server(string pipeName, IDeterminant determinant)
        {
            if (pipeName == null)
                throw new ArgumentNullException(nameof(pipeName));

            PipeSecurity pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
            pipeStream = NamedPipeServerStreamAcl.Create(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, pipeSecurity);
            this.determinant = determinant;
        }

        public void Start()
        {
            Transport transport = new Transport(pipeStream);
            try
            {
                pipeStream.WaitForConnection();

                while (pipeStream.IsConnected)
                {
                    byte[] buffer = transport.GetData();
                    Command com = determinant.Define(buffer);
                    com.ExecuteCommand(transport);
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