using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class class_record
    {
        public class_record()
        {
            this.class_record_detail = new List<class_record_detail>();
        }

        public int class_record_id { get; set; }
        public int schedule_id { get; set; }
        public int teacher_id { get; set; }
        public decimal teacher_check_rate { get; set; }
        public Nullable<int> assistant_id { get; set; }
        public Nullable<decimal> assistant_check_rate { get; set; }
        public int student_number { get; set; }
        public Nullable<int> student_limit { get; set; }
        public Nullable<int> amount_receivable { get; set; }
        public Nullable<int> actual_amount { get; set; }
        public Nullable<int> is_checked { get; set; }
        public Nullable<int> check_record_id { get; set; }
        public Nullable<int> assistant_is_checked { get; set; }
        public Nullable<int> assistant_check_record_id { get; set; }
        public string note { get; set; }
        public virtual class_schedule class_schedule { get; set; }
        public virtual ICollection<class_record_detail> class_record_detail { get; set; }

        public virtual teachers_check_record teacher_check_record { get; set; }
        public virtual teachers_check_record assistant_check_record { get; set; }
    }
}
