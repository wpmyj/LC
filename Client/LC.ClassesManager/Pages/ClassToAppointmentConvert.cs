using LC.Model.Business.ClassModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.ScheduleView;
using Telerik.Windows.DragDrop.Behaviors;

namespace LC.ClassesManager.Pages
{
    public class ClassToAppointmentConvert : DataConverter
    {
        public override string[] GetConvertToFormats()
        {
            return new string[] { typeof(ScheduleViewDragDropPayload).FullName, typeof(ClassDisplayModel).FullName };
        }

        public override object ConvertTo(object data, string format)
        {

            var payload = DataObjectHelper.GetData(data, typeof(ScheduleViewDragDropPayload), false) as ScheduleViewDragDropPayload;
            if (payload != null)
            {
                var cdm = payload.DraggedAppointments;
                return cdm.OfType<ScheduleAppointment>().Select(a => new ClassDisplayModel { Name = a.Subject,Id=a.ClassId });
            }
            return null;
        }
    }
}
