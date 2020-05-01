using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace FileShareRepositoryMover.Services
{
    class MysqlActions
    {
        public static DataSet QueryResults(string connectionString, string query)
        {
            DataSet results = new DataSet();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
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
