using PipeProtocolTransport;
using System;
using System.IO;

namespace CommandsKit
{
    public class UnknowCom : Command
    {
        public readonly byte[] bytes = new byte[0];
        public UnknowCom(byte[] bytes)
        {

            typeCom = (byte)TypeCommand.UNKNOW;
            if (bytes != null)
            {
                this.bytes = bytes;
            }

        }
        public override byte[] ToBytes()
        {
            byte[] data = new byte[1 + bytes.Length];

            data[0] = typeCom;
            Array.Copy(bytes, 0, data, 1, bytes.Length);

            return data;
        }

        public override void ExecuteCommand(Transport transport, ref FileStream fileStream) { }

        public static UnknowCom ToCommand(byte[] data)
        {
            return new UnknowCom(data);
        }
    }
}
