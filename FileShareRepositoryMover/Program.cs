using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FileShareRepositoryMover
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, dynamic> files = Services.OrginRequests.GetFileData();
            int fileCount = 0;
            foreach(Dictionary<string, dynamic> file in files.Values)
            {
                Console.WriteLine(file["fid"] + " - " + file["uri"]);
                if(System.IO.File.Exists(file["uri"]))
                {
                    fileCount += 1;
                    Console.WriteLine("FILE EXISTS");
                }
                else
                {
                    Console.WriteLine("NO FILE");
                }
            }
            Console.WriteLine(fileCount.ToString() + " FILES");
            Console.WriteLine("MIGRATION COMPLETE");
            Console.ReadLine();
        }
    }
}
