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
   public class PlanTaskBatchVehicleService : IPlanTaskBatchVehicleService
    {
       private Repository<PlanTaskBatchVehicle> _planTaskBatchVehicleDal;
       private UnitOfWork _unitOfWork;

       public PlanTaskBatchVehicleService(Repository<PlanTaskBatchVehicle> planTaskBatchVehicleDal,
                                            UnitOfWork unitOfWork)
       {
           _planTaskBatchVehicleDal = planTaskBatchVehicleDal;
           _unitOfWork = unitOfWork;
       }

       public void RefreshData()
       {
           _planTaskBatchVehicleDal.RefreshData(); 
       }


       /// <summary>
       /// 内部车辆新增
       /// </summary>
       /// <param name="plantaskbatch"></param>
       /// <param name="planVehicleList"></param>
       public void AddPlanTaskBatchVehicle(PlanTaskBatch plantaskbatch, List<PlanTaskBatchVehicle> planVehicleList)
       {
           try
           {
               List<PlanTaskBatchVehicle> oldplanVehicleList = _planTaskBatchVehicleDal.Find(ss => ss.plantask_batch_number == plantaskbatch.plantask_batch_number).Entities.ToList();
               if (oldplanVehicleList != null && oldplanVehicleList.Count > 0)
               {
                   foreach (PlanTaskBatchVehicle planvehicle in planVehicleList)
                   {
                       //查找是否存在要保存的项
                       if (!oldplanVehicleList.Any(ss => ss.plantask_batch_number == planvehicle.plantask_batch_number && ss.vehicle_id ==planvehicle.vehicle_id ))
                       {
                           _unitOfWork.AddAction(planvehicle, DataActions.Add);
                       }
                   }
               }
               else
               {
                   foreach (PlanTaskBatchVehicle planvehicle in planVehicleList)
                   {
                       _unitOfWork.AddAction(planvehicle, DataActions.Add);
                   }
               }
           }
           catch (RepositoryException ex)
           {
               throw ex;
           }
       }


       public void DelPlanTaskBatchVehicle(List<PlanTaskBatchVehicle> planVehicleList, List<PlanTaskBatchVehicle> newplanVehicle)
       {
           try
           {
               foreach (PlanTaskBatchVehicle planVehicle in planVehicleList)
               {
                   _unitOfWork.AddAction(planVehicle, DataActions.Delete);
               }
               //_planTaskBatchSiteScaleDal.DeleteAll(planSiteList);
               if (newplanVehicle != null)
               {
                   foreach (PlanTaskBatchVehicle planVehicle in newplanVehicle)
                   {
                       //_planTaskBatchSiteScaleDal.Add(planscale);
                       _unitOfWork.AddAction(planVehicle, DataActions.Add);
                   }
               }
           }
           catch (RepositoryException ex)
           {
               throw ex;
           }
       }

       public void UpdatePlanTaskBatchVehicle(PlanTaskBatch plantaskbatch, List<PlanTaskBatchVehicle> planVehicleList)
       {
           try
           {
               List<PlanTaskBatchVehicle> oldplanVehicleList = _planTaskBatchVehicleDal.Find(ss => ss.plantask_batch_number == plantaskbatch.plantask_batch_number).Entities.ToList();
               if (oldplanVehicleList != null && oldplanVehicleList.Count > 0)
               {
                   foreach (PlanTaskBatchVehicle planvehicle in planVehicleList)
                   {
                       //查找是否存在要保存的项
                       if (!oldplanVehicleList.Any(ss => ss.plantask_batch_number == planvehicle.plantask_batch_number && ss.vehicle_id == planvehicle.vehicle_id))
                       {
                           _unitOfWork.AddAction(planvehicle, DataActions.Add);
                       }
                   }
               }
               else
               {
                   foreach (PlanTaskBatchVehicle planvehicle in planVehicleList)
                   {
                       _unitOfWork.AddAction(planvehicle, DataActions.Add);
                   }
               }

               //查找之前选择的菜单是否已删除
               if (oldplanVehicleList != null)
               {
                   //原有单头所含明细不为空，则需要判断是否有删除项
                   foreach (PlanTaskBatchVehicle planvehicle in oldplanVehicleList.Where(x => !planVehicleList.Any(u => u.vehicle_id == x.vehicle_id && u.plantask_batch_number == x.plantask_batch_number)).ToList())
                   {
                       _unitOfWork.AddAction(planvehicle, DataActions.Delete);
                   }
               }
               if (plantaskbatch.PlanTask.plan_status == (int)PlanTaskStatus.执行)
               {
                   //执行状态则更新车辆在线信息
                   foreach (PlanTaskBatchVehicle planvehicle in planVehicleList)
                   {
                       planvehicle.InnerVehicle.inner_vehicle_online = true;
                       _unitOfWork.AddAction(planvehicle.InnerVehicle, DataActions.Update);
                   }
               }
           }
           catch (RepositoryException ex)
           {
               throw ex;
           }
       }
    }
}
