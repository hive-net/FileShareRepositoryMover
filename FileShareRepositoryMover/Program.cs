using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShareRepositoryMover
{
    class Program
    {
        static void Main(string[] args)
        {
            Services.TransferData transferData = new Services.TransferData();
            Console.WriteLine("MIGRATION COMPLETE");
            Console.ReadLine();
        }
    }
}
