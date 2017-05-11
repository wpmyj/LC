using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IPlanTaskInOutWarehouseService
    {

        //已经入库的仓库列表
        IEnumerable<PlanTaskInWarehouse> GetPlanTaskInWarehouseList(string plan_task_id, int invmas_id);


        //已经出库的仓库列表
        IEnumerable<PlanTaskOutWarehouse> GetPlanTaskOutWarehouseList(string plantask_number, int invmas_id);

        /// <summary>
        /// 新增入库的仓库列表
        /// </summary>
        /// <param name="newplantaskInHouse"></param>
        /// <param name="newwarehouseinfor"></param>
        /// <param name="planSiteList"></param>
        /// <param name="planvehicleList"></param>
        /// <param name="workplaceList"></param>
        /// <param name="OrgDepCode"></param>
        /// <returns></returns>
        void AddPlantaskInWarehouse(PlanTask plantask,List< PlanTaskInWarehouse> plantaskInHouseList,List< PlanTaskBatchSiteScale> planSiteList,List<PlanTaskBatchVehicle > planvehicleList,List< PlanTaskBatchWorkPlace> workplaceList,string OrgDepCode,bool needQuality);
 
        /// <summary>
        /// 新增出库的仓库列表
        /// </summary>
        /// <param name="plantask"></param>
        /// <param name="newplantaskout"></param>
        /// <param name="newwarehouseinfor"></param>
        /// <returns></returns>
        void AddPlanTaskOutWarehouse(PlanTask plantask, List<PlanTaskOutWarehouse> plantaskoutHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> workplaceList, string OrgDepCode);

        /// <summary>
        /// 更新作业计划
        /// </summary>
        /// <param name="plantask">作业计划</param>
        /// <param name="plantaskInHouseList">入仓点</param>
        /// <param name="plantaskoutHouseList">出仓点</param>
        /// <param name="planSiteList">地磅</param>
        /// <param name="planvehicleList">内部车</param>
        /// <param name="workplaceList">作业点</param>
        /// <param name="OrgDepCode">组织编号</param>
        void UpdatePlanTask(PlanTask plantask, List<PlanTaskInWarehouse> plantaskInHouseList, List<PlanTaskOutWarehouse> plantaskoutHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> workplaceList, string OrgDepCode, bool isRun = false);

        /// <summary>
        /// 执行入库操作
        /// </summary>
        void DoPlanTsakInWarehouse(PlanTask plantask, PlanTaskBatch plantaskbatch, List<PlanTaskInWarehouse> plantaskInHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> workplaceList, string OrgDepCode);

        /// <summary>
        /// 执行出库操作
        /// </summary>
        void DoPlanTaskOutWarehouse(PlanTask plantask, PlanTaskBatch plantaskbatch, List<PlanTaskOutWarehouse> plantaskOutHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> workplaceList, string OrgDepCode);
        ///// <summary>
        ///// 新增入库的仓库列表(有WarehouseStoreInfor信息)
        ///// </summary>
        ///// <param name="newplantaskInHouse"></param>
        ///// <param name="newwarehouseinfor"></param>
        ///// <param name="planSiteList"></param>
        ///// <param name="planvehicleList"></param>
        ///// <param name="workplaceList"></param>
        ///// <returns></returns>
        //PlanTaskInWarehouse UpPlantaskInWarehouse(PlanTaskInWarehouse newplantaskInHouse, WarehouseStoreInfo newwarehouseinfor, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> workplaceList);
        ////所有入库的仓库列表
        //IEnumerable<CustomInOutWarehouse> GetAllInWarehouseList(int warehouse_id);
   

        ////所有出库的仓库列表
        //IEnumerable<CustomInOutWarehouse> GetAllOutWarehouseList(int warehouse_id);

        IEnumerable<PlanTaskInWarehouse> GetPlanTaskInWarehouseListWithBatch(string plantask_batch_number);
        void AddPlanTaskInWarehouseWithChangeWarehouse(PlanTaskInWarehouse planTaskInWarehouse);
        IEnumerable<PlanTaskOutWarehouse> GetPlanTaskOutWarehouseListWithBatch(string plantask_batch_number);
        void AddPlanTaskOutWarehouseWithChangeWarehouse(PlanTaskOutWarehouse planTaskOutWarehouse);


        /// <summary>
        /// 倒仓
        /// </summary>
        /// <param name="plantask">计划</param>
        /// <param name="plantaskbatch">计划批次</param>
        /// <param name="plantaskOutHouseList">计划出库列表</param>
        /// <param name="plantaskInHouseList">计划入库列表</param>
        /// <param name="planSiteList">计划磅秤列表</param>
        /// <param name="planvehicleList">计划内部车列表</param>
        /// <param name="workplaceList">计划工作地点列表</param>
        /// <param name="OrgDepCode">组织机构号</param>
        void DoInOutWarehouse(PlanTask plantask, PlanTaskBatch plantaskbatch, List<PlanTaskOutWarehouse> plantaskOutHouseList, List<PlanTaskInWarehouse> plantaskInHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> workplaceList, string OrgDepCode);

        void RefreshData();

        void UpdatePlanTaskUnitOfWordWithoutSave(PlanTask plantask, List<PlanTaskInWarehouse> plantaskInHouseList, List<PlanTaskOutWarehouse> plantaskoutHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> workplaceList, string OrgDepCode, bool isRun);
   
    }
}
