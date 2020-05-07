using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FileShareRepositoryMover.Models;

namespace FileShareRepositoryMover.Services
{
    class DownloadFile
    {
        public static LocalFile GetFile(string fileId, string fileName, string blobFileName)
        {
            string folderPath = "C:\\Temp\\Mover";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            LocalFile fileData = new LocalFile();
            fileData.FileName = fileName;
            fileData.FilePath = folderPath + "\\" + fileName;
            fileData.Url = "";

            return fileData;
        }
    }
}
