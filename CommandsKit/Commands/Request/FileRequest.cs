using PipeProtocolTransport;
using System.Text;
using System.Xml.Linq;

namespace CommandsKit
{
    public class FileRequest : Command
    {
        private readonly byte numBlock;
        private readonly string fileInfo;
        private readonly byte[] fileData;

        public FileRequest(byte numBlock, string fileInfo, byte[] fileData)
        {
            if (fileInfo == null || fileInfo == "")
                throw new ArgumentNullException(nameof(fileInfo));
            if (fileData == null)
                throw new ArgumentNullException(nameof(fileData));

            byte[] fileInfoBytes = Encoding.UTF8.GetBytes(fileInfo);
            if(fileInfoBytes.Length > byte.MaxValue)
                throw new ArgumentException($"Size must be less then {byte.MaxValue}", nameof(fileInfoBytes));
            if (fileData.Length + fileInfoBytes.Length > Command.maxLengthData - 3)
                throw new ArgumentException($"Size {nameof(fileInfo)} and {nameof(fileData)} must be less then {Command.maxLengthData - 3}");

            typeCom = (byte)TypeCommand.FILE_REQUEST;
            this.numBlock = numBlock;
            this.fileInfo = fileInfo;
            this.fileData = fileData;
        }

        public override byte[] ToBytes()
        {
            byte[] fileInfoBytes = Encoding.UTF8.GetBytes(fileInfo);

            byte[] data = new byte[3 + fileInfoBytes.Length + fileData.Length];

            data[0] = typeCom;
            data[1] = numBlock;
            data[2] = (byte)fileInfoBytes.Length;

            Array.Copy(fileInfoBytes, 0, data, 3, fileInfoBytes.Length);
            Array.Copy(fileData, 0, data, 3 + fileInfoBytes.Length, fileData.Length);

            return data;
        }

        public override void ExecuteCommand(Transport transport)
        {
            FileInfo fileInfo = new FileInfo(this.fileInfo);

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
            string fileInfo = Encoding.UTF8.GetString(fileInfoBytes);

            byte[] fileData = new byte[data.Length - fileInfoBytes.Length - 2];
            Array.Copy(data, 2 + fileInfoBytes.Length, fileData, 0, fileData.Length);

            return new FileRequest(numBlock, fileInfo, fileData);
        }
    }
}
