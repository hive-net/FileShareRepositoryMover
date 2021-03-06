﻿using System;
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

        public static Dictionary<int, string> GetNodeLinks()
        {
            Dictionary<int, string> nodeLinks = new Dictionary<int, string>();
            string query = Services.Helper.StringLoader("FileShareRepositoryMover.MySQLQueries.LinkNodes.sql");
            DataSet results = MysqlActions.QueryResults(mysqlConnection, query);
            foreach(DataRow row in results.Tables[0].Rows)
            {
                nodeLinks.Add(Convert.ToInt32(row[0].ToString()), row[1].ToString());
            }

            return nodeLinks;
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
                string uri = row["uri"].ToString();
                uri = uri.Replace(@"public://", @"C:\Temp\").Replace(@"/", @"\");

                if(System.IO.File.Exists(uri))
                {
                    int fid = Convert.ToInt32(row["fid"].ToString());
                    Dictionary<string, dynamic> file = new Dictionary<string, dynamic>();
                    file.Add("fid", fid);
                    file.Add("nid", Convert.ToInt32(row["nid"].ToString()));
                    file.Add("filename", row["filename"].ToString());
                    file.Add("uri", uri);
                    file.Add("filemime", row["filemime"].ToString());
                    file.Add("title", row["title"].ToString());

                    files.Add(fid, file);
                }
            }

            return files;
        }

        public static Dictionary<int, int> NodeActualContentCount()
        {
            Dictionary<int, dynamic> nodeData = GetNodeData();
            Dictionary<int, string> nodeLinkCounts = GetNodeLinks();
            Dictionary<int, dynamic> fileData = GetFileData();

            Dictionary<int, int> nodeActualCount = new Dictionary<int, int>();

            foreach(int node in nodeData.Keys)
            {
                int content = 0;

                if(nodeLinkCounts.ContainsKey(node))
                {
                    content += 1;
                }

                foreach(Dictionary<string, dynamic> file in fileData.Values)
                {
                    if(file["nid"] == node)
                    {
                        content += 1;
                    }
                }

                nodeActualCount.Add(node, content);
            }

            return nodeActualCount;
        }

        public static List<string> GetNodeKeyWords(int nid)
        {
            List<string> keywords = new List<string>();

            string query = "SELECT td.name FROM taxonomy_index ti JOIN taxonomy_term_data td ON td.tid = ti.tid AND td.vid = 1 WHERE ti.nid = " + nid.ToString() + " GROUP BY td.name";
            DataSet results = MysqlActions.QueryResults(mysqlConnection, query);

            foreach(DataRow row in results.Tables[0].Rows)
            {
                keywords.Add(row[0].ToString());
            }

            return keywords;
        }

        public static string GetFileDescription(int nid, int fid)
        {
            string query = "SELECT field_add_file_description FROM field_data_field_add_file WHERE entity_id = " + nid.ToString() + " AND field_add_file_fid = " + fid.ToString() + " GROUP BY field_add_file_description;";
            DataSet results = MysqlActions.QueryResults(mysqlConnection, query);

            string description;

            if (results.Tables[0].Rows.Count > 0)
            {
                description = results.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                description = null;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                query = "SELECT title FROM node WHERE nid = " + nid.ToString() + ";";
                results = MysqlActions.QueryResults(mysqlConnection, query);

                if (results.Tables[0].Rows.Count > 0)
                {
                    description = results.Tables[0].Rows[0][0].ToString();
                }
            }

            return description;
        }

        public static string GetNodeTitle(int nid)
        {
            string query = "SELECT title FROM node WHERE nid = " + nid.ToString() + ";";
            DataSet results = MysqlActions.QueryResults(mysqlConnection, query);

            string description;

            if (results.Tables[0].Rows.Count > 0)
            {
                description = results.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                description = null;
            }

            return description;
        }
    }
}
