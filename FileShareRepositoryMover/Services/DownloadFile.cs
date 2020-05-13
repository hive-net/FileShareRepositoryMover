using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            string sessionCookie = System.Configuration.ConfigurationManager.AppSettings["SessionCookie"].ToString();

            string folderPath = "C:\\Temp\\Mover";
            string fileUrl = System.Configuration.ConfigurationManager.AppSettings["Website"].ToString();
            string loginUrl = System.Configuration.ConfigurationManager.AppSettings["Website"].ToString();
            fileUrl = @"https://" + fileUrl + @"/index.php?option=com_easysocial&view=groups&layout=preview&fileid=" + fileId + @"&tmpl=component";
            loginUrl = @"https://" + loginUrl + @"/index.php/login";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            LocalFile fileData = new LocalFile();
            fileData.FileName = fileName;
            fileData.FilePath = folderPath + "\\" + blobFileName;
            fileData.Url = fileUrl;


            try
            {
                using (WebClient client = new WebClient())
                {
                    //NameValueCollection values = new NameValueCollection
                    //{
                    //    { "username", userName },
                    //    { "password", password }
                    //};
                    //client.UploadValues(new Uri(loginUrl), "POST", values);

                    client.Headers.Add("Cookie", sessionCookie);

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
