using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.ClassModel
{
    public class ClassTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalLessons { get; set; }
        public int UnitPrice { get; set; }
        public double TeacherRate { get; set; }
        public double AssistantRate { get; set; }
        public double ConsultantRate { get; set; }
        public int StudentLimit { get; set; }
        public bool IsActive { get; set; }
        public string Des { get; set; }
        /// <summary>
        /// 主要用于做班级选择树
        /// </summary>
        public IList<ClassDisplayModel> classDisplayModels { get; set; }
    }
}
