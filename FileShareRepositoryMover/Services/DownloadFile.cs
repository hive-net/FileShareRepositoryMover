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
            string userName = System.Configuration.ConfigurationManager.AppSettings["UserName"].ToString();
            string password = System.Configuration.ConfigurationManager.AppSettings["Password"].ToString();

            string folderPath = "C:\\Temp\\Mover";
            string fileUrl = System.Configuration.ConfigurationManager.AppSettings["Website"].ToString();
            fileUrl = @"https://" + fileUrl + @"/index.php?option=com_easysocial&view=groups&layout=preview&fileid=" + fileId + @"&tmpl=component";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            LocalFile fileData = new LocalFile();
            fileData.FileName = fileName;
            fileData.FilePath = folderPath + "\\" + blobFileName;
            fileData.Url = fileUrl;

            //***********************************************
            //USE WEBCLIENT TO DOWNLOAD FILE TO FILEPATH HERE
            //***********************************************

            /*
            string formUrl = System.Configuration.ConfigurationManager.AppSettings["Website"].ToString();
            formUrl += @"/index.php/login";
            string formParams = string.Format("username={0}&password={1}", userName, password);
            string cookieHeader;
            WebRequest req = WebRequest.Create(formUrl);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(formParams);
            req.ContentLength = bytes.Length;

            using (Stream os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
            }

            WebResponse resp = req.GetResponse();
            cookieHeader = resp.Headers["Set-cookie"];
            */

            try
            {
                using (WebClient client = new WebClient())
                {
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));
                    client.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";
                    client.DownloadFile(fileUrl, fileData.FilePath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                throw;
            }


            return fileData;
        }
    }
}
