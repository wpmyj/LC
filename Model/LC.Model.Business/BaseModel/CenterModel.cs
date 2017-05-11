using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.BaseModel
{
    public class CenterModel
    {
        /// <summary>
        /// 中心id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 中心名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 中心地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 中心联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 中心主要负责会籍顾问ID
        /// </summary>
        public int ConsultantId { get; set; }
        /// <summary>
        /// 中心主要负责会籍顾问名称
        /// </summary>
        public string ConsultantName { get; set; }
    }
}
