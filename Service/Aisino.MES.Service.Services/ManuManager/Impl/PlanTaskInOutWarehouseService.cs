using System.Collections.Generic;
using System.Linq;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Repository;
using Aisino.MES.Service.SysManager;
using Aisino.MES.Service.WarehouseManager;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Service.StoreProcedures;
using Aisino.MES.Service.EnrolmentManager;

namespace Aisino.MES.Service.ManuManager.Impl
{

    public class PlanTaskInOutWarehouseService : IPlanTaskInOutWarehouseService
    {
        private Repository<PlanTaskInWarehouse> _planTaskInWarehouseDal;
        private Repository<PlanTaskOutWarehouse> _planTaskOutWarehouseDal;
        private RepositoryCustomerQuary _repositoryCustomerQuary;
        private ISysBillNoService _sysBillNoService;
        private IWarehouseStoreInfoService _wareHouseStoreInforService;
        private IWarehouseInOutRecordService _wareHouseInOUTService;
        SPGetSysDateTimeService _sPGetSysDateTimeService;
        private Repository<PlanTask> _planTaskDal;
        private Repository<PlanTaskBatch> _planTaskBatchDal;
        private IPlanTaskBatchSiteScaleService _planScaleService;
        private IPlanTaskBatchVehicleService _planVehicleService;
        private IPlanTaskBatchWorkPlaceService _planWorkPlaceService;
        private Repository<Enrolment> _enrolmentDal;
        private Repository<InnerVehicle> _innervehicle;
        private IEnrolmentService _enrolmentService;
        private IPlanTaskBatchService _plantaskbatchService;
        private IPlanTaskService _plantaskService;
        private UnitOfWork _unitOfWork;


        public PlanTaskInOutWarehouseService(Repository<PlanTaskInWarehouse> planTaskInWarehouseDal,
                                             Repository<PlanTaskOutWarehouse> planTaskOutWarehouseDal,
                                             RepositoryCustomerQuary repositoryCustomerQuary,
                                             ISysBillNoService sysBillNoService,
                                             IWarehouseStoreInfoService wareHouseStoreInforService,
                                             IWarehouseInOutRecordService wareHouseInOUTService,
                                             SPGetSysDateTimeService sPGetSysDateTimeService,
                                             Repository<PlanTask> planTaskDal,
                                             Repository<PlanTaskBatch> planTaskBatchDal,
                                             IPlanTaskBatchSiteScaleService planScaleService,
                                             IPlanTaskBatchVehicleService planVehicleService,
                                             IPlanTaskBatchWorkPlaceService planWorkPlaceService,
                                             Repository<Enrolment> enrolmentDal,
                                             Repository<InnerVehicle> innervehicle,
                                             IEnrolmentService enrolmentService,
                                             IPlanTaskBatchService plantaskbatchService,
                                             IPlanTaskService plantaskService,
                                             UnitOfWork unitOfWork)
        {
            _planTaskInWarehouseDal = planTaskInWarehouseDal;
            _planTaskOutWarehouseDal = planTaskOutWarehouseDal;
            _repositoryCustomerQuary = repositoryCustomerQuary;
            _sysBillNoService = sysBillNoService;
            _wareHouseStoreInforService = wareHouseStoreInforService;
            _wareHouseInOUTService = wareHouseInOUTService;
            _sPGetSysDateTimeService = sPGetSysDateTimeService;
            _planTaskDal = planTaskDal;
            _planTaskBatchDal = planTaskBatchDal;
            _planScaleService = planScaleService;
            _planVehicleService = planVehicleService;
            _planWorkPlaceService = planWorkPlaceService;
            _enrolmentDal = enrolmentDal;
            _innervehicle = innervehicle;
            _enrolmentService = enrolmentService;
            _plantaskbatchService = plantaskbatchService;
            _plantaskService = plantaskService;
            _unitOfWork = unitOfWork;
        }

        //已经入库的仓库列表
        public IEnumerable<PlanTaskInWarehouse> GetPlanTaskInWarehouseList(string plan_task_id, int invmas_id)
        {
            return _planTaskInWarehouseDal.GetAll().Entities.Where(d => d.PlanTaskBatch.plantask_number == plan_task_id && d.goods_kind == invmas_id);
        }

