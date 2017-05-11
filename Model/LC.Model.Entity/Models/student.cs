using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class student
    {
        public student()
        {
            this.classess = new List<classes>();
            this.student_recharge_detail = new List<student_recharge_detail>();
            this.consultants = new List<consultant>();
        }

        public int student_id { get; set; }
        public Nullable<int> center_id { get; set; }
        public string moms_name { get; set; }
        public string dads_name { get; set; }
        public System.DateTime students_birthdate { get; set; }
        public string students_name { get; set; }
        public string students_nickname { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string relationship { get; set; }
        public string extra_info { get; set; }
        public string original_class { get; set; }
        public string grade { get; set; }
        public string moms_phone { get; set; }
        public string dads_phone { get; set; }
        public string school { get; set; }
        public Nullable<int> remaining_balance { get; set; }
        public string google_contacts_id { get; set; }
        public string rfid_tag { get; set; }
        public int status { get; set; }
        public Nullable<decimal> consultant_check_rate { get; set; }
        public virtual ICollection<classes> classess { get; set; }
        public virtual ICollection<student_recharge_detail> student_recharge_detail { get; set; }
        public virtual ICollection<consultant> consultants { get; set; }
        public virtual status status1 { get; set; }
    }
}
