using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class SysUser
    {
        public SysUser()
        {
            this.student_recharge_detail = new List<student_recharge_detail>();
            this.teachers_check_record = new List<teachers_check_record>();
            this.SysRoles = new List<SysRole>();
            this.SysMenus = new List<SysMenu>();
        }

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
        public string Remark { get; set; }
        public bool IsOnline { get; set; }
        public virtual ICollection<student_recharge_detail> student_recharge_detail { get; set; }
        public virtual ICollection<teachers_check_record> teachers_check_record { get; set; }
        public virtual ICollection<SysRole> SysRoles { get; set; }
        public virtual ICollection<SysMenu> SysMenus { get; set; }
    }
}
