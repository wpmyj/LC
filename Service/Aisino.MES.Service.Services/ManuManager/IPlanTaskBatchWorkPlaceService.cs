using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
   public interface IPlanTaskBatchWorkPlaceService
    {
       void RefreshData();

       void AddPlanTaskBatchWorkPlace(PlanTaskBatch plantaskbatch, List<PlanTaskBatchWorkPlace> workplaceList);

       void UpdatePlanTaskBatchWorkPlace(PlanTaskBatch plantaskbatch, List<PlanTaskBatchWorkPlace> workplaceList);

       void DelPlanTaskBatchWorkPlace(List<PlanTaskBatchWorkPlace> planWorkPlaceList, List<PlanTaskBatchWorkPlace> newplanWorkPlace);
    }
}
