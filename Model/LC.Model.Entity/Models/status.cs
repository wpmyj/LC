using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class status
    {
        public status()
        {
            this.class_schedule = new List<class_schedule>();
            this.students = new List<student>();
            this.teachers = new List<teacher>();
        }

        public int id { get; set; }
        public string cat { get; set; }
        public string description { get; set; }
        public virtual ICollection<class_schedule> class_schedule { get; set; }
        public virtual ICollection<student> students { get; set; }
        public virtual ICollection<teacher> teachers { get; set; }

        public virtual ICollection<class_record_detail> classRecordDetails { get; set; }


    }
}
