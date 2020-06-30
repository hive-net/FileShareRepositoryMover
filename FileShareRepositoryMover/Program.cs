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
            Services.FileTransfer folder = new Services.FileTransfer();
            Console.WriteLine("MIGRATION COMPLETE");
            Console.ReadLine();
        }
    }
}
