using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class center_classrooms
    {
        public center_classrooms()
        {
            this.schedule = new List<class_schedule>();
        }
        public int classroom_id { get; set; }
        public int center_id { get; set; }
        public string classroom_name { get; set; }
        public Nullable<int> upper_limit { get; set; }

        public virtual ICollection<class_schedule> schedule { get; set; }

        public virtual center parentCenter { get; set; }
    }
}
