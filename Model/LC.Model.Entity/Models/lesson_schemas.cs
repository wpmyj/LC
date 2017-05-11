using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class lesson_schemas
    {
        public lesson_schemas()
        {
            this.class_schedule = new List<class_schedule>();
            this.lessons_new_vocabulary = new List<lessons_new_vocabulary>();
        }

        public int lesson_schemas_id { get; set; }
        public int class_type_id { get; set; }
        public string level_name { get; set; }
        public string lesson_name { get; set; }
        public Nullable<int> sequence_num { get; set; }
        
        public virtual ICollection<class_schedule> class_schedule { get; set; }
        public virtual ICollection<lessons_new_vocabulary> lessons_new_vocabulary { get; set; }
    }
}
