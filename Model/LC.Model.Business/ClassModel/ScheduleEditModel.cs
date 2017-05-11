using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.ClassModel
{
    public class ScheduleEditModel
    {
        public int ScheduleId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public DateTime RealDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int AssistantId { get; set; }
        public string AssistantName { get; set; }
        public int ClassroomId { get; set; }
        public string ClassroomName { get; set; }
        public int SchemasId { get; set; }
        public string SchemasText { get; set; }
        public int Status { get; set; }
        public string StatusDes { get; set; }
        public List<int> AttendedStudentIds { get; set; }
        public string LessonName { get; set; }
        public string Note { get; set; }


        public void InitEditModel(class_schedule schedule,int attended)
        {
            this.ScheduleId = schedule.schedule_id;
            this.ClassId = schedule.class_id;
            this.ClassName = schedule.wclass.class_name;
            this.RealDate = schedule.real_date;
            this.StartTime = schedule.start_time;
            this.EndTime = schedule.end_date;
            this.TeacherId = schedule.teacher_id;
            this.TeacherName = schedule.teacher.name;
            this.LessonName = schedule.lesson_schemas_text;
            if(schedule.assistant_id.HasValue)
            {
                this.AssistantId = schedule.assistant_id.Value;
                this.AssistantName = schedule.assistant.name;
            }
            else
            {
                this.AssistantId = 0;
                this.AssistantName = "";
            }
            this.ClassroomId = schedule.classroom_id;
            this.ClassroomName = schedule.classroom.classroom_name;
            this.Status = schedule.status;
            this.StatusDes = schedule.schedulestatus.description;
            this.Note = schedule.note;

            this.AttendedStudentIds = new List<int>();
            if(schedule.classrecords != null && schedule.classrecords.Count > 0)
            {
                foreach(class_record_detail crd in schedule.classrecords.First().class_record_detail)
                {
                    if (crd.attendance_status == attended)
                    {
                        this.AttendedStudentIds.Add(crd.student_id);
                    }
                }
            }
        }
    }
}
