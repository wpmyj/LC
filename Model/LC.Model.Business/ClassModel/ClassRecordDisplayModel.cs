using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.ClassModel
{
    public class ClassRecordDisplayModel
    {
        public string ClassName { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int StudentNum { get; set; }
        public int StudentLimit { get; set; }
        public decimal Money { get; set; }
        public bool IsChecked { get; set; }
    }
}
