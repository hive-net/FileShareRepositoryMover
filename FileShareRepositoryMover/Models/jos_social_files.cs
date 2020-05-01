using System;
using System.Collections.Generic;

namespace FileShareRepositoryMover.Models
{
    public class jos_social_files
    {
        public int? id { get; set; }
        public int collection_id { get; set; }
        public string name { get; set; }
        public int? hits { get; set; }
        public string hash { get; set; }
        public string sub { get; set; }
        public int? uid { get; set; }
        public string type { get; set; }
        public DateTime? created { get; set; }
        public int? user_id { get; set; }
        public int? size { get; set; }
        public string mime { get; set; }
        public int? state { get; set; }
        public string storage { get; set; }
    }
}
