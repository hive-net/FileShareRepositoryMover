using System;
using System.Collections.Generic;

namespace FileShareRepositoryMover.Models
{
    public class LinkResources
    {
        public Guid Resourceid { get; set; }
        public Guid CommunityId { get; set; }
        public string LinkUrl { get; set; }
    }
}
