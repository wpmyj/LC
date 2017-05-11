using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class class_record_detail
    {
        public class_record_detail()
        {
            this.student_recharge_detail = new List<student_recharge_detail>();
        }
        public int class_record_detail_id { get; set; }
        public Nullable<int> class_record_id { get; set; }
        public int student_id { get; set; }
        public Nullable<int> register_time { get; set; }
        public int attendance_status { get; set; }
        public Nullable<int> consultants_id { get; set; }
        public Nullable<int> consultant_check_record_id { get; set; }
        public Nullable<decimal> consultant_check_rate { get; set; }
        public bool is_checked { get; set; }
        public virtual class_record class_record { get; set; }
        public virtual consultant_check_record consultant_check_record { get; set; }
        public virtual consultant consultant { get; set; }
        public virtual ICollection<student_recharge_detail> student_recharge_detail { get; set; }
        public virtual status detailStatus { get; set; }
    }
}
