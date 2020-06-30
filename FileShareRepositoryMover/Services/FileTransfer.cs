using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileShareRepositoryMover.Services
{
    class FileTransfer
    {
        public FileTransfer()
        {
            folders = new FolderCreation();

            foreach(Dictionary<string, dynamic> sec in folders.SecondaryNodes.Values)
            {
                Console.WriteLine(sec["nid"] + " - " + sec["folderId"]);
            }
        }

        Services.FolderCreation folders;
    }
}
