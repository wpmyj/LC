using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.TeacherModel
{
    public class TeacherClassRecordDetailModel
    {
        public string typename { get; set; }
        public string classname { get; set; }
        public DateTime starttime { get; set; }
        public int classrecordid { get; set; }
        public string studentname { get; set; }
        public string nickname { get; set; }
        public string teachername { get; set; }
        public DateTime realdate { get; set; }
        public int unitprice { get; set; }
        public Decimal rate { get; set; }
        public int studentlimit { get; set; }
        public int studentnumber { get; set; }
        public int realnum { get; set; }
        public int receivalbemoney{get;set;}
        public int actualmoney { get; set; }
    }
}
