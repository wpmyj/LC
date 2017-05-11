using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.Service.StoreProcedures;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class PlanTaskStepStatuService : IPlanTaskStepStatuService
    {
        private Repository<PlanTaskStepStatu> _planTaskStepStatuDal;
        SPGetSysDateTimeService _sPGetSysDateTimeService;

        public PlanTaskStepStatuService(Repository<PlanTaskStepStatu> planTaskStepStatuDal,
                                        SPGetSysDateTimeService sPGetSysDateTimeService)
        {
            _planTaskStepStatuDal = planTaskStepStatuDal;
            _sPGetSysDateTimeService = sPGetSysDateTimeService;
        }


        /// <summary>
        /// 更新计划操作状态
        /// </summary>
        /// <param name="newPlanTaskStepStatu"></param>
        /// <returns></returns>
        public PlanTaskStepStatu UpPlanTaskStepStatu(PlanTaskStepStatu newPlanTaskStepStatu)
        {
            newPlanTaskStepStatu.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
            newPlanTaskStepStatu.status = true;
            _planTaskStepStatuDal.Update(newPlanTaskStepStatu);
            return newPlanTaskStepStatu;
        }
    }
}
