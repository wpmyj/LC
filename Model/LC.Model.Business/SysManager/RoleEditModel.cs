
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 角色编辑对象
    /// </summary>
    public class RoleEditModel : ModelBase
    {

        /// <summary>
        /// 角色实体
        /// </summary>
        private SysRole Role;

        public string RoleCode { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> Stopped { get; set; }

        public List<UserEditModel> SysUsers { get; set; }
        public List<RightEditModel> SysRights { get; set; }

        public void InitEditModel(SysRole sysRole)
        {
            this.Role = sysRole;

            this.RoleCode = sysRole.RoleCode;
            this.Name = sysRole.Name;
            this.Remark = sysRole.Remark;
            this.Stopped = sysRole.Stopped;

            if (sysRole.SysRights != null)
            {
                ICollection<SysRight> sysRights = sysRole.SysRights;
                this.SysRights = new List<RightEditModel>();
                foreach (SysRight sysRight in sysRights)
                {
                    RightEditModel right = new RightEditModel();
                    right.InitEditModel(sysRight);
                    this.SysRights.Add(right);
                }
            }
            else
            {
                this.SysRights = null;
            }

            if (sysRole.SysUsers != null)
            {
                ICollection<SysUser> sysUsers = sysRole.SysUsers;
                this.SysUsers = new List<UserEditModel>();
                foreach (SysUser sysUser in sysUsers)
                {
                    UserEditModel user = new UserEditModel();
                    user.InitEditModel(sysUser);
                    this.SysUsers.Add(user);
                }
            }
            else
            {
                this.SysUsers = null;
            }
        }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {
            return "";
        }

    }//end RoleEditModel

}//end namespace SysManager