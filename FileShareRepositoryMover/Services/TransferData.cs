using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileShareRepositoryMover.Models;

namespace FileShareRepositoryMover.Services
{
    class TransferData
    {
        public TransferData()
        {
            collections = new List<jos_social_files_collections>();
            folderDictionary = new Dictionary<int, Folders>();
            social_Files = new List<jos_social_files>();
            resources = new List<Resources>();

            PopulateCollections();
            AddTopLevelFolder();
            BuildFolderDictionary();
            GetFiles();
            PopulateResources();
        }

        private string mssqlConnection = System.Configuration.ConfigurationManager.ConnectionStrings["CollabConnectionString"].ToString();
        private string mysqlConnection = System.Configuration.ConfigurationManager.ConnectionStrings["JoomlaConnectionString"].ToString();
        private Guid communityId = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["CommunityId"].ToString());

        private List<jos_social_files_collections> collections;
        private Dictionary<int, Folders> folderDictionary;
        private Guid topFolderId;

        private List<jos_social_files> social_Files;
        private List<Resources> resources;

        private void PopulateCollections()
        {
            //collections = new List<jos_social_files_collections>();
            string getCollections = @"SELECT * FROM jos_social_files_collections;";
            DataSet collectionsDS = MysqlActions.QueryResults(mysqlConnection, getCollections);

            foreach (DataRow row in collectionsDS.Tables[0].Rows)
            {
                jos_social_files_collections file_collection = new jos_social_files_collections
                {
                    created = Convert.ToDateTime(row["created"]),
                    desc = row["desc"].ToString(),
                    id = Convert.ToInt32(row["id"]),
                    owner_id = Convert.ToInt32(row["owner_id"]),
                    owner_type = row["owner_type"].ToString(),
                    title = row["title"].ToString(),
                    user_id = Convert.ToInt32(row["user_id"])
                };

                collections.Add(file_collection);
            }
        }

        private void AddTopLevelFolder()
        {
            jos_social_files_collections file_collection = new jos_social_files_collections
            {
                created = DateTime.Now,
                desc = "",
                id = 0,
                owner_id = 0,
                owner_type = "",
                title = "",
                user_id = 0
            };

            collections.Add(file_collection);

            Guid? folderId = CheckForTopLevelFolder();
            if (folderId != null)
            {
                topFolderId = Guid.Parse(folderId.ToString());
            }
            else
            {
                topFolderId = Guid.NewGuid();

                Folders newFolder = new Folders
                {
                    FolderId = topFolderId,
                    CommunityId = communityId,
                    ParentFolderId = null,
                    FolderLevel = 0,
                    FolderName = "",
                    ClusterId = null,
                    ClusterType = null
                };

                AddFolder(newFolder);
            }

        }

        private Guid AddFolder(Folders folder)
        {
            string insertFolder = @"INSERT INTO Folders (
    FolderId,
    CommunityId,
    ParentFolderId,
    FolderLevel,
    FolderName,
    ClusterType,
    ClusterId
    )
    VALUES (
    @FolderId,
    @CommunityId,
    @ParentFolderId,
    @FolderLevel,
    @FolderName,
    @ClusterType,
    @ClusterId
    );
    SELECT GETDATE();";

            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("FolderId", folder.FolderId);
            parameters.Add("CommunityId", folder.CommunityId);
            parameters.Add("ParentFolderId", folder.ParentFolderId);
            parameters.Add("FolderLevel", folder.FolderLevel);
            parameters.Add("FolderName", folder.FolderName);
            parameters.Add("ClusterType", folder.ClusterType);
            parameters.Add("ClusterId", folder.ClusterId);

            DataSet results = MssqlActions.QueryResults(mssqlConnection, insertFolder, parameters);

