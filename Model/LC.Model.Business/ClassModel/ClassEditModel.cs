using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.ClassModel
{
    public class ClassEditModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public int LastCount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public String TypeName { get; set; }

        public bool IsActive { get; set; }

        public void InitEditModel(classes classesEntity)
        {
            this.Id = classesEntity.class_id;
            this.EndDate = classesEntity.end_date.Value;
            this.LastCount = classesEntity.last_count;
            this.Name = classesEntity.class_name;
            this.StartDate = classesEntity.start_date.Value;
            this.TypeId = classesEntity.class_type;
            this.TypeName = classesEntity.parentClassTypes.name;
            this.IsActive = classesEntity.is_active;
        }
    }
}
