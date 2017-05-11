using LC.Model.Business.ClassModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls.ScheduleView;
using Telerik.Windows.Controls.TreeView;
using Telerik.Windows.DragDrop.Behaviors;

namespace LC.ClassesManager.Pages
{
    public class ScheduleViewDragDropBehavior:Telerik.Windows.Controls.ScheduleViewDragDropBehavior
    {
        public object cdms { get; set; }

        public override IEnumerable<IOccurrence> ConvertDraggedData(object data)
        {
            
            if (DataObjectHelper.GetDataPresent(data, typeof(ClassDisplayModel), false))
            {
                var cdms = DataObjectHelper.GetData(data, typeof(ClassDisplayModel), false) as IEnumerable;
                if (cdms != null)
                {
                    return cdms.OfType<ClassDisplayModel>().Select(c => new ScheduleAppointment { Subject = c.Name,ClassId=c.Id}); ;
                }
            }

            return base.ConvertDraggedData(data);
        }

    }
}
