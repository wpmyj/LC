using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class class_types
    {
        public class_types()
        {
            this.subclasses = new List<classes>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public int total_lessons { get; set; }
        public int unit_price { get; set; }
        public decimal commission_rate_teacher { get; set; }
        public decimal commission_rate_assistant { get; set; }
        public decimal commission_rate_consultant { get; set; }
        public int student_limit { get; set; }
        public bool is_active { get; set; }
        public string description { get; set; }

        public virtual ICollection<classes> subclasses { get; set; }
    }
}
