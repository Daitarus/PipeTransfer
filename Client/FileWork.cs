using System.Collections.Generic;
using System.IO;
using System;
using System.IO.Compression;
using Microsoft.Win32;

namespace Client
{
    internal static class FileWork
    {
        private const string nameParentKey = "SOFTWARE";
        private const string nameKey = "PipeTransferClient";
        private const string nameValue = "FilesDate";


        public static List<FileInfo> GetFilesForDate(List<FileInfo> files, DateTime date)
        {
            List<FileInfo> newFiles = new List<FileInfo>();
            foreach (FileInfo file in files)
            {
                if (DateTime.Compare(file.LastWriteTime, date) > 0)
                {
                    newFiles.Add(file);
                }
            }
            return newFiles;
        }
        public static List<FileInfo> GetFilesForExtensions(List<FileInfo> files, string[] extensions)
        {
            List<FileInfo> newFiles = new List<FileInfo>();
            foreach (FileInfo file in files)
            {
                if (extensions != null && extensions.Length > 0)
                {
                    foreach (string extension in extensions)
                    {
                        if (file.Extension == extension)
                        {
                            newFiles.Add(file);
                        }
                    }
                }
                else
                {
                    newFiles.Add(file);
                }
            }
            return newFiles;
        }

        public static void CopyToZip(string nameZip, List<FileInfo> files, string notCopyPath)
        {
            notCopyPath += '\\';
            bool isFileInZip = false;

            using (FileStream fstream = new FileStream(nameZip, FileMode.Create))
            using (ZipArchive zip = new ZipArchive(fstream, ZipArchiveMode.Create))
            {
                foreach (FileInfo file in files)
                {
                    string newName = file.FullName.Replace(notCopyPath, "");
                    try
                    {
                        zip.CreateEntryFromFile(file.FullName, newName);
                        isFileInZip = true;
                    }
                    catch { }
                }
            }

            if (!isFileInZip)
            {
                FileInfo zip = new FileInfo(nameZip);
                if (zip.Exists)
                    zip.Delete();
            }
        }

        public static DateTime GetDateFromRegistry()
        {
            DateTime date = DateTime.MinValue;

            RegistryKey softwareRegistry = Registry.CurrentUser.OpenSubKey(nameParentKey);
            RegistryKey pipeTransferRegistry = softwareRegistry.OpenSubKey(nameKey, true);

            if (pipeTransferRegistry != null)
            {
                long binaryDate = 0;
                if (long.TryParse(Convert.ToString(pipeTransferRegistry.GetValue(nameValue)), out binaryDate))
                {
                    date = DateTime.FromBinary(binaryDate);
                }
                pipeTransferRegistry.Close();
            }
            softwareRegistry.Close();

            return date;
        }
        public static void AddDateToRegistry(DateTime date)
        {
            RegistryKey softwareRegistry = Registry.CurrentUser.OpenSubKey(nameParentKey, true);
            RegistryKey pipeTransferRegistry = softwareRegistry.OpenSubKey(nameKey, true);

            if (pipeTransferRegistry == null)
            {
                pipeTransferRegistry = softwareRegistry.CreateSubKey(nameKey, true);
            }

            pipeTransferRegistry.SetValue(nameValue, (long)date.ToBinary());

            pipeTransferRegistry.Close();
            softwareRegistry.Close();
        }
    }
}
