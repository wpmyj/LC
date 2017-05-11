using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class consultant
    {
        public consultant()
        {
            this.centers = new List<center>();
            this.class_record_detail = new List<class_record_detail>();
            this.consultant_check_record = new List<consultant_check_record>();
            this.students = new List<student>();
        }

        public int consultant_id { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public decimal commission_rate { get; set; }
        public virtual ICollection<center> centers { get; set; }
        public virtual ICollection<class_record_detail> class_record_detail { get; set; }
        public virtual ICollection<consultant_check_record> consultant_check_record { get; set; }
        public virtual ICollection<student> students { get; set; }
    }
}
