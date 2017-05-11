using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
   public interface IPlanTaskBatchSiteScaleService
    {
       void AddPlanTaskBatchSiteScale(PlanTaskBatch plantaskbatch, List<PlanTaskBatchSiteScale> planSiteList);

       void DelPlanTaskBatchSiteScale(List<PlanTaskBatchSiteScale> oldplanSiteList,List<PlanTaskBatchSiteScale> newplanscale);
       void UpPlanTaskBatchSiteScale(PlanTaskBatch plantaskbatch, List<PlanTaskBatchSiteScale> planSiteList);
       void RefreshData();
    }
}
