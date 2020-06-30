using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FileShareRepositoryMover.Services
{
    class FolderCreation
    {
        public FolderCreation()
        {
            communityId = Services.ProductionRequests.GetCommunityId();
            BuildPrimaryNodes();
            BuildSecondaryNodes();
        }

        public Dictionary<int, dynamic> PrimaryNodes;
        public Dictionary<int, dynamic> SecondaryNodes;
        private static string mysqlConnection = System.Configuration.ConfigurationManager.ConnectionStrings["JoomlaConnectionString"].ToString();
        private Guid communityId;

        private void BuildPrimaryNodes()
        {
            string query = "SELECT * FROM taxonomy_term_data WHERE tid in (1200,1251)";
            DataSet results = Services.MysqlActions.QueryResults(mysqlConnection, query);

            PrimaryNodes = new Dictionary<int, dynamic>();

            foreach(DataRow row in results.Tables[0].Rows)
            {
                Dictionary<string, dynamic> node = new Dictionary<string, dynamic>();
                node.Add("tid", Convert.ToInt32(row["tid"].ToString()));
                node.Add("vid", Convert.ToInt32(row["vid"].ToString()));
                node.Add("name", row["name"].ToString());

                Guid folderId = Guid.NewGuid();
                //Guid folderId = Services.ProductionRequests.CheckAddFolder(communityId, PrimaryNodes[tid]["folderId"], 1, nodeData[nid]["title"]);
                node.Add("folderId", folderId);

                node.Add("parentFolderId", null);
                node.Add("folderLevel", 1);

                PrimaryNodes.Add(Convert.ToInt32(row["tid"].ToString()), node);
            }

            Dictionary<string, dynamic> generalNode = new Dictionary<string, dynamic>();
            generalNode.Add("tid", 0);
            generalNode.Add("vid", 30);
            generalNode.Add("name", "General");

            Guid generalFolderId = Guid.NewGuid();
            //Guid folderId = Services.ProductionRequests.CheckAddFolder(communityId, PrimaryNodes[tid]["folderId"], 1, nodeData[nid]["title"]);
            generalNode.Add("folderId", generalFolderId);

            generalNode.Add("parentFolderId", null);
            generalNode.Add("folderLevel", 1);

            PrimaryNodes.Add(0, generalNode);
        }

        private void BuildSecondaryNodes()
        {
            Dictionary<int, dynamic> nodeData = Services.OrginRequests.GetNodeData();
            Dictionary<int, int> nodeCounts = Services.OrginRequests.GetNodeCounts();

            SecondaryNodes = new Dictionary<int, dynamic>();

            foreach (KeyValuePair<int, int> count in nodeCounts)
            {
                int nid = count.Key;
                Dictionary<string, dynamic> node = new Dictionary<string, dynamic>();

                if (count.Value == 1)
                {
                    int tid = nodeData[nid]["tid"];
                    node.Add("nid", nid);
                    node.Add("tid", nodeData[nid]["tid"]);
                    node.Add("title", nodeData[nid]["title"]);
                    node.Add("folderName", PrimaryNodes[tid]["name"]);
                    node.Add("folderId", PrimaryNodes[tid]["folderId"]);
                    node.Add("parentFolderId", null);
                    node.Add("folderLevel", 1);

                    SecondaryNodes.Add(nid, node);
                }
                else
                {
                    int tid = nodeData[nid]["tid"];
                    node.Add("nid", nid);
                    node.Add("tid", nodeData[nid]["tid"]);
                    node.Add("title", nodeData[nid]["title"]);
                    node.Add("folderName", nodeData[nid]["title"]);

                    Guid folderId = Guid.NewGuid();
                    //Guid folderId = Services.ProductionRequests.CheckAddFolder(communityId, PrimaryNodes[tid]["folderId"], 2, nodeData[nid]["title"]);
                    node.Add("folderId", folderId);

                    node.Add("parentFolderId", PrimaryNodes[tid]["folderId"]);
                    node.Add("folderLevel", 1);

                    SecondaryNodes.Add(nid, node);
                }
            }
        }
    }
}
