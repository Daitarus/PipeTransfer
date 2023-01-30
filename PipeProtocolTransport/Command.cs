using System.IO;

namespace PipeProtocolTransport
{
    public abstract class Command
    {
        public const int maxLengthData = Transport.maxLengthData;

        public byte typeCom;

        public abstract byte[] ToBytes();

        public abstract void ExecuteCommand(Transport transport, ref FileStream fileStream);
    }
}
