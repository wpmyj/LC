using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.PageModel
{
    public class AisinoPageIList<T>
    {
        public int totalCount;
        public IList<T> Entities { get; set; }
        public bool NoErrors { get; set; }
        public string Message { get; set; }
    }
}
