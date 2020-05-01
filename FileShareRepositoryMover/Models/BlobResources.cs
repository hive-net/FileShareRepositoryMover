using System;
using System.Collections.Generic;

namespace FileShareRepositoryMover.Models
{
    public class BlobResources
    {
        public Guid Resourceid { get; set; }
        public Guid CommunityId { get; set; }
        public string BlobFileName { get; set; }
        public string BlobFileType { get; set; }
    }
}
