using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 模块显示模型
    /// </summary>
    public class ModuleDisplayModel : ModelBase
    {

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否已经停用
        /// </summary>
        public bool Stopped { get; set; }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {

            return "";
        }

    }//end ModuleDisplayModel

}//end namespace SysManager