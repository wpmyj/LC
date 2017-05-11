using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class class_schedule
    {
        public class_schedule()
        {
            this.classrecords = new List<class_record>();
        }

        public int schedule_id { get; set; }
        public int class_id { get; set; }
        public System.DateTime real_date { get; set; }
        public System.DateTime start_time { get; set; }
        public System.DateTime end_date { get; set; }
        public int teacher_id { get; set; }
        public Nullable<int> assistant_id { get; set; }
        public int classroom_id { get; set; }
        public Nullable<int> lesson_schemas_id { get; set; }
        public string lesson_schemas_text { get; set; }
        public int status { get; set; }

        public string note { get; set; }
        public virtual ICollection<class_record> classrecords { get; set; }
        public virtual classes wclass { get; set; }
        public virtual lesson_schemas schemas { get; set; }
        public virtual status schedulestatus { get; set; }
        public virtual teacher teacher { get; set; }
        public virtual teacher assistant { get; set; }
        public virtual center_classrooms classroom { get; set; }
    }
}
