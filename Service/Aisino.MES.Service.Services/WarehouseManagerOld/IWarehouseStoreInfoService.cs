using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.WarehouseManager
{
    public interface IWarehouseStoreInfoService
    {
        /// <summary>
        /// 根据存放的货物品种获得当前可用的仓房
        /// </summary>
        /// <param name="goodsKindId">货品id</param>
        /// <returns>符合条件的仓储保管帐</returns>
        IEnumerable<WarehouseStoreInfo> FindUsingWarehouseStoreInfoByGoodsKind(int goodsKindId);

        /// <summary>
        /// 根据批次明细更新仓储保管帐
        /// </summary>
        /// <param name="planTaskBatchDetail">批次明细</param>
        /// <returns>更新后的仓储保管帐</returns>
        WarehouseStoreInfo UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(PlanTaskBatchDetail planTaskBatchDetail);

        /// <summary>
        /// 根据批次冲补记录更新仓储保管帐
        /// </summary>
        /// <param name="planTaskBatchAdjust">冲补记录</param>
        /// <param name="inoutType">出入库方向</param>
        /// <returns>更新后的仓储保管帐</returns>
        WarehouseStoreInfo UpdateWarehouseStoreInfoWithPlanTaskBatchAdjust(PlanTaskBatchAdjust planTaskBatchAdjust,PlanTaskBatch plantaskBatch);

        /// <summary>
        /// 新增仓库存储信息
        /// </summary>
        /// <param name="newWarehouseStoreInfo"></param>
        /// <returns></returns>
        WarehouseStoreInfo AddWarehouseStoreWithPlanTaskInOutHouse(WarehouseStoreInfo newWarehouseStoreInfo);


        /// <summary>
        /// 更新仓库存储信息
        /// </summary>
        /// <param name="newWarehouseStoreInfo"></param>
        /// <returns></returns>
        WarehouseStoreInfo UpWarehouseStoreInfoWithPlanTaskInOutHouse(WarehouseStoreInfo newWarehouseStoreInfo);

        /// <summary>
        /// 根据PlanTaskBatchDetail中的磅单号回滚称重信息
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        /// <returns></returns>
        WarehouseStoreInfo CancelWarehouseStoreInfoWithScaleBill(PlanTaskBatchDetail planTaskBatchDetail);

        /// <summary>
        /// 根据PlanTaskBatchDetail中的称重号回滚称重信息
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        /// <returns></returns>
        WarehouseStoreInfo CancelWarehouseStoreInfoWithWeightNumber(PlanTaskBatchDetail planTaskBatchDetail);

        /// <summary>
        /// 更新出入库凭证
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        void UpDateCertificate(PlanTaskBatchDetail planTaskBatchDetail);

        /// <summary>
        /// 撤销回滚出入库凭证数据
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        void CancelDateCertificate(PlanTaskBatchDetail planTaskBatchDetail);

        /// <summary>
        /// 更新合同客户计划架空管理(计划完成时更新)
        /// </summary>
        /// <param name="planTask"></param>
        void UpDateBusinessWeight(PlanTask planTask);
        void UpDateBusinessWeight(PlanTaskBatchDetail planTaskBatchDetail, decimal dFinishedCount);

        void RefreshData();

        void UdDateContractWeight(PlanTaskBatchDetail planTaskBatchDetail);
    }
}
