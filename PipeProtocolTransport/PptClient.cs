using System.IO.Pipes;

namespace PipeProtocolTransport
{
    public sealed class PptClient
    {
        private NamedPipeClientStream pipeStream;
        private int timeWaitConnection;

        public PptClient(string pipeName, string serverName, int timeWaitConnection)
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

        public void SendCommand(Command command)
        {
            if (pipeStream.IsConnected)
            {
                Transport transport = new Transport(pipeStream);

                byte[] buffer = command.ToBytes();

                try
                {
                    transport.SendData(buffer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        public void Close()
        {
            pipeStream.Close();
        }
    }
}
