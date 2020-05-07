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
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CollabConnectionString"].ToString();
            MetaData = new Dictionary<string, string>();
        }

        public Dictionary<string, string> MetaData { get; set; }
        public string ContainerName { get; set; }
        public string BlobFileName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public string UploadStreamToBlob()
        {
            MetaData.Add("FileName", FileName);
            ContainerName = ContainerName.ToLower();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(ContainerName);
            blobContainer.CreateIfNotExistsAsync();
            BlobFileName = BlobFileName;
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(BlobFileName);
            foreach (KeyValuePair<string, string> pair in MetaData)
            {
                blockBlob.Metadata.Add(pair.Key, pair.Value);
            }
            using (System.IO.Stream stream = System.IO.File.OpenRead(FilePath))
            {
                blockBlob.UploadFromStreamAsync(stream);
            }
            return BlobFileName;
        }
    }
}
