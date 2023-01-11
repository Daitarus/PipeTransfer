using PipeProtocolTransport;
using System.Text;
using System.Configuration;
using System.Data;

namespace CommandsKit
{
    public class FileRequest : Command
    {
        public const int maxSizeInfoAndData = Command.maxLengthData - 10;

        private readonly int numBlock;
        private readonly int allBlock;
        private readonly byte[] fileInfo;
        private readonly byte[] fileData;

        public FileRequest(int numBlock, int allBlock, string fileInfo, byte[] fileData) : this(numBlock, allBlock, Encoding.UTF8.GetBytes(fileInfo), fileData) { }

        public FileRequest(int numBlock, int allBlock, byte[] fileInfo, byte[] fileData)
        {
            if (fileInfo == null || fileInfo.Length == 0)
                throw new ArgumentNullException(nameof(fileInfo));
            if (fileData == null || fileData.Length == 0)
                throw new ArgumentNullException(nameof(fileData));
            if (fileInfo.Length > byte.MaxValue)
                throw new ArgumentException($"Size must be less then {byte.MaxValue}", nameof(fileInfo));
            if (fileData.Length + fileInfo.Length > maxSizeInfoAndData)
                throw new ArgumentException($"Size {nameof(fileInfo)} and {nameof(fileData)} must be less then {maxSizeInfoAndData}");

            typeCom = (byte)TypeCommand.FILE_REQUEST;
            this.numBlock = numBlock;
            this.allBlock = allBlock;
            this.fileInfo = fileInfo;
            this.fileData = fileData;
        }

        public override byte[] ToBytes()
        {
            byte[] numBlockBytes = BitConverter.GetBytes(numBlock);
            byte[] allBlockBytes = BitConverter.GetBytes(allBlock);

            List<byte> data = new List<byte>();

            data.Add(typeCom);
            data.AddRange(numBlockBytes);
            data.AddRange(allBlockBytes);
            data.Add((byte)fileInfo.Length);
            data.AddRange(fileInfo);
            data.AddRange(fileData);

            return data.ToArray();
        }

        public override void ExecuteCommand(Transport transport, ref FileStream? fileStream)
        {
            if (numBlock == 0)
            {
                string dirPath = ConfigurationManager.AppSettings["dirPath"];
                string fileInfoStr = Encoding.UTF8.GetString(this.fileInfo);

                if (dirPath != null && dirPath != "")
                {
                    if (dirPath[dirPath.Length - 1] != '\\')
                    {
                        dirPath += '\\';
                    }

                    fileInfoStr = dirPath + fileInfoStr;
                }

                FileInfo fileInfo = new FileInfo(fileInfoStr);

                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                fileStream = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }

            if(fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            fileStream.Write(fileData);

            if (numBlock == allBlock - 1)
                fileStream?.Close();
        }

        public static FileRequest ToCommand(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            int startPosition = 0;

            byte[] numBlockBytes = new byte[4];
            Array.Copy(data, startPosition, numBlockBytes, 0, numBlockBytes.Length);
            int numBlock = BitConverter.ToInt32(numBlockBytes);
            startPosition += numBlockBytes.Length;

            byte[] allBlockBytes = new byte[4];
            Array.Copy(data, startPosition, allBlockBytes, 0, allBlockBytes.Length);
            int allBlock = BitConverter.ToInt32(allBlockBytes);
            startPosition += allBlockBytes.Length;

            byte fileInfoLength = data[startPosition];
            startPosition += 1;

            byte[] fileInfoBytes = new byte[fileInfoLength];
            Array.Copy(data, startPosition, fileInfoBytes, 0, fileInfoBytes.Length);
            startPosition += fileInfoBytes.Length;

            byte[] fileData = new byte[data.Length - startPosition];
            Array.Copy(data, startPosition, fileData, 0, fileData.Length);

            return new FileRequest(numBlock, allBlock, fileInfoBytes, fileData);
        }
    }
}
