using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.BaseModel
{
    public class ConsultantModel
    {
        /// <summary>
        /// 主键自增
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 会籍顾问姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 会籍顾问简称
        /// </summary>
        public string abbreviation { get; set; }
        /// <summary>
        /// 会籍顾问提成比例，暂时以班级类型中的提成比例为准
        /// </summary>
        public decimal CommissionRate { get; set; }
    }
}
