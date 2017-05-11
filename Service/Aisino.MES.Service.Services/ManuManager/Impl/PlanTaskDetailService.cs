using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Repository.Repositories;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class PlanTaskDetailService : IPlanTaskDetailService
    {
        private Repository<PlanTaskDetail> _planTaskDetailDal;

        public PlanTaskDetailService(Repository<PlanTaskDetail> planTaskDetailDal)
        {
            _planTaskDetailDal = planTaskDetailDal;
        }
        
        public PlanTaskDetail AddPlanTaskDetail(PlanTaskDetail newPlantaskDetail)
        {
            PlanTaskDetail rtPlanTaskDetail = null;
            try
            {
                _planTaskDetailDal.Add(newPlantaskDetail);
                rtPlanTaskDetail = newPlantaskDetail;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加生产计划信息失败！", ex);
            }
            return rtPlanTaskDetail;
        }

        public PlanTaskDetail UpdPlanTaskDetail(PlanTaskDetail updPlantaskDetail)
        {
            PlanTaskDetail rtPlanTaskDetail = null;
            try
            {
                _planTaskDetailDal.Update(updPlantaskDetail);
                rtPlanTaskDetail = updPlantaskDetail;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("修改生产计划信息失败！", ex);
            }
            return rtPlanTaskDetail;
        }

        public void DelPlantaskDetail(PlanTaskDetail delPlantaskDetail)
        {
            try
            {
                _planTaskDetailDal.Delete(delPlantaskDetail);
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除生产计划信息失败！", ex);
            }
        }

        public IEnumerable<PlanTaskDetail> SelectAllPlanDetails()
        {
            return _planTaskDetailDal.GetAll().Entities;
        }


        //public PlanTaskDetail AddPlanDetailWithPlantask(PlanTask newPlantask, PlanTaskBatchDetail newPlantaskDetail)
        //{
        //    PlanTaskDetail rtPlanTaskDetail = null;
        //    try
        //    {
               
        //        //_planTaskDetailDal.Add(newPlantaskDetail);
        //        //rtPlanTaskDetail = newPlantaskDetail;
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        throw new AisinoMesServiceException("添加生产计划信息失败！", ex);
        //    }
        //    return rtPlanTaskDetail;
        //}
    }
}
