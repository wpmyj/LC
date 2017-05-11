using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.WarehouseManager
{
    public interface IWarehouseInOutRecordService
    {
        /// <summary>
        /// 根据计划批次明细更新出入库记录
        /// </summary>
        /// <param name="warehouseStoreInfo">仓储保管帐信息</param>
        /// <param name="planTaskBatchDetail">作业批次明细</param>
        /// <returns>更新的出入库记录</returns>
        WarehouseInOutRecord UpdateWarehouseInOutRecordWithStoreInfoAndBatchDetail(WarehouseStoreInfo warehouseStoreInfo,WarehouseStoreInfo outWarehouseStoreInfo,PlanTaskBatchDetail planTaskBatchDetail,string orgCode);

        /// <summary>
        /// 根据计划批次冲补记录更新出入库记录
        /// </summary>
        /// <param name="warehouseStoreInfo">仓储保管帐信息</param>
        /// <param name="planTaskBatchAdjust">作业批次冲补记录</param>
        /// <returns>更新的出入库记录</returns>
        WarehouseInOutRecord UpdateWarehouseInOutRecordWithStoreInfoAndBatchAdjust(WarehouseStoreInfo warehouseStoreInfo, PlanTaskBatchAdjust planTaskBatchAdjust,PlanTaskBatch plantaskBatch);


        /// <summary>
        /// 新增出入库记录
        /// </summary>
        /// <param name="newWarehouseInOutRecord"></param>
        /// <returns></returns>
        WarehouseInOutRecord AddWarehouseInOutRecordWithPlanTaskInHouse(WarehouseInOutRecord newWarehouseInOutRecord);

        void RefreshData();
    }
}
