using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 操作方法显示类型
    /// </summary>
    public class FunctionDisplayModel : ModelBase
    {

        /// <summary>
        /// 方法编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 方法类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {

            return "";
        }

    }//end FunctionDisplayModel

}//end namespace SysManager
