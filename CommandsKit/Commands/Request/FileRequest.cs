using PipeProtocolTransport;
using System.Text;
using System.Configuration;

namespace CommandsKit
{
    public class FileRequest : Command
    {
        private readonly byte numBlock;
        private readonly byte[] fileInfo;
        private readonly byte[] fileData;

        public FileRequest(byte numBlock, string fileInfo, byte[] fileData) : this(numBlock, Encoding.UTF8.GetBytes(fileInfo), fileData) { }

        public FileRequest(byte numBlock, byte[] fileInfo, byte[] fileData)
        {
            if (fileInfo == null || fileInfo.Length == 0)
                throw new ArgumentNullException(nameof(fileInfo));
            if (fileData == null)
                throw new ArgumentNullException(nameof(fileData));
            if (fileInfo.Length > byte.MaxValue)
                throw new ArgumentException($"Size must be less then {byte.MaxValue}", nameof(fileInfo));
            if (fileData.Length + fileInfo.Length > Command.maxLengthData - 3)
                throw new ArgumentException($"Size {nameof(fileInfo)} and {nameof(fileData)} must be less then {Command.maxLengthData - 3}");

            typeCom = (byte)TypeCommand.FILE_REQUEST;
            this.numBlock = numBlock;
            this.fileInfo = fileInfo;
            this.fileData = fileData;
        }

        public override byte[] ToBytes()
        {
            byte[] data = new byte[3 + fileInfo.Length + fileData.Length];

            data[0] = typeCom;
            data[1] = numBlock;
            data[2] = (byte)fileInfo.Length;

            Array.Copy(fileInfo, 0, data, 3, fileInfo.Length);
            Array.Copy(fileData, 0, data, 3 + fileInfo.Length, fileData.Length);

            return data;
        }

        public override void ExecuteCommand(Transport transport)
        {
            string dirPath = ConfigurationManager.AppSettings["dirPath"];
            string fileInfoStr = Encoding.UTF8.GetString(this.fileInfo);

            if (dirPath != null)
            {
                if (dirPath[dirPath.Length - 1]!='\\')
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

            FileMode fmode = FileMode.Append;
            if (numBlock == 0)
            {
                fmode = FileMode.Create;
            }

            using (FileStream fstream = new FileStream(fileInfo.FullName, fmode, FileAccess.Write, FileShare.ReadWrite))
            {
                fstream.Write(fileData);
            }
        }

        public static FileRequest ToCommand(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            byte numBlock = data[0];
            byte fileInfoLength = data[1];

            byte[] fileInfoBytes = new byte[fileInfoLength];
            Array.Copy(data, 2, fileInfoBytes, 0, fileInfoBytes.Length);

            byte[] fileData = new byte[data.Length - fileInfoBytes.Length - 2];
            Array.Copy(data, 2 + fileInfoBytes.Length, fileData, 0, fileData.Length);

            return new FileRequest(numBlock, fileInfoBytes, fileData);
        }
    }
}
