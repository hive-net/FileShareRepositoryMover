using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShareRepositoryMover.Services
{
    class FileTransfer
    {
        public FileTransfer()
        {
            CommunityId = Services.ProductionRequests.GetCommunityId();
            folders = new FolderCreation();
            fileData = Services.OrginRequests.GetFileData();
            LinkNodes = Services.OrginRequests.GetNodeLinks();
            PopulateResources();
        }

        public Services.FolderCreation folders;
        public Dictionary<int, dynamic> fileData;
        public Dictionary<int, string> LinkNodes;
        public List<Models.Resources> Resources;
        public Guid CommunityId;

        public void PopulateResources()
        {
            foreach (Dictionary<string, dynamic> file in fileData.Values)
            {
                int nid = file["nid"];
                int fid = file["fid"];
                Guid folderId = folders.SecondaryNodes[nid]["folderId"];
                string filePath = file["uri"];
                string resourceDescription = Services.OrginRequests.GetFileDescription(nid, fid);

                resourceDescription = resourceDescription.Replace("&#39;","'");
                resourceDescription = resourceDescription.Replace("&amp;", "&");
                resourceDescription = resourceDescription.Replace("&nbsp;", " ");

                Models.Resources resource = new Models.Resources();
                resource.ResourceId = Guid.NewGuid();
                resource.CommunityId = CommunityId;
                resource.FolderId = folderId;
                resource.ResourceName = file["filename"];
                resource.ResourceDescription = resourceDescription;
                resource.ClusterType = 2;
                resource.ClusterId = "93";
                resource.ModifiedOn = DateTime.Now;
                resource.ModifiedBy = "RepositoryMover";

                Services.ProductionRequests.InsertResource(resource);

                string fileExtension = System.IO.Path.GetExtension(filePath);
                string blobFileName = resource.CommunityId.ToString().Replace("-", "").ToLower() + "/" + resource.ResourceId.ToString().Replace("-","").ToLower() + fileExtension;

                Models.BlobResources blobResource = new Models.BlobResources();

                blobResource.Resourceid = resource.ResourceId;
                blobResource.CommunityId = CommunityId;
                blobResource.BlobFileName = blobFileName;
                blobResource.BlobFileType = file["filemime"];

                Services.ProductionRequests.InsertBlobResource(blobResource);

                List<string> keywords = Services.OrginRequests.GetNodeKeyWords(nid);
                foreach(string keyword in keywords)
                {
                    Services.ProductionRequests.InsertResourceKeyWord(resource.ResourceId, keyword);
                }


                Services.BlobFileManager blobFileManager = new BlobFileManager();
                blobFileManager.BlobFileName = resource.ResourceId.ToString().Replace("-", "").ToLower() + fileExtension;
                blobFileManager.ContainerName = System.Configuration.ConfigurationManager.AppSettings["BlobContainer"].ToString();
                blobFileManager.ContentType = file["filemime"];
                blobFileManager.FolderName = resource.CommunityId.ToString().Replace("-", "").ToLower();
                blobFileManager.FileName = System.IO.Path.GetFileName(filePath);
                blobFileManager.FilePath = filePath;

                blobFileManager.UploadStreamToBlob();

                Console.WriteLine(blobFileName);
            }

            foreach(KeyValuePair<int, string> link in LinkNodes)
            {
                int nid = link.Key;
                string uri = link.Value;
                Guid resourceId = Guid.NewGuid();
                Guid folderId = folders.SecondaryNodes[nid]["folderId"];
                string resourceDescription = Services.OrginRequests.GetNodeTitle(nid);

                resourceDescription = resourceDescription.Replace("&#39;", "'");
                resourceDescription = resourceDescription.Replace("&amp;", "&");
                resourceDescription = resourceDescription.Replace("&nbsp;", " ");

                Models.Resources resource = new Models.Resources();
                resource.CommunityId = CommunityId;
                resource.ResourceId = resourceId;
                resource.FolderId = folderId;
                resource.ResourceName = folders.SecondaryNodes[nid]["title"];
                resource.ResourceDescription = resourceDescription;
                resource.ClusterType = 2;
                resource.ClusterId = "93";
                resource.ModifiedOn = DateTime.Now;
                resource.ModifiedBy = "RepositoryMover";

                Services.ProductionRequests.InsertResource(resource);

                Models.LinkResources linkResource = new Models.LinkResources();
                linkResource.CommunityId = CommunityId;
                linkResource.Resourceid = resourceId;
                linkResource.LinkUrl = uri;

                Services.ProductionRequests.InsertLinkResource(linkResource);

                List<string> keywords = Services.OrginRequests.GetNodeKeyWords(nid);
                foreach (string keyword in keywords)
                {
                    Services.ProductionRequests.InsertResourceKeyWord(resource.ResourceId, keyword);
                }

                Console.WriteLine(folders.SecondaryNodes[nid]["title"]);
            }
        }
    }
}
