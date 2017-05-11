using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.ClassModel
{
    public class ScheduleDisplayModel
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int ClassId { get; set; }
        public DateTime RealDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TeacherName { get; set; }
        public string AssistantName { get; set; }
        public string ClassroomName { get; set; }
        public string SchemasText { get; set; }
        public string Status { get; set; }

        public string Note { get; set; }
    }
}
