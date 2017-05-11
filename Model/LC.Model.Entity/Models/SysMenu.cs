using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class SysMenu
    {
        public SysMenu()
        {
            this.SubMenu = new List<SysMenu>();
            this.SysRights = new List<SysRight>();
            this.SysSubSystems = new List<SysSubSystem>();
            this.SysUsers = new List<SysUser>();
        }

        public string MenuCode { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Nullable<int> ShowIndex { get; set; }
        public string Remark { get; set; }
        public MenuType Type { get; set; }
        public Nullable<int> Layer { get; set; }
        public string ModuleCode { get; set; }
        public string FunctionCode { get; set; }
        public byte[] BigImage { get; set; }
        public byte[] SmallImage { get; set; }
        public Nullable<bool> ShowImage { get; set; }
        public Nullable<bool> ShowText { get; set; }
        public string ControlType { get; set; }
        public string ImagePosition { get; set; }
        public string ParentCode { get; set; }
        public virtual SysFunction ParentFunction { get; set; }
        public virtual SysModule ParentModule { get; set; }
        public virtual SysMenu ParentMenu { get; set; }
        public virtual ICollection<SysMenu> SubMenu { get; set; }
        public virtual ICollection<SysRight> SysRights { get; set; }
        public virtual ICollection<SysSubSystem> SysSubSystems { get; set; }
        public virtual ICollection<SysUser> SysUsers { get; set; }
    }
}