        //已经出库的仓库列表
        public IEnumerable<PlanTaskOutWarehouse> GetPlanTaskOutWarehouseList(string plantask_number, int invmas_id)
        {
            return _planTaskOutWarehouseDal.GetAll().Entities.Where(d => d.PlanTaskBatch.plantask_number == plantask_number && d.goods_kind == invmas_id);

        }


        //所有入库的仓库列表
        //public IEnumerable<CustomInOutWarehouse> GetAllInWarehouseList(int warehouse_id)
        //{
        //    string strSQL = "select p.plan_start_time, p.plan_limit_time, w.plan_task_in_amount as amount, 1 as op ";
        //    strSQL += " from PlanTaskInWarehouse w, PlanTask p";
        //    strSQL += " where w.plan_task_id = p.id";
        //    strSQL += " and w.warehouse_id = " + warehouse_id.ToString();
        //    strSQL += " order by p.plan_start_time, p.plan_limit_time";

        //    IEnumerable<CustomInOutWarehouse> inoutList = _repositoryCustomerQuary.QueryCustomerObjectByESql<CustomInOutWarehouse>(strSQL);

        //    //DbContext ctx = _planTaskInWarehouseDal.ctx;
        //    //IEnumerable<CustomInOutWarehouse> inoutList = ctx.Database.SqlQuery<CustomInOutWarehouse>(strSQL);


        //    return inoutList;
        //}

        ////所有出库的仓库列表
        //public IEnumerable<CustomInOutWarehouse> GetAllOutWarehouseList(int warehouse_id)
        //{
        //    string strSQL = "select p.plan_start_time, p.plan_limit_time, w.plan_task_out_amount as amount, -1 as op ";
        //    strSQL += " from PlanTaskOutWarehouse w, PlanTask p";

        //    strSQL += " where w.plan_task_id = p.id";
        //    strSQL += " and w.warehouse_id = " + warehouse_id.ToString();
        //    strSQL += " order by p.plan_start_time, p.plan_limit_time";


        //    IEnumerable<CustomInOutWarehouse> inoutList = _repositoryCustomerQuary.QueryCustomerObjectByESql<CustomInOutWarehouse>(strSQL);

        //    //DbContext ctx = _planTaskOutWarehouseDal.ctx;
        //    //IEnumerable<CustomInOutWarehouse> inoutList = ctx.Database.SqlQuery<CustomInOutWarehouse>(strSQL);


        //    return inoutList;

        //}

        public static IEnumerable<TA> GetTA<TA>(IEnumerable<object> array)
        {
            return array.OfType<TA>();
        }

