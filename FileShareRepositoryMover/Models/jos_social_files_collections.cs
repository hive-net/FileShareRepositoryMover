using System;
using System.Collections.Generic;

namespace FileShareRepositoryMover.Models
{
    public class jos_social_files_collections
    {
        public int id { get; set; }
        public int owner_id { get; set; }
        public string owner_type { get; set; }
        public int? user_id { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public DateTime? created { get; set; }
    }
}
