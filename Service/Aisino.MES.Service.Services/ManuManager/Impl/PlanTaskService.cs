using System.Collections.Generic;
using System.Linq;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Service.SysManager;
using Aisino.MES.Service.WarehouseManager;
using Aisino.MES.Service.ManuManager;
using System;
using Aisino.MES.DAL.Repository.Helpers;
using Aisino.MES.Service.BaseManager;
using Aisino.MES.Service.StoreProcedures;
using Aisino.MES.Model.NewModels;


namespace Aisino.MES.Service.ManuManager.Impl
{
    public class PlanTaskService : IPlanTaskService
    {
        private Repository<PlanTask> _planTaskDal;
        private Repository<PlanTaskDetail> _planTaskDetailDal;
        private Repository<PlanTaskType> _planTaskTypeDal;
        private Repository<PlanTaskStepStatu> _planTaskStepStatuDal;
        private UnitOfWork _unitOfWork;
        private ISysBillNoService _sysBillNoService;
        private IWarehouseAmountService _warehouseAmountService;
        private IPlanTaskBatchService _planTaskBatchService;
        private IRFIDTagIssueService _rfidTagIssueService;
        private IWarehouseBaseService _warehouseBaseService;
        private IWarehouseStoreInfoService _warehouseStoreInfoService;
        SPGetSysDateTimeService _sPGetSysDateTimeService;
        private IPlanTaskBatchStepStatuService _planTaskBatchStepStatuService;



        public PlanTaskService(Repository<PlanTask> planTaskDal,
                                Repository<PlanTaskDetail> planTaskDetailDal,
                                Repository<PlanTaskType> planTaskTypeDal,
                                Repository<PlanTaskStepStatu> planTaskStepStatuDal,
                                UnitOfWork unitOfWork,
                                ISysBillNoService sysBillNoService,
                                IWarehouseAmountService warehouseAmountService,
                                IPlanTaskBatchService planTaskBatchService,
                                IRFIDTagIssueService rfidTagIssueService,
                                IWarehouseBaseService warehouseBaseService,
                                IWarehouseStoreInfoService warehouseStoreInfoService,
                                 SPGetSysDateTimeService sPGetSysDateTimeService,
                                IPlanTaskBatchStepStatuService planTaskBatchStepStatuService)
        {
            _planTaskDal = planTaskDal;
            _planTaskDetailDal = planTaskDetailDal;
            _planTaskTypeDal = planTaskTypeDal;
            _planTaskStepStatuDal = planTaskStepStatuDal;

            _unitOfWork = unitOfWork;
            _sysBillNoService = sysBillNoService;
            _warehouseAmountService = warehouseAmountService;
            _planTaskBatchService = planTaskBatchService;

            _rfidTagIssueService = rfidTagIssueService;
            _warehouseBaseService = warehouseBaseService;
            _warehouseStoreInfoService = warehouseStoreInfoService;

            _sPGetSysDateTimeService = sPGetSysDateTimeService;
            _planTaskBatchStepStatuService = planTaskBatchStepStatuService;
        }

        #region 生产计划
        /// <summary>
        /// 根据生产计划状态来获取生产计划
        /// </summary>
        /// <param name="plan_status">plan_status=0, 所有未下达的生产计划；plan_status!=0, 某一状态的生产计划 </param>
        /// <returns>生产计划</returns>
        public RepositoryResultList<PlanTask> SelectPlanTaskByStatus(int plan_status, PagingCriteria pagingCriteria)
        {
            //_planTaskDal.GetAll();
            if (plan_status > 0)
            {
                return _planTaskDal.Find(t => t.plan_status == plan_status, pagingCriteria);
            }
            else
            {
                //选择全部，设置条件为真
                return _planTaskDal.Find(t => t.bill_number_id > 0, pagingCriteria);
            }
            //if (plan_status == 0)
            //    return _planTaskDal.ReloadGetAll().Entities.Where(d => d.plan_status <= (int)PlanTaskStatus.确认).ToList();
            //else
            //    return _planTaskDal.ReloadGetAll().Entities.Where(d => d.plan_status == plan_status).ToList();
        }

        public RepositoryResultList<PlanTask> SelectPlanTaskByCondition(int plan_status, int traficType, DateTime planDate, string planType, string planNumber, PagingCriteria pagingCriteria)
        {
            var queryPlanTask = PredicateBuilder.True<PlanTask>();
            if (plan_status > 0)
            {
                //选择了计划状态
                queryPlanTask = queryPlanTask.And(p => p.plan_status == plan_status);
            }
            if (traficType > 0)
            {
                //如果等于0 则表示选择了全部，不用拼接条件
                if (traficType == 1)
                {
                    //如果选择了运输类型，1为外部车
                    queryPlanTask = queryPlanTask.And(p => p.is_outvehicle == true);
                }
                else
                {
                    //非外部车
                    queryPlanTask = queryPlanTask.And(p => p.is_outvehicle == false);
                }
            }
            if (planDate != null && planDate != DateTime.MinValue)
            {
                queryPlanTask = queryPlanTask.And(p => p.create_time.HasValue && p.create_time.Value.ToString("yyyyMMdd") == planDate.ToString("yyyyMMdd"));
            }
            if (planType != "all")
            {
                //不是全部类型
                queryPlanTask = queryPlanTask.And(p => p.plantask_type_code == planType);
            }
            if (planNumber != null && planNumber.Trim() != "")
            {
                queryPlanTask = queryPlanTask.And(p => p.plantask_number == planNumber);
            }

            return _planTaskDal.Find(queryPlanTask, pagingCriteria);
        }

        //根据strWhere来获得未下达的生产计划
        public IEnumerable<PlanTask> SelectPlanTaskBySql(string strWhere)
        {

            string esql = "select * from PlanTask";
            esql += strWhere;

            return _planTaskDal.QueryByESql(esql).Entities;
        }

        public IEnumerable<PlanTask> SelectAllPlanTask()
        {
            return _planTaskDal.GetAll().Entities.ToList();
        }

        /// <summary>
        /// 根据isAssigned获取生产计划
        /// </summary>
        /// <param name="isAssigned">isAssigned=true, 已下达，已排产；isAssigned=false, 未下达 </param>
        /// <returns>生产计划</returns>
        public IEnumerable<PlanTask> SelectAllPlanTask(bool isAssigned)
        {
            IEnumerable<PlanTask> planTasks;
            if (isAssigned)
                planTasks = _planTaskDal.Find(d => d.plan_status >= (int)PlanTaskStatus.下达 && d.plan_status < (int)PlanTaskStatus.完成).Entities;
            else
                //return _planTaskDal.GetAll().Entities.Where(d => d.plan_status < (int)PlanTaskStatus.下达).ToList();
                planTasks = _planTaskDal.GetAll().Entities;
            return planTasks;
        }

        //获得所有需要同步的计划
        public IEnumerable<PlanTask> SelectAllNeedSyncPlanTask()
        {
            return _planTaskDal.Find(pt => pt.plan_status == (int)PlanTaskStatus.下达 && pt.sync_status.Value).Entities;
        }

        public PlanTask GetPlanTask(string planNum)
        {
            var planTask = _planTaskDal.Single(s => s.plantask_number == planNum);
            if (planTask.HasValue)
            {
                return planTask.Entity;
            }
            else
            {
                return null;
            }
        }

