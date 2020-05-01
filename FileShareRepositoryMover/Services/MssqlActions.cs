using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace FileShareRepositoryMover.Services
{
    class MssqlActions
    {
        public static DataSet QueryResults(string connectionString, string query, Dictionary<string, dynamic> parameters)
        {
            DataSet results = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (KeyValuePair<string, dynamic> pair in parameters)
                            {
                                if (pair.Value != null)
                                {
                                    command.Parameters.AddWithValue(pair.Key, pair.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(pair.Key, DBNull.Value);
                                }
                            }

                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                try
                                {
                                    adapter.Fill(results);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                    Console.ReadLine();
                                    throw;
                                }
                            }
                        }
                    }

                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.ReadLine();
                    throw;
                }
            }

            return results;
        }
    }
}
