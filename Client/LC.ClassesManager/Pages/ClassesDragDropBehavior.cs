using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.DragDrop.Behaviors;

namespace LC.ClassesManager.Pages
{
    public class ClassesDragDropBehavior : ListBoxDragDropBehavior
    {
        public ClassesDragDropBehavior()
        {

        }

        // this override enables only copies of objects, not their movement
        //protected override bool IsMovingItems(DragDropState state)
        //{
        //    return false;
        //}

        //protected override IEnumerable<object> CopyDraggedItems(DragDropState state)
        //{
        //    return base.CopyDraggedItems(state);
        //}
    }
}
