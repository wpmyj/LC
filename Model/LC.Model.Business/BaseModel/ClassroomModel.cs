using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.BaseModel
{
    public class ClassroomModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 教室名称
        /// </summary>
        public string Name{get;set;}
        /// <summary>
        /// 所属中心编号
        /// </summary>
        public int CenterId { get; set; }
        /// <summary>
        /// 所属中心名称
        /// </summary>
        public string CenterName { get; set; }
        /// <summary>
        /// 该教室可容纳学员人数上限
        /// </summary>
        public int UpperLimit { get; set; }
    }
}
