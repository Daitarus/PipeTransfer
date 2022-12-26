﻿using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PipeProtocolTransport
{
    internal class Transport
    {
        public const int maxLengthData = 524288000;   //500Mb

        PipeStream pipeStream;

        public Transport(PipeStream pipeStream)
        {
            if (pipeStream == null) 
                throw new ArgumentNullException(nameof(pipeStream));

            this.pipeStream = pipeStream;
        }

        public void SendData(byte[] data)
        {
            if (!pipeStream.CanWrite)
                throw new Exception($"{nameof(pipeStream)} can not write");
            if(data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length > maxLengthData)
                throw new ArgumentException($"Size must be less than {maxLengthData}", nameof(data));

            data = AddLength(data);

            pipeStream.Write(data, 0, data.Length);
        }

        public byte[] GetData()
        {
            if (!pipeStream.CanRead)
                throw new Exception($"{nameof(pipeStream)} can not read");

            byte[] lengthBytes = new byte[4];
            pipeStream.Read(lengthBytes, 0, lengthBytes.Length);
            int length = BitConverter.ToInt32(lengthBytes);

            if (length > maxLengthData)
                throw new Exception($"Size buffer {nameof(pipeStream)} must be less than {maxLengthData}");

            byte[] data = new byte[length];
            int byteCounter = 0, byteCounterOld = 0;

            while (byteCounter < length)
            {
                byteCounter += pipeStream.Read(data, 0, length - byteCounterOld);
            }

            return data;
        }

        private byte[] AddLength(byte[] data)
        {
            byte[] length = BitConverter.GetBytes(data.Length);

            List<byte> newData = new List<byte>();
            newData.AddRange(length);
            newData.AddRange(data);

            return newData.ToArray();
        }
    }
}
