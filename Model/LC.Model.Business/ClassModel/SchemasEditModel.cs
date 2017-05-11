using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.ClassModel
{
    public class SchemasEditModel
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string LevelName { get; set; }
        public string LessonName { get; set; }
        public int Seq { get; set; }
    }
}
