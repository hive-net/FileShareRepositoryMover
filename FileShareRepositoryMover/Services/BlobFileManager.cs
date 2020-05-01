using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /*
        public async Task<BlobSasInfo> GetBlobSasUri(Guid communityId, string blobFileName)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Utilities.GuidToString(communityId));
            await blobContainer.CreateIfNotExistsAsync();

            // Get a reference to a blob within the container.
            // Note that the blob may not exist yet, but a SAS can still be created for it.
            var blob = blobContainer.GetBlockBlobReference(blobFileName);

            // Create a new access policy and define its constraints.
            // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad hoc SAS, and
            // to construct a shsared access policy that is saved to the container's shared access policies.
            var adHocSas = new SharedAccessBlobPolicy()
            {
                // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write |
                              SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.List
            };

            // Generate the shared access signature on the blob, setting the constraints directly on the signature.
            var sasBlobToken = blobContainer.GetSharedAccessSignature(adHocSas);

            // Return the URI string for the container, including the SAS token.
            return new BlobSasInfo
            {
                Uri = blob.Uri + sasBlobToken,
                ContainerName = blobContainer.Name,
                BlobFileName = blobFileName
            };
        }
        */
    }
}
