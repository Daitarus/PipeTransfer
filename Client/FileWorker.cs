using CommandsKit;
using PipeProtocolTransport;
using System.Text;
using ConsoleWorker;
using System.Timers;

namespace Client
{
    internal static class FileWorker
    {
        private static DateTime time = DateTime.MinValue;

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

                //test timer
                var timer = new System.Timers.Timer(1000);
                timer.AutoReset = true;
                timer.Enabled = true;
                DateTime start = DateTime.Now;
                //

                using (FileStream fileStream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    for (int i = 0; i < amountBlocks; i++)
                    {
                        byte[] fileBlock = ReadFileBlock(fileStream, i * maxLengthBlock, maxLengthBlock, fileInfo.Length);
                        Command com = new FileRequest(i, amountBlocks, fileInfoBytes, fileBlock);
                        if (!client.SendCommand(com))
                            break;

                        //timer++
                        timer.Elapsed += OnTimedEvent;
                        //

                        string outStr = CreatorOutString.GetLoadString(fileInfo.Name, i, amountBlocks);
                        Console.Write(outStr);                       
                    }
                    Console.WriteLine();
                    Console.WriteLine("Time spend: {0}", time - start);
                }
            }
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            time = e.SignalTime;
        }
    }
}
