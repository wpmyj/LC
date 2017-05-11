using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IPlanTaskDetailService
    {
        PlanTaskDetail AddPlanTaskDetail(PlanTaskDetail newPlantaskDetail);
        PlanTaskDetail UpdPlanTaskDetail(PlanTaskDetail updPlantaskDetail);
        void DelPlantaskDetail(PlanTaskDetail delPlantaskDetail);

        IEnumerable<PlanTaskDetail> SelectAllPlanDetails();

        //PlanTaskDetail AddPlanDetailWithPlantask(PlanTask newPlantask,PlanTaskDetail newPlantaskDetail);
        
    }
}
