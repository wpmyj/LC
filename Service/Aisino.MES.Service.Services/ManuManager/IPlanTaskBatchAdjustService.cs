using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
   public interface IPlanTaskBatchAdjustService
    {
       PlanTaskBatchAdjust AddPlantaskAdjustWithWarehouseInfor(PlanTaskBatchAdjust newplanAdjust, string OrgDepCode);

       /// <summary>
       /// 根据作业计划批次新增冲补记录
       /// </summary>
       /// <param name="newPlanAdjust">需要新增的冲补记录</param>
       /// <param name="plantaskBatch">相关作业计划批次</param>
       /// <param name="orgDeptCode">组织机构编号</param>
       /// <returns>冲补记录</returns>
       PlanTaskBatchAdjust AddPlantaskAdjustWithPlantaskBatchInfo(PlanTaskBatchAdjust newPlanAdjust, PlanTaskBatch plantaskBatch, string orgDeptCode);
    }
}
