using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;

namespace Aisino.MES.Service.ManuManager
{
    public interface IPlanTaskBatchService
    {
        #region 计划批次操作
        /// <summary>
        /// 根据计划创建计划批次
        /// 一般用于计划第一次创建的时候
        /// </summary>
        /// <param name="planTask">传入的计划</param>
        /// <returns></returns>
        PlanTaskBatch AddPlanTaskBatchWithPlanTask(PlanTask planTask,PlanTaskType planTaskType);

        /// <summary>
        /// 复制上一条批次，并根据此新建一批次
        /// 一般用于值仓时由于粮食质量原因引起的换仓操作，且新建批次必须需要检化验
        /// </summary>
        /// <param name="PlanTaskBatch">传入的上一条批次</param>
        /// <returns></returns>
        PlanTaskBatch AddPlanTaskBatchWithPlanTaskBatch(PlanTaskBatch planTaskBatch);

        /// <summary>
        /// 根据界面新建的计划批次新增
        /// </summary>
        /// <param name="plantask">所依赖的计划</param>
        /// <param name="planTaskBatch">需要新增的计划批次</param>
        /// <returns></returns>
        PlanTaskBatch AddPlanTaskBatchWithCreate(PlanTask plantask,PlanTaskBatch planTaskBatch);

        /// <summary>
        /// 根据计划批次编号获取计划批次
        /// </summary>
        /// <param name="batchNumber"> 计划批次编号</param>
        /// <returns></returns>
        PlanTaskBatch GetSinglePlanTaskBatchWithNumber(string batchNumber);

        /// <summary>
        /// 根据内部车辆查找正在执行的计划批次
        /// </summary>
        /// <param name="innerVehicleId">内部车辆id</param>
        /// <returns>正在执行的计划批次</returns>
        PlanTaskBatch GetRunningPlanTaskBatchByInnerVehicle(string innerVehicleId);

        /// <summary>
        /// 根据计划编号获取当前正在执行的批次
        /// </summary>
        /// <param name="plantaskNumber">计划编号</param>
        /// <returns>正在执行的批次</returns>
        PlanTaskBatch GetRunningPlanTaskBatchByPlanTaskNumber(string plantaskNumber);

        IEnumerable<PlanTaskBatch> GetEnrolmentListIsRuning();

        //查找未扦样作业计划
        RepositoryResultList<PlanTaskBatch> SelectPlanTaskBatchWithNoAssay(int PageNumber, int PageSize);

        //查找已扦样作业计划
        RepositoryResultList<PlanTaskBatch> SelectPlanTaskBatchWithAssay(int userId,int PageNumber, int PageSize,string vehiclePlate);

        //更新计划批次
        PlanTaskBatch UpdatePlantaskBatch(PlanTaskBatch upplantaskbatch);

        PlanTaskBatch UpPlantaskBatchWithUnit(PlanTaskBatch upplantaskbatch);

        #endregion

        #region 计划批次明细操作
        /// <summary>
        /// 根据计划批次编号及内部车辆查找未完成的批次称重明细
        /// </summary>
        /// <param name="planTaskBatchNumber">批次编号</param>
        /// <param name="vehicleId">内部车辆id</param>
        /// <returns>作业批次明细</returns>
        PlanTaskBatchDetail GetNotFinishBatchDetailByNumberAndVehicle(string planTaskBatchNumber, string vehicleId);

        /// <summary>
        /// 根据磅单号查询批次称重明细
        /// </summary>
        /// <param name="ScaleBillNumber">磅单号</param>
        /// <returns></returns>
        PlanTaskBatchDetail GetPlanTaskBatchDetailByScaleBillNumber(string ScaleBillNumber);

        /// <summary>
        /// 根据称重号查询PlanTaskBatchDetail
        /// </summary>
        /// <param name="WeightNumber">称重号</param>
        /// <returns></returns>
        PlanTaskBatchDetail GetPlanTaskBatchDetailByWeightNumber(string WeightNumber);

        /// <summary>
        /// 根据Generateid查询PlanTaskBatch
        /// </summary>
        /// <param name="WeightNumber">称重号</param>
        /// <returns></returns>
        PlanTaskBatch GetSinglePlanTaskBatchWithGenerateid(int generateid);

        /// <summary>
        /// 根据标签卡主标签号获取批次
        /// </summary>
        /// <param name="rfid">标签卡</param>
        /// <returns></returns>
        PlanTaskBatch GetSinglePlanTaskBatchWithRfid(string rfidNumber);

        /// <summary>
        /// 完成值仓确认
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        void UpdatePlanTaskBatchDetailByDutyConfirm(PlanTaskBatchDetail planTaskBatchDetail);

        /// <summary>
        /// 刷新数据
        /// </summary>
        void RefreshData();
        #endregion

        PlanTaskBatch GetPlanTaskBatchByCardID(string card_mainid);

        PlanTaskBatch GetPlanTaskBatchByDiBangCode(string code);

        void UpdatePlantaskAndBatch(PlanTask thePT, PlanTaskBatch thePTBatch);

        /// <summary>
        /// 根据计划批次编号获得该计划批次所有执行过程中使用过的仓房
        /// </summary>
        /// <param name="plantaskBatchNum">计划批次编号</param>
        /// <returns>所有使用过的仓房信息</returns>
        IList<Warehouse> FindPlanTaskBatchWarehousList(string plantaskBatchNum);

        /// <summary>
        /// 根据计划批次编号以及选择的仓房信息绑定执行中选择的货位
        /// </summary>
        /// <param name="plantaskBatchNum">计划批次编号</param>
        /// <param name="warehouseId">仓房信息</param>
        /// <returns>货位列表</returns>
        IList<GoodsLocation> FindPlanTaskBatchLocationList(string plantaskBatchNum,string warehouseId);

        List<PlanTaskBatch> GetRunningPlanTaskBatchOutWarehouseInnerVehicleWithWharf();
        List<PlanTaskBatchDetail> GetPlanTaskBatchDetailWithOnlyGross(PlanTaskBatch planTaskBatch);
        List<PlanTaskBatch> GePlanTaskBatchsByPlanTaskBatchNumber(string[] plantaskbatchnumbers);
        PlanTaskBatch GetSinglePlanTaskBatchWithGenerateidWithAll(int generateid);
        PlanTaskBatch GetSinglePlanTaskBatchWithGenerateidOnlyBatch(int generateid);
    }
}