        public PlanTask AddPlanTask(PlanTask newPlanTask, int billNoID)
        {
            PlanTask returnPlanTask = null;

            try
            {
                _unitOfWork.Actions.Clear();

                if (billNoID > 0)
                {
                    newPlanTask.plantask_number = _sysBillNoService.GetBillNo(billNoID);
                }

                _unitOfWork.AddAction(newPlanTask, DataActions.Add);
                _unitOfWork.Save();

                returnPlanTask = newPlanTask;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnPlanTask;
        }

        /// <summary>
        /// 随报港单生产提交自动产生相关计划单据
        /// </summary>
        /// <param name="newEnrolment"></param>
        /// <returns></returns>
        public PlanTask AddPlanTaskWithEnrolment(Enrolment newEnrolment, PlanTaskType planTaskType, string OrgDepCode)
        {
            PlanTask newPlanTask = null;
            try
            {
                //分别设置所用的单据编号id,报港单编号，货物类型，计划类型，计划编号，计划执行数量
                //由于是根据报港单产生，所以状态直接为提交。默认不同步到dcs
                newPlanTask = new PlanTask();
                newPlanTask.bill_number_id = planTaskType.bill_number_id;
                newPlanTask.enrolment_number = newEnrolment.enrolment_number;
                newPlanTask.goods_kind = newEnrolment.goods_kind;
                newPlanTask.plantask_type_code = planTaskType.plantask_type_code;
                newPlanTask.plantask_number = _sysBillNoService.GetBillNo(newPlanTask.bill_number_id.Value, OrgDepCode);
                newPlanTask.plan_count = newEnrolment.enrolment_count;
                newPlanTask.plan_status = (int)PlanTaskStatus.确认;
                newPlanTask.sync_status = false;
                newPlanTask.create_time = newEnrolment.enrolment_date;
                if (newEnrolment.EnrolmentType.PlanTaskType.weight_mode != null)
                {
                    newPlanTask.weight_mode = (int)newEnrolment.EnrolmentType.PlanTaskType.weight_mode;
                }
                else
                {
                    newPlanTask.weight_mode = (int)WeightMode.先毛后皮;
                }
                if (newEnrolment != null && newEnrolment.inout_type == (int)InOutType.出库)
                {
                    //出库状态中没有检化验，没有定价过程，所以需要在此赋值
                    if (newEnrolment.Contract != null && newEnrolment.Contract.grain_price != null)
                    {
                        newPlanTask.unit_price = newEnrolment.Contract.grain_price / 1000;
                    }
                }
                newPlanTask.first_tare_con = false;
                newPlanTask.is_outvehicle = newEnrolment.EnrolmentBasicTypeTraffic.isoutcar;
                
                //newPlanTask.is_settlemented = (int)PlanTaskSettlement.未结算;
                newPlanTask.Enrolment = newEnrolment;
                _unitOfWork.AddAction(newPlanTask, DataActions.Add);

                //创建plantaskdetail操作记录
                //操作记录编号为计划编号+三位流水号；操作人员为报港人员；状态为创建；顺序号为1；创建时间为报港单确认时间
                PlanTaskDetail newPlanTaskDetail = new PlanTaskDetail();
                newPlanTaskDetail.plantask_detail_number = newPlanTask.plantask_number + string.Format("{0:D3}", 1);
                newPlanTaskDetail.plantask_number = newPlanTask.plantask_number;
                newPlanTaskDetail.operator_user = newEnrolment.enrolment_register;
                newPlanTaskDetail.operator_type = (int)PlanTaskOPTType.创建;
                newPlanTaskDetail.seq = 1;
                newPlanTaskDetail.start_time = newEnrolment.enrolment_date;
                _unitOfWork.AddAction(newPlanTaskDetail, DataActions.Add);

                BuildPlanTaskStepStatu(newEnrolment, newPlanTask);

                //添加默认批次
                _planTaskBatchService.AddPlanTaskBatchWithPlanTask(newPlanTask, planTaskType);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return newPlanTask;
        }

        public PlanTask UpdatePlanTask(PlanTask updPlanTask)
        {
            PlanTask returnPlanTask = null;
            try
            {
                _planTaskDal.Update(updPlanTask);

                returnPlanTask = updPlanTask;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnPlanTask;
        }

        public PlanTask UpdatePlanTask(PlanTask plantask, List<PlanTaskInWarehouse> plantaskInHouseList, List<PlanTaskOutWarehouse> plantaskOutHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> workplaceList, string OrgDepCode)
        {
            try
            {
                PlanTaskBatch planTaskBatch = plantask.PlanTaskBatches.LastOrDefault();
                //设置仓房
                if (plantaskInHouseList != null && plantaskInHouseList.Count > 0)
                {
                    SetPlanTaskInWarehouse(plantask, plantaskInHouseList);
                }
                if (plantaskOutHouseList != null && plantaskOutHouseList.Count > 0)
                {
                    SetPlanTaskOutWarehouse(plantask, plantaskOutHouseList);
                }
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return plantask;
        }


        //更新计划入库表， 计划出库表和生产计划表
        public PlanTask UpdatePlanTask(PlanTask updPlanTask, bool outWarehouseUpdateFlag, List<PlanTaskOutWarehouse> outWarehouseList, bool inWarehouseUpdateFlag, List<PlanTaskInWarehouse> inWarehouseList)
        {
            PlanTask returnPlanTask = null;
            try
            {

                if (outWarehouseUpdateFlag)
                {

                    //删除原来的仓库
                    //foreach (PlanTaskOutWarehouse planTaskOutWarehouse in updPlanTask.PlanTaskOutWarehouses)
                    //{

                    //    //返回原来的预出库数量
                    //    WarehouseAmount warehouseAmount = _warehouseAmountService.GetWarehouseAmount(planTaskOutWarehouse.invmas_id, planTaskOutWarehouse.warehouse_id);

                    //    if (warehouseAmount != null)
                    //    {
                    //        warehouseAmount.expectation_out_amount -= planTaskOutWarehouse.plan_task_out_amount;
                    //        _warehouseAmountService.UpdateWarehouseAmount_Unitwork(warehouseAmount);
                    //    }

                    //    //删除原来的出库仓库
                    //    _unitOfWork.AddAction(planTaskOutWarehouse, DataActions.Delete);
                    //}

                    ////重新分配仓库
                    //foreach (PlanTaskOutWarehouse planTaskOutWarehouse in outWarehouseList)
                    //{
                    //    //加入的预出库数量
                    //    WarehouseAmount warehouseAmount = _warehouseAmountService.GetWarehouseAmount(planTaskOutWarehouse.invmas_id, planTaskOutWarehouse.warehouse_id);

                    //    if (warehouseAmount != null)
                    //    {
                    //        //更新
                    //        warehouseAmount.expectation_out_amount += planTaskOutWarehouse.plan_task_out_amount;
                    //        _warehouseAmountService.UpdateWarehouseAmount_Unitwork(warehouseAmount);
                    //    }
                    //    else
                    //    {
                    //        //新增
                    //        warehouseAmount = new WarehouseAmount();

                    //        warehouseAmount.invmas_id = planTaskOutWarehouse.invmas_id;
                    //        warehouseAmount.warehouse_id = planTaskOutWarehouse.warehouse_id;
                    //        warehouseAmount.current_amount = 0;
                    //        warehouseAmount.expectation_out_amount = 0;

                    //        warehouseAmount.expectation_out_amount = planTaskOutWarehouse.plan_task_out_amount;

                    //        _warehouseAmountService.AddWarehouseAmount_Unitwork(warehouseAmount);
                    //    }

                    //    //增加出库仓库
                    //    _unitOfWork.AddAction(planTaskOutWarehouse, DataActions.Add);
                    //}
                }


                if (inWarehouseUpdateFlag)
                {
                    //删除原来的入库仓库， 返回原来的预入库数量
                    //foreach (PlanTaskInWarehouse planTaskInWarehouse in updPlanTask.PlanTaskInWarehouses)
                    //{
                    //    //返回原来的预入库数量
                    //    WarehouseAmount warehouseAmount = _warehouseAmountService.GetWarehouseAmount(planTaskInWarehouse.invmas_id, planTaskInWarehouse.warehouse_id);

                    //    if (warehouseAmount != null)
                    //    {
                    //        warehouseAmount.expectation_in_amount -= planTaskInWarehouse.plan_task_in_amount;
                    //        _warehouseAmountService.UpdateWarehouseAmount_Unitwork(warehouseAmount);
                    //    }

                    //    //删除原来的入库仓库
                    //    _unitOfWork.AddAction(planTaskInWarehouse, DataActions.Delete);
                    //}

                    ////重新分配仓库
                    //foreach (PlanTaskInWarehouse planTaskInWarehouse in inWarehouseList)
                    //{
                    //    //加入的预入库数量
                    //    WarehouseAmount warehouseAmount = _warehouseAmountService.GetWarehouseAmount(planTaskInWarehouse.invmas_id, planTaskInWarehouse.warehouse_id);

                    //    if (warehouseAmount != null)
                    //    {
                    //        //更新
                    //        warehouseAmount.expectation_in_amount += planTaskInWarehouse.plan_task_in_amount;
                    //        _warehouseAmountService.UpdateWarehouseAmount_Unitwork(warehouseAmount);
                    //    }
                    //    else
                    //    {
                    //        //新增
                    //        warehouseAmount = new WarehouseAmount();

                    //        warehouseAmount.invmas_id = planTaskInWarehouse.invmas_id;
                    //        warehouseAmount.warehouse_id = planTaskInWarehouse.warehouse_id;
                    //        warehouseAmount.current_amount = 0;
                    //        warehouseAmount.expectation_out_amount = 0;

                    //        warehouseAmount.expectation_in_amount = planTaskInWarehouse.plan_task_in_amount;

                    //        _warehouseAmountService.AddWarehouseAmount_Unitwork(warehouseAmount);
                    //    }

                    //    //增加入库仓库
                    //    _unitOfWork.AddAction(planTaskInWarehouse, DataActions.Add);
                    //}

                }


                _unitOfWork.AddAction(updPlanTask, DataActions.Update);

                _unitOfWork.Save();


                returnPlanTask = updPlanTask;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnPlanTask;
        }


        public PlanTask DeletePlanTask(PlanTask delPlanTask)
        {
            PlanTask planTask = null;
            try
            {
                //删除明细记录
                /*List<BorLineSection> borLineSectionLst = _borLineSectionDal.Find(d => d.bor_line_id == delBorLine.id).Entities.ToList();
                foreach (BorLineSection borLineSection in borLineSectionLst)
                {
                    _unitOfWork.AddAction(borLineSection, DataActions.Delete);
                }*/
                _unitOfWork.Actions.Clear();
                _unitOfWork.AddAction(delPlanTask, DataActions.Delete);

                _unitOfWork.Save();

                planTask = delPlanTask;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return planTask;



        }
        public bool CheckPlanTaskCodeExist(string planTaskNo)
        {
            var planTask = _planTaskDal.Single(d => d.plantask_number == planTaskNo);
            if (planTask.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public PlanTask AddPlanTask_Unitwork(PlanTask newPlanTask)
        {
            PlanTask returnPlanTask = null;
            try
            {
                _unitOfWork.AddAction(newPlanTask, DataActions.Add);
                returnPlanTask = newPlanTask;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnPlanTask;
        }
        #endregion 生产计划

        #region 生产计划类别
        public IEnumerable<PlanTaskType> SelectAllPlanTaskType()
        {
            return _planTaskTypeDal.GetAll().Entities.ToList();
        }


        public PlanTaskType GetPlanTaskType(string planTaskTypeCode)
        {
            var planTaskType = _planTaskTypeDal.Single(s => s.plantask_type_code == planTaskTypeCode);
            if (planTaskType.HasValue)
            {
                return planTaskType.Entity;
            }
            else
            {
                return null;
            }
        }

        public PlanTaskType AddPlanTaskType(PlanTaskType newPlanTaskType)
        {
            PlanTaskType returnPlanTaskType = null;
            try
            {
                _planTaskTypeDal.Add(newPlanTaskType);

                returnPlanTaskType = newPlanTaskType;

            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnPlanTaskType;
        }

        public PlanTaskType UpdatePlanTaskType(PlanTaskType updPlanTaskType)
        {
            PlanTaskType returnPlanTaskType = null;
            try
            {
                _planTaskTypeDal.Update(updPlanTaskType);
                returnPlanTaskType = updPlanTaskType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnPlanTaskType;
        }

        public PlanTaskType DeletePlanTaskType(PlanTaskType delPlanTaskType)
        {
            PlanTaskType returnPlanTaskType = null;
            try
            {
                _planTaskTypeDal.Delete(delPlanTaskType);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnPlanTaskType;
        }

        public void DeletePlanTaskTypeList(List<PlanTaskType> lstDelPlanTaskType)
        {
            try
            {
                _planTaskTypeDal.DeleteAll(lstDelPlanTaskType);
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除计划类别信息失败！", ex);
            }
        }

        public bool CheckPlanTaskTypeCodeExist(string planTaskTypeCode)
        {
            var planTaskType = _planTaskTypeDal.Single(d => d.plantask_type_code == planTaskTypeCode);
            if (planTaskType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckPlanTaskTypeNameExist(string planTaskTypeName)
        {
            var planTaskType = _planTaskTypeDal.Single(d => d.plantask_type_name == planTaskTypeName);
            if (planTaskType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion 生产计划类别

        #region 入库出库
        public void UpdateWarehouseInList(string number, IList<PlanTaskInWarehouse> inWarehouseList)
        {
            try
            {
                //IList<PlanTaskInWarehouse> oldInWarehouseList = GetPlanTask(number).PlanTaskInWarehouses.ToList();

                ////原先没有的，现在要添加
                //if (oldInWarehouseList != null && oldInWarehouseList.Count > 0)
                //{
                //    foreach (PlanTaskInWarehouse inWarehouse in inWarehouseList)
                //    {
                //        if (!oldInWarehouseList.Any(old => old.warehouse_id == inWarehouse.warehouse_id && old.plan_task_id == inWarehouse.plan_task_id))
                //        {
                //            _unitOfWork.AddAction(inWarehouse, DataActions.Add);
                //        }
                //    }
                //}
                //else
                //{
                //    foreach (PlanTaskInWarehouse inWarehouse in inWarehouseList)
                //    {
                //        _unitOfWork.AddAction(inWarehouse, DataActions.Add);
                //    }
                //}

                ////原先有的，现在没了，要删除
                //if (oldInWarehouseList != null)
                //{
                //    foreach (PlanTaskInWarehouse warehouse in oldInWarehouseList.Where(old => !inWarehouseList.Any(list => list.plan_task_id == old.plan_task_id && list.warehouse_id == old.warehouse_id)))
                //    {
                //        _unitOfWork.AddAction(warehouse, DataActions.Delete);
                //    }
                //}

                //_unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void UpdateWarehouseOutList(string number, IList<PlanTaskOutWarehouse> outWarehouseList)
        {
            try
            {
                //IList<PlanTaskInWarehouse> oldInWarehouseList = GetPlanTask(id).PlanTaskInWarehouses.ToList();

                ////原先没有的，现在要添加
                //if (oldInWarehouseList != null && oldInWarehouseList.Count > 0)
                //{
                //    foreach (PlanTaskOutWarehouse outWarehouse in outWarehouseList)
                //    {
                //        if (!oldInWarehouseList.Any(old => old.warehouse_id == outWarehouse.warehouse_id && old.plan_task_id == outWarehouse.plan_task_id))
                //        {
                //            _unitOfWork.AddAction(outWarehouse, DataActions.Add);
                //        }
                //    }
                //}
                //else
                //{
                //    foreach (PlanTaskOutWarehouse outWarehouse in outWarehouseList)
                //    {
                //        _unitOfWork.AddAction(outWarehouse, DataActions.Add);
                //    }
                //}

                ////原先有的，现在没了，要删除
                //if (oldInWarehouseList != null)
                //{
                //    foreach (PlanTaskInWarehouse warehouse in oldInWarehouseList.Where(old => !outWarehouseList.Any(list => list.plan_task_id == old.plan_task_id && list.warehouse_id == old.warehouse_id)))
                //    {
                //        _unitOfWork.AddAction(warehouse, DataActions.Delete);
                //    }
                //}

                //_unitOfWork.Save();

            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        #endregion 入库出库

        #region 称重上传
        /// <summary>
        /// 
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
        /// <param name="strErrorMessage">错误信息</param>
        /// <returns></returns>
        public bool WeightUpload(string vehicleRFIDTag, string scaleBillNumber, string weighRecordNumber, int weighType,
            string weightCount, DateTime weightTime, int goodsID, string workNumber, int wareHouseID, int outWareHouseID,
            ref string strErrorMessage, ref string strTime, QuantityRecordDetail quantityDetail)
        {
            strTime += "步骤二10：" + DateTime.Now.ToString() + ";                                   ";
            PlanTaskBatch planTaskBatch = _planTaskBatchService.GetSinglePlanTaskBatchWithGenerateid(int.Parse(workNumber));
            strTime += "步骤二11：" + DateTime.Now.ToString() + ";                                   ";
            if (planTaskBatch == null)
            {
                strErrorMessage = "未找到计划批次信息";
                return true;
            }

            PlanTask plantask = planTaskBatch.PlanTask;
            if (plantask == null)
            {
                strErrorMessage = "未找到计划信息";
                return true;
            }

            try
            {
                Warehouse warehouse = _warehouseBaseService.GetWarehouseByGenerateid(wareHouseID);
                Warehouse warehouseOut = _warehouseBaseService.GetWarehouseByGenerateid(outWareHouseID);
                strTime += "步骤二13：" + DateTime.Now.ToString() + ";                                   ";

                _unitOfWork.Actions.Clear();
                string vehicleid = null;
                InnerVehicle innerVehicle = _rfidTagIssueService.FindVehicleRFIDTagIssueByCode(vehicleRFIDTag);
                strTime += "步骤二14：" + DateTime.Now.ToString() + ";                                   ";

                if (innerVehicle != null)
                {
                    vehicleid = innerVehicle.inner_vehicle_id;
                }
                else
                {
                    vehicleid = null;
                }

                PlanTaskBatchDetail planTaskBatchDetail = null;

                planTaskBatchDetail = _planTaskBatchService.GetPlanTaskBatchDetailByScaleBillNumber(scaleBillNumber);
                strTime += "步骤二15：" + DateTime.Now.ToString() + ";                                   ";

                //如果Detail为空或者净重时间不为空，则新增一条Detail记录
                PlanTaskBatchDetail planTaskBatchDetailTemp = null;

                //if (!(planTaskBatchDetail != null && planTaskBatchDetail.weight_time == null))
                if (planTaskBatchDetail == null || planTaskBatchDetail.plantask_batch_detail_number == null)
                {
                    if (plantask.Enrolment == null)
                    {
                        planTaskBatchDetailTemp = this.WeightUploadAddPlanTaskBatchDetail(planTaskBatch, warehouse.warehouse_id, (int)InOutType.入库, weighType,
                            weightCount, weightTime, weighRecordNumber, scaleBillNumber, vehicleid, warehouseOut.warehouse_id);
                    }
                    else
                    {
                        planTaskBatchDetailTemp = this.WeightUploadAddPlanTaskBatchDetail(planTaskBatch, warehouse.warehouse_id, plantask.Enrolment.inout_type, weighType,
                            weightCount, weightTime, weighRecordNumber, scaleBillNumber, vehicleid, string.Empty);
                    }
                }
                else
                {
                    if (planTaskBatchDetail.weight_number == weighRecordNumber)
                    {
                        return true;
                    }
                    if (plantask.Enrolment == null)
                    {
                        planTaskBatchDetailTemp = this.WeightUploadUpdatePlanTaskBatchDetail(planTaskBatchDetail, warehouse.warehouse_id, (int)InOutType.入库, weighType,
                            weightCount, weightTime, weighRecordNumber, scaleBillNumber, vehicleid, warehouseOut.warehouse_id);
                    }
                    else
                    {
                        planTaskBatchDetailTemp = this.WeightUploadUpdatePlanTaskBatchDetail(planTaskBatchDetail, warehouse.warehouse_id, plantask.Enrolment.inout_type, weighType,
                            weightCount, weightTime, weighRecordNumber, scaleBillNumber, vehicleid, string.Empty);
                    }
                }
                QuantityRecordHead quantityHead = quantityDetail.QuantityRecordHead;
                quantityHead.plantask_batch_detail_number = planTaskBatchDetailTemp.plantask_batch_detail_number;

                quantityDetail.is_error = false;
                quantityDetail.is_sync = true;

                _unitOfWork.AddAction(quantityHead, DAL.Enums.DataActions.Update);
                _unitOfWork.AddAction(quantityDetail, DAL.Enums.DataActions.Update);

                if (weighType == 1)
                {
                    AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.MAO_ZHONG);
                }
                else
                {
                    AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_ZHONG);
                }
                
                if (planTaskBatchDetailTemp.weight_time != null)
                {
                    strTime += "步骤二16：" + DateTime.Now.ToString() + ";                                   ";
                    //更新批次皮重,毛重,净重……
                    WeightUploadUpdatePlanTaskBatch(planTaskBatch, planTaskBatchDetailTemp);

                    //更新计划皮重，毛重，实际重量……
                    WeightUploadUpdatePlanTask(plantask, planTaskBatchDetailTemp);
                    strTime += "步骤二17：" + DateTime.Now.ToString() + ";                                   ";
                }
                

                //外部车如果产生净重，则把该计划置为完成状态
                //判断的时候需要增加是否通过流量秤计量，需要流量秤计量，则不完成该计划
                if (planTaskBatch.bor_weight == null || (planTaskBatch.bor_weight.HasValue && planTaskBatch.bor_weight.Value == false))
                {
                    if (innerVehicle == null)
                    {
                        if (planTaskBatchDetailTemp.inout_type == 1 && planTaskBatchDetailTemp.gross_time != null && planTaskBatchDetailTemp.tare_time != null && plantask.series_work == false
                            //&& (plantask.series_work == false || planTaskBatchDetailTemp.duty_confirm == 2 || planTaskBatchDetailTemp.duty_confirm == 3))
                            && (planTaskBatchDetailTemp.duty_confirm == null || planTaskBatchDetailTemp.duty_confirm == 0 || planTaskBatchDetailTemp.duty_confirm == 1 || planTaskBatchDetailTemp.duty_confirm == 3 || planTaskBatchDetailTemp.duty_confirm == 6))
                        {
                            FinishPlanTask(plantask, planTaskBatchDetailTemp);
                            FinishPlanTaskBatch(planTaskBatch, planTaskBatchDetailTemp, false);
                            _warehouseStoreInfoService.UpDateBusinessWeight(planTaskBatchDetailTemp, planTaskBatchDetailTemp.weight.Value);
                            _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                            _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                            AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_WC);
                        }
                        else if (planTaskBatchDetailTemp.inout_type == 1 && planTaskBatchDetailTemp.gross_time != null && planTaskBatchDetailTemp.tare_time != null && plantask.series_work == true
                            && (planTaskBatchDetailTemp.duty_confirm == null || planTaskBatchDetailTemp.duty_confirm == 0 || planTaskBatchDetailTemp.duty_confirm == 1 || planTaskBatchDetailTemp.duty_confirm == 3 || planTaskBatchDetailTemp.duty_confirm == 6))
                        {
                            _warehouseStoreInfoService.UpDateBusinessWeight(planTaskBatchDetailTemp, planTaskBatchDetailTemp.weight.Value);
                            _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                            _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                        }    
                        else if (planTaskBatchDetailTemp.inout_type == 2 && planTaskBatchDetailTemp.gross_time != null && planTaskBatchDetailTemp.tare_time != null && plantask.series_work == false
                            && (planTaskBatchDetailTemp.duty_confirm == null || planTaskBatchDetailTemp.duty_confirm == 0 || planTaskBatchDetailTemp.duty_confirm == 1 || planTaskBatchDetailTemp.duty_confirm == 3 || planTaskBatchDetailTemp.duty_confirm == 6))
                        {
                            if (plantask.Enrolment.CShipingCertificate != null)
                            {
                                //出库有凭证
                                decimal dShipCount = plantask.Enrolment.CShipingCertificate.shiping_count.Value;
                                decimal dFinishedCount = plantask.Enrolment.CShipingCertificate.finished_count.Value;
                                if (planTaskBatchDetail.weight <= dShipCount - dFinishedCount)
                                {
                                    FinishPlanTask(plantask, planTaskBatchDetailTemp);
                                    planTaskBatch.final_statement_weight = 0;
                                    FinishPlanTaskBatch(planTaskBatch, planTaskBatchDetailTemp, true);
                                    _warehouseStoreInfoService.UpDateBusinessWeight(planTaskBatchDetailTemp, planTaskBatchDetailTemp.weight.Value);
                                    _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                                    _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                                    AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_WC);
                                    AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_JS);
                                    UpdatePlanTaskStepStatuJieSuan(plantask);
                                }
                                else
                                {
                                    FinishPlanTask(plantask, planTaskBatchDetailTemp);
                                    FinishPlanTaskBatch(planTaskBatch, planTaskBatchDetailTemp, false);
                                    _warehouseStoreInfoService.UpDateBusinessWeight(planTaskBatchDetailTemp, planTaskBatchDetailTemp.weight.Value);
                                    _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                                    _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                                    AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_WC);
                                }
                            }
                            else
                            {
                                strTime += "步骤二18：" + DateTime.Now.ToString() + ";                                   ";
                                FinishPlanTask(plantask, planTaskBatchDetailTemp);
                                strTime += "步骤二19：" + DateTime.Now.ToString() + ";                                   ";
                                planTaskBatch.final_statement_weight = planTaskBatchDetailTemp.weight;
                                FinishPlanTaskBatch(planTaskBatch, planTaskBatchDetailTemp, true);
                                strTime += "步骤二20：" + DateTime.Now.ToString() + ";                                   ";
                                _warehouseStoreInfoService.UpDateBusinessWeight(planTaskBatchDetailTemp, planTaskBatchDetailTemp.weight.Value);
                                strTime += "步骤二21：" + DateTime.Now.ToString() + ";                                   ";
                                _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                                strTime += "步骤二22：" + DateTime.Now.ToString() + ";                                   ";
                                _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                                strTime += "步骤二23：" + DateTime.Now.ToString() + ";                                   ";
                                AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_WC);
                                strTime += "步骤二24：" + DateTime.Now.ToString() + ";                                   ";
                                AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_JS);
                                strTime += "步骤二25：" + DateTime.Now.ToString() + ";                                   ";
                                UpdatePlanTaskStepStatuJieSuan(plantask);
                                strTime += "步骤二26：" + DateTime.Now.ToString() + ";                                   ";
                            }
                        }
                        else if (planTaskBatchDetailTemp.inout_type == 2 && planTaskBatchDetailTemp.gross_time != null && planTaskBatchDetailTemp.tare_time != null && plantask.series_work == true
                           && (planTaskBatchDetailTemp.duty_confirm == null || planTaskBatchDetailTemp.duty_confirm == 0 || planTaskBatchDetailTemp.duty_confirm == 1 || planTaskBatchDetailTemp.duty_confirm == 3 || planTaskBatchDetailTemp.duty_confirm == 6))
                        {
                            if (plantask.Enrolment.CShipingCertificate != null)
                            {
                                //出库有凭证
                                decimal dShipCount = plantask.Enrolment.CShipingCertificate.shiping_count.Value;
                                decimal dFinishedCount = plantask.Enrolment.CShipingCertificate.finished_count.Value;
                                if (planTaskBatchDetail.weight <= dShipCount - dFinishedCount)
                                {
                                    planTaskBatch.final_statement_weight = 0;
                                    _planTaskBatchService.UpPlantaskBatchWithUnit(planTaskBatch);
                                    _warehouseStoreInfoService.UpDateBusinessWeight(planTaskBatchDetailTemp, planTaskBatchDetailTemp.weight.Value);
                                    _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                                    _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                                    AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_WC);
                                    AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_JS);
                                    UpdatePlanTaskStepStatuJieSuan(plantask);
                                }
                                else
                                {
                                    if (dShipCount - dFinishedCount <= 0)
                                    {
                                        planTaskBatch.final_statement_weight = planTaskBatchDetail.weight;
                                    }
                                    else
                                    {
                                        planTaskBatch.final_statement_weight = planTaskBatchDetail.weight - (dShipCount - dFinishedCount);
                                    }
                                    _planTaskBatchService.UpPlantaskBatchWithUnit(planTaskBatch);
                                    _warehouseStoreInfoService.UpDateBusinessWeight(planTaskBatchDetailTemp, planTaskBatchDetailTemp.weight.Value);
                                    _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                                    _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                                    AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_WC);
                                }
                            }
                            else
                            {
                                //FinishPlanTask(plantask, planTaskBatchDetailTemp);
                                planTaskBatch.final_statement_weight = planTaskBatchDetailTemp.weight;
                                _planTaskBatchService.UpPlantaskBatchWithUnit(planTaskBatch);
                                //FinishPlanTaskBatch(planTaskBatch, planTaskBatchDetailTemp, true);
                                _warehouseStoreInfoService.UpDateBusinessWeight(planTaskBatchDetailTemp, planTaskBatchDetailTemp.weight.Value);
                                _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                                _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                                AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_WC);
                                AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_JS);
                                UpdatePlanTaskStepStatuJieSuan(plantask);
                            }
                        }
                    }
                    else
                    {
                        decimal dFinalStatementWeight = 0;
                        if (planTaskBatchDetailTemp.inout_type == 2 && plantask.Enrolment.CShipingCertificate != null)
                        {
                            decimal dShipCount = plantask.Enrolment.CShipingCertificate.shiping_count.Value;
                            decimal dFinishedCount = plantask.Enrolment.CShipingCertificate.finished_count.Value;

                            if (planTaskBatchDetailTemp.weight > (dShipCount - dFinishedCount))
                            {
                                dFinalStatementWeight = planTaskBatchDetailTemp.weight.Value - (dShipCount - dFinishedCount);
                            }
                        }

                        if (planTaskBatchDetailTemp.gross_time != null && planTaskBatchDetailTemp.tare_time != null
                            && (planTaskBatchDetailTemp.duty_confirm == 3))
                        {
                            FinishPlanTask(plantask, planTaskBatchDetailTemp);
                            if (planTaskBatchDetailTemp.inout_type == 2 && plantask.Enrolment.CShipingCertificate != null)
                            {
                                planTaskBatch.final_statement_weight = dFinalStatementWeight;
                            }
                            FinishPlanTaskBatch(planTaskBatch, planTaskBatchDetailTemp, false);
                            //_warehouseStoreInfoService.UpDateBusinessWeight(plantask);
                            _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                            _warehouseStoreInfoService.UdDateContractWeight(planTaskBatchDetailTemp);
                            _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                            AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_WC);
                            if (planTaskBatchDetailTemp.inout_type == 2)
                            {
                                AddPlanTaskBatchStepStatusWithUnit(planTaskBatch.plantask_number, planTaskBatch.plantask_batch_number, PlanTaskBatchStepCode.PI_CI_JS);
                                UpdatePlanTaskStepStatuJieSuan(plantask);
                            }
                        }
                        else if (planTaskBatchDetailTemp.gross_time != null && planTaskBatchDetailTemp.tare_time != null)
                        {
                            if (planTaskBatchDetailTemp.inout_type == 2)
                            {
                                planTaskBatch.final_statement_weight = dFinalStatementWeight;
                                _planTaskBatchService.UpPlantaskBatchWithUnit(planTaskBatch);
                            }
                            _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                            _warehouseStoreInfoService.UdDateContractWeight(planTaskBatchDetailTemp);
                            _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                        }
                    }

                    ////如果Detail产生净重
                    ////新增WarehouseInOutRecordDetail 
                    ////更新WarehouseInOutRecord
                    ////更新WarehouseStoreInfo(如果没有该仓库有效仓储,新增一条记录)
                    //if (planTaskBatchDetailTemp != null && planTaskBatchDetailTemp.weight_time != null)
                    //{
                    //    _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                    //    _warehouseStoreInfoService.UpDateCertificate(planTaskBatchDetailTemp);
                    //}
                }

                if (planTaskBatchDetailTemp.duty_confirm != null && planTaskBatchDetailTemp.duty_confirm == 3)
                {
                    if (planTaskBatchDetailTemp.weight_time != null)
                    {
                        _planTaskBatchService.AddPlanTaskBatchWithPlanTaskBatch(planTaskBatch);
                    }
                }
                if (innerVehicle != null)
                {
                    if (warehouse != null)
                    {
                        UpdatePlanTaskInWarehouseWithVehicle(planTaskBatch, vehicleid, warehouse.warehouse_id);
                    }
                    if (warehouseOut != null)
                    {
                        UpdatePlanTaskOutWarehouseWithVehicle(planTaskBatch, vehicleid, warehouseOut.warehouse_id);
                    }
                }
                strTime += "步骤二27：" + DateTime.Now.ToString() + ";                                   ";

                _unitOfWork.Save();
                strTime += "步骤二28：" + DateTime.Now.ToString() + ";                                   ";
            }
            catch (RepositoryException ex)
            {
                strErrorMessage = ex.Message;
                throw ex;
            }
            if (strErrorMessage.Trim().Length > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 上传净重
        /// </summary>
        /// <param name="netWeight"></param>
        /// <param name="workNumber">(计划批次的generate_id)</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool NetWeightUpload(int netWeight, string workNumber, ref string strErrorMessage)
        {
            PlanTaskBatch planTaskBatch = _planTaskBatchService.GetSinglePlanTaskBatchWithGenerateid(int.Parse(workNumber));
            if (planTaskBatch == null)
            {
                strErrorMessage = "未找到计划批次信息";
                return false;
            }
            Enrolment enrolment = planTaskBatch.PlanTask.Enrolment;
            string strWareHouse = string.Empty;
            if (enrolment.inout_type == (int)InOutType.入库)
            {
                strWareHouse = planTaskBatch.PlanTaskInWarehouses.Last().warehouse_id;
            }
            else
            {
                strWareHouse = planTaskBatch.PlanTaskOutWarehouses.Last().warehouse_id;
            }

            try
            {
                _unitOfWork.Actions.Clear();

                //新增一条Detail记录
                PlanTaskBatchDetail planTaskBatchDetailTemp = this.WeightUploadAddPlanTaskBatchDetailNetWeight(planTaskBatch, strWareHouse, enrolment.inout_type,
                        netWeight.ToString(), _sPGetSysDateTimeService.GetSysDateTime());

                //更新批次皮重,毛重,净重……
                WeightUploadUpdatePlanTaskBatchNetWeight(planTaskBatch, planTaskBatchDetailTemp);

                //更新计划皮重，毛重，实际重量……
                WeightUploadUpdatePlanTaskNetWeight(planTaskBatch.PlanTask, planTaskBatchDetailTemp);

                //如果Detail产生净重
                //新增WarehouseInOutRecordDetail 
                //更新WarehouseInOutRecord
                //更新WarehouseStoreInfo(如果没有该仓库有效仓储,新增一条记录)
                if (planTaskBatchDetailTemp != null && planTaskBatchDetailTemp.weight_time != null)
                {
                    _warehouseStoreInfoService.RefreshData();
                    _warehouseStoreInfoService.UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(planTaskBatchDetailTemp);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                strErrorMessage = ex.Message;
                throw ex;
            }
            if (strErrorMessage.Trim().Length > 0)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 取消磅单
        /// <summary>
        /// 根据磅单号回滚榜单
        /// </summary>
        /// <param name="ScaleBillNumber">磅单号</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool CancelVehicleScaleBill(string ScaleBillNumber, ref string strErrorMessage)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                //
                PlanTaskBatchDetail planTaskBatchDetail = _planTaskBatchService.GetPlanTaskBatchDetailByScaleBillNumber(ScaleBillNumber);
                if (planTaskBatchDetail == null)
                {
                    strErrorMessage = "根据磅单号未找到称重详细记录";
                    return false;
                }

                if (planTaskBatchDetail.weight_time != null)
                {
                    //回滚仓房信息
                    _warehouseStoreInfoService.CancelWarehouseStoreInfoWithScaleBill(planTaskBatchDetail);
                    _warehouseStoreInfoService.CancelDateCertificate(planTaskBatchDetail);

                    //回滚计划
                    PlanTask planTask = CancelScalePlanTask(planTaskBatchDetail);
                    if (planTask == null)
                    {
                        strErrorMessage = "根据磅单号未找到计划信息";
                        return false;
                    }
                    UnFinishPlanTask(planTask, planTaskBatchDetail);

                    //回滚计划批次
                    PlanTaskBatch planTaskBatch = CancelScalePlanTaskBatch(planTaskBatchDetail);
                    if (planTaskBatch == null)
                    {
                        strErrorMessage = "根据磅单号未找到计划批次信息";
                        return false;
                    }
                    UnFinishPlanTaskBatch(planTaskBatch, planTaskBatchDetail);
                }

                //作废称重详细信息
                CancelScalePlanTaskBatchDetail(ScaleBillNumber, planTaskBatchDetail);

                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                strErrorMessage = ex.Message;
                throw ex;
            }
            if (strErrorMessage.Trim().Length > 0)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 取消称重
        /// <summary>
        /// 取消称重
        /// </summary>
        /// <param name="WeighRecordNumber">称重单号</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool CancelVehicleWeighBill(string WeighRecordNumber, ref string strErrorMessage)
        {
            try
            {
                _unitOfWork.Actions.Clear();

                PlanTaskBatchDetail planTaskBatchDetail = _planTaskBatchService.GetPlanTaskBatchDetailByWeightNumber(WeighRecordNumber);

                if (planTaskBatchDetail == null)
                {
                    strErrorMessage = "根据磅单号未找到称重详细记录";
                    return false;
                }

                //回滚计划批次
                PlanTaskBatch planTaskBatch = CancelScalePlanTaskBatchByWeighRecordNumber(planTaskBatchDetail);
                if (planTaskBatch == null)
                {
                    strErrorMessage = "根据磅单号未找到计划批次信息";
                    return false;
                }
                UnFinishPlanTaskBatch(planTaskBatch, planTaskBatchDetail);

                //回滚计划
                PlanTask planTask = CancelScalePlanTaskByWeighRecordNumber(planTaskBatchDetail);
                if (planTask == null)
                {
                    strErrorMessage = "根据磅单号未找到计划信息";
                    return false;
                }
                UnFinishPlanTask(planTask, planTaskBatchDetail);

                //回滚仓房信息
                _warehouseStoreInfoService.CancelWarehouseStoreInfoWithWeightNumber(planTaskBatchDetail);

                //作废称重详细信息
                planTaskBatchDetail = CancelScalePlanTaskBatchDetaiByWeighRecordNumber(WeighRecordNumber);

                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                strErrorMessage = ex.Message;
                throw ex;
            }
            if (strErrorMessage.Trim().Length > 0)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region 私有方法
        private void BuildPlanTaskStepStatu(Enrolment enrolment, PlanTask plantask)
        {
            for (int i = 0; i < 5; i++)
            {
                PlanTaskStepStatu ptss = new PlanTaskStepStatu();
                ptss.plantask_number = plantask.plantask_number;
                switch (i)
                {
                    case 0:
                        ptss.step_code = PlanTaskStepCode.BAO_GANG_TJ;
                        ptss.status = true;
                        ptss.operate_time = enrolment.enrolment_date;
                        break;
                    case 1:
                        ptss.step_code = PlanTaskStepCode.BAO_GANG_QR;
                        ptss.status = (enrolment.status == (int)EnrolmentStatue.确认);
                        ptss.operate_time = enrolment.confirm_date;
                        break;
                    case 2:
                        ptss.step_code = PlanTaskStepCode.JI_HUA_BZ;
                        ptss.status = false;
                        break;
                    case 3:
                        ptss.step_code = PlanTaskStepCode.JI_HUA_ZX;
                        ptss.status = false;
                        break;
                    case 4:
                        ptss.step_code = PlanTaskStepCode.JIE_SUAN;
                        ptss.status = false;
                        break;
                    default:
                        break;
                }
                _unitOfWork.AddAction(ptss, DataActions.Add);
            }
        }

        #region 上传称重信息
        /// <summary>
        /// 
        /// </summary>
        /// <param name="planTaskBatch"></param>
        /// <param name="WeighType"></param>
        /// <param name="PlanTaskBatchDetail"></param>
        /// <param name="WeightCount"></param>
        private void WeightUploadUpdatePlanTask(PlanTask planTask, PlanTaskBatchDetail planTaskBatchDetail)
        {

            if (planTask.tare_weight == null)
            {
                planTask.tare_weight = planTaskBatchDetail.tare_weight;
            }
            else
            {
                planTask.tare_weight += planTaskBatchDetail.tare_weight;
            }

            if (planTask.gross_weight == null)
            {
                planTask.gross_weight = planTaskBatchDetail.gross_weight;
            }
            else
            {
                planTask.gross_weight += planTaskBatchDetail.gross_weight;
            }


            if (planTaskBatchDetail.weight_time != null)
            {
                if (planTask.plan_weight == null)
                {
                    planTask.plan_weight = planTaskBatchDetail.weight;
                }
                else
                {
                    planTask.plan_weight += planTaskBatchDetail.weight;
                }
            }
            _unitOfWork.AddAction(planTask, DataActions.Update);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="planTaskBatch"></param>
        /// <param name="WeighType"></param>
        /// <param name="PlanTaskBatchDetail"></param>
        /// <param name="WeightCount"></param>
        private void WeightUploadUpdatePlanTaskNetWeight(PlanTask planTask, PlanTaskBatchDetail planTaskBatchDetail)
        {
            if (planTask.tare_weight == null)
            {
                planTask.tare_weight = planTaskBatchDetail.tare_weight;
            }
            else
            {
                planTask.tare_weight += planTaskBatchDetail.tare_weight;
            }

            if (planTask.gross_weight == null)
            {
                planTask.gross_weight = planTaskBatchDetail.gross_weight;
            }
            else
            {
                planTask.gross_weight += planTaskBatchDetail.gross_weight;
            }

            if (planTask.plan_weight == null)
            {
                planTask.plan_weight = planTaskBatchDetail.weight;
            }
            else
            {
                planTask.plan_weight += planTaskBatchDetail.weight;
            }
            _unitOfWork.AddAction(planTask, DataActions.Update);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="planTaskBatch"></param>
        /// <param name="WeighType"></param>
        /// <param name="WeightCount"></param>
        private void WeightUploadUpdatePlanTaskBatch(PlanTaskBatch planTaskBatch, PlanTaskBatchDetail planTaskBatchDetail)
        {

            if (planTaskBatch.tare_weight == null)
            {
                planTaskBatch.tare_weight = planTaskBatchDetail.tare_weight;
            }
            else
            {
                planTaskBatch.tare_weight += planTaskBatchDetail.tare_weight;
            }

            if (planTaskBatch.gross_weight == null)
            {
                planTaskBatch.gross_weight = planTaskBatchDetail.gross_weight;
            }
            else
            {
                planTaskBatch.gross_weight += planTaskBatchDetail.gross_weight;
            }

            if (planTaskBatch.batch_count == null)
            {
                planTaskBatch.batch_count = planTaskBatchDetail.weight;
            }
            else
            {
                planTaskBatch.batch_count += planTaskBatchDetail.weight;
            }

            _unitOfWork.AddAction(planTaskBatch, DataActions.Update);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="planTaskBatch"></param>
        /// <param name="WeighType"></param>
        /// <param name="WeightCount"></param>
        private void WeightUploadUpdatePlanTaskBatchNetWeight(PlanTaskBatch planTaskBatch, PlanTaskBatchDetail planTaskBatchDetail)
        {
            if (planTaskBatch.tare_weight == null)
            {
                planTaskBatch.tare_weight = planTaskBatchDetail.tare_weight;
            }
            else
            {
                planTaskBatch.tare_weight += planTaskBatchDetail.tare_weight;
            }

            if (planTaskBatch.gross_weight == null)
            {
                planTaskBatch.gross_weight = planTaskBatchDetail.gross_weight;
            }
            else
            {
                planTaskBatch.gross_weight += planTaskBatchDetail.gross_weight;
            }

            if (planTaskBatch.batch_count == null)
            {
                planTaskBatch.batch_count = planTaskBatchDetail.weight;
            }
            else
            {
                planTaskBatch.batch_count += planTaskBatchDetail.weight;
            }
            _unitOfWork.AddAction(planTaskBatch, DataActions.Update);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        /// <param name="WareHouseID"></param>
        /// <param name="inouttype"></param>
        /// <param name="WeighType"></param>
        /// <param name="WeightCount"></param>
        /// <param name="WeightTime"></param>
        /// <param name="WeighRecordNumber"></param>
        /// <param name="ScaleBillNumber"></param>
        private PlanTaskBatchDetail WeightUploadUpdatePlanTaskBatchDetail(PlanTaskBatchDetail planTaskBatchDetail, string WareHouseID, int inouttype,
                int WeighType, string WeightCount, DateTime WeightTime, string WeighRecordNumber, string ScaleBillNumber, string vehicleid, string outWareHouseID)
        {
           
            planTaskBatchDetail.warehouse_id = WareHouseID;

            if (outWareHouseID.Trim().Length > 0)
            {
                planTaskBatchDetail.outwarehouse = outWareHouseID;
            }

            //称重类型 0皮重，1毛重
            if (WeighType == 0)
            {
                planTaskBatchDetail.tare_weight = Decimal.Parse(WeightCount);
                planTaskBatchDetail.tare_time = WeightTime;
                planTaskBatchDetail.isgross = false;
            }
            else
            {
                planTaskBatchDetail.gross_weight = Decimal.Parse(WeightCount);
                planTaskBatchDetail.gross_time = WeightTime;
                planTaskBatchDetail.isgross = true;
            }
            if (planTaskBatchDetail.gross_weight != null && planTaskBatchDetail.tare_weight != null)
            {
                Decimal weight = planTaskBatchDetail.gross_weight.Value - planTaskBatchDetail.tare_weight.Value;
                planTaskBatchDetail.weight = weight;
                planTaskBatchDetail.weight_time = WeightTime;
            }

            //磅单编号
            planTaskBatchDetail.weight_number = WeighRecordNumber;
            //车辆称重作业地磅系统采集记录编号
            planTaskBatchDetail.scale_number = ScaleBillNumber;
            //单据状态：0 废除； 1 完成
            planTaskBatchDetail.bill_status = 1;
            //外部车辆为空，内部车辆流程暂时不考虑
            if (vehicleid != null)
            {
                planTaskBatchDetail.vehicle_id = vehicleid;
            }
            else
            {
                planTaskBatchDetail.vehicle_id = null;
            }

            _unitOfWork.AddAction(planTaskBatchDetail, DataActions.Update);
            return planTaskBatchDetail;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="planTaskBatch"></param>
        /// <param name="WareHouseID"></param>
        /// <param name="inouttype"></param>
        /// <param name="WeighType"></param>
        /// <param name="WeightCount"></param>
        /// <param name="WeightTime"></param>
        /// <param name="WeighRecordNumber"></param>
        /// <param name="ScaleBillNumber"></param>
        private PlanTaskBatchDetail WeightUploadAddPlanTaskBatchDetail(PlanTaskBatch planTaskBatch, string WareHouseID, int inouttype,
                int WeighType, string WeightCount, DateTime WeightTime, string WeighRecordNumber, string ScaleBillNumber, string vehicleid, string outWareHouseID)
        {
            int iCount = planTaskBatch.PlanTaskBatchDetails.Count;
            PlanTaskBatchDetail planTaskBatchDetail = new PlanTaskBatchDetail();

            planTaskBatchDetail.plantask_batch_detail_number = planTaskBatch.plantask_batch_number + (iCount + 1).ToString("D3");
            planTaskBatchDetail.plantask_batch_number = planTaskBatch.plantask_batch_number;

            planTaskBatchDetail.warehouse_id = WareHouseID;
            if (outWareHouseID.Trim().Length > 0)
            {
                planTaskBatchDetail.outwarehouse = outWareHouseID;
            }

            //出入库标识 1：入；2：出
            planTaskBatchDetail.inout_type = inouttype;
            //称重类型 0皮重，1毛重
            if (WeighType == 0)
            {
                planTaskBatchDetail.tare_weight = Decimal.Parse(WeightCount);
                planTaskBatchDetail.tare_time = WeightTime;
                planTaskBatchDetail.isgross = false;
            }
            else
            {
                planTaskBatchDetail.gross_weight = Decimal.Parse(WeightCount);
                planTaskBatchDetail.gross_time = WeightTime;
                planTaskBatchDetail.isgross = true;
            }

            //磅单编号
            planTaskBatchDetail.weight_number = WeighRecordNumber;
            //车辆称重作业地磅系统采集记录编号
            planTaskBatchDetail.scale_number = ScaleBillNumber;
            //值仓
            //planTaskBatchDetail.duty_confirm = true;
            //单据状态：0 废除； 1 完成
            planTaskBatchDetail.bill_status = 1;
            //外部车辆为空，内部车辆流程暂时不考虑
            if (vehicleid != null)
            {
                planTaskBatchDetail.vehicle_id = vehicleid;
            }
            else
            {
                planTaskBatchDetail.vehicle_id = null;
            }

            string strWorkPlaceId = string.Empty;
            string strSiteScaleId = string.Empty;

            if (planTaskBatch.PlanTaskBatchWorkPlaces != null && planTaskBatch.PlanTaskBatchWorkPlaces.Count > 0)
            {
                strWorkPlaceId = planTaskBatch.PlanTaskBatchWorkPlaces.FirstOrDefault().workplace_id;
            }
            if (planTaskBatch.PlanTaskBatchSiteScales != null && planTaskBatch.PlanTaskBatchSiteScales.Count > 0)
            {
                strSiteScaleId = planTaskBatch.PlanTaskBatchSiteScales.FirstOrDefault().site_scale_id;
            }

            //作业场所ID;PlanTaskBatchWorkPlace.workplace_id
            if (strWorkPlaceId.Trim().Length > 0)
            {
                planTaskBatchDetail.workplace_id = strWorkPlaceId;
            }
            //使用地磅id
            if (strSiteScaleId.Trim().Length > 0)
            {
                planTaskBatchDetail.sitescale_id = strSiteScaleId;
            }

            _unitOfWork.AddAction(planTaskBatchDetail, DataActions.Add);
            return planTaskBatchDetail;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="planTaskBatch"></param>
        /// <param name="WareHouseID"></param>
        /// <param name="inouttype"></param>
        /// <param name="WeighType"></param>
        /// <param name="WeightCount"></param>
        /// <param name="WeightTime"></param>
        /// <param name="WeighRecordNumber"></param>
        /// <param name="ScaleBillNumber"></param>
        private PlanTaskBatchDetail WeightUploadAddPlanTaskBatchDetailNetWeight(PlanTaskBatch planTaskBatch, string WareHouseID, int inouttype,
                string WeightCount, DateTime WeightTime)
        {
            int iCount = planTaskBatch.PlanTaskBatchDetails.Count;
            PlanTaskBatchDetail planTaskBatchDetail = new PlanTaskBatchDetail();

            planTaskBatchDetail.plantask_batch_detail_number = planTaskBatch.plantask_batch_number + (iCount + 1).ToString("D3");
            planTaskBatchDetail.plantask_batch_number = planTaskBatch.plantask_batch_number;
            planTaskBatchDetail.warehouse_id = WareHouseID;
            //出入库标识 1：入；2：出
            planTaskBatchDetail.inout_type = inouttype;

            planTaskBatchDetail.tare_weight = 0;
            planTaskBatchDetail.tare_time = WeightTime;
            planTaskBatchDetail.gross_weight = Decimal.Parse(WeightCount);
            planTaskBatchDetail.gross_time = WeightTime;
            planTaskBatchDetail.weight = Decimal.Parse(WeightCount);
            planTaskBatchDetail.weight_time = WeightTime;
            planTaskBatchDetail.isgross = false;

            //值仓
            planTaskBatchDetail.duty_confirm = (int)DutyConfirm.值仓确认;
            //单据状态：0 废除； 1 完成
            planTaskBatchDetail.bill_status = 1;

            string strWorkPlaceId = string.Empty;
            string strSiteScaleId = string.Empty;

            if (planTaskBatch.PlanTaskBatchWorkPlaces != null && planTaskBatch.PlanTaskBatchWorkPlaces.Count > 0)
            {
                strWorkPlaceId = planTaskBatch.PlanTaskBatchWorkPlaces.FirstOrDefault().workplace_id;
            }
            if (planTaskBatch.PlanTaskBatchSiteScales != null && planTaskBatch.PlanTaskBatchSiteScales.Count > 0)
            {
                strSiteScaleId = planTaskBatch.PlanTaskBatchSiteScales.FirstOrDefault().site_scale_id;
            }

            //作业场所ID;PlanTaskBatchWorkPlace.workplace_id
            if (strWorkPlaceId.Trim().Length > 0)
            {
                planTaskBatchDetail.workplace_id = strWorkPlaceId;
            }
            //使用地磅id
            if (strSiteScaleId.Trim().Length > 0)
            {
                planTaskBatchDetail.sitescale_id = strSiteScaleId;
            }
            _unitOfWork.AddAction(planTaskBatchDetail, DataActions.Add);
            return planTaskBatchDetail;
        }

        private void FinishPlanTask(PlanTask planTask, PlanTaskBatchDetail planTaskBatchDetail)
        {
            if (planTaskBatchDetail.gross_time != null && planTaskBatchDetail.tare_time != null)
            {
                planTask.plan_status = (int)PlanTaskStatus.完成;
                //planTask.is_settlemented = (int)PlanTaskSettlement.已结算;
                planTask.real_end_time = _sPGetSysDateTimeService.GetSysDateTime();
                _unitOfWork.AddAction(planTask, DataActions.Update);
            }
        }

        private void UnFinishPlanTask(PlanTask planTask, PlanTaskBatchDetail planTaskBatchDetail)
        {
            if (planTaskBatchDetail.gross_time != null && planTaskBatchDetail.tare_time != null)
            {
                planTask.plan_status = (int)PlanTaskStatus.执行;
                //planTask.is_settlemented = (int)PlanTaskSettlement.未结算;
                planTask.real_end_time = null;
                _unitOfWork.AddAction(planTask, DataActions.Update);
            }
        }

        private void PousePlanTask(PlanTask planTask)
        {
            planTask.plan_status = (int)PlanTaskStatus.暂停;
            planTask.real_end_time = _sPGetSysDateTimeService.GetSysDateTime();
            _unitOfWork.AddAction(planTask, DataActions.Update);
        }

        private void FinishPlanTaskBatch(PlanTaskBatch planTaskBatch, PlanTaskBatchDetail planTaskBatchDetail, bool settlenentedFlag)
        {
            if (planTaskBatchDetail.gross_time != null && planTaskBatchDetail.tare_time != null)
            {
                planTaskBatch.batch_finish_time = _sPGetSysDateTimeService.GetSysDateTime();
                if (settlenentedFlag)
                {
                    planTaskBatch.is_settlemented = true;
                }

                _unitOfWork.AddAction(planTaskBatch, DataActions.Update);
            }
        }

        private void UnFinishPlanTaskBatch(PlanTaskBatch planTaskBatch, PlanTaskBatchDetail planTaskBatchDetail)
        {
            if (planTaskBatchDetail.gross_time != null && planTaskBatchDetail.tare_time != null)
            {
                planTaskBatch.batch_finish_time = null;
                //planTaskBatch.is_settlemented = false;
                _unitOfWork.AddAction(planTaskBatch, DataActions.Update);
            }
        }
        #endregion

        #region 取消榜单
        /// <summary>
        /// 根据磅单号置称重详细记录为无效
        /// </summary>
        /// <param name="ScaleBillNumber">磅单号</param>
        /// <returns></returns>
        private PlanTaskBatchDetail CancelScalePlanTaskBatchDetail(string ScaleBillNumber, PlanTaskBatchDetail planTaskBatchDetail)
        {
            if (planTaskBatchDetail == null)
            {
                return null;
            }
            //单据状态：0 废除； 1 完成
            planTaskBatchDetail.bill_status = 0;
            _unitOfWork.AddAction(planTaskBatchDetail, DataActions.Update);
            return planTaskBatchDetail;
        }

        /// <summary>
        /// 回滚PlanTaskBatch 某一条称重详细记录的称重记录
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        /// <returns></returns>
        private PlanTaskBatch CancelScalePlanTaskBatch(PlanTaskBatchDetail planTaskBatchDetail)
        {
            PlanTaskBatch planTaskBatch = planTaskBatchDetail.PlanTaskBatch;

            decimal dGrossWeight = planTaskBatchDetail.gross_weight.Value;
            decimal dTareWeight = planTaskBatchDetail.tare_weight.Value;
            decimal dWeight = planTaskBatchDetail.weight.Value;

            planTaskBatch.gross_weight -= dGrossWeight;
            planTaskBatch.tare_weight -= dTareWeight;
            planTaskBatch.batch_count -= dWeight;

            _unitOfWork.AddAction(planTaskBatch, DataActions.Update);
            return planTaskBatch;
        }

        /// <summary>
        /// 回滚PlanTask 某一条称重详细记录的称重记录
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        /// <returns></returns>
        private PlanTask CancelScalePlanTask(PlanTaskBatchDetail planTaskBatchDetail)
        {
            PlanTask planTask = planTaskBatchDetail.PlanTaskBatch.PlanTask;

            decimal dGrossWeight = planTaskBatchDetail.gross_weight.Value;
            decimal dTareWeight = planTaskBatchDetail.tare_weight.Value;
            decimal dWeight = planTaskBatchDetail.weight.Value;

            planTask.gross_weight -= dGrossWeight;
            planTask.tare_weight -= dTareWeight;
            planTask.plan_weight -= dWeight;

            _unitOfWork.AddAction(planTask, DataActions.Update);
            return planTask;
        }
        #endregion

        #region 取消称重
        /// <summary>
        /// 根据称重编号回滚称重详细记录
        /// </summary>
        /// <param name="WeighRecordNumber">称重编号</param>
        /// <returns></returns>
        private PlanTaskBatchDetail CancelScalePlanTaskBatchDetaiByWeighRecordNumber(string WeighRecordNumber)
        {
            PlanTaskBatchDetail planTaskBatchDetail = _planTaskBatchService.GetPlanTaskBatchDetailByWeightNumber(WeighRecordNumber);
            if (planTaskBatchDetail == null)
            {
                return null;
            }
            if ((bool)planTaskBatchDetail.isgross)
            {
                planTaskBatchDetail.gross_time = null;
                planTaskBatchDetail.gross_weight = null;
            }
            else
            {
                planTaskBatchDetail.tare_time = null;
                planTaskBatchDetail.tare_weight = null;
            }

            planTaskBatchDetail.weight = null;
            planTaskBatchDetail.weight_time = null;

            _unitOfWork.AddAction(planTaskBatchDetail, DataActions.Update);
            return planTaskBatchDetail;
        }

        /// <summary>
        /// 回滚PlanTaskBatch 某一条称重详细记录的称重记录
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        /// <returns></returns>
        private PlanTaskBatch CancelScalePlanTaskBatchByWeighRecordNumber(PlanTaskBatchDetail planTaskBatchDetail)
        {
            PlanTaskBatch planTaskBatch = planTaskBatchDetail.PlanTaskBatch;

            decimal dGrossWeight = 0;
            decimal dTareWeight = 0;
            decimal dWeight = 0;
            if (planTaskBatchDetail.gross_weight != null)
            {
                dGrossWeight = planTaskBatchDetail.gross_weight.Value;
            }
            if (planTaskBatchDetail.tare_weight != null)
            {
                dTareWeight = planTaskBatchDetail.tare_weight.Value;
            }
            if (planTaskBatchDetail.weight != null)
            {
                dWeight = planTaskBatchDetail.weight.Value;
            }

            if ((bool)planTaskBatchDetail.isgross)
            {
                planTaskBatch.gross_weight -= dGrossWeight;
                planTaskBatch.batch_count -= dWeight;
            }
            else
            {
                planTaskBatch.tare_weight -= dTareWeight;
                planTaskBatch.batch_count -= dWeight;
            }
            _unitOfWork.AddAction(planTaskBatch, DataActions.Update);
            return planTaskBatch;
        }

        /// <summary>
        /// 回滚PlanTask 某一条称重详细记录的称重记录
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        /// <returns></returns>
        private PlanTask CancelScalePlanTaskByWeighRecordNumber(PlanTaskBatchDetail planTaskBatchDetail)
        {
            PlanTask planTask = planTaskBatchDetail.PlanTaskBatch.PlanTask;

            decimal dGrossWeight = 0;
            decimal dTareWeight = 0;
            decimal dWeight = 0;

            if (planTaskBatchDetail.gross_weight != null)
            {
                dGrossWeight = planTaskBatchDetail.gross_weight.Value;
            }
            if (planTaskBatchDetail.tare_weight != null)
            {
                dTareWeight = planTaskBatchDetail.tare_weight.Value;
            }
            if (planTaskBatchDetail.weight != null)
            {
                dWeight = planTaskBatchDetail.weight.Value;
            }

            if ((bool)planTaskBatchDetail.isgross)
            {
                planTask.gross_weight -= dGrossWeight;
                planTask.plan_weight -= dWeight;
            }
            else
            {
                planTask.tare_weight -= dTareWeight;
                planTask.plan_weight -= dWeight;
            }
            _unitOfWork.AddAction(planTask, DataActions.Update);
            return planTask;
        }
        #endregion

        #region 倒仓
        public void DoInOutWarehouse(PlanTask plantask, PlanTaskBatch plantaskbatch, List<PlanTaskOutWarehouse> plantaskOutHouseList, List<PlanTaskInWarehouse> plantaskInHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> planworkplaceList, string OrgDepCode)
        {
            try
            {
                #region 倒仓出仓
                //倒仓出库
                List<PlanTaskOutWarehouse> planOuthouseList = plantaskbatch.PlanTaskOutWarehouses.ToList();// GetPlanTaskOutWarehouseList(plantask.plantask_number, plantask.goods_kind.Value).ToList();
                //原先没有的，现在要添加
                #region 更新出仓
                if (planOuthouseList != null && planOuthouseList.Count > 0)
                {
                    foreach (PlanTaskOutWarehouse sru in plantaskOutHouseList)
                    {
                        if (!planOuthouseList.Any(old => old.plantask_batch_number == sru.plantask_batch_number && old.warehouse_id == sru.warehouse_id))
                        {
                            sru.plantask_batch_number = plantaskbatch.plantask_batch_number;
                            _unitOfWork.AddAction(sru, DataActions.Add);
                        }
                    }
                }
                else
                {
                    foreach (PlanTaskOutWarehouse sru in plantaskOutHouseList)
                    {
                        sru.plantask_batch_number = plantaskbatch.plantask_batch_number;
                        _unitOfWork.AddAction(sru, DataActions.Add);
                    }
                }

                //原先有的，现在没了，要删除
                if (planOuthouseList != null)
                {
                    foreach (PlanTaskOutWarehouse sru in planOuthouseList.Where(old => !plantaskOutHouseList.Any(list => list.plantask_batch_number == old.plantask_batch_number && old.warehouse_id == list.warehouse_id)))
                    {
                        _unitOfWork.AddAction(sru, DataActions.Delete);
                    }
                }
                #endregion
                #region 更新预出库

                #endregion
                #endregion


                #region 倒仓入仓
                //倒仓入库
                List<PlanTaskInWarehouse> planoldInhouseList = plantaskbatch.PlanTaskInWarehouses.ToList();
                if (planoldInhouseList != null && planoldInhouseList.Count > 0)
                {
                    foreach (PlanTaskInWarehouse sru in plantaskInHouseList)
                    {
                        if (!planoldInhouseList.Any(old => old.plantask_batch_number == sru.plantask_batch_number && old.warehouse_id == sru.warehouse_id))
                        {
                            sru.plantask_batch_number = plantaskbatch.plantask_batch_number;
                            _unitOfWork.AddAction(sru, DataActions.Add);
                        }
                    }
                }
                else
                {
                    foreach (PlanTaskInWarehouse sru in plantaskInHouseList)
                    {
                        sru.plantask_batch_number = plantaskbatch.plantask_batch_number;
                        _unitOfWork.AddAction(sru, DataActions.Add);
                    }
                }

                //原先有的，现在没了，要删除
                if (planoldInhouseList != null)
                {
                    foreach (PlanTaskInWarehouse sru in planoldInhouseList.Where(old => !plantaskInHouseList.Any(list => list.plantask_batch_number == old.plantask_batch_number && old.warehouse_id == list.warehouse_id)))
                    {
                        _unitOfWork.AddAction(sru, DataActions.Delete);
                    }
                }
                #endregion


                #region 其他信息
                //磅点信息
                if (planSiteList != null)
                {
                    if (planSiteList.Count > 0)
                    {
                        List<PlanTaskBatchSiteScale> oldsite = plantaskbatch.PlanTaskBatchSiteScales.ToList();
                        if (oldsite.Count > 0)
                        {
                            foreach (PlanTaskBatchSiteScale ptbss in planSiteList)
                            {
                                ptbss.plantask_batch_number = plantaskbatch.plantask_batch_number;
                                if (!oldsite.Any(old => old.plantask_batch_number == ptbss.plantask_batch_number && old.site_scale_id == ptbss.site_scale_id))
                                {
                                    ptbss.plantask_batch_number = plantaskbatch.plantask_batch_number;
                                    _unitOfWork.AddAction(ptbss, DataActions.Add);
                                }
                            }
                            //_planScaleService.DelPlanTaskBatchSiteScale(plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchSiteScales.ToList(), planSiteList);

                        }
                        else
                        {
                            foreach (PlanTaskBatchSiteScale ptbss in planSiteList)
                            {
                                ptbss.plantask_batch_number = plantaskbatch.plantask_batch_number;
                                _unitOfWork.AddAction(ptbss, DataActions.Add);
                            }
                            //_planScaleService.AddPlanTaskBatchSiteScale(plantask.PlanTaskBatches.LastOrDefault(), planSiteList);
                        }


                        //原先有的，现在没了，要删除
                        if (oldsite != null)
                        {
                            foreach (PlanTaskBatchSiteScale sru in oldsite.Where(old => !planSiteList.Any(list => list.plantask_batch_number == old.plantask_batch_number && old.site_scale_id == list.site_scale_id)))
                            {
                                _unitOfWork.AddAction(sru, DataActions.Delete);
                            }
                        }
                    }
                }


                //内部车辆信息
                if (planvehicleList != null)
                {
                    List<PlanTaskBatchVehicle> oldvehicle = plantaskbatch.PlanTaskBatchVehicles.ToList();
                    if (planvehicleList.Count > 0)
                    {
                        if (oldvehicle.Count > 0)
                        {
                            foreach (PlanTaskBatchVehicle vehicle in planvehicleList)
                            {
                                vehicle.plantask_batch_number = plantaskbatch.plantask_batch_number;
                                if (!oldvehicle.Any(old => old.plantask_batch_number == vehicle.plantask_batch_number && old.vehicle_id == vehicle.vehicle_id))
                                {
                                    vehicle.plantask_batch_number = plantaskbatch.plantask_batch_number;
                                    _unitOfWork.AddAction(vehicle, DataActions.Add);
                                    vehicle.InnerVehicle.inner_vehicle_online = true;
                                    _unitOfWork.AddAction(vehicle.InnerVehicle, DataActions.Update);
                                }
                            }
                        }
                        else
                        {
                            foreach (PlanTaskBatchVehicle vehicle in planvehicleList)
                            {
                                vehicle.plantask_batch_number = plantaskbatch.plantask_batch_number;
                                _unitOfWork.AddAction(vehicle, DataActions.Add);
                                vehicle.InnerVehicle.inner_vehicle_online = true;
                                _unitOfWork.AddAction(vehicle.InnerVehicle, DataActions.Update);
                            }
                            //_planVehicleService.AddPlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault(), planvehicleList);
                            //for (int i = 0; i < planvehicleList.Count; i++)
                            //{
                            //    planvehicleList[i].InnerVehicle.inner_vehicle_online = true;
                            //    _innervehicle.Update(planvehicleList[i].InnerVehicle);
                            //}
                        }


                        //原先有的，现在没了，要删除
                        if (oldvehicle != null)
                        {
                            foreach (PlanTaskBatchVehicle sru in oldvehicle.Where(old => !planvehicleList.Any(list => list.plantask_batch_number == old.plantask_batch_number && old.vehicle_id == list.vehicle_id)))
                            {
                                _unitOfWork.AddAction(sru, DataActions.Delete);
                                sru.InnerVehicle.inner_vehicle_online = false;
                                _unitOfWork.AddAction(sru, DataActions.Update);
                            }
                        }
                    }
                }


                //码头加卸粮坑
                if (planworkplaceList != null)
                {
                    if (planworkplaceList.Count > 0)
                    {
                        List<PlanTaskBatchWorkPlace> oldworkplace = plantaskbatch.PlanTaskBatchWorkPlaces.ToList();
                        if (oldworkplace.Count > 0)
                        {
                            foreach (PlanTaskBatchWorkPlace workplace in planworkplaceList)
                            {
                                workplace.plantask_batch_number = plantaskbatch.plantask_batch_number;
                                if (!oldworkplace.Any(old => old.plantask_batch_number == workplace.plantask_batch_number && old.workplace_id == workplace.workplace_id))
                                {
                                    workplace.plantask_batch_number = plantaskbatch.plantask_batch_number;
                                    _unitOfWork.AddAction(workplace, DataActions.Add);
                                }
                            }
                        }
                        else
                        {
                            foreach (PlanTaskBatchWorkPlace workplace in planworkplaceList)
                            {
                                workplace.plantask_batch_number = plantaskbatch.plantask_batch_number;
                                _unitOfWork.AddAction(workplace, DataActions.Add);
                            }
                        }
                        //原先有的，现在没了，要删除
                        if (oldworkplace != null)
                        {
                            foreach (PlanTaskBatchWorkPlace sru in oldworkplace.Where(old => !planworkplaceList.Any(list => list.plantask_batch_number == old.plantask_batch_number && old.workplace_id == list.workplace_id)))
                            {
                                _unitOfWork.AddAction(sru, DataActions.Delete);
                            }
                        }
                    }
                }

                #endregion
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion

        public PlanTask AddPlanTaskWithInOutWarehouse(Decimal count, GoodsKind goodkinds, PlanTaskType planTaskType, string OrgDepCode, int accountId,
                                                List<PlanTaskOutWarehouse> planTaskOutWarehouses, List<PlanTaskInWarehouse> planTaskInWarehouses,
            List<PlanTaskBatchSiteScale> planTaskBatchSiteScales, List<PlanTaskBatchVehicle> planTaskBatchVehicles, List<PlanTaskBatchWorkPlace> planTaskBatchWorkPlaces, bool Isadd)
        {
            PlanTask newPlanTask = null;
            try
            {
                //分别设置所用的单据编号id,报港单编号，货物类型，计划类型，计划编号，计划执行数量
                //由于是根据报港单产生，所以状态直接为提交。默认不同步到dcs
                newPlanTask = new PlanTask();
                newPlanTask.bill_number_id = planTaskType.bill_number_id;
                newPlanTask.goods_kind = goodkinds.goods_kind_id;
                newPlanTask.plantask_type_code = planTaskType.plantask_type_code;
                newPlanTask.plantask_number = _sysBillNoService.GetBillNo(newPlanTask.bill_number_id.Value, OrgDepCode);
                newPlanTask.plan_count = count;
                if (Isadd)
                {
                    newPlanTask.plan_status = (int)PlanTaskStatus.确认;
                }
                else
                {
                    newPlanTask.plan_status = (int)PlanTaskStatus.执行;
                    newPlanTask.real_start_time = _sPGetSysDateTimeService.GetSysDateTime();
                }
                newPlanTask.sync_status = false;
                newPlanTask.create_time = _sPGetSysDateTimeService.GetSysDateTime();
                newPlanTask.weight_mode = (int)WeightMode.先毛后皮;
                newPlanTask.first_tare_con = false;
                newPlanTask.is_outvehicle = false;
                _unitOfWork.AddAction(newPlanTask, DataActions.Add);

                //创建plantaskdetail操作记录
                //操作记录编号为计划编号+三位流水号；操作人员为报港人员；状态为创建；顺序号为1；创建时间为报港单确认时间
                PlanTaskDetail newPlanTaskDetail = new PlanTaskDetail();
                newPlanTaskDetail.plantask_detail_number = newPlanTask.plantask_number + string.Format("{0:D3}", 1);
                newPlanTaskDetail.plantask_number = newPlanTask.plantask_number;
                newPlanTaskDetail.operator_user = accountId;
                newPlanTaskDetail.operator_type = (int)PlanTaskOPTType.创建;
                newPlanTaskDetail.seq = 1;
                newPlanTaskDetail.start_time = _sPGetSysDateTimeService.GetSysDateTime();
                _unitOfWork.AddAction(newPlanTaskDetail, DataActions.Add);


                BuildPlanTaskStepStatuInOutHouse(newPlanTask);
                //添加默认批次
                PlanTaskBatch ptb = _planTaskBatchService.AddPlanTaskBatchWithPlanTask(newPlanTask, planTaskType);

                DoInOutWarehouse(newPlanTask, ptb, planTaskOutWarehouses, planTaskInWarehouses, planTaskBatchSiteScales, planTaskBatchVehicles, planTaskBatchWorkPlaces, OrgDepCode);

                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return newPlanTask;
        }

        public PlanTask AddPlanTaskWithInOutWarehouse(PlanTask newPlanTask, PlanTaskBatch newPlantaskBatch, string OrgDepCode, int accountId,
                                                List<PlanTaskOutWarehouse> planTaskOutWarehouses, List<PlanTaskInWarehouse> planTaskInWarehouses,
            List<PlanTaskBatchSiteScale> planTaskBatchSiteScales, List<PlanTaskBatchVehicle> planTaskBatchVehicles, List<PlanTaskBatchWorkPlace> planTaskBatchWorkPlaces, bool Isadd)
        {
            try
            {
                newPlanTask.plantask_number = _sysBillNoService.GetBillNo(newPlanTask.bill_number_id.Value, OrgDepCode);
                if (Isadd)
                {
                    newPlanTask.plan_status = (int)PlanTaskStatus.确认;
                }
                else
                {
                    newPlanTask.plan_status = (int)PlanTaskStatus.执行;
                    newPlanTask.real_start_time = _sPGetSysDateTimeService.GetSysDateTime();
                }
                newPlanTask.sync_status = false;
                newPlanTask.create_time = _sPGetSysDateTimeService.GetSysDateTime();
                newPlanTask.first_tare_con = false;
                newPlanTask.is_outvehicle = false;
                _unitOfWork.AddAction(newPlanTask, DataActions.Add);

                //创建plantaskdetail操作记录
                //操作记录编号为计划编号+三位流水号；操作人员为报港人员；状态为创建；顺序号为1；创建时间为报港单确认时间
                PlanTaskDetail newPlanTaskDetail = new PlanTaskDetail();
                newPlanTaskDetail.plantask_detail_number = newPlanTask.plantask_number + string.Format("{0:D3}", 1);
                newPlanTaskDetail.plantask_number = newPlanTask.plantask_number;
                newPlanTaskDetail.operator_user = accountId;
                newPlanTaskDetail.operator_type = (int)PlanTaskOPTType.创建;
                newPlanTaskDetail.seq = 1;
                newPlanTaskDetail.start_time = _sPGetSysDateTimeService.GetSysDateTime();
                _unitOfWork.AddAction(newPlanTaskDetail, DataActions.Add);


                BuildPlanTaskStepStatuInOutHouse(newPlanTask);
                //添加默认批次
                PlanTaskBatch ptb = _planTaskBatchService.AddPlanTaskBatchWithCreate(newPlanTask, newPlantaskBatch);

                DoInOutWarehouse(newPlanTask, ptb, planTaskOutWarehouses, planTaskInWarehouses, planTaskBatchSiteScales, planTaskBatchVehicles, planTaskBatchWorkPlaces, OrgDepCode);

                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return newPlanTask;
        }

        private void BuildPlanTaskStepStatuInOutHouse(PlanTask plantask)
        {
            for (int i = 0; i < 5; i++)
            {
                PlanTaskStepStatu ptss = new PlanTaskStepStatu();
                ptss.plantask_number = plantask.plantask_number;
                switch (i)
                {
                    case 0:
                        ptss.step_code = PlanTaskStepCode.BAO_GANG_TJ;
                        ptss.status = true;
                        ptss.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
                        break;
                    case 1:
                        ptss.step_code = PlanTaskStepCode.BAO_GANG_QR;
                        ptss.status = (plantask.plan_status == (int)PlanTaskStatus.确认);
                        ptss.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
                        break;
                    case 2:
                        ptss.step_code = PlanTaskStepCode.JI_HUA_BZ;
                        ptss.status = false;
                        break;
                    case 3:
                        ptss.step_code = PlanTaskStepCode.JI_HUA_ZX;
                        ptss.status = false;
                        break;
                    case 4:
                        ptss.step_code = PlanTaskStepCode.JIE_SUAN;
                        ptss.status = false;
                        break;
                    default:
                        break;
                }
                _unitOfWork.AddAction(ptss, DataActions.Add);
            }
        }

        public void RefreshData()
        {
            this._planTaskDal.RefreshData();
            this._planTaskBatchService.RefreshData();
            this._planTaskDetailDal.RefreshData();
            this._planTaskStepStatuDal.RefreshData();
            this._planTaskTypeDal.RefreshData();
        }

        public IEnumerable<PlanTask> SelectAllPlanTaskByIDCard(string IDCard)
        {
            return _planTaskDal.Find(pt =>
                                    (pt.is_settlemented.HasValue && pt.is_settlemented.Value != (int)PlanTaskSettlement.已结算)
                                  && (pt.Enrolment.carrier_id == IDCard || pt.Enrolment.owner_id == IDCard)
                                  , "PlanTaskBatches", "PlanTaskBatches.BusinessDisposes", "Enrolment", "Enrolment.Contract"
                                    ).Entities;
        }

        public void UpdatePlanTaskWithWarehouseStore(PlanTask upplantask)
        {
            try
            {
                bool issettle = false;
                foreach (PlanTaskBatch plantaskbatchs in upplantask.PlanTaskBatches.ToList())
                {
                    if (plantaskbatchs.final_statement_weight == null || plantaskbatchs.final_statement_weight == 0)
                    {
                        issettle = true;
                        plantaskbatchs.is_settlemented = true;
                        _planTaskBatchService.UpPlantaskBatchWithUnit(plantaskbatchs);
                    }

                }
                upplantask.real_end_time = _sPGetSysDateTimeService.GetSysDateTime();
                _unitOfWork.AddAction(upplantask, DataActions.Update);
                if (issettle == true)
                {
                    PlanTaskStepStatu getplanstep = upplantask.PlanTaskStepStatus.Single(d => d.step_code == "结算" && d.plantask_number == upplantask.plantask_number);
                    if (getplanstep.operate_time == null)
                    {
                        getplanstep.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
                        getplanstep.status = true;
                        _unitOfWork.AddAction(getplanstep, DataActions.Update);
                    }
                }

                PlanTaskBatchStepStatus ptbss = new PlanTaskBatchStepStatus();
                ptbss.plantask_batch_number = upplantask.PlanTaskBatches.Last().plantask_batch_number;
                ptbss.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
                ptbss.step_code = PlanTaskBatchStepCode.PI_CI_WC;
                ptbss.plantask_number = upplantask.plantask_number;

                _unitOfWork.AddAction(ptbss, DataActions.Add);
               
                //_warehouseStoreInfoService.UpDateBusinessWeight(upplantask);
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void FinshPlanTask(PlanTask upplantask)
        {
            upplantask.real_end_time = _sPGetSysDateTimeService.GetSysDateTime();
            _planTaskDal.Update(upplantask);
            upplantask.PlanTaskBatches.LastOrDefault().batch_finish_time = _sPGetSysDateTimeService.GetSysDateTime();
            _planTaskBatchService.UpdatePlantaskBatch(upplantask.PlanTaskBatches.LastOrDefault());
        }


        public RepositoryResultList<PlanTask> FindPagePlanTaskByCondition(int planstatus, string plantypecode, string plannum, DateTime planbegain, DateTime planend, PagingCriteria paging)
        {
            var queryPlantask = PredicateBuilder.True<PlanTask>();
            if (planstatus > 0)
            {
                queryPlantask = queryPlantask.And(e => e.plan_status == planstatus);
            }
            if (plantypecode != "" && plantypecode != "all")
            {
                queryPlantask = queryPlantask.And(e => e.plantask_type_code.Contains(plantypecode));
            }
            if (plannum != "")
            {
                queryPlantask = queryPlantask.And(e => e.plantask_number.Contains(plannum));
            }
            if (planbegain != null && planend == null)
            {
                queryPlantask = queryPlantask.And(e => e.real_start_time >= planbegain);
            }
            if (planbegain == null && planend != null)
            {
                queryPlantask = queryPlantask.And(e => e.real_end_time <= planend);
            }
            if (planbegain != null && planend != null && planbegain != DateTime.MinValue && planend != DateTime.MinValue)
            {
                queryPlantask = queryPlantask.And(e => e.real_start_time >= planbegain && e.real_end_time <= planend);
            }

            return _planTaskDal.Find(queryPlantask, paging);
        }

        public RepositoryResultList<PlanTask> FindPageFastByCondition(int enrolment_status, int assay_status, int plan_status, string plateNumber, string ownerName, DateTime planbegain, DateTime planend)
        {
            var queryFast = PredicateBuilder.True<PlanTask>();
            queryFast = queryFast.And(p => p.Enrolment!= null && p.Enrolment.inout_type == 1);
            if (enrolment_status > 0)
            {
                //选择了报港状态
                queryFast = queryFast.And(p => p.Enrolment.status == enrolment_status);
            }
            if (assay_status > 0)
            {
                //选择了化验状态
                queryFast = queryFast.And(p => p.PlanTaskBatches.Single().Samples.LastOrDefault().Assays.LastOrDefault().status == assay_status);
            }
            if(plan_status>0)
            {
                //选择了计划状态
                queryFast = queryFast.And(p => p.plan_status == plan_status);
            }
            if (planbegain != null && planend == null)
            {
                queryFast = queryFast.And(p => p.real_start_time >= DateTime.Parse(planbegain.ToShortDateString()));
            }
            if (planbegain == null && planend != null)
            {
                queryFast = queryFast.And(p => p.real_end_time <= DateTime.Parse(planend.ToShortDateString()));
            }
            if (planbegain != null && planend != null && planbegain != DateTime.MinValue && planend != DateTime.MinValue)
            {
                queryFast = queryFast.And(p => p.create_time != null && p.create_time.Value != null && p.create_time.Value.Date >= planbegain.Date && p.create_time.Value.Date < planend.Date.AddDays(1));
            }
            if (ownerName != null && ownerName != "")
            {
                queryFast = queryFast.And(p => p.owner_name.Contains(ownerName));
            }
            if (plateNumber != null && plateNumber != "")
            {
                queryFast = queryFast.And(p => p.plate_number.Contains(plateNumber));
            }
            //, new string[] { "Enrolment", "PlanTaskBatches", "PlanTaskBatches.PlanTaskBatchDetails", "PlanTaskBatches.PlanTaskBatchDetails.Warehouse", "PlanTaskBatches.PlanTaskInWarehouses", "PlanTaskBatches.PlanTaskOutWarehouses", "PlanTaskBatches.PlanTaskBatchVehicles", "PlanTaskBatches.PlanTaskBatchSiteScales", "PlanTaskBatches.PlanTaskBatchWorkPlaces" }
            return _planTaskDal.Find(queryFast);
        }

        public RepositoryResultList<PlanTask> FindPagePlanTaskByCondition(int plan_status, int traficType, DateTime planDate, string planType, string planNumber, DateTime planbegain, DateTime planend, PagingCriteria pagingCriteria)
        {
            var queryPlanTask = PredicateBuilder.True<PlanTask>();
            if (plan_status > 0)
            {
                //选择了计划状态
                queryPlanTask = queryPlanTask.And(p => p.plan_status == plan_status);
            }
            if (traficType > 0)
            {
                //如果等于0 则表示选择了全部，不用拼接条件
                if (traficType == 1)
                {
                    //如果选择了运输类型，1为外部车
                    queryPlanTask = queryPlanTask.And(p => p.is_outvehicle == true);
                }
                else
                {
                    //非外部车
                    queryPlanTask = queryPlanTask.And(p => p.is_outvehicle == false);
                }
            }
            if (planDate != null && planDate != DateTime.MinValue)
            {
                queryPlanTask = queryPlanTask.And(p => p.create_time.HasValue && p.create_time.Value.ToString("yyyyMMdd") == planDate.ToString("yyyyMMdd"));
            }
            if (planType != "all")
            {
                //不是全部类型
                queryPlanTask = queryPlanTask.And(p => p.plantask_type_code == planType);
            }
            if (planNumber != null && planNumber.Trim() != "")
            {
                queryPlanTask = queryPlanTask.And(p => p.plantask_number == planNumber);
            }
            if (planbegain != null && planend == null)
            {
                queryPlanTask = queryPlanTask.And(e => e.real_start_time >= planbegain);
            }
            if (planbegain == null && planend != null)
            {
                queryPlanTask = queryPlanTask.And(e => e.real_end_time <= planend);
            }
            if (planbegain != null && planend != null && planbegain != DateTime.MinValue && planend != DateTime.MinValue)
            {
                queryPlanTask = queryPlanTask.And(e => e.real_start_time >= planbegain && e.real_end_time <= planend);
            }

            return _planTaskDal.Find(queryPlanTask, pagingCriteria, new string[] { "Enrolment", "PlanTaskBatches", "PlanTaskBatches.PlanTaskBatchDetails", "PlanTaskBatches.PlanTaskInWarehouses", "PlanTaskBatches.PlanTaskOutWarehouses", "PlanTaskBatches.PlanTaskBatchVehicles", "PlanTaskBatches.PlanTaskBatchSiteScales", "PlanTaskBatches.PlanTaskBatchWorkPlaces" });
        }

        public PlanTask UpPlantaskWithUnit(PlanTask upplantask)
        {

            _unitOfWork.AddAction(upplantask, DataActions.Update);
            return upplantask;
        }

        public RepositoryResultList<PlanTask> GetPlantaskBatchListForFastInfo(string enrolment_number, string plate_number, string warehouse_name, Nullable<decimal> gross_weight, Nullable<decimal> tare_weight, Nullable<decimal> weight, int unit_price, string owner_name)
        {
            var queryPlanTask = PredicateBuilder.True<PlanTask>();
            queryPlanTask = queryPlanTask.And(qp => qp.Enrolment != null);
            if (plate_number != null && plate_number.Trim() != "")
            {
                queryPlanTask = queryPlanTask.And(p => p.plate_number == plate_number);
            }
            else
            { 
            
            }
            if (enrolment_number != null && enrolment_number.Trim() != "")
            {
                queryPlanTask = queryPlanTask.And(p => p.enrolment_number == enrolment_number);
            }
            if (warehouse_name != null && warehouse_name.Trim() != "") ;
            {
                queryPlanTask = queryPlanTask.And(p => p.warehouse_name == warehouse_name);
            }
            if(gross_weight !=null)
            {
                queryPlanTask = queryPlanTask.And(p => p.gross_weight == gross_weight);
            }
            if (tare_weight != null)
            {
                queryPlanTask = queryPlanTask.And(p => p.tare_weight == tare_weight);
            }
            if (weight != null)
            {
                queryPlanTask = queryPlanTask.And(p => p.weight == weight);
            }
            if (unit_price != null)
            {
                queryPlanTask = queryPlanTask.And(p => p.unit_price == unit_price);
            }
            if (owner_name != null && owner_name.Trim() != "")
            {
                queryPlanTask = queryPlanTask.And(p => p.owner_name == owner_name);
            }
            return _planTaskDal.Find(queryPlanTask, new string[] { "Enrolment", "PlanTaskBatches",  "PlanTaskBatches.PlanTaskBatchDetails", "PlanTaskBatches.PlanTaskInWarehouses", "PlanTaskBatches.PlanTaskOutWarehouses", "PlanTask" });
        }

        public RepositoryResultList<PlanTask> GetPlantaskListForStatementInfo(DateTime dtStart, DateTime dtEnd, int warehouse_id, int grain_id, string statementMan, string clientName, string trafficNumber, int statement_status)
        {
            var queryPlantask = PredicateBuilder.True<PlanTask>();
            queryPlantask = queryPlantask.And(qp => qp.Enrolment != null);
            if (statement_status > 0)
            {
                queryPlantask = queryPlantask.And(e => e.is_settlemented.HasValue && e.is_settlemented.Value == statement_status);
            }
            ////queryPlantask = queryPlantask.And(e => e.plan_status.HasValue && e.plan_status.Value == (int)PlanTaskStatus.完成);

            if (dtStart.Date.ToString("yyyyMMdd") != "00010101")
            {
                queryPlantask = queryPlantask.And(e => e.PlanTaskBatches.Any(pb => pb.FinalStatementItemDetails.Count > 0 && pb.FinalStatementItemDetails.Any(fs => fs.FinalStatementBill.bill_create_time.Substring(0, 8).CompareTo(dtStart.ToString("yyyyMMdd")) >= 0)));
            }
            if (dtEnd.Date.ToString("yyyyMMdd") != "00010101")
            {
                queryPlantask = queryPlantask.And(e => e.PlanTaskBatches.Any(pb => pb.FinalStatementItemDetails.Count > 0 && pb.FinalStatementItemDetails.Any(fs => fs.FinalStatementBill.bill_create_time.Substring(0, 8).CompareTo(dtEnd.AddDays(1).ToString("yyyyMMdd")) < 0)));
            }
            if (warehouse_id != 0)
            {
                queryPlantask = queryPlantask.And(e => e.PlanTaskBatches.Any(
                                                   batch => batch.PlanTaskInWarehouses.Any(ptin => ptin.Warehouse.generate_id == warehouse_id)
                                                     || batch.PlanTaskOutWarehouses.Any(ptout => ptout.OutWarehouse.generate_id == warehouse_id)
                                                      ));
            }
            if (grain_id != 0)
            {
                queryPlantask = queryPlantask.And(e => e.Enrolment != null && e.Enrolment.goods_kind == grain_id);
            }
            if (clientName != string.Empty)
            {
                queryPlantask = queryPlantask.And(e => e.Enrolment != null && e.Enrolment.owner_name == clientName);
            }
            if (trafficNumber != string.Empty)
            {
                queryPlantask = queryPlantask.And(e => e.Enrolment != null && e.Enrolment.plate_number == trafficNumber);
            }
            if (statementMan != string.Empty)
            {
                queryPlantask = queryPlantask.And(e =>
                    (e.FinalStatementItemDetails.Count > 0 && e.FinalStatementItemDetails.Any(fs => fs.FinalStatementBill.bill_create_man == statementMan))
                 || (e.FinalStatementItemDetails.Count == 0 && e.PlanTaskBatches.Any(ptbatch => ptbatch.FinalStatementItemDetails.Any(fs => fs.FinalStatementBill.bill_create_man == statementMan)))
                 );
            }
            //return _planTaskDal.Find(queryPlantask);
            return _planTaskDal.Find(queryPlantask, new string[] { "Enrolment", "PlanTaskBatches", "PlanTaskBatches.FinalStatementItemDetails", "PlanTaskBatches.PlanTaskBatchDetails", "PlanTaskBatches.PlanTaskInWarehouses", "PlanTaskBatches.PlanTaskOutWarehouses", "BusinessDisposes", "PlanTaskBatches.BusinessDisposes" });
        }

        #region 私有方法
        private void SetPlanTaskInWarehouse(PlanTask plantask, List<PlanTaskInWarehouse> plantaskInHouseList)
        {
            PlanTaskBatch planTaskBatch = plantask.PlanTaskBatches.LastOrDefault();
            List<PlanTaskInWarehouse> oldPlanTaskInWarehouse = planTaskBatch.PlanTaskInWarehouses.ToList();

            if (oldPlanTaskInWarehouse != null && oldPlanTaskInWarehouse.Count > 0)
            {
                foreach (PlanTaskInWarehouse planTaskInWarehouse in plantaskInHouseList)
                {
                    //查找选择的仓房是否已存在
                    if (!oldPlanTaskInWarehouse.Any(d => d.plantask_batch_number == planTaskInWarehouse.plantask_batch_number && d.warehouse_id == planTaskInWarehouse.warehouse_id))
                    {
                        _unitOfWork.AddAction(planTaskInWarehouse, DataActions.Add);
                    }
                }
            }
            else
            {
                foreach (PlanTaskInWarehouse planTaskInWarehouse in plantaskInHouseList)
                {
                    _unitOfWork.AddAction(planTaskInWarehouse, DataActions.Add);
                }
            }

            //查找之前选择的菜单是否已删除
            if (oldPlanTaskInWarehouse != null)
            {
                //原有单头所含明细不为空，则需要判断是否有删除项
                foreach (PlanTaskInWarehouse planTaskInWarehouseRemove in oldPlanTaskInWarehouse.Where(x => !plantaskInHouseList.Any(u => u.warehouse_id == x.warehouse_id && u.plantask_batch_number == x.plantask_batch_number)).ToList())
                {
                    _unitOfWork.AddAction(planTaskInWarehouseRemove, DataActions.Delete);
                }
            }
        }

        private void SetPlanTaskOutWarehouse(PlanTask plantask, List<PlanTaskOutWarehouse> plantaskOutHouseList)
        {
            PlanTaskBatch planTaskBatch = plantask.PlanTaskBatches.LastOrDefault();
            List<PlanTaskOutWarehouse> oldPlanTaskOutWarehouse = planTaskBatch.PlanTaskOutWarehouses.ToList();

            if (oldPlanTaskOutWarehouse != null && oldPlanTaskOutWarehouse.Count > 0)
            {
                foreach (PlanTaskOutWarehouse planTaskOutWarehouse in plantaskOutHouseList)
                {
                    //查找选择的菜单是否已存在
                    if (!oldPlanTaskOutWarehouse.Any(d => d.plantask_batch_number == planTaskOutWarehouse.plantask_batch_number && d.warehouse_id == planTaskOutWarehouse.warehouse_id))
                    {
                        _unitOfWork.AddAction(planTaskOutWarehouse, DataActions.Add);
                    }
                }
            }
            else
            {
                foreach (PlanTaskOutWarehouse planTaskOutWarehouse in plantaskOutHouseList)
                {
                    _unitOfWork.AddAction(planTaskOutWarehouse, DataActions.Add);
                }
            }

            //查找之前选择的菜单是否已删除
            if (oldPlanTaskOutWarehouse != null)
            {
                //原有单头所含明细不为空，则需要判断是否有删除项
                foreach (PlanTaskOutWarehouse planTaskOutWarehouseRemove in oldPlanTaskOutWarehouse.Where(x => !plantaskOutHouseList.Any(u => u.warehouse_id == x.warehouse_id && u.plantask_batch_number == x.plantask_batch_number)).ToList())
                {
                    _unitOfWork.AddAction(planTaskOutWarehouseRemove, DataActions.Delete);
                }
            }
        }

        private void AddPlanTaskBatchStepStatusWithUnit(string planTaskNum, string planTaskBatchNum, string stepCode)
        {
            PlanTaskBatchStepStatus planTaskBatchStepStatus = new PlanTaskBatchStepStatus();
            planTaskBatchStepStatus.plantask_batch_number = planTaskBatchNum;
            planTaskBatchStepStatus.plantask_number = planTaskNum;
            planTaskBatchStepStatus.step_code = stepCode;
            planTaskBatchStepStatus.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
            _unitOfWork.AddAction(planTaskBatchStepStatus, DataActions.Add);
        }

        private void UpdatePlanTaskStepStatuJieSuan(PlanTask plantask)
        {
            var planTaskStepStatusTemp = plantask.PlanTaskStepStatus.Single(ps => ps.step_code == PlanTaskStepCode.JIE_SUAN);
            if (planTaskStepStatusTemp == null)
            {
                return;
            }
            planTaskStepStatusTemp.status = true;
            planTaskStepStatusTemp.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
            _unitOfWork.AddAction(planTaskStepStatusTemp, DataActions.Update);
        }
        #endregion


        public IEnumerable<PlanTask> FindPlantaskListByContract(Contract cont)
        {
            IEnumerable<PlanTask> rtPlantaskList = _planTaskDal.Find(p => p.Enrolment.Contract == cont && p.plan_status == (int)PlanTaskStatus.完成
                && p.is_settlemented.HasValue && p.is_settlemented.Value != (int)PlanTaskSettlement.已结算
                , new string[] { "Enrolment", "Enrolment.Contract", "PlanTaskBatches", "BusinessDisposes", "PlanTaskBatches.BusinessDisposes" }).Entities;
            return rtPlantaskList;
        }

        #region 导仓多对多换仓（Update写法）
        private void UpdatePlanTaskInWarehouseWithVehicle(PlanTaskBatch planTaskBatch, string vehicleId, string warehouseID)
        {

            if (planTaskBatch.PlanTaskInWarehouses == null || planTaskBatch.PlanTaskInWarehouses.Count == 0)
            {
                return;
            }
            foreach (PlanTaskInWarehouse planTaskInWarehouse in planTaskBatch.PlanTaskInWarehouses)
            {
                if (planTaskInWarehouse.warehouse_id != warehouseID && planTaskInWarehouse.vehicle_id != null && planTaskInWarehouse.vehicle_id.Trim().Length > 0 && planTaskInWarehouse.vehicle_id.Contains(vehicleId + ";"))
                {
                    planTaskInWarehouse.vehicle_id = planTaskInWarehouse.vehicle_id.Replace(vehicleId + ";", string.Empty);
                    _unitOfWork.AddAction(planTaskInWarehouse, DataActions.Update);
                    break;
                }
                if (planTaskInWarehouse.warehouse_id != warehouseID && planTaskInWarehouse.vehicle_id != null && planTaskInWarehouse.vehicle_id.Trim().Length > 0 && planTaskInWarehouse.vehicle_id.Contains(";" + vehicleId))
                {
                    planTaskInWarehouse.vehicle_id = planTaskInWarehouse.vehicle_id.Replace(";" + vehicleId, string.Empty);
                    _unitOfWork.AddAction(planTaskInWarehouse, DataActions.Update);
                    break;
                }
            }

            PlanTaskInWarehouse planTaskInWarehouseUdp = planTaskBatch.PlanTaskInWarehouses.Where(iw => iw.warehouse_id == warehouseID).Last();
            if (planTaskInWarehouseUdp == null || (planTaskInWarehouseUdp.vehicle_id != null && planTaskInWarehouseUdp.vehicle_id.Trim().Length > 0 && planTaskInWarehouseUdp.vehicle_id.Contains(vehicleId)))
            {
                return;
            }
            if (planTaskInWarehouseUdp.vehicle_id == null || planTaskInWarehouseUdp.vehicle_id.Trim().Length == 0)
            {
                planTaskInWarehouseUdp.vehicle_id = vehicleId + ";";
            }
            else
            {
                planTaskInWarehouseUdp.vehicle_id = planTaskInWarehouseUdp.vehicle_id + vehicleId + ";";
            }
            _unitOfWork.AddAction(planTaskInWarehouseUdp, DataActions.Update);
        }

        private void UpdatePlanTaskOutWarehouseWithVehicle(PlanTaskBatch planTaskBatch, string vehicleId, string warehouseID)
        {
            if (planTaskBatch.PlanTaskOutWarehouses == null || planTaskBatch.PlanTaskOutWarehouses.Count == 0)
            {
                return;
            }
            foreach (PlanTaskOutWarehouse planTaskOutWarehouse in planTaskBatch.PlanTaskOutWarehouses)
            {
                if (planTaskOutWarehouse.warehouse_id != warehouseID && planTaskOutWarehouse.vehicle_id != null && planTaskOutWarehouse.vehicle_id.Trim().Length > 0 && planTaskOutWarehouse.vehicle_id.Contains(vehicleId + ";"))
                {
                    planTaskOutWarehouse.vehicle_id = planTaskOutWarehouse.vehicle_id.Replace(vehicleId + ";", string.Empty);
                    _unitOfWork.AddAction(planTaskOutWarehouse, DataActions.Update);
                    break;
                }
                if (planTaskOutWarehouse.warehouse_id != warehouseID && planTaskOutWarehouse.vehicle_id != null && planTaskOutWarehouse.vehicle_id.Trim().Length > 0 && planTaskOutWarehouse.vehicle_id.Contains(";" + vehicleId))
                {
                    planTaskOutWarehouse.vehicle_id = planTaskOutWarehouse.vehicle_id.Replace(";" + vehicleId, string.Empty);
                    _unitOfWork.AddAction(planTaskOutWarehouse, DataActions.Update);
                    break;
                }
            }
            PlanTaskOutWarehouse planTaskOutWarehouseUdp = planTaskBatch.PlanTaskOutWarehouses.Where(iw => iw.warehouse_id == warehouseID).Last();
            if (planTaskOutWarehouseUdp == null || (planTaskOutWarehouseUdp.vehicle_id != null && planTaskOutWarehouseUdp.vehicle_id.Trim().Length > 0 && planTaskOutWarehouseUdp.vehicle_id.Contains(vehicleId)))
            {
                return;
            }
            if (planTaskOutWarehouseUdp.vehicle_id == null || planTaskOutWarehouseUdp.vehicle_id.Trim().Length == 0)
            {
                planTaskOutWarehouseUdp.vehicle_id = vehicleId + ";";
            }
            else
            {
                planTaskOutWarehouseUdp.vehicle_id = planTaskOutWarehouseUdp.vehicle_id + vehicleId + ";";
            }
            _unitOfWork.AddAction(planTaskOutWarehouseUdp, DataActions.Update);
        }
        #endregion
    }
}