        public void AddPlantaskInWarehouse(PlanTask plantask, List<PlanTaskInWarehouse> plantaskInHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> planworkplaceList, string OrgDepCode,bool needQuality)
        {
            //PlanTaskInWarehouse rtplabinhouse = null;
            try
            {
                PlanTaskBatch planTaskBatch = plantask.PlanTaskBatches.LastOrDefault();
                planTaskBatch.need_quality_test = needQuality;
                SetPlanTaskInWarehouse(plantask, plantaskInHouseList);


                //磅点信息
                if (planSiteList != null)
                {
                    if (planSiteList.Count > 0)
                    {
                        _planScaleService.AddPlanTaskBatchSiteScale(plantask.PlanTaskBatches.LastOrDefault(), planSiteList);
                    }
                }

                //内部车辆信息
                if (planvehicleList != null)
                {
                    if (planvehicleList.Count > 0)
                    {
                        _planVehicleService.AddPlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault(), planvehicleList);
                        for (int i = 0; i < planvehicleList.Count; i++)
                        {
                            planvehicleList[i].InnerVehicle.inner_vehicle_online = true;
                            _innervehicle.Update(planvehicleList[i].InnerVehicle);
                        }
                    }
                }

                //码头加卸粮坑
                if (planworkplaceList != null)
                {
                    if (planworkplaceList.Count > 0)
                    {
                        _planWorkPlaceService.AddPlanTaskBatchWorkPlace(plantask.PlanTaskBatches.LastOrDefault(), planworkplaceList);
                    }
                }
                _unitOfWork.AddAction(planTaskBatch, DataActions.Update);
                _planTaskDal.Update(plantask);
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void AddPlanTaskOutWarehouse(PlanTask plantask, List<PlanTaskOutWarehouse> plantaskoutHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> planworkplaceList, string OrgDepCode)
        {
            //PlanTaskOutWarehouse rtnplanouthouse = null;
            try
            {
                SetPlanTaskOutWarehouse(plantask, plantaskoutHouseList);

                //磅点信息
                if (planSiteList != null)
                {
                    if (planSiteList.Count > 0)
                    {
                        _planScaleService.AddPlanTaskBatchSiteScale(plantask.PlanTaskBatches.LastOrDefault(), planSiteList);
                    }
                }

                //内部车辆信息
                if (planvehicleList != null)
                {
                    if (planvehicleList.Count > 0)
                    {
                        _planVehicleService.AddPlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault(), planvehicleList);
                        for (int i = 0; i < planvehicleList.Count; i++)
                        {
                            planvehicleList[i].InnerVehicle.inner_vehicle_online = true;
                            _innervehicle.Update(planvehicleList[i].InnerVehicle);
                        }
                    }
                }

                //码头加卸粮坑
                if (planworkplaceList != null)
                {
                    if (planworkplaceList.Count > 0)
                    {
                        _planWorkPlaceService.AddPlanTaskBatchWorkPlace(plantask.PlanTaskBatches.LastOrDefault(), planworkplaceList);
                    }
                }

                _planTaskDal.Update(plantask);
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            //return rtnplanouthouse;
        }


        public void UpdatePlanTask(PlanTask plantask, List<PlanTaskInWarehouse> plantaskInHouseList, List<PlanTaskOutWarehouse> plantaskoutHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> workplaceList, string OrgDepCode,bool isRun)
        {
            try
            {
                PlanTaskBatch planTaskBatch = plantask.PlanTaskBatches.LastOrDefault();
                //更新出入库仓房
                if (plantaskInHouseList != null && plantaskInHouseList.Count > 0)
                {
                    SetPlanTaskInWarehouse(plantask, plantaskInHouseList);
                }
                if (plantaskoutHouseList != null && plantaskoutHouseList.Count > 0)
                {
                    SetPlanTaskOutWarehouse(plantask, plantaskoutHouseList);
                }

                //磅点信息
                if (planSiteList != null && planSiteList.Count > 0)
                {
                    _planScaleService.UpPlanTaskBatchSiteScale(plantask.PlanTaskBatches.LastOrDefault(), planSiteList);
                }

                //码头加卸粮坑
                if (workplaceList != null && workplaceList.Count > 0)
                {
                    _planWorkPlaceService.UpdatePlanTaskBatchWorkPlace(plantask.PlanTaskBatches.LastOrDefault(), workplaceList);
                }

                PlanTaskStepStatu getplanstep = plantask.PlanTaskStepStatus.Single(d => d.step_code == "作业计划编制" && d.plantask_number == plantask.plantask_number);
                if (getplanstep.operate_time == null)
                {
                    getplanstep.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
                    getplanstep.status = true;
                    _unitOfWork.AddAction(getplanstep, DataActions.Update);
                }

                if (isRun)
                {
                    DoPlanTask(ref plantask, ref planTaskBatch);
                }

                //内部车辆信息，必须放在计划可能执行之后，否则不能拿到计划执行状态，就无法更新车辆在线状态
                if (planvehicleList != null && planvehicleList.Count > 0)
                {
                    _planVehicleService.UpdatePlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault(), planvehicleList);
                }

                _unitOfWork.AddAction(planTaskBatch, DataActions.Update);
                _unitOfWork.AddAction(plantask, DataActions.Update);
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void UpdatePlanTaskUnitOfWordWithoutSave(PlanTask plantask, List<PlanTaskInWarehouse> plantaskInHouseList, List<PlanTaskOutWarehouse> plantaskoutHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> workplaceList, string OrgDepCode, bool isRun)
        {
            try
            {
                PlanTaskBatch planTaskBatch = plantask.PlanTaskBatches.LastOrDefault();
                //更新出入库仓房
                if (plantaskInHouseList != null && plantaskInHouseList.Count > 0)
                {
                    SetPlanTaskInWarehouse(plantask, plantaskInHouseList);
                }
                if (plantaskoutHouseList != null && plantaskoutHouseList.Count > 0)
                {
                    SetPlanTaskOutWarehouse(plantask, plantaskoutHouseList);
                }

                //磅点信息
                if (planSiteList != null && planSiteList.Count > 0)
                {
                    _planScaleService.UpPlanTaskBatchSiteScale(plantask.PlanTaskBatches.LastOrDefault(), planSiteList);
                }

                //码头加卸粮坑
                if (workplaceList != null && workplaceList.Count > 0)
                {
                    _planWorkPlaceService.UpdatePlanTaskBatchWorkPlace(plantask.PlanTaskBatches.LastOrDefault(), workplaceList);
                }

                PlanTaskStepStatu getplanstep = plantask.PlanTaskStepStatus.Single(d => d.step_code == "作业计划编制" && d.plantask_number == plantask.plantask_number);
                if (getplanstep.operate_time == null)
                {
                    getplanstep.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
                    getplanstep.status = true;
                    _unitOfWork.AddAction(getplanstep, DataActions.Update);
                }

                if (isRun)
                {
                    DoPlanTask(ref plantask, ref planTaskBatch);
                }

                //内部车辆信息，必须放在计划可能执行之后，否则不能拿到计划执行状态，就无法更新车辆在线状态
                if (planvehicleList != null && planvehicleList.Count > 0)
                {
                    _planVehicleService.UpdatePlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault(), planvehicleList);
                }

                _unitOfWork.AddAction(planTaskBatch, DataActions.Update);
                _unitOfWork.AddAction(plantask, DataActions.Update);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        private void DoPlanTask(ref PlanTask plantask,ref PlanTaskBatch plantaskbatch)
        {
            string plantaskNumber = plantask.plantask_number;
            //更新计划编辑
            plantask.real_start_time = _sPGetSysDateTimeService.GetSysDateTime();
            if (plantaskbatch.need_bor.HasValue && plantaskbatch.need_bor.Value == true)
            {
                plantask.plan_status = (int)PlanTaskStatus.下达;
            }
            else
            {
                plantask.plan_status = (int)PlanTaskStatus.执行;
            }

            //更新计划批次
            plantaskbatch.batch_start_time = _sPGetSysDateTimeService.GetSysDateTime();

            //更新报港单状态
            if (plantask.PlanTaskType.warehouse_control_mode != (int)WarehouseControlMode.出入仓)
            {
                plantask.Enrolment.status = (int)EnrolmentStatue.执行中;
                _unitOfWork.AddAction(plantask.Enrolment, DataActions.Update);
            }

            PlanTaskStepStatu getplanstep = plantask.PlanTaskStepStatus.Single(d => d.step_code == "作业计划执行" && d.plantask_number == plantaskNumber);
            if (getplanstep.operate_time == null)
            {
                getplanstep.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
                getplanstep.status = true;
                _unitOfWork.AddAction(getplanstep, DataActions.Update);
            }
        }

        public void DoPlanTsakInWarehouse(PlanTask plantask, PlanTaskBatch plantaskbatch, List<PlanTaskInWarehouse> plantaskInHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> planworkplaceList, string OrgDepCode)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                SetPlanTaskInWarehouse(plantask, plantaskInHouseList);

                //磅点信息
                if (planSiteList != null)
                {
                    if (planSiteList.Count > 0)
                    {
                        if (plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchSiteScales.Count > 0)
                        {
                            _planScaleService.DelPlanTaskBatchSiteScale(plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchSiteScales.ToList(), planSiteList);
                        }
                        else
                        {
                            _planScaleService.AddPlanTaskBatchSiteScale(plantask.PlanTaskBatches.LastOrDefault(), planSiteList);
                        }
                    }
                }

                //内部车辆信息
                if (planvehicleList != null)
                {
                    if (planvehicleList.Count > 0)
                    {
                        if (plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchVehicles.Count > 0)
                        {
                            _planVehicleService.DelPlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchVehicles.ToList(), planvehicleList);
                            for (int i = 0; i < planvehicleList.Count; i++)
                            {
                                planvehicleList[i].InnerVehicle.inner_vehicle_online = true;
                                _innervehicle.Update(planvehicleList[i].InnerVehicle);
                            }
                        }
                        else
                        {
                            _planVehicleService.AddPlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault(), planvehicleList);
                            for (int i = 0; i < planvehicleList.Count; i++)
                            {
                                planvehicleList[i].InnerVehicle.inner_vehicle_online = true;
                                _innervehicle.Update(planvehicleList[i].InnerVehicle);
                            }
                        }
                    }
                }
                else
                {
                    _planVehicleService.DelPlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchVehicles.ToList(), planvehicleList);
                }


