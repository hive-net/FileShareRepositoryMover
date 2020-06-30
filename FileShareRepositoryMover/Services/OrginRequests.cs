using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FileShareRepositoryMover.Services
{
    class OrginRequests
    {
        private static string mysqlConnection = System.Configuration.ConfigurationManager.ConnectionStrings["JoomlaConnectionString"].ToString();

        public static Dictionary<int,dynamic> GetFiles()
        {
            Dictionary<int, dynamic> files = new Dictionary<int, dynamic>();
            string query = Services.Helper.StringLoader("FileShareRepositoryMover.MySQLQueries.FileNodes.sql");
            DataSet results = MysqlActions.QueryResults(mysqlConnection, query);
            foreach(DataRow row in results.Tables[0].Rows)
            {
                Dictionary<string, dynamic> file = new Dictionary<string, dynamic>();
                foreach(DataColumn column in results.Tables[0].Columns)
                {
                    file.Add(column.ColumnName.ToString(), row[column.ColumnName].ToString());
                }
                files.Add(Convert.ToInt32(file["fid"]), file);
            }
            return files;
        }

        public static Dictionary<int, int> GetNodeCounts()
        {
            Dictionary<int, int> nodes = new Dictionary<int, int>();
            string query = Services.Helper.StringLoader("FileShareRepositoryMover.MySQLQueries.NodeContentCounts.sql");
            DataSet results = MysqlActions.QueryResults(mysqlConnection, query);
            foreach(DataRow row in results.Tables[0].Rows)
            {
                nodes.Add(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]));
            }

            return nodes;
        }

        public static Dictionary<int, dynamic> GetNodeData()
        {
            Dictionary<int, dynamic> nodes = new Dictionary<int, dynamic>();
            string query = Services.Helper.StringLoader("FileShareRepositoryMover.MySQLQueries.NodeData.sql");
            DataSet results = MysqlActions.QueryResults(mysqlConnection, query);
            foreach(DataRow row in results.Tables[0].Rows)
            {
                Dictionary<string, dynamic> node = new Dictionary<string, dynamic>();
                node.Add("nid", Convert.ToInt32(row["nid"].ToString()));
                node.Add("tid", Convert.ToInt32(row["tid"].ToString()));
                node.Add("ContentCount", row["ContentCount"].ToString());
                node.Add("title", row["title"].ToString());

                nodes.Add(Convert.ToInt32(row["nid"]), node);
            }

            return nodes;
        }

        public static Dictionary<int, dynamic> GetFileData()
        {
            Dictionary<int, dynamic> files = new Dictionary<int, dynamic>();
            string query = Services.Helper.StringLoader("FileShareRepositoryMover.MySQLQueries.FileNodes.sql");
            DataSet results = MysqlActions.QueryResults(mysqlConnection, query);
            foreach (DataRow row in results.Tables[0].Rows)
            {
                int fid = Convert.ToInt32(row["fid"].ToString());
                Dictionary<string, dynamic> file = new Dictionary<string, dynamic>();
                file.Add("fid", fid);
                file.Add("nid", Convert.ToInt32(row["nid"].ToString()));
                file.Add("filename", row["filename"].ToString());
                file.Add("uri", row["uri"].ToString());
                file.Add("filemime", row["filemime"].ToString());
                file.Add("title", row["title"].ToString());

                files.Add(fid, file);
            }

            return files;
        }
    }
}
