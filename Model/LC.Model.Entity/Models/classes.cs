using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class classes
    {
        public classes()
        {
            this.class_schedule = new List<class_schedule>();
            this.students = new List<student>();
        }

        public int class_id { get; set; }
        public string class_name { get; set; }
        public int class_type { get; set; }
        public int last_count { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public bool is_active { get; set; }
        public virtual ICollection<class_schedule> class_schedule { get; set; }
        public virtual ICollection<student> students { get; set; }

        public virtual class_types parentClassTypes { get; set; }
    }
}
