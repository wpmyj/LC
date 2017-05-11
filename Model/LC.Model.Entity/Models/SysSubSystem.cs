using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class SysSubSystem
    {
        public SysSubSystem()
        {
            this.SysMenus = new List<SysMenu>();
        }

        public string SubSystemCode { get; set; }
        public string SubSystemName { get; set; }
        public string Remark { get; set; }
        public MetroTypes MetroType { get; set; }
        public string MetroBackColor { get; set; }
        public string MetroForeColor { get; set; }
        public string IconString { get; set; }
        public virtual ICollection<SysMenu> SysMenus { get; set; }
    }
}
