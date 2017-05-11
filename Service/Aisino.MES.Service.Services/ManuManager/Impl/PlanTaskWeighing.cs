using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;

namespace Aisino.MES.Service.ManuManager.Impl
{
    //public class PlanTaskWeighing : IPlanTaskService
    //{
    //    private Repository<PlanTask> _planTaskDal;
    //    private Repository<PlanTaskBatch> _planTaskBatchDal;
    //    private Repository<PlanTaskBatchDetail> _planTaskBatchDetail;
    //    private UnitOfWork _unitOfWork;

    //    public PlanTaskWeighing(Repository<PlanTask> planTaskDal,
    //                            Repository<PlanTaskBatch> planTaskBatchDal,
    //                            Repository<PlanTaskBatchDetail> planTaskBatchDetail, 
    //                            UnitOfWork unitOfWork)   
    //    {
    //        _planTaskDal = planTaskDal;
    //        _planTaskBatchDal = planTaskBatchDal;
    //        _planTaskBatchDetail = planTaskBatchDetail;
    //        _unitOfWork = unitOfWork;
    //    }

    //    /// <summary>
    //    /// 计划执行时，称重数量更新
    //    /// </summary>
    //    /// <param name="dWeight"></param>
    //    /// <param name="strRfidCardNum"></param>
    //    /// <returns></returns>
    //    public bool UpdatePlanTaskBatchDetail(double dWeight, string strRfidCardNum)
    //    {
    //        return false;
 
    //    }


    //}
}
