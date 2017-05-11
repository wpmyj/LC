using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class SysRole
    {
        public SysRole()
        {
            this.SysRights = new List<SysRight>();
            this.SysUsers = new List<SysUser>();
        }

        public string RoleCode { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> Stopped { get; set; }
        public virtual ICollection<SysRight> SysRights { get; set; }
        public virtual ICollection<SysUser> SysUsers { get; set; }
    }
}
