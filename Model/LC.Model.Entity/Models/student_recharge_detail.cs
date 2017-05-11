using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class student_recharge_detail
    {
        public int recharge_id { get; set; }
        public int student_id { get; set; }
        public int amount { get; set; }
        public int inout_type { get; set; }
        public DateTime incur_time { get; set; }
        public string recharge_user { get; set; }
        public Nullable<int> class_record_detail_id { get; set; }
        public virtual student student { get; set; }
        public virtual SysUser SysUser { get; set; }
        public virtual class_record_detail class_record_detail { get; set; }
    }
}
