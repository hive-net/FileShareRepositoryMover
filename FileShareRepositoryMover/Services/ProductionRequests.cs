using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FileShareRepositoryMover.Services
{
    class ProductionRequests
    {
        private static string mssqlConnection = System.Configuration.ConfigurationManager.ConnectionStrings["CollabConnectionString"].ToString();

        public static Guid GetCommunityId()
        {
            string query = "SELECT TOP 1 CommunityID FROM Communities WHERE CommunityName = @CommunityName";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("CommunityName",System.Configuration.ConfigurationManager.AppSettings["Community"].ToString());
            DataSet results = Services.MssqlActions.QueryResults(mssqlConnection,query,parameters);

            Guid communityId;

            try
            {
                communityId = Guid.Parse(results.Tables[0].Rows[0][0].ToString());
            }
            catch
            {
                communityId = Guid.Parse("00000000-0000-0000-0000-000000000000");
            }
            return communityId;
        }

        public static Guid CheckAddFolder(Guid communityId, string parentFolderId, int folderLevel, string folderName)
        {
            string query;
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("CommunityId", communityId);
            try
            {
                Guid foo = Guid.Parse(parentFolderId);
                parameters.Add("ParentFolderId", foo);
                query = "SELECT TOP 1 FolderId FROM Folders WHERE CommunityId = @CommunityId AND ParentFolderId = @ParentFolderId AND FolderName = @FolderName AND FolderLevel = @FolderLevel AND DeletedOn IS NULL";

            }
            catch
            {
                query = "SELECT TOP 1 FolderId FROM Folders WHERE CommunityId = @CommunityId AND ParentFolderId IS NULL AND FolderName = @FolderName AND FolderLevel = @FolderLevel AND DeletedOn IS NULL";
            }
            parameters.Add("FolderLevel", folderLevel);
            parameters.Add("FolderName", folderName);
            DataSet results = Services.MssqlActions.QueryResults(mssqlConnection, query, parameters);

            Guid folderId;

            try
            {
                folderId = Guid.Parse(results.Tables[0].Rows[0][0].ToString());
            }
            catch
            {
                folderId = CreateNewFolder(communityId, parentFolderId, folderLevel, folderName);
            }

            return folderId;
        }

        public static Guid CreateNewFolder(Guid communityId, string parentFolderId, int folderLevel, string folderName)
        {
            Guid folderId = Guid.NewGuid();
            string query = "INSERT INTO Folders (FolderId,CommunityId,ParentFolderId,FolderLevel,FolderName,ClusterType,ClusterId,CreatedOn) "
                + "VALUES (@FolderId,@CommunityId,@ParentFolderId,@FolderLevel,@FolderName,@ClusterType,@ClusterId,GETDATE()); SELECT GETDATE()";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("FolderId", folderId);
            parameters.Add("CommunityId", communityId);
            parameters.Add("ParentFolderId", parentFolderId);
            parameters.Add("FolderLevel", folderLevel);
            parameters.Add("FolderName", folderName);
            parameters.Add("ClusterType", 2);
            parameters.Add("ClusterId", 93);
            DataSet results = Services.MssqlActions.QueryResults(mssqlConnection, query, parameters);

            return folderId;
        }

        public static Guid GetFolderId(Guid communityId, string parentFolderId, int folderLevel, string folderName)
        {
            string query;
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("CommunityId", communityId);
            try
            {
                Guid foo = Guid.Parse(parentFolderId);
                parameters.Add("ParentFolderId", foo);
                query = "SELECT TOP 1 FolderId FROM Folders WHERE CommunityId = @CommunityId AND ParentFolderId = @ParentFolderId AND FolderName = @FolderName AND FolderLevel = @FolderLevel AND DeletedOn IS NULL";

            }
            catch
            {
                query = "SELECT TOP 1 FolderId FROM Folders WHERE CommunityId = @CommunityId AND ParentFolderId IS NULL AND FolderName = @FolderName AND FolderLevel = @FolderLevel AND DeletedOn IS NULL";
            }
            parameters.Add("FolderLevel", folderLevel);
            parameters.Add("FolderName", folderName);
            DataSet results = Services.MssqlActions.QueryResults(mssqlConnection, query, parameters);

            Guid folderId;

            try
            {
                folderId = Guid.Parse(results.Tables[0].Rows[0][0].ToString());
            }
            catch
            {
                //folderId = Guid.Parse("00000000-0000-0000-0000-000000000000");
                folderId = Guid.NewGuid();
            }
            return folderId;
        }

        public static void InsertResource(Models.Resources resource)
        {
            string query = "INSERT INTO Resources VALUES (@ResourceId,@CommunityId,@FolderId,@ResourceName,@ResourceDescription,@Metadata,@ClusterType,@ClusterId,GETDATE(),'ICN-MOVER',NULL,NULL); SELECT GETDATE()";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("ResourceId",resource.ResourceId );
            parameters.Add("CommunityId",resource.CommunityId);
            parameters.Add("FolderId",resource.FolderId);
            parameters.Add("ResourceName",resource.ResourceName);
            parameters.Add("ResourceDescription",resource.ResourceDescription);
            parameters.Add("Metadata",resource.Metadata);
            parameters.Add("ClusterType",resource.ClusterType);
            parameters.Add("ClusterId",resource.ClusterId);
            DataSet results = Services.MssqlActions.QueryResults(mssqlConnection, query, parameters);
        }

        public static void InsertBlobResource(Models.BlobResources blobResource)
        {
            string query = "INSERT INTO BlobResources VALUES (@ResourceId,@CommunityId,@BlobFileName,@BlobFileType,NULL); SELECT GETDATE()";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("ResourceId",blobResource.Resourceid);
            parameters.Add("CommunityId",blobResource.CommunityId);
            parameters.Add("BlobFileName",blobResource.BlobFileName);
            parameters.Add("BlobFileType",blobResource.BlobFileType);
            DataSet results = Services.MssqlActions.QueryResults(mssqlConnection, query, parameters);
        }

        public static void InsertLinkResource(Models.LinkResources linkResource)
        {
            string query = "INSERT INTO LinkResources VALUES (@ResourceId,@CommunityId,@LinkUrl); SELECT GETDATE()";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("ResourceId", linkResource.Resourceid);
            parameters.Add("CommunityId", linkResource.CommunityId);
            parameters.Add("LinkUrl", linkResource.LinkUrl);
            DataSet results = Services.MssqlActions.QueryResults(mssqlConnection, query, parameters);
        }
    }
}
