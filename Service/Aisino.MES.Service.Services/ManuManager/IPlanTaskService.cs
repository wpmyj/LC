using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Helpers;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.NewModels;

namespace Aisino.MES.Service.ManuManager
{
    public interface IPlanTaskService
    {
        IEnumerable<PlanTask> SelectAllPlanTask();

        /// <summary>
        /// 根据条件查询相应的计划
        /// </summary>
        /// <param name="plan_status">计划状态</param>
        /// <param name="traficType">运输方式</param>
        /// <param name="planDate">计划日期</param>
        /// <param name="planType">计划类型</param>
        /// <param name="planNumber">计划编号</param>
        /// <param name="pagingCriteria"></param>
        /// <returns></returns>
        RepositoryResultList<PlanTask> SelectPlanTaskByCondition(int plan_status, int traficType, DateTime planDate, string planType, string planNumber, PagingCriteria pagingCriteria);

        // 根据生产计划状态来获取生产计划
        RepositoryResultList<PlanTask> SelectPlanTaskByStatus(int plan_status, PagingCriteria pagingCriteria);

        //根据strWhere来获得未下达的生产计划
        IEnumerable<PlanTask> SelectPlanTaskBySql(string strWhere);

        // 根据isAssigned获取生产计划
        IEnumerable<PlanTask> SelectAllPlanTask(bool isAssigned);

        //根据身份证获取生成计划
        IEnumerable<PlanTask> SelectAllPlanTaskByIDCard(string IDCard);

        //获得所有需要同步的计划
        IEnumerable<PlanTask> SelectAllNeedSyncPlanTask();
        PlanTask GetPlanTask(string planNum);
        PlanTask AddPlanTask(PlanTask newPlanTask, int billNoID);
        PlanTask AddPlanTaskWithEnrolment(Enrolment newEnrolment, PlanTaskType planTaskType, string OrgDepCode);
        PlanTask UpdatePlanTask(PlanTask updPlanTask);
     
        //更新计划入库表， 计划出库表和生产计划表
        PlanTask UpdatePlanTask(PlanTask updPlanTask, bool outWarehouseUpdateFlag, List<PlanTaskOutWarehouse> outWarehouseList, bool inWarehouseUpdateFlag, List<PlanTaskInWarehouse> inWarehouseList);

        PlanTask DeletePlanTask(PlanTask delPlanTask);
        bool CheckPlanTaskCodeExist(string planTaskNo);

        IEnumerable<PlanTaskType> SelectAllPlanTaskType();
        PlanTaskType GetPlanTaskType(string planTaskTypeCode);
        PlanTaskType AddPlanTaskType(PlanTaskType newPlanTaskType);
        PlanTaskType UpdatePlanTaskType(PlanTaskType updPlanTaskType);
        PlanTaskType DeletePlanTaskType(PlanTaskType delPlanTaskType);
        void DeletePlanTaskTypeList(List<PlanTaskType> lstDelPlanTaskType);
        bool CheckPlanTaskTypeCodeExist(string planTaskTypeCode);
        bool CheckPlanTaskTypeNameExist(string planTaskTypeName);

 
        void UpdateWarehouseInList(string number, IList<PlanTaskInWarehouse> inWarehouseList);

        void UpdateWarehouseOutList(string number, IList<PlanTaskOutWarehouse> outWarehouseList);

        PlanTask AddPlanTask_Unitwork(PlanTask newPlanTask);

        /// <summary>
        /// 称重上传
        /// </summary>
        /// <param name="vehicleRFIDTag">RFID标签号码</param>
        /// <param name="scaleBillNumber">磅单编号</param>
        /// <param name="weighRecordNumber">称重编号</param>
        /// <param name="weighType">称重类型 0皮重，1毛重</param>
        /// <param name="weightCount">称重重量，以千克为单位的数字</param>
        /// <param name="weighTime">称重时间</param>
        /// <param name="goodsID">粮食品种编号</param>
        /// <param name="workNumber">作业编号(计划批次的generate_id)</param>
        /// <param name="wareHouseID">仓库编号</param>
        /// <param name="outWareHouseID">出库仓库编号(仅导仓时用)</param>
        /// <param name="strErrorMessage">错误信息</param>
        /// <returns></returns>
        bool WeightUpload(string vehicleRFIDTag, string scaleBillNumber, string weighRecordNumber, int weighType,
           string weightCount, DateTime weightTime, int goodsID, string workNumber, int wareHouseID, int outWareHouseID, ref string strErrorMessage, ref string strTime, QuantityRecordDetail quantityDetail);

