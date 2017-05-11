using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.ClassModel
{
    public class ClassDisplayModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LastCount { get; set; }

        public String StartDate { get; set; }

        public String EndDate { get; set; }

        public int TypeId { get; set; }

        public string TypeName { get; set; }
        public bool IsActive { get; set; }

        public ClassDisplayModel Copy()
        {
            return new ClassDisplayModel
            {
                Name = this.Name,
                Id = this.Id
            };
        }
    }
}
