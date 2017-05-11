using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 用户信息显示对象
    /// </summary>
    public class UserDisplayModel : ModelBase
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 联系电话
        /// 默认使用移动电话，如果没有则使用办公室电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 是否部门领导
        /// true：是
        /// false：不是
        /// </summary>
        public bool IsLeader { get; set; }
        /// <summary>
        /// 是否停用
        /// true：停用
        /// false：未停用
        /// </summary>
        public bool Stopped { get; set; }
        /// <summary>
        /// 是否在线
        /// true：在线
        /// false：不在线
        /// </summary>
        public bool IsOnline { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

    }//end UserDisplayModel

}//end namespace SysManager