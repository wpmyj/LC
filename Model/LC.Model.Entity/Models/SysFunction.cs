using System;
using System.Collections.Generic;

namespace LC.Model.Entity.Models
{
    public partial class SysFunction
    {
        public SysFunction()
        {
            this.SysMenus = new List<SysMenu>();
        }

        public string FunctionCode { get; set; }
        public string Name { get; set; }
        public FunctionType Type { get; set; }
        public string Assembly { get; set; }
        public string ClassName { get; set; }
        public string OperationCode { get; set; }
        public string OperationName { get; set; }
        public string Params { get; set; }
        public string Remark { get; set; }
        public string ModuleCode { get; set; }
        public virtual SysModule SysModule { get; set; }
        public virtual ICollection<SysMenu> SysMenus { get; set; }
    }
}
