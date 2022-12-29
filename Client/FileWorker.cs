using CommandsKit;
using PipeProtocolTransport;
using System.Text;

namespace Client
{
    internal static class FileWorker
    {
        public static byte[] ReadFileBlock(FileInfo fileInfo, long start, int length)
        {
            byte[] fileBlock = new byte[0];

            if (fileInfo.Exists)
            {
                if (fileInfo.Length < start + length)
                {
                    length = (int)(fileInfo.Length - start);
                }

                fileBlock = new byte[length];

                using (FileStream fstream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fstream.Seek(start, SeekOrigin.Begin);
                    fstream.Read(fileBlock);
                }
            }

            return fileBlock;
        }

        public static void SendFile(FileInfo fileInfo, PptClient client)
        {
            if (fileInfo.Exists)
            {
                byte[] fileInfoBytes = Encoding.UTF8.GetBytes(fileInfo.Name);

                int maxLengthBlock = Command.maxLengthData - 3 - fileInfoBytes.Length;
                byte amountBlocks = (byte)Math.Ceiling((double)fileInfo.Length / (double)maxLengthBlock);

                for (byte i = 0; i < amountBlocks; i++) 
                {
                    if(i == amountBlocks - 1)
                    { }
                    byte[] fileBlock = ReadFileBlock(fileInfo, i * maxLengthBlock, maxLengthBlock);
                    SendFileBlock(i, fileInfoBytes, fileBlock, client);
                }
            }
        }

        private static void SendFileBlock(byte numBlock, byte[] fileInfo, byte[] fileBlock, PptClient client)
        {
            Command com = new FileRequest(numBlock, fileInfo, fileBlock);
            client.SendCommand(com);
        }

        private static void SendFileBlock(byte numBlock, string fileInfo, byte[] fileBlock, PptClient client)
        {
            Command com = new FileRequest(numBlock, fileInfo, fileBlock);
            client.SendCommand(com);
        }
    }
}
