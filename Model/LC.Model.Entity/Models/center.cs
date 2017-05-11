using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class center
    {
        public center()
        {
            this.subclassrooms = new List<center_classrooms>();
        }

        public int center_id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public Nullable<int> consultant_id { get; set; }
        public virtual consultant consultants { get; set; }

        public virtual ICollection<center_classrooms> subclassrooms { get; set; }
    }
}
