using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class students_log
    {
        public int log_id { get; set; }
        public Nullable<int> student_id { get; set; }
        public string who { get; set; }
        public byte[] time { get; set; }
        public string log { get; set; }
    }
}
