using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.ScheduleView;

namespace LC.ClassesManager.Pages
{
    public class ScheduleAppointment : Appointment
    {
        private int scheduleId;

        public int ScheduleId
        {
            get
            {
                return this.Storage<ScheduleAppointment>().scheduleId;
            }
            set
            {
                var storage = this.Storage<ScheduleAppointment>();
                if (storage.scheduleId != value)
                {
                    storage.scheduleId = value;
                    this.OnPropertyChanged(() => this.scheduleId);
                }
            }
        }

        private int classId;

        public int ClassId
        {
            get
            {
                return this.Storage<ScheduleAppointment>().classId;
            }
            set
            {
                var storage = this.Storage<ScheduleAppointment>();
                if (storage.classId != value)
                {
                    storage.classId = value;
                    this.OnPropertyChanged(() => this.classId);
                }
            }
        }

        private string teacherName;
        public string TeacherName
        {
            get
            {
                return this.Storage<ScheduleAppointment>().teacherName;
            }
            set
            {
                var storage = this.Storage<ScheduleAppointment>();
                if (storage.teacherName != value)
                {
                    storage.teacherName = value;
                    this.OnPropertyChanged(() => this.teacherName);
                }
            }
        }

        private string assistantName;
        public string AssistantName
        {
            get
            {
                return this.Storage<ScheduleAppointment>().assistantName;
            }
            set
            {
                var storage = this.Storage<ScheduleAppointment>();
                if (storage.assistantName != value)
                {
                    storage.assistantName = value;
                    this.OnPropertyChanged(() => this.assistantName);
                }
            }
        }

        private string classroomName;
        public string ClassroomName
        {
            get
            {
                return this.Storage<ScheduleAppointment>().classroomName;
            }
            set
            {
                var storage = this.Storage<ScheduleAppointment>();
                if (storage.classroomName != value)
                {
                    storage.classroomName = value;
                    this.OnPropertyChanged(() => this.classroomName);
                }
            }
        }

        private string className;
        public string ClassName
        {
            get
            {
                return this.Storage<ScheduleAppointment>().className;
            }
            set
            {
                var storage = this.Storage<ScheduleAppointment>();
                if (storage.className != value)
                {
                    storage.className = value;
                    this.OnPropertyChanged(() => this.className);
                }
            }
        }

        public override IAppointment Copy()
        {
            var newAppointment = new ScheduleAppointment();
            newAppointment.CopyFrom(this);
            return newAppointment;
        }
        public override void CopyFrom(IAppointment other)
        {
            var task = other as ScheduleAppointment;
            if (task != null)
            {
                this.TeacherName = task.TeacherName;
                this.ClassId = task.ClassId;
                this.ScheduleId = task.ScheduleId;
                this.ClassName = task.ClassName;
                this.ClassroomName = task.ClassroomName;
                this.AssistantName = task.AssistantName;
                this.Start = task.Start;
                this.End = task.End;
                this.Category = task.Category;
            }
            base.CopyFrom(other);
        }
    }
}
