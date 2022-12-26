using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PipeProtocolTransport
{
    public class Client
    {
        private NamedPipeClientStream pipeStream;
        private int timeWaitConnection;

        public Client(string pipeName, string serverName, int timeWaitConnection)
        {
            if(pipeName == null)
                throw new ArgumentNullException(nameof(pipeName));
            if(serverName == null)
                throw new ArgumentNullException(nameof(serverName));
            if(timeWaitConnection <= 0)
                throw new ArgumentException("Value must be more 0", nameof(timeWaitConnection));

            pipeStream = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut);
            this.timeWaitConnection = timeWaitConnection;
        }

        public void Start()
        {
            try
            {
                pipeStream.Connect(timeWaitConnection);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void SendByte(byte[] bytes)
        {
            Transport transport = new Transport(pipeStream);
            try
            {
                transport.SendData(bytes);
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