        /// <summary>
        /// 净重上传
        /// </summary>
        /// <param name="netWeight">净重</param>
        /// <param name="workNumber">作业编号(计划批次的generate_id)</param>
        /// <param name="strErrorMessage">错误信息</param>
        /// <returns></returns>
        bool NetWeightUpload(int netWeight, string workNumber, ref string strErrorMessage);

        /// <summary>
        /// 根据磅单号回滚榜单
        /// </summary>
        /// <param name="ScaleBillNumber">磅单号</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        bool CancelVehicleScaleBill(string ScaleBillNumber, ref string strErrorMessage);

        /// <summary>
        /// 取消称重
        /// </summary>
        /// <param name="WeighRecordNumber">称重单号</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        bool CancelVehicleWeighBill(string WeighRecordNumber, ref string strErrorMessage);

        /// <summary>
        /// 添加计划，计划批次信息
        /// </summary>
        /// <returns></returns>
        PlanTask AddPlanTaskWithInOutWarehouse(Decimal count, GoodsKind goodkinds, PlanTaskType planTaskType, string OrgDepCode, int accountId,
                                                List<PlanTaskOutWarehouse> planTaskOutWarehouses, List<PlanTaskInWarehouse> planTaskInWarehouses,
            List<PlanTaskBatchSiteScale> planTaskBatchSiteScales, List<PlanTaskBatchVehicle> planTaskBatchVehicles, List<PlanTaskBatchWorkPlace> planTaskBatchWorkPlaces,bool Isadd);


        PlanTask AddPlanTaskWithInOutWarehouse(PlanTask newPlantask,PlanTaskBatch newPlantaskBatch, string OrgDepCode, int accountId,
                                                List<PlanTaskOutWarehouse> planTaskOutWarehouses, List<PlanTaskInWarehouse> planTaskInWarehouses,
            List<PlanTaskBatchSiteScale> planTaskBatchSiteScales, List<PlanTaskBatchVehicle> planTaskBatchVehicles, List<PlanTaskBatchWorkPlace> planTaskBatchWorkPlaces, bool Isadd);

        /// <summary>
        /// 刷新数据
        /// </summary>
        void RefreshData();

        void UpdatePlanTaskWithWarehouseStore(PlanTask upplantask);

        /// <summary>
        /// 计划完成时间
        /// </summary>
        /// <param name="upplantask"></param>
        void FinshPlanTask(PlanTask upplantask);

        /// <summary>
        /// 根据条件分页查询符合的计划单信息
        /// </summary>
        /// <param name="planstatus">计划单状态</param>
        /// <param name="plantypecode">计划类型</param>
        /// <param name="plannum">计划单号</param>
        /// <param name="planbegain">计划开始时间</param>
        /// <param name="planend">计划结束时间</param>
        /// <param name="paging">分页信息</param>
        /// <returns>符合条件的计划单列表</returns>
        RepositoryResultList<PlanTask> FindPagePlanTaskByCondition(int planstatus, string plantypecode, string plannum, DateTime planbegain, DateTime planend, PagingCriteria paging);

        PlanTask UpPlantaskWithUnit(PlanTask upplantask);

        RepositoryResultList<PlanTask> FindPagePlanTaskByCondition(int plan_status, int traficType, DateTime planDate, string planType, string planNumber, DateTime planbegain, DateTime planend, PagingCriteria pagingCriteria);
        RepositoryResultList<PlanTask> FindPageFastByCondition(int enrolment_status, int assay_status, int plan_status, string plateNumber, string ownerName, DateTime planbegain, DateTime planend);
        RepositoryResultList<PlanTask> GetPlantaskListForStatementInfo(DateTime dtStart, DateTime dtEnd, int warehouse_id, int grain_id, string statementMan, string clientName, string trafficNumber, int statement_status);
        RepositoryResultList<PlanTask> GetPlantaskBatchListForFastInfo(string enrolment_number, string plate_number, string warehouse_name, Nullable<decimal> gross_weight, Nullable<decimal> tare_weight, Nullable<decimal> weight, int unit_price, string owner_name);

        //void AddPlanTaskBatchStepStatus(string planTaskNum, string planTaskBatchNum, string stepCode);

        /// <summary>
        /// 获取码头ID
        /// </summary>
        /// <param name="vehicleRFIDTag">主卡编号</param>
        /// <returns></returns>
        //string GetWharf(string vehicleRFIDTag);

        //List<VehicleWharf> GetInVehicleWharf();

        IEnumerable<PlanTask> FindPlantaskListByContract(Contract cont);
    }
}
