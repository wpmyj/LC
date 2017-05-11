using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class consultant_check_record
    {
        public consultant_check_record()
        {
            this.class_record_detail = new List<class_record_detail>();
        }

        public int consultant_check_record_id { get; set; }
        public int consultant_id { get; set; }
        public System.DateTime check_time { get; set; }
        public decimal total_money { get; set; }
        public string check_user { get; set; }
        public string check_month { get; set; }
        public virtual ICollection<class_record_detail> class_record_detail { get; set; }
        public virtual consultant consultant { get; set; }
    }
}
