using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    public class SubSystemDisplayModel : ModelBase
    {
        public string SubSystemCode { get; set; }

        public string SubSystemName { get; set; }

        public string MetroType { get; set; }

        public string Remark { get; set; }

        public string BackColor { get; set; }

        public string ForeColor { get; set; }
        public string IconString { get; set; }
        public override string ToString()
        {
            return this.SubSystemName;
        }
    }
}
