using System;
using System.Collections.Generic;

namespace FileShareRepositoryMover.Models
{
    public class Folders
    {
        public Guid FolderId { get; set; }
        public Guid CommunityId { get; set; }
        public Guid? ParentFolderId { get; set; }
        public int FolderLevel { get; set; }
        public string FolderName { get; set; }
        public int? ClusterType { get; set; }
        public string ClusterId { get; set; }
    }
}
