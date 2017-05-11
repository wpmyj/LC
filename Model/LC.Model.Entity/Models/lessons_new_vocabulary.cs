using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class lessons_new_vocabulary
    {
        public int id { get; set; }
        public int lesson_schemas_id { get; set; }
        public string topic_name { get; set; }
        public string vocab_type { get; set; }
        public string vocab_text { get; set; }
        public virtual lesson_schemas lesson_schemas { get; set; }
    }
}