                //码头加卸粮坑
                if (planworkplaceList != null)
                {
                    if (planworkplaceList.Count > 0)
                    {
                        if (plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchWorkPlaces.Count > 0)
                        {
                            _planWorkPlaceService.DelPlanTaskBatchWorkPlace(plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchWorkPlaces.ToList(), planworkplaceList);
                        }
                        else
                        {
                            _planWorkPlaceService.AddPlanTaskBatchWorkPlace(plantask.PlanTaskBatches.LastOrDefault(), planworkplaceList);
                        }
                    }
                }

                //更新计划编辑
                plantask.real_start_time = _sPGetSysDateTimeService.GetSysDateTime();
                plantask.plan_status = (int)PlanTaskStatus.执行;
                _plantaskService.UpPlantaskWithUnit(plantask);

                //更新计划批次
                plantaskbatch.batch_start_time = _sPGetSysDateTimeService.GetSysDateTime();
                _plantaskbatchService.UpPlantaskBatchWithUnit(plantaskbatch);

                //更新报港单状态
                if (plantask.PlanTaskType.warehouse_control_mode != (int)WarehouseControlMode.出入仓)
                {
                    plantask.Enrolment.status = (int)EnrolmentStatue.执行中;
                    _enrolmentService.UpenrolmentWithUnit(plantask.Enrolment);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }


