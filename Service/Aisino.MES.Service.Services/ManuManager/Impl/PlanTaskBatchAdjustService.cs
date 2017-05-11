using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.Service.WarehouseManager;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.Service.SysManager;
using Aisino.MES.Service.StoreProcedures;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.ManuManager.Impl
{
   public class PlanTaskBatchAdjustService : IPlanTaskBatchAdjustService
    {
       private Repository<PlanTaskBatchAdjust> _planTaskAdjustDal;
       private UnitOfWork _unitOfWork;
       private IWarehouseStoreInfoService _warehouseStoreInfoService;
       private ISysBillNoService _sysBillNoService;
       SPGetSysDateTimeService _sPGetSysDateTimeService;

       public PlanTaskBatchAdjustService(Repository<PlanTaskBatchAdjust> planTaskAdjustDal,
                                         UnitOfWork unitOfWork,
                                         IWarehouseStoreInfoService warehouseStoreInfoService,
                                         ISysBillNoService sysBillNoService,
                                         SPGetSysDateTimeService sPGetSysDateTimeService
                                        )
       {
           _planTaskAdjustDal = planTaskAdjustDal;
           _unitOfWork = unitOfWork;
           _warehouseStoreInfoService = warehouseStoreInfoService;
           _sysBillNoService = sysBillNoService;
           _sPGetSysDateTimeService = sPGetSysDateTimeService;
       }

       public PlanTaskBatchAdjust AddPlantaskAdjustWithWarehouseInfor(PlanTaskBatchAdjust newplanAdjust,string OrgDepCode)
       {
           PlanTaskBatchAdjust rtnplantaskbatchAdjust = null;
           try
           {
               _unitOfWork.Actions.Clear();
               int billId = _sysBillNoService.GetBillNoID("MAN", "PAN");
               newplanAdjust.plantask_batch_adjust_number = _sysBillNoService.GetBillNo(billId, OrgDepCode);
               newplanAdjust.adjust_time = _sPGetSysDateTimeService.GetSysDateTime();
               _unitOfWork.AddAction(newplanAdjust, DataActions.Add);

               //_warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchAdjust(newplanAdjust);
               _unitOfWork.Save();
           }
           catch (RepositoryException ex)
           {
               throw ex;
           }
           return rtnplantaskbatchAdjust;
       }

       public PlanTaskBatchAdjust AddPlantaskAdjustWithPlantaskBatchInfo(PlanTaskBatchAdjust newPlanAdjust, PlanTaskBatch plantaskBatch, string orgDeptCode)
       {
           try
           {
               //新建冲补单
               _unitOfWork.Actions.Clear();
               int billId = _sysBillNoService.GetBillNoID("MAN", "PAN");
               newPlanAdjust.plantask_batch_adjust_number = _sysBillNoService.GetBillNo(billId, orgDeptCode);
               newPlanAdjust.adjust_time = _sPGetSysDateTimeService.GetSysDateTime();
               _unitOfWork.AddAction(newPlanAdjust, DataActions.Add);

               //更新批次净重
               if (newPlanAdjust.adjust_type == (int)plantaskahjusttype.增加)
               {
                   plantaskBatch.batch_count += newPlanAdjust.adjust_count;
               }
               else
               {
                   plantaskBatch.batch_count -= newPlanAdjust.adjust_count;
               }
               _unitOfWork.AddAction(plantaskBatch, DataActions.Update);

               //更新库存记录
               _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchAdjust(newPlanAdjust, plantaskBatch);

               _unitOfWork.Save();
           }
           catch (RepositoryException ex)
           {
               throw new AisinoMesServiceException("新增作业计划冲补记录失败！", ex);
           }
           catch (Exception ex)
           {
               throw new AisinoMesServiceException("新增作业计划冲补记录失败！", ex);
           }

           return newPlanAdjust;
       }
    }
}
