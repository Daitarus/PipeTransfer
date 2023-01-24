using CommandsKit;
using PipeProtocolTransport;
using System.Text;

namespace Client
{
    internal static class FileTransport
    {
        private static byte[] ReadFileBlock(FileStream fileStream, long start, int lengthBlock, long lengthFile)
        {
            if (lengthFile < start + lengthBlock)
                lengthBlock = (int)(lengthFile - start);

            byte[] fileBlock = new byte[lengthBlock];

            fileStream.Seek(start, SeekOrigin.Begin);
            fileStream.Read(fileBlock);

            return fileBlock;
        }

        public static void SendFile(FileInfo fileInfo, PptClient client)
        {
            if (fileInfo.Exists)
            {
                byte[] fileInfoBytes = Encoding.UTF8.GetBytes(fileInfo.Name);

                int maxLengthBlock = FileRequest.maxSizeInfoAndData - fileInfoBytes.Length;
                int amountBlocks = (int)Math.Ceiling((double)fileInfo.Length / (double)maxLengthBlock);

                using (FileStream fileStream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    for (int i = 0; i < amountBlocks; i++)
                    {
                        byte[] fileBlock = ReadFileBlock(fileStream, i * maxLengthBlock, maxLengthBlock, fileInfo.Length);
                        Command com = new FileRequest(i, amountBlocks, fileInfoBytes, fileBlock);
                        if (!client.SendCommand(com))
                            break;            
                    }
                }
            }
        }
    }
}
