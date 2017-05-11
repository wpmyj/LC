using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;
using Aisino.MES.DAL.Interfaces;

namespace Aisino.MES.Service.ManuManager.Impl
{
   public class PlanTaskBatchWorkPlaceService : IPlanTaskBatchWorkPlaceService
    {
       private Repository<PlanTaskBatchWorkPlace> _planTaskBatchWorkPlaceDal;
       private UnitOfWork _unitOfWork;

       public PlanTaskBatchWorkPlaceService(Repository<PlanTaskBatchWorkPlace> planTaskBatchWorkPlaceDal,
                                            UnitOfWork unitOfWork)
       {
           _planTaskBatchWorkPlaceDal = planTaskBatchWorkPlaceDal;
           _unitOfWork = unitOfWork;
       }


       public void RefreshData()
       {
           _planTaskBatchWorkPlaceDal.RefreshData();
       }

       /// <summary>
       /// 工作地点新增
       /// </summary>
       /// <param name="plantaskbatch"></param>
       /// <param name="workplaceList"></param>
       public void AddPlanTaskBatchWorkPlace(PlanTaskBatch plantaskbatch, List<PlanTaskBatchWorkPlace> workplaceList)
       {
           try
           {
               List<PlanTaskBatchWorkPlace> oldplanWorkPlaceList = _planTaskBatchWorkPlaceDal.Find(ss => ss.plantask_batch_number == plantaskbatch.plantask_batch_number).Entities.ToList();
               if (oldplanWorkPlaceList != null && oldplanWorkPlaceList.Count > 0)
               {
                   foreach (PlanTaskBatchWorkPlace planworkplace in workplaceList)
                   {
                       //查找是否存在要保存的项
                       if (!oldplanWorkPlaceList.Any(ss => ss.plantask_batch_number == planworkplace.plantask_batch_number && ss.workplace_id == planworkplace.workplace_id))
                       {
                           _unitOfWork.AddAction(planworkplace, DataActions.Add);
                       }
                   }
               }
               else
               {
                   foreach (PlanTaskBatchWorkPlace planworkplace in workplaceList)
                   {
                       _unitOfWork.AddAction(planworkplace, DataActions.Add);
                   }
               }
           }
           catch (RepositoryException ex)
           {
               throw ex;
           }
       }

       public void UpdatePlanTaskBatchWorkPlace(PlanTaskBatch plantaskbatch, List<PlanTaskBatchWorkPlace> workplaceList)
       {
           try
           {
               List<PlanTaskBatchWorkPlace> oldplanWorkPlaceList = _planTaskBatchWorkPlaceDal.Find(ss => ss.plantask_batch_number == plantaskbatch.plantask_batch_number).Entities.ToList();
               if (oldplanWorkPlaceList != null && oldplanWorkPlaceList.Count > 0)
               {
                   foreach (PlanTaskBatchWorkPlace planworkplace in workplaceList)
                   {
                       //查找是否存在要保存的项
                       if (!oldplanWorkPlaceList.Any(ss => ss.plantask_batch_number == planworkplace.plantask_batch_number && ss.workplace_id == planworkplace.workplace_id))
                       {
                           _unitOfWork.AddAction(planworkplace, DataActions.Add);
                       }
                   }
               }
               else
               {
                   foreach (PlanTaskBatchWorkPlace planworkplace in workplaceList)
                   {
                       _unitOfWork.AddAction(planworkplace, DataActions.Add);
                   }
               }

               //查找之前选择的菜单是否已删除
               if (oldplanWorkPlaceList != null)
               {
                   //原有单头所含明细不为空，则需要判断是否有删除项
                   foreach (PlanTaskBatchWorkPlace planworkplace in oldplanWorkPlaceList.Where(x => !workplaceList.Any(u => u.workplace_id == x.workplace_id && u.plantask_batch_number == x.plantask_batch_number)).ToList())
                   {
                       _unitOfWork.AddAction(planworkplace, DataActions.Delete);
                   }
               }
           }
           catch (RepositoryException ex)
           {
               throw ex;
           }
       }


       public void DelPlanTaskBatchWorkPlace(List<PlanTaskBatchWorkPlace> planWorkPlaceList, List<PlanTaskBatchWorkPlace> newplanWorkPlace)
       {
           try
           {
               foreach (PlanTaskBatchWorkPlace planWorkPlace in planWorkPlaceList)
               {
                   _unitOfWork.AddAction(planWorkPlace, DataActions.Delete);
               }
               //_planTaskBatchSiteScaleDal.DeleteAll(planSiteList);
               foreach (PlanTaskBatchWorkPlace planWorkPlace in newplanWorkPlace)
               {
                   //_planTaskBatchSiteScaleDal.Add(planscale);
                   _unitOfWork.AddAction(planWorkPlace, DataActions.Add);
               }
           }
           catch (RepositoryException ex)
           {
               throw ex;
           }
       }
    }
}
