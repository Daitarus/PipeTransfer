using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace PipeProtocolTransport
{
    public sealed class PptServer
    {       
        NamedPipeServerStream pipeStream;
        IDeterminant determinant;

        public PptServer(string pipeName, IDeterminant determinant)
        {
            if (pipeName == null)
                throw new ArgumentNullException(nameof(pipeName));

            PipeSecurity pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
            pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, pipeSecurity);
            this.determinant = determinant;
        }

        public void Start()
        {
            FileStream fileStream = null;
            Transport transport = new Transport(pipeStream);

            try
            {
                pipeStream.WaitForConnection();

                while (pipeStream.IsConnected)
                {
                    byte[] buffer = transport.GetData();
                    Command com = determinant.Define(buffer);
                    com.ExecuteCommand(transport, ref fileStream);
                }
            }
            catch (Exception ex)
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