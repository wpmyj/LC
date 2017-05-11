using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
   public interface IPlanTaskStepStatuService
    {
       /// <summary>
       /// 更新计划操作状态
       /// </summary>
       /// <param name="newPlanTaskStepStatu"></param>
       /// <returns></returns>
       PlanTaskStepStatu UpPlanTaskStepStatu(PlanTaskStepStatu newPlanTaskStepStatu);
    }
}
