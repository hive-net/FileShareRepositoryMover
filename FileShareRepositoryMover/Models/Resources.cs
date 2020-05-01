using System;
using System.Collections.Generic;

namespace FileShareRepositoryMover.Models
{
    public class Resources
    {
        public Guid ResourceId { get; set; }
        public Guid CommunityId { get; set; }
        public Guid FolderId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceDescription { get; set; }
        public string Metadata { get; set; }
        public int? ClusterType { get; set; }
        public string ClusterId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