            return folder.FolderId;
        }

        private Guid? CheckForTopLevelFolder()
        {
            string checkForFolder = @"SELECT FolderId FROM Folders WHERE CommunityId = @CommunityId AND FolderLevel = @FolderLevel;";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();

            parameters.Add("CommunityId", communityId);
            parameters.Add("FolderLevel", 0);

            DataSet results = MssqlActions.QueryResults(mssqlConnection, checkForFolder, parameters);

            if (results.Tables[0].Rows[0][0].ToString() != null)
            {
                if (results.Tables[0].Rows.Count > 0)
                {
                    return Guid.Parse(results.Tables[0].Rows[0][0].ToString());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private Guid? GetFolderId(jos_social_files_collections collection)
        {
            string checkForFolder = @"SELECT FolderId FROM Folders WHERE CommunityId = @CommunityId AND FolderName = @FolderName AND FolderLevel = @FolderLevel;";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();

            parameters.Add("CommunityId", communityId);
            parameters.Add("Foldername", collection.title);
            if (collection.id == 0)
            {
                parameters.Add("FolderLevel", 0);
            }
            else
            {
                parameters.Add("FolderLevel", 1);
            }

            DataSet results = MssqlActions.QueryResults(mssqlConnection, checkForFolder, parameters);

            if (results.Tables[0].Rows[0][0].ToString() != null)
            {
                if (results.Tables[0].Rows.Count > 0)
                {
                    return Guid.Parse(results.Tables[0].Rows[0][0].ToString());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void BuildFolderDictionary()
        {
            //folderDictionary = new Dictionary<int, Folders>();
            foreach (jos_social_files_collections collection in collections)
            {
                Folders folder = new Folders();
                if (collection.id != 0)
                {
                    folder.CommunityId = communityId;
                    folder.ParentFolderId = topFolderId;
                    folder.FolderLevel = 1;
                    folder.FolderName = collection.title;
                    folder.ClusterType = null;
                    folder.ClusterId = null;

                    Guid? getFolderId = GetFolderId(collection);
                    if (getFolderId != null)
                    {
                        folder.FolderId = Guid.Parse(getFolderId.ToString());
                    }
                    else
                    {
                        folder.FolderId = Guid.NewGuid();
                        AddFolder(folder);
                    }
                }
                else
                {
                    folder.FolderId = topFolderId;
                    folder.CommunityId = communityId;
                    folder.ParentFolderId = null;
                    folder.FolderLevel = 0;
                    folder.FolderName = collection.title;
                    folder.ClusterType = null;
                    folder.ClusterId = null;
                }

                folderDictionary.Add(collection.id, folder);
            }
        }

        private void GetFiles()
        {
            //social_Files = new List<jos_social_files>();
            string getFiles = @"SELECT * FROM jos_social_files;";

            DataSet results = MysqlActions.QueryResults(mysqlConnection, getFiles);

            foreach (DataRow row in results.Tables[0].Rows)
            {
                if (folderDictionary.ContainsKey(Convert.ToInt32(row["collection_id"])))
                {
                    jos_social_files file = new jos_social_files();
                    file.collection_id = Convert.ToInt32(row["collection_id"]);
                    file.created = Convert.ToDateTime(row["created"]);
                    file.hash = row["hash"].ToString();
                    file.hits = Convert.ToInt32(row["hits"]);
                    file.id = Convert.ToInt32(row["id"]);
                    file.mime = row["mime"].ToString();
                    file.name = row["name"].ToString();
                    file.size = Convert.ToInt32(row["size"]);
                    file.state = Convert.ToInt32(row["state"]);
                    file.storage = row["storage"].ToString();
                    file.type = row["type"].ToString();
                    file.uid = Convert.ToInt32(row["uid"]);
                    file.user_id = Convert.ToInt32(row["user_id"]);

                    social_Files.Add(file);
                }
            }
        }

        private void PopulateResources()
        {
            //resources = new List<Resources>();

            foreach (jos_social_files file in social_Files)
            {
                Resources resource = new Resources();
                resource.ClusterId = null;
                resource.ClusterType = null;
                resource.CommunityId = communityId;
                resource.DeletedOn = null;
                resource.FolderId = folderDictionary[file.collection_id].FolderId;
                resource.Metadata = null;
                resource.ModifiedBy = "RepositoryMover";
                resource.ModifiedOn = DateTime.Now;
                resource.ResourceDescription = null;
                resource.ResourceId = Guid.NewGuid();
                resource.ResourceName = file.name;

                string blobFileName = resource.ResourceId.ToString().Replace("-","");
                blobFileName += System.IO.Path.GetExtension(file.name);

                LocalFile local = DownloadFile.GetFile(file.id.ToString(), file.name, blobFileName);
                BlobFileManager fileManager = new BlobFileManager();
                fileManager.BlobFileName = blobFileName;
                fileManager.ContainerName = System.Configuration.ConfigurationManager.AppSettings["BlobContainer"].ToString().ToLower();
                fileManager.FilePath = local.FilePath;
                fileManager.FolderName = communityId.ToString().Replace("-", "");
                string returnedBlobName = fileManager.UploadStreamToBlob();
                InsertResources(resource);
                resources.Add(resource);

                Console.WriteLine("COPIED " + local.FileName + " TO BLOB: " + blobFileName);
                Console.WriteLine("SAVED LOCALLY TO: " + local.FilePath);
            }
        }

        private void InsertResources(Resources resource)
        {
            string insertResource = @"INSERT INTO Resources (
    ResourceId,
    CommunityId,
    ResourceName,
    ResourceDescription,
    Metadata,
    ClusterType,
    ClusterId,
    ModifiedOn,
    ModifiedBy)
    VALUES
    (
    @ResourceId,
    @CommunityId,
    @ResourceName,
    @ResourceDescription,
    @MetaData,
    @ClusterType,
    @ClusterId,
    @ModifiedOn,
    @ModifiedBy
    );
    SELECT GETDATE();";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();

            parameters.Add("ResourceId", resource.ResourceId);
            parameters.Add("CommunityId", resource.CommunityId);
            parameters.Add("ResourceName", resource.ResourceName);
            parameters.Add("ResourceDescription", resource.ResourceDescription);
            parameters.Add("Metadata", resource.Metadata);
            parameters.Add("ClusterType", resource.ClusterType);
            parameters.Add("ClusterId", resource.ClusterId);
            parameters.Add("ModifiedOn", resource.ModifiedOn);
            parameters.Add("ModifiedBy", resource.ModifiedBy);

            DataSet results = MssqlActions.QueryResults(mssqlConnection, insertResource, parameters);
        }
    }
}
