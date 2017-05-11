using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class teacher
    {
        public teacher()
        {
            this.class_schedule = new List<class_schedule>();
            this.class_schedule1 = new List<class_schedule>();
            this.teachers_check_record = new List<teachers_check_record>();
        }

        public int teacher_id { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public int status { get; set; }
        public string UserCode { get; set; }
        public virtual ICollection<class_schedule> class_schedule { get; set; }
        public virtual ICollection<class_schedule> class_schedule1 { get; set; }
        public virtual status status1 { get; set; }
        public virtual ICollection<teachers_check_record> teachers_check_record { get; set; }
    }
}
