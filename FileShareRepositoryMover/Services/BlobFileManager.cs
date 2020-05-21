using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FileShareRepositoryMover.Services
{
    class BlobFileManager
    {
        private readonly string _connectionString;
        public BlobFileManager()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BlobConnectionString"].ToString();
            MetaData = new Dictionary<string, string>();
        }

        public Dictionary<string, string> MetaData { get; set; }
        public string ContainerName { get; set; }
        public string BlobFileName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FolderName { get; set; }
        public string ContentType { get; set; }

        public string UploadStreamToBlob()
        {
            MetaData.Add("FileName", FileName);
            ContainerName = ContainerName.ToLower();
            //ContainerName = "testarea";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(ContainerName);
            blobContainer.CreateIfNotExistsAsync();
            //BlobFileName = FolderName + "//" + BlobFileName;
            CloudBlobDirectory blobFolder = blobContainer.GetDirectoryReference(FolderName);
            //CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(BlobFileName);
            CloudBlockBlob blockBlob = blobFolder.GetBlockBlobReference(BlobFileName);
            blockBlob.Properties.ContentType = ContentType;
            foreach (KeyValuePair<string, string> pair in MetaData)
            {
                if (pair.Value != null)
                {
                    blockBlob.Metadata.Add(pair.Key, pair.Value);
                }
                else
                {
                    blockBlob.Metadata.Add(pair.Key, "");
                }
            }
            using (System.IO.Stream stream = System.IO.File.OpenRead(FilePath))
            {
                blockBlob.UploadFromStream(stream);
            }
            return BlobFileName;
        }
    }
}
