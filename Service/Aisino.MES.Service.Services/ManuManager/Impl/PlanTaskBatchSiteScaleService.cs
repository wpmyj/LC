using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.ManuManager.Impl
{
   public class PlanTaskBatchSiteScaleService : IPlanTaskBatchSiteScaleService
    {
       private Repository<PlanTaskBatchSiteScale> _planTaskBatchSiteScaleDal;
       private UnitOfWork _unitOfWork;

       public PlanTaskBatchSiteScaleService(Repository<PlanTaskBatchSiteScale> planTaskBatchSiteScaleDal,
                                            UnitOfWork unitOfWork)
       {
           _planTaskBatchSiteScaleDal = planTaskBatchSiteScaleDal;
           _unitOfWork = unitOfWork;
       }


       /// <summary>
       /// 磅秤信息新增
       /// </summary>
       /// <param name="plantaskbatch"></param>
       /// <param name="plantaskInHouseList"></param>
       public void AddPlanTaskBatchSiteScale(PlanTaskBatch plantaskbatch, List< PlanTaskBatchSiteScale> planSiteList)
       {
           try
           {
               List<PlanTaskBatchSiteScale> oldplanSiteList = _planTaskBatchSiteScaleDal.Find(ss => ss.plantask_batch_number == plantaskbatch.plantask_batch_number).Entities.ToList();
               if (oldplanSiteList != null && oldplanSiteList.Count > 0)
               {
                   foreach (PlanTaskBatchSiteScale plansite in planSiteList)
                   {
                       //查找是否存在要保存的项
                       if (!oldplanSiteList.Any(ss => ss.plantask_batch_number == plansite.plantask_batch_number && ss.site_scale_id == plansite.site_scale_id))
                       {
                           _unitOfWork.AddAction(plansite, DataActions.Add);
                       }
                   }
               }
               else
               {
                   foreach (PlanTaskBatchSiteScale plansite in planSiteList)
                   {
                       _unitOfWork.AddAction(plansite, DataActions.Add);
                   }
               }
           }
           catch (RepositoryException ex)
           {
               throw ex;
           }
       }


       public void DelPlanTaskBatchSiteScale(List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchSiteScale> newplanscale)
       {
           try
           {
               foreach (PlanTaskBatchSiteScale planscale in planSiteList)
               {
                   _unitOfWork.AddAction(planscale, DataActions.Delete);
               }
               //_planTaskBatchSiteScaleDal.DeleteAll(planSiteList);
               foreach (PlanTaskBatchSiteScale planscale in newplanscale)
               {
                   //_planTaskBatchSiteScaleDal.Add(planscale);
                   _unitOfWork.AddAction(planscale, DataActions.Add);
               }
           }
           catch (RepositoryException ex)
           {
               throw ex;
           }
       }

       public void UpPlanTaskBatchSiteScale(PlanTaskBatch plantaskbatch, List<PlanTaskBatchSiteScale> planSiteList)
       {
           List<PlanTaskBatchSiteScale> oldPlanTaskBatchSiteScale = plantaskbatch.PlanTaskBatchSiteScales.ToList();

           if (oldPlanTaskBatchSiteScale != null && oldPlanTaskBatchSiteScale.Count > 0)
           {
               foreach (PlanTaskBatchSiteScale planSiteScale in planSiteList)
               {
                   //查找选择的菜单是否已存在
                   if (!oldPlanTaskBatchSiteScale.Any(d => d.plantask_batch_number == planSiteScale.plantask_batch_number && d.site_scale_id == planSiteScale.site_scale_id))
                   {
                       _unitOfWork.AddAction(planSiteScale, DataActions.Add);
                   }
               }
           }
           else
           {
               foreach (PlanTaskBatchSiteScale planSiteScale in planSiteList)
               {
                   _unitOfWork.AddAction(planSiteScale, DataActions.Add);
               }
           }

           //查找之前选择的菜单是否已删除
           if (oldPlanTaskBatchSiteScale != null)
           {
               //原有单头所含明细不为空，则需要判断是否有删除项
               foreach (PlanTaskBatchSiteScale planSiteScale in oldPlanTaskBatchSiteScale.Where(x => !planSiteList.Any(u => u.site_scale_id == x.site_scale_id && u.plantask_batch_number == x.plantask_batch_number)).ToList())
               {
                   _unitOfWork.AddAction(planSiteScale, DataActions.Delete);
               }
           }
       }

       public void RefreshData()
       {
           this._planTaskBatchSiteScaleDal.RefreshData();
       }
    }
}