        public void DoPlanTaskOutWarehouse(PlanTask plantask, PlanTaskBatch plantaskbatch, List<PlanTaskOutWarehouse> plantaskOutHouseList, List<PlanTaskBatchSiteScale> planSiteList, List<PlanTaskBatchVehicle> planvehicleList, List<PlanTaskBatchWorkPlace> planworkplaceList, string OrgDepCode)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                SetPlanTaskOutWarehouse(plantask, plantaskOutHouseList);

                //磅点信息
                if (planSiteList != null)
                {
                    if (planSiteList.Count > 0)
                    {
                        _planScaleService.AddPlanTaskBatchSiteScale(plantask.PlanTaskBatches.LastOrDefault(), planSiteList);
                    }
                }

                //内部车辆信息
                if (planvehicleList != null)
                {
                    if (planvehicleList.Count > 0)
                    {
                        if (plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchVehicles.Count > 0)
                        {
                            _planVehicleService.DelPlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchVehicles.ToList(), planvehicleList);
                            for (int i = 0; i < planvehicleList.Count; i++)
                            {
                                planvehicleList[i].InnerVehicle.inner_vehicle_online = true;
                                _innervehicle.Update(planvehicleList[i].InnerVehicle);
                            }
                        }
                        else
                        {
                            _planVehicleService.AddPlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault(), planvehicleList);
                            for (int i = 0; i < planvehicleList.Count; i++)
                            {
                                planvehicleList[i].InnerVehicle.inner_vehicle_online = true;
                                _innervehicle.Update(planvehicleList[i].InnerVehicle);
                            }
                        }
                    }
                }
                else
                {
                    _planVehicleService.DelPlanTaskBatchVehicle(plantask.PlanTaskBatches.LastOrDefault().PlanTaskBatchVehicles.ToList(), planvehicleList);
                }

                //码头加卸粮坑
                if (planworkplaceList != null)
                {
                    if (planworkplaceList.Count > 0)
                    {
                        _planWorkPlaceService.AddPlanTaskBatchWorkPlace(plantask.PlanTaskBatches.LastOrDefault(), planworkplaceList);
                    }
                }

                //更新计划编辑
                plantask.real_start_time = _sPGetSysDateTimeService.GetSysDateTime();
                plantask.plan_status = (int)PlanTaskStatus.执行;
                _plantaskService.UpPlantaskWithUnit(plantask);

                //更新计划批次
                plantaskbatch.batch_start_time = _sPGetSysDateTimeService.GetSysDateTime();
                _plantaskbatchService.UpPlantaskBatchWithUnit(plantaskbatch);

                //更新报港单状态
                if (plantask.PlanTaskType.warehouse_control_mode != (int)WarehouseControlMode.出入仓)
                {
                    plantask.Enrolment.status = (int)EnrolmentStatue.执行中;
                    _enrolmentService.UpenrolmentWithUnit(plantask.Enrolment);
                }

                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void AddPlanTaskInWarehouseWithChangeWarehouse(PlanTaskInWarehouse planTaskInWarehouse)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                _unitOfWork.AddAction(planTaskInWarehouse, DataActions.Add);
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void AddPlanTaskOutWarehouseWithChangeWarehouse(PlanTaskOutWarehouse planTaskOutWarehouse)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                _unitOfWork.AddAction(planTaskOutWarehouse, DataActions.Add);
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="plan_task_id"></param>
        /// <param name="invmas_id"></param>
        /// <returns></returns>
        public IEnumerable<PlanTaskInWarehouse> GetPlanTaskInWarehouseListWithBatch(string plantask_batch_number)
        {
            return _planTaskInWarehouseDal.GetAll().Entities.Where(d => d.plantask_batch_number == plantask_batch_number);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plantask_batch_number"></param>
        /// <returns></returns>
        public IEnumerable<PlanTaskOutWarehouse> GetPlanTaskOutWarehouseListWithBatch(string plantask_batch_number)
        {
            return _planTaskOutWarehouseDal.GetAll().Entities.Where(d => d.plantask_batch_number == plantask_batch_number);

        }

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



                //更新计划编辑
                plantask.real_start_time = _sPGetSysDateTimeService.GetSysDateTime();
                plantask.plan_status = (int)PlanTaskStatus.执行;
                _unitOfWork.AddAction(plantask, DataActions.Update);

                //更新计划批次
                plantaskbatch.batch_start_time = _sPGetSysDateTimeService.GetSysDateTime();
                _unitOfWork.AddAction(plantaskbatch, DataActions.Update);
                #endregion
            }

            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void RefreshData()
        {
            _planTaskInWarehouseDal.RefreshData();
            _planTaskOutWarehouseDal.RefreshData();
            _planVehicleService.RefreshData();
            _planWorkPlaceService.RefreshData();
            _innervehicle.RefreshData();
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
                    if (!oldPlanTaskInWarehouse.Any(d => d.plantask_batch_number == planTaskInWarehouse.plantask_batch_number && d.warehouse_id == planTaskInWarehouse.warehouse_id && d.vehicle_id ==planTaskInWarehouse.vehicle_id))
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
                foreach (PlanTaskInWarehouse planTaskInWarehouseRemove in oldPlanTaskInWarehouse.Where(x => !plantaskInHouseList.Any(u => u.warehouse_id == x.warehouse_id && u.plantask_batch_number == x.plantask_batch_number&&u.vehicle_id ==x.vehicle_id)).ToList())
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
                    if (!oldPlanTaskOutWarehouse.Any(d => d.plantask_batch_number == planTaskOutWarehouse.plantask_batch_number && d.warehouse_id == planTaskOutWarehouse.warehouse_id&&d.vehicle_id ==planTaskOutWarehouse.vehicle_id))
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
                foreach (PlanTaskOutWarehouse planTaskOutWarehouseRemove in oldPlanTaskOutWarehouse.Where(x => !plantaskOutHouseList.Any(u => u.warehouse_id == x.warehouse_id && u.plantask_batch_number == x.plantask_batch_number&&u.vehicle_id==x.vehicle_id)).ToList())
                {
                    _unitOfWork.AddAction(planTaskOutWarehouseRemove, DataActions.Delete);
                }
            }
        }
        #endregion
    }
}
