using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 权限显示对象
    /// </summary>
    public class RightDisplayModel : ModelBase
    {

        /// <summary>
        /// 权限编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否停用
        /// </summary>
        public bool Stopped { get; set; }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {

            return "";
        }

    }//end RightDisplayModel

}//end namespace SysManager