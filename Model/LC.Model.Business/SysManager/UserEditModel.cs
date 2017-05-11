
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 用户编辑对象
    /// </summary>
    public class UserEditModel : ModelBase
    {

        /// <summary>
        /// 用户实体对象
        /// </summary>
        private SysUser User;

        public string UserCode { get; set; }
        public string Name { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public bool NeedChangePassword { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public UserSex Sex { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string OfficialPhone { get; set; }
        public string Position { get; set; }
        public bool Stopped { get; set; }
        public bool IsLeader { get; set; }
        public bool IsOnline { get; set; }
        public string Remark { get; set; }
        public byte[] Picture { get; set; }
        public string DepartmentCode { get; set; }

        public void InitEditModel(SysUser sysUser)
        {
            this.User = sysUser;

            this.Birthday = sysUser.Birthday;
            this.Email = sysUser.Email;
            this.IsLeader = sysUser.IsLeader;
            this.IsOnline = sysUser.IsOnline;
            this.LoginName = sysUser.LoginName;
            this.Mobile = sysUser.Mobile;
            this.Name = sysUser.Name;
            this.NeedChangePassword = sysUser.NeedChangePassword;
            this.OfficialPhone = sysUser.OfficialPhone;
            this.Password = sysUser.Password;
            this.Position = sysUser.Position;
            this.Remark = sysUser.Remark;
            this.Sex = sysUser.Sex;
            this.Stopped = sysUser.Stopped;
            this.UserCode = sysUser.UserCode;
        }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

    }//end UserEditModel

}//end namespace SysManager