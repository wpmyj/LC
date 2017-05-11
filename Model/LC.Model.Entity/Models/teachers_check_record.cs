using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class teachers_check_record
    {
        public teachers_check_record()
        {
            this.classRecords = new List<class_record>();
            this.assistantRecords = new List<class_record>();
        }
        public int check_record_id { get; set; }
        public int teacher_id { get; set; }
        public DateTime check_time { get; set; }
        public decimal total_money { get; set; }
        public Nullable<int> check_rate { get; set; }
        public string check_month { get; set; }
        public string check_user { get; set; }
        public virtual SysUser SysUser { get; set; }
        public virtual teacher teacher { get; set; }

        public virtual ICollection<class_record> classRecords { get; set; }

        public virtual ICollection<class_record> assistantRecords { get; set; }
    }
}
