using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileShareRepositoryMover.Models;

namespace FileShareRepositoryMover.Services
{
    class FolderStructure
    {
        public FolderStructure()
        {
            collections = new List<jos_social_files_collections>();
            clusters = new List<jos_social_clusters>();
            folders = new List<Folders>();
            FolderDictionary = new Dictionary<int, Folders>();
        }

        private string mssqlConnection = System.Configuration.ConfigurationManager.ConnectionStrings["CollabConnectionString"].ToString();
        private string mysqlConnection = System.Configuration.ConfigurationManager.ConnectionStrings["JoomlaConnectionString"].ToString();
        private Guid communityId = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["CommunityId"].ToString());

        private List<jos_social_files_collections> collections;
        private List<jos_social_clusters> clusters;
        private List<Folders> folders;
        public Dictionary<int, Folders> FolderDictionary;

        public Dictionary<int, Folders> GetFolders()
        {
            GetClusters();
            GetCollections();
            BuildFolders();

            return FolderDictionary;
        }

        private void GetClusters()
        {
            string query = "SELECT * FROM jos_social_clusters;";

            DataSet results = MysqlActions.QueryResults(mysqlConnection, query);

            foreach (DataRow row in results.Tables[0].Rows)
            {
                jos_social_clusters cluster = new jos_social_clusters
                {
                    id = Convert.ToInt32(row["id"].ToString()),
                    category_id = Convert.ToInt32(row["category_id"].ToString()),
                    cluster_type = row["cluster_type"].ToString(),
                    title = row["title"].ToString()
                };

                clusters.Add(cluster);
            }
        }

        private void GetCollections()
        {
            string query = "SELECT * FROM jos_social_files_collections;";

            DataSet results = MysqlActions.QueryResults(mysqlConnection, query);

            foreach (DataRow row in results.Tables[0].Rows)
            {
                jos_social_files_collections collection = new jos_social_files_collections
                {
                    id = Convert.ToInt32(row["id"].ToString()),
                    owner_id = Convert.ToInt32(row["owner_id"].ToString()),
                    owner_type = row["owner_type"].ToString(),
                    user_id = Convert.ToInt32(row["user_id"].ToString()),
                    title = row["title"].ToString(),
                    desc = row["desc"].ToString(),
                    created = Convert.ToDateTime(row["created"].ToString())
                };

                collections.Add(collection);
            }
        }

        private void BuildFolders()
        {
            foreach (jos_social_clusters cluster in clusters)
            {
                string retrievedId = GetFolderID(communityId, cluster.title, null);
                Folders folder = new Folders();
                
                if (retrievedId == null)
                {
                    folder.FolderId = Guid.NewGuid();
                }
                else
                {
                    try
                    {
                        folder.FolderId = Guid.Parse(retrievedId);
                    }
                    catch
                    {
                        retrievedId = null;
                        folder.FolderId = Guid.NewGuid();
                    }
                }

                folder.CommunityId = communityId;
                folder.ParentFolderId = null;
                folder.FolderLevel = 1;
                folder.FolderName = cluster.title;

                if (cluster.cluster_type.ToLower() == "community")
                {
                    folder.ClusterType = 1;
                }
                else if (cluster.cluster_type.ToLower() == "group")
                {
                    folder.ClusterType = 2;
                }
                else if (cluster.cluster_type.ToLower() == "event")
                {
                    folder.ClusterType = 3;
                }
                else if (cluster.cluster_type.ToLower() == "pdsaproject")
                {
                    folder.ClusterType = 4;
                }
                else
                {
                    folder.ClusterType = 2;
                }

                folder.ClusterId = cluster.id.ToString();
                folder.CreatedOn = DateTime.Now;

                if (retrievedId == null)
                {
                    AddFolder(folder);
                }

                FolderDictionary.Add(cluster.id, folder);
            }

            foreach (jos_social_files_collections collection in collections)
            {
                Guid? parentFolderId = null;

                Folders folder = new Folders();

                folder.CommunityId = communityId;

                if (FolderDictionary.ContainsKey(collection.owner_id))
                {
                    parentFolderId = FolderDictionary[collection.owner_id].FolderId;
                    folder.ParentFolderId = parentFolderId;
                    folder.FolderLevel = FolderDictionary[collection.owner_id].FolderLevel + 1;
                }
                else
                {
                    folder.ParentFolderId = null;
                    folder.FolderLevel = 1;
                }

                folder.FolderName = collection.title;

                string retrievedId = GetFolderID(communityId, folder.FolderName, parentFolderId);

                if (retrievedId == null)
                {
                    folder.FolderId = Guid.NewGuid();
                }
                else
                {
                    try
                    {
                        folder.FolderId = Guid.Parse(retrievedId);
                    }
                    catch
                    {
                        retrievedId = null;
                        folder.FolderId = Guid.NewGuid();
                    }
                }

                if (collection.owner_type.ToLower() == "community")
                {
                    folder.ClusterType = 1;
                }
                else if (collection.owner_type.ToLower() == "group")
                {
                    folder.ClusterType = 2;
                }
                else if (collection.owner_type.ToLower() == "event")
                {
                    folder.ClusterType = 3;
                }
                else if (collection.owner_type.ToLower() == "pdsaproject")
                {
                    folder.ClusterType = 4;
                }
                else
                {
                    folder.ClusterType = 2;
                }

                folder.ClusterId = collection.owner_id.ToString();
                folder.CreatedOn = DateTime.Now;

                if (retrievedId == null)
                {
                    AddFolder(folder);
                }

                FolderDictionary.Add(collection.id, folder);
            }
        }

        private void AddFolder(Folders folder)
        {
            string query = @"INSERT INTO Folders (FolderId,CommunityId,ParentFolderId,FolderLevel,FolderName,ClusterType,ClusterId,CreatedOn)
                VALUES (@FolderId,@CommunityId,@ParentFolderId,@FolderLevel,@FolderName,@ClusterType,@ClusterId,@CreatedOn); SELECT GETDATE();";

            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("FolderId", folder.FolderId);
            parameters.Add("CommunityId", folder.CommunityId);
            parameters.Add("ParentFolderId", folder.ParentFolderId);
            parameters.Add("FolderLevel", folder.FolderLevel);
            parameters.Add("FolderName", folder.FolderName);
            parameters.Add("ClusterType", folder.ClusterType);
            parameters.Add("ClusterId", folder.ClusterId);
            parameters.Add("CreatedOn", folder.CreatedOn);

            DataSet results = MssqlActions.QueryResults(mssqlConnection, query, parameters);
        }

        private string GetFolderID(Guid CommunityId, string FolderName, Guid? ParentFolderId)
        {
            string folderId = null;
            string query;

            if (ParentFolderId != null)
            {
                query = "SELECT FolderId FROM Folders WHERE CommunityId = @CommunityID AND FolderName = @FolderName AND ParentFolderId = @ParentFolderId;";
            }
            else
            {
                query = "SELECT FolderId FROM Folders WHERE CommunityId = @CommunityID AND FolderName = @FolderName AND ParentFolderId IS NULL;";
            }

            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            parameters.Add("CommunityID", CommunityId);
            parameters.Add("FolderName", FolderName);
            parameters.Add("ParentFolderId", ParentFolderId);

            DataSet results = MssqlActions.QueryResults(mssqlConnection, query, parameters);

            if (results.Tables.Count > 0)
            {
                if (results.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        folderId = results.Tables[0].Rows[0][0].ToString();
                    }
                    catch
                    {
                        folderId = null;
                    }
                }
            }

            return folderId;
        }
    }
}
