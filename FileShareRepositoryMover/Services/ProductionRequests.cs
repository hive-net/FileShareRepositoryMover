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
            string query = "INSERT INTO Folder (FolderId,CommunityId,ParentFolderId,FolderLevel,FolderName,ClusterType,ClusterId,CreatedOn) "
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
    }
}
