using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Service.BaseManager;
using Aisino.MES.Service.ManuManager;
using Aisino.MES.Service.QualityManager;
using Aisino.MES.Service.SysManager;
using Aisino.MES.Service.BusinessManager;
using Aisino.MES.Service.WarehouseManager;
using Aisino.MES.Service.EnrolmentManager;
using Aisino.MES.Service.StoreProcedures;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;
using System.Linq.Expressions;
using Aisino.MES.DAL.Repository.Helpers;

namespace Aisino.MES.Service.QuantityManager.Impl
{
    public class LoadometerManagerService : ILoadometerManagerService
    {
        #region unity加载Service，含构造函数
        private IRFIDTagIssueService _rfidTagIssueService;
        private IRFIDTagService _rfidTagService;
        private IPlanTaskBatchService _planTaskBatchService;
        private IWarehouseBaseService _warehouseBaseService;
        private IBaseManageService _baseManageService;
        private ISampleService _sampleService;
        private IAssayService _assayService;
        private IPlanTaskService _planTaskService;
        private IBusinessDisposeService _businessDisposeService;
        private ISysDepartmentUserService _sysDepartmentUserService;
        private ISysMenuService _sysMenuService;
        private IEnrolmentService _enrolmentService;
        private IPlanTaskBatchSiteScaleService _planTaskBatchSiteScaleService;
        private IPlanTaskInOutWarehouseService _planTaskInOutWarehouseService;
        private SPGetSysDateTimeService _sPGetSysDateTimeService;
        private Repository<QuantityRecordHead> _quantityHeadDal;
        private Repository<QuantityRecordDetail> _quantityDetailDal;
        private UnitOfWork _unitOfWork;

        public LoadometerManagerService(IRFIDTagService rfidtagService, IRFIDTagIssueService rfidTagIssueService, IPlanTaskBatchService planTaskBatchService,
            IWarehouseBaseService warehouseBaseService, IBaseManageService baseManageService, ISampleService sampleService,
            IAssayService assayService, IPlanTaskService planTaskService, IBusinessDisposeService businessDisposeService,
            ISysDepartmentUserService sysDepartmentUserService, ISysMenuService sysMenuService, IEnrolmentService enrolmentService,
            IPlanTaskBatchSiteScaleService planTaskBatchSiteScaleService, IPlanTaskInOutWarehouseService planTaskInOutWarehouseService,
            SPGetSysDateTimeService sPGetSysDateTimeService, Repository<QuantityRecordHead> quantityHeadDal, Repository<QuantityRecordDetail> quantityDetailDal,
            UnitOfWork unitOfWork)
        {
            this._rfidTagService = rfidtagService;
            this._rfidTagIssueService = rfidTagIssueService;
            this._planTaskBatchService = planTaskBatchService;
            this._warehouseBaseService = warehouseBaseService;
            this._baseManageService = baseManageService;
            this._sampleService = sampleService;
            this._assayService = assayService;
            this._planTaskService = planTaskService;
            this._businessDisposeService = businessDisposeService;
            this._sysDepartmentUserService = sysDepartmentUserService;
            this._sysMenuService = sysMenuService;
            this._enrolmentService = enrolmentService;
            this._planTaskBatchSiteScaleService = planTaskBatchSiteScaleService;
            this._planTaskInOutWarehouseService = planTaskInOutWarehouseService;
            _sPGetSysDateTimeService = sPGetSysDateTimeService;
            this._quantityHeadDal = quantityHeadDal;
            this._quantityDetailDal = quantityDetailDal;
            this._unitOfWork = unitOfWork;
        }
        #endregion

        #region 实现接口方法
        public CheckResult CheckVehicleWeight(string VehicleRFIDTag, string TagType, string ScaleID)
        {
            CheckResult checkResult = new CheckResult();
            try
            {
                RFIDTag rfidTagTemp = null;

                if (TagType == "mainid")
                {
                    rfidTagTemp = _rfidTagService.GetRFIDTagByMainId(VehicleRFIDTag);
                }
                else
                {
                    rfidTagTemp = _rfidTagService.GetRFIDTagBySubId(VehicleRFIDTag);
                }
                if (rfidTagTemp != null)
                {
                    InnerVehicle innerVehicle = _rfidTagIssueService.FindVehicleRFIDTagIssueByCode(rfidTagTemp.tag_main_id);
                    if (innerVehicle != null)
                    {
                        //内部车辆用卡                                         
                        PlanTaskBatch planTaskBatch = _planTaskBatchService.GetRunningPlanTaskBatchByInnerVehicle(innerVehicle.inner_vehicle_id);
                        if (planTaskBatch != null)
                        {
                            checkResult = CheckCanOnDB(planTaskBatch.PlanTask.Enrolment, planTaskBatch, innerVehicle, ScaleID);
                        }
                        else
                        {
                            checkResult.ValidResult = false;
                            checkResult.InvalidReason = DBReason.NOTPLAN;
                        }
                    }
                    else
                    {
                        Enrolment enrolment = _rfidTagIssueService.FindRunnigRFIDTagIssueByCode(rfidTagTemp.tag_main_id);
                        //报港用卡
                        if (enrolment == null)
                        {
                            checkResult.ValidResult = false;
                            checkResult.InvalidReason = DBReason.NOTENROLMENT;
                        }
                        else
                        {
                            if (enrolment.EnrolmentBasicTypeTraffic.isoutcar)
                            {
                                checkResult = CheckCanOnDB(enrolment, enrolment.PlanTasks.Last().PlanTaskBatches.Last(), innerVehicle, ScaleID);
                            }
                            else
                            {
                                //船运则不可使用
                                checkResult.ValidResult = false;
                                checkResult.InvalidReason = DBReason.ISSHIP;
                            }
                        }
                    }
                }
                else
                {
                    checkResult.ValidResult = false;
                    checkResult.InvalidReason = DBReason.CARDNOTFOUND;
                }
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("判定车辆上磅出错", ex);
            }
            finally
            {
            }

            return checkResult;
        }

        public QuantityRecordHead UpdateQuantity(string tagId, string plantaskBatchNumber, int plantaskBatchId, int warehouseId, string plateNumber, int weightMode, string scaleId, int weight, int weightType, int operatorUser, DateTime weightTime, ref string strTime)
        {
            strTime += "步骤二01：" + DateTime.Now.ToString() + ";                                   ";
            QuantityRecordHead quantityHead = GetLastQuantityHeadByTagIdAndPlanNumber(tagId, plantaskBatchNumber);
            strTime += "步骤二02：" + DateTime.Now.ToString() + ";                                   ";
            QuantityRecordDetail quantityDetail = new QuantityRecordDetail();
            if (quantityHead == null)
            {
                //没有相应的称重磅单，则需要新增
                quantityHead = new QuantityRecordHead();
                quantityHead.is_cancel = false;
                quantityHead.plate_number = plateNumber;
                quantityHead.tag_id = tagId;
                quantityHead.weight_mode = weightMode;
                quantityHead.warehouse_id = warehouseId;

                _unitOfWork.AddAction(quantityHead, DAL.Enums.DataActions.Add);

                //同时新建第一个称重单
                quantityDetail.is_error = true;
                quantityDetail.operator_user = operatorUser;
                quantityDetail.quantity_record_head_id = quantityHead.id;
                quantityDetail.scale_id = scaleId;
                quantityDetail.weight = weight;
                quantityDetail.is_sync = false;
                if (weightTime == DateTime.MinValue)
                {
                    quantityDetail.weight_time = _sPGetSysDateTimeService.GetSysDateTime();
                }
                else
                {
                    quantityDetail.weight_time = weightTime;
                }
                quantityDetail.weight_type = weightType;

                _unitOfWork.AddAction(quantityDetail, DAL.Enums.DataActions.Add);
            }
            else
            {
                //有相应的磅单，但是还没有产生重量，则缺少一个称重记录
                quantityDetail.is_error = true;
                quantityDetail.operator_user = operatorUser;
                quantityDetail.quantity_record_head_id = quantityHead.id;
                quantityDetail.scale_id = scaleId;
                quantityDetail.weight = weight;
                quantityDetail.is_sync = false;
                if (weightTime == DateTime.MinValue)
                {
                    quantityDetail.weight_time = _sPGetSysDateTimeService.GetSysDateTime();
                }
                else
                {
                    quantityDetail.weight_time = weightTime;
                }
                quantityDetail.weight_type = weightType;

                _unitOfWork.AddAction(quantityDetail, DAL.Enums.DataActions.Add);

                if (weightType == (int)WeightType.毛重)
                {
                    //当前重量为毛重，则净重需要减去上一个重量
                    quantityHead.weight_amount = weight - quantityHead.QuantityRecordDetails.Last().weight;
                }
                else
                {
                    quantityHead.weight_amount = quantityHead.QuantityRecordDetails.Last().weight - weight;
                }
                quantityHead.warehouse_id = warehouseId;

                _unitOfWork.AddAction(quantityHead, DAL.Enums.DataActions.Update);
            }
            _unitOfWork.Save();
            strTime += "步骤二03：" + DateTime.Now.ToString() + ";                                   ";

            //保存称重记录后再调用上传称重记录，防止更新出错
            UploadDataResult udr = UploadVehicleWeighInfo(quantityDetail, plantaskBatchId, ref strTime);
            ////if (udr.ResponseResult)
            ////{
            ////    //更新计划内容成功，则更新称重记录
            ////    PlanTaskBatchDetail ptbd = _planTaskBatchService.GetPlanTaskBatchDetailByScaleBillNumber(quantityHead.id.ToString());
            ////    quantityHead.plantask_batch_detail_number = ptbd.plantask_batch_detail_number;

            ////    quantityDetail.is_error = false;
            ////    quantityDetail.is_sync = true;

            ////    _unitOfWork.AddAction(quantityHead, DAL.Enums.DataActions.Update);
            ////    _unitOfWork.AddAction(quantityDetail, DAL.Enums.DataActions.Update);
            ////    _unitOfWork.Save();
            ////}
            return quantityHead;
        }

        public UploadDataResult CancelVehicleScaleBill(int ScaleBillNumber)
        {
            string strErrorMessage = string.Empty;
            bool blResult = false;

            try
            {
                QuantityRecordHead quantityHead = _quantityHeadDal.Single(qh => qh.id == ScaleBillNumber).Entity;
                if (quantityHead != null)
                {
                    quantityHead.is_cancel = true;
                    _unitOfWork.AddAction(quantityHead, DAL.Enums.DataActions.Update);
                    _planTaskService.CancelVehicleScaleBill(ScaleBillNumber.ToString(), ref strErrorMessage);

                    _unitOfWork.Save();
                }
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("磅单撤销出错", ex);
            }

            UploadDataResult uploadDataResult = new UploadDataResult();
            uploadDataResult.ResponseResult = blResult;
            uploadDataResult.FailedReason = strErrorMessage;

            return uploadDataResult;
        }

        public IEnumerable<QuantityRecordHead> FindQuantityHeadByDateAndScale(DateTime weightDate, string scaleId, bool withCancel)
        {
            RepositoryResultList<QuantityRecordHead> quantityHeadResultList;
            if (withCancel == true)
            {
                //含有废除的磅单，则不作为条件进入
                quantityHeadResultList = _quantityHeadDal.Find(qh => qh.QuantityRecordDetails.Any(qd => qd.weight_time.Date == weightDate.Date && qd.scale_id == scaleId), new string[] { "QuantityRecordDetails" });
            }
            else
            {
                quantityHeadResultList = _quantityHeadDal.Find(qh => qh.is_cancel == false && qh.QuantityRecordDetails.Any(qd => qd.weight_time.Date == weightDate.Date && qd.scale_id == scaleId), new string[] { "QuantityRecordDetails" });
            }

            if (quantityHeadResultList.Entities != null && quantityHeadResultList.Entities.Count() > 0)
            {
                return quantityHeadResultList.Entities.OrderBy(qh => qh.is_cancel);
            }
            else
            {
                return null;
            }
        }

        public QuantityRecordHead GetQuantityHeadByID(int id)
        {
            RepositoryResultSingle<QuantityRecordHead> singleQuantityHead = _quantityHeadDal.Single(qh => qh.id == id, new string[] { "QuantityRecordDetails" });
            if (singleQuantityHead.HasValue)
            {
                return singleQuantityHead.Entity;
            }
            else
            {
                return null;
            }
        }

        public RepositoryResultList<QuantityRecordHead> GetQuantityHeadByBatchNums(String[] PlanTaskBatchNums)
        {
            string strSql = "Select * from QuantityRecordHead where 1 = 1 ";
            string strWhere = string.Empty;
            foreach (string planTaskBatchNum in PlanTaskBatchNums)
            {
                if (strWhere.Trim().Length == 0)
                {
                    strWhere = "and (plantask_batch_detail_number like '" + planTaskBatchNum + "%'";
                    continue;
                }
                strWhere = strWhere + " or plantask_batch_detail_number like '" + planTaskBatchNum + "%'";
            }
            if (strWhere.Trim().Length > 0)
            {
                strWhere = strWhere + ")";
            }
            return _quantityHeadDal.QueryByESql(strSql + strWhere + "Order By plantask_batch_detail_number Asc");

            //var queryHead = PredicateBuilder.True<QuantityRecordHead>();
            //foreach (string planTaskBatchNum in PlanTaskBatchNums)
            //{
            //    queryHead = queryHead.And(wh => wh.plantask_batch_detail_number.Contains(planTaskBatchNum));
            //}
            //return _quantityHeadDal.Find(queryHead, new string[] { "QuantityRecordDetails" });
        }
        #endregion

        #region 内部方法
        private CheckResult CheckCanOnDB(Enrolment enrolment, PlanTaskBatch planTaskBatch, InnerVehicle innerVehicle, string ScaleID)
        {
            CheckResult checkResult = new CheckResult();
            try
            {
                #region 外部车辆
                if (innerVehicle == null)
                {
                    //外部车运
                    if (enrolment.status == (int)EnrolmentStatue.执行中)
                    {
                        PlanTask planTask = enrolment.PlanTasks.FirstOrDefault();
                        if (planTask.plan_status == (int)PlanTaskStatus.执行 || planTask.plan_status == (int)PlanTaskStatus.下达)
                        {
                            //获取当前计划的最后一条执行批次信息
                            PlanTaskBatch lastPlanTaskBatch = _planTaskBatchService.GetRunningPlanTaskBatchByPlanTaskNumber(planTask.plantask_number);
                            PlanTaskBatchDetail lastPlanTaskBatchDetail = null;
                            if (lastPlanTaskBatch.PlanTaskBatchDetails != null && lastPlanTaskBatch.PlanTaskBatchDetails.Count > 0)
                            {
                                IList<PlanTaskBatchDetail> ptbds = lastPlanTaskBatch.PlanTaskBatchDetails.Where(w => w.bill_status != 0).ToList();
                                if (ptbds != null && ptbds.Count > 0)
                                {
                                    lastPlanTaskBatchDetail = ptbds.Last();
                                }
                                else
                                {
                                    lastPlanTaskBatchDetail = null;
                                }
                            }
                            if (planTaskBatch.PlanTaskBatchSiteScales != null && planTaskBatch.PlanTaskBatchSiteScales.Where(s => s.SiteScale.scale_id.ToString() == ScaleID).Count() == 0)
                            {
                                //与计划批次相关磅秤不为空，且当前传入磅秤号不在其中
                                //如果相关磅秤为空，则默认为所有磅秤皆可使用，所以不进行磅秤判断
                                checkResult.ValidResult = false;
                                checkResult.InvalidReason = DBReason.SCALEERROR;
                            }
                            else if ((lastPlanTaskBatch.PlanTaskBatchDetails == null || lastPlanTaskBatch.PlanTaskBatchDetails.Count == 0) && lastPlanTaskBatch.PlanTask.Enrolment.need_quality_value)
                            {
                                if (planTaskBatch.Samples != null && planTaskBatch.Samples.Count > 0 && planTaskBatch.Samples.Last().Assays.Last().assay_result == null)
                                {                                    
                                    checkResult.ValidResult = false;
                                    checkResult.InvalidReason = DBReason.WAITASSAY;
                                }
                                else if (planTaskBatch.Samples != null && planTaskBatch.Samples.Count > 0 && planTaskBatch.Samples.Last().Assays.Last().assay_result == 1)
                                {                                    
                                    //正在执行的外部车辆出入库作业计划
                                    //不需要值仓或者该计划批次还没有出入库明细产生的情况下
                                    //都可以上磅，上磅过程中毛皮重量有系统自行判断
                                    checkResult.ValidResult = true;
                                    checkResult.ResultWorkInfo = BuildOutVehicleEnrollInfo(enrolment, lastPlanTaskBatch, lastPlanTaskBatch.PlanTask);
                                    checkResult.EnrolmentInfo = enrolment;
                                    checkResult.PlantaskBatchInfo = planTaskBatch;
                                }
                                else if (planTaskBatch.Samples != null && planTaskBatch.Samples.Count > 0 && planTaskBatch.Samples.Last().Assays.Last().assay_result == 2)
                                {
                                    checkResult.ValidResult = false;
                                    checkResult.InvalidReason = DBReason.ASSAYNOTPASS;
                                }
                                else
                                {
                                    checkResult.ValidResult = false;
                                    checkResult.InvalidReason = DBReason.WAITASSAY;
                                }
                            }
                            else if (lastPlanTaskBatchDetail == null && lastPlanTaskBatch.PlanTask.Enrolment.need_quality_value == false)
                            {
                                //正在执行的外部车辆出入库作业计划
                                //不需要值仓或者该计划批次还没有出入库明细产生的情况下
                                //都可以上磅，上磅过程中毛皮重量有系统自行判断
                                checkResult.ValidResult = true;
                                checkResult.ResultWorkInfo = BuildOutVehicleEnrollInfo(enrolment, lastPlanTaskBatch, lastPlanTaskBatch.PlanTask);
                                checkResult.EnrolmentInfo = enrolment;
                                checkResult.PlantaskBatchInfo = planTaskBatch;
                            }
                            else
                            {
                                //获取最后一笔入库明细记录
                                
                                PlanTaskType playTaskType = lastPlanTaskBatch.PlanTask.Enrolment.EnrolmentType.PlanTaskType;
                                if (enrolment.inout_type == (int)InOutType.入库)
                                {
                                    if (planTaskBatch.PlanTaskInWarehouses == null || planTaskBatch.PlanTaskInWarehouses.Count == 0)
                                    {                                        
                                        checkResult.ValidResult = false;
                                        checkResult.InvalidReason = DBReason.NOTWAREHOUSE;
                                    }
                                    //外部车辆入库，需要值仓
                                    //如果有毛重并且已经确认卸货就能上磅
                                    else if ((planTaskBatch.need_onduty != null && planTaskBatch.need_onduty == true)
                                        && lastPlanTaskBatchDetail != null && lastPlanTaskBatchDetail.gross_time != null && lastPlanTaskBatchDetail.tare_time == null 
                                        && lastPlanTaskBatchDetail.duty_confirm != null && lastPlanTaskBatchDetail.duty_confirm.Value != 0)
                                    {                                    
                                        checkResult.ValidResult = true;
                                        checkResult.ResultWorkInfo = BuildOutVehicleEnrollInfo(enrolment, lastPlanTaskBatch, planTaskBatch.PlanTask);
                                        checkResult.EnrolmentInfo = enrolment;
                                        checkResult.PlantaskBatchInfo = planTaskBatch;
                                    }
                                    else if ((planTaskBatch.need_onduty != null && planTaskBatch.need_onduty == true)
                                        && lastPlanTaskBatchDetail != null && lastPlanTaskBatchDetail.gross_time != null && lastPlanTaskBatchDetail.tare_time == null
                                        && (lastPlanTaskBatchDetail.duty_confirm == null || lastPlanTaskBatchDetail.duty_confirm == null || lastPlanTaskBatchDetail.duty_confirm.Value == 0))
                                    {
                                        checkResult.ValidResult = false;
                                        checkResult.InvalidReason = DBReason.NOTUNLOAD;
                                    }
                                    else if ((planTaskBatch.PlanTask.series_work == null || planTaskBatch.PlanTask.series_work == false)
                                        && lastPlanTaskBatchDetail != null && lastPlanTaskBatchDetail.gross_time != null && lastPlanTaskBatchDetail.tare_time != null)
                                    {
                                        checkResult.ValidResult = false;
                                        checkResult.InvalidReason = DBReason.SERIESNOTPASS;
                                    }
                                    else
                                    {
                                        checkResult.ValidResult = true;
                                        checkResult.ResultWorkInfo = BuildOutVehicleEnrollInfo(enrolment, lastPlanTaskBatch, planTaskBatch.PlanTask);
                                        checkResult.EnrolmentInfo = enrolment;
                                        checkResult.PlantaskBatchInfo = planTaskBatch;
                                    }
                                }
                                else
                                {
                                    if (planTaskBatch.PlanTaskOutWarehouses == null || planTaskBatch.PlanTaskOutWarehouses.Count == 0)
                                    {
                                        checkResult.ValidResult = false;
                                        checkResult.InvalidReason = DBReason.NOTWAREHOUSE;
                                    }
                                    //外部车辆出库，需要值仓
                                    //如果有皮重并且已经确认卸货就能上磅                                    
                                    if ((playTaskType.need_onduty != null && playTaskType.need_onduty == true)
                                       && lastPlanTaskBatchDetail != null && lastPlanTaskBatchDetail.tare_time != null && lastPlanTaskBatchDetail.duty_confirm != null && (lastPlanTaskBatchDetail.duty_confirm.Value != 0))
                                    {
                                        checkResult.ValidResult = true;
                                        checkResult.ResultWorkInfo = BuildOutVehicleEnrollInfo(enrolment, lastPlanTaskBatch, planTaskBatch.PlanTask);
                                        checkResult.EnrolmentInfo = enrolment;
                                        checkResult.PlantaskBatchInfo = planTaskBatch;
                                    }
                                    else if ((planTaskBatch.need_onduty != null && planTaskBatch.need_onduty == true)
                                        && lastPlanTaskBatchDetail != null && lastPlanTaskBatchDetail.tare_time != null
                                        && (lastPlanTaskBatchDetail.duty_confirm == null || lastPlanTaskBatchDetail.duty_confirm == null || lastPlanTaskBatchDetail.duty_confirm.Value == 0))
                                    {
                                        checkResult.ValidResult = false;
                                        checkResult.InvalidReason = DBReason.NOTUNLOAD;
                                    }
                                    else if ((planTaskBatch.PlanTask.series_work == null || planTaskBatch.PlanTask.series_work == false)
                                        && lastPlanTaskBatchDetail != null && lastPlanTaskBatchDetail.gross_time != null && lastPlanTaskBatchDetail.tare_time != null)
                                    {
                                        checkResult.ValidResult = false;
                                        checkResult.InvalidReason = DBReason.SERIESNOTPASS;
                                    }
                                    else
                                    {
                                        checkResult.ValidResult = true;
                                        checkResult.ResultWorkInfo = BuildOutVehicleEnrollInfo(enrolment, lastPlanTaskBatch, planTaskBatch.PlanTask);
                                        checkResult.EnrolmentInfo = enrolment;
                                        checkResult.PlantaskBatchInfo = lastPlanTaskBatch;
                                    }
                                }
                            }
                        }
                        else
                        {
                            checkResult.ValidResult = false;
                            checkResult.InvalidReason = DBReason.NOTPROGRESS;
                        }
                    }
                    else
                    {
                        checkResult.ValidResult = false;
                        checkResult.InvalidReason = DBReason.NOTPROGRESS;
                    }
                }
                #endregion

                #region 内部车辆 出库/入库
                else if (enrolment != null && innerVehicle != null)
                {
                    //内部车
                    if (planTaskBatch.PlanTask.plan_status == (int)PlanTaskStatus.执行 || planTaskBatch.PlanTask.plan_status == (int)PlanTaskStatus.下达)
                    {
                        PlanTaskBatchDetail lastPlanTaskBatchDetail = planTaskBatch.PlanTaskBatchDetails.
                               Where(ptbd => ptbd.InnerVehicle.inner_vehicle_id == innerVehicle.inner_vehicle_id && ptbd.weight == null && ptbd.bill_status != 0).LastOrDefault();

                        if (planTaskBatch.PlanTaskBatchSiteScales != null && planTaskBatch.PlanTaskBatchSiteScales.Where(s => s.SiteScale.scale_id.ToString() == ScaleID).Count() == 0)
                        {
                            //与计划批次相关磅秤不为空，且当前传入磅秤号不在其中
                            //如果相关磅秤为空，则默认为所有磅秤皆可使用，所以不进行磅秤判断
                            checkResult.ValidResult = false;
                            checkResult.InvalidReason = DBReason.SCALEERROR;
                        }
                        else if (enrolment.inout_type == (int)InOutType.入库 && (planTaskBatch.PlanTaskInWarehouses == null || planTaskBatch.PlanTaskInWarehouses.Count == 0))
                        {
                            checkResult.ValidResult = false;
                            checkResult.InvalidReason = DBReason.NOTWAREHOUSE;
                        }
                        else if (enrolment.inout_type == (int)InOutType.出库 && (planTaskBatch.PlanTaskOutWarehouses == null || planTaskBatch.PlanTaskOutWarehouses.Count == 0))
                        {
                            checkResult.ValidResult = false;
                            checkResult.InvalidReason = DBReason.NOTWAREHOUSE;
                        }
                        else if (planTaskBatch.need_onduty.Value == false)
                        {
                            //正在执行的出入库作业计划
                            //不需要值仓或者该计划批次还没有出入库明细产生的情况下
                            //都可以上磅，上磅过程中毛皮重量有系统自行判断
                            checkResult.ValidResult = true;
                            checkResult.ResultWorkInfo = BuildInVehicleEnrollInfo(enrolment, planTaskBatch, innerVehicle, lastPlanTaskBatchDetail);
                            checkResult.EnrolmentInfo = enrolment;
                            checkResult.PlantaskBatchInfo = planTaskBatch;
                        }
                        else if (planTaskBatch.need_onduty.Value == true)
                        {
                            //获取对应车辆最后一笔没有净重的明细

                            //内部车辆，入库需要值仓
                            //如果有相应的数据，则说明已经有毛重或皮重，则还需要判定已经确认卸货就能上磅
                            if (lastPlanTaskBatchDetail != null && (lastPlanTaskBatchDetail.duty_confirm != null && lastPlanTaskBatchDetail.duty_confirm.Value != 0))
                            {
                                checkResult.ValidResult = true;
                                checkResult.ResultWorkInfo = BuildInVehicleEnrollInfo(enrolment, planTaskBatch, innerVehicle, lastPlanTaskBatchDetail);
                                checkResult.EnrolmentInfo = enrolment;
                                checkResult.PlantaskBatchInfo = planTaskBatch;
                            }
                            else if (lastPlanTaskBatchDetail != null && (lastPlanTaskBatchDetail.duty_confirm == null || lastPlanTaskBatchDetail.duty_confirm.Value == 0))
                            {
                                if (enrolment.inout_type == (int)InOutType.入库 && planTaskBatch.PlanTask.weight_mode == (int)WeightMode.先皮后毛 && lastPlanTaskBatchDetail.weight_time != null)
                                {
                                    checkResult.ValidResult = false;
                                    checkResult.InvalidReason = DBReason.NOTUNLOAD;
                                }
                                else if (enrolment.inout_type == (int)InOutType.入库 && planTaskBatch.PlanTask.weight_mode == (int)WeightMode.先毛后皮 && lastPlanTaskBatchDetail.tare_time == null)
                                {
                                    checkResult.ValidResult = false;
                                    checkResult.InvalidReason = DBReason.NOTUNLOAD;
                                }
                                else if (enrolment.inout_type == (int)InOutType.出库 && planTaskBatch.PlanTask.weight_mode == (int)WeightMode.先皮后毛 && lastPlanTaskBatchDetail.gross_time == null)
                                {
                                    checkResult.ValidResult = false;
                                    checkResult.InvalidReason = DBReason.NOTUNLOAD;
                                }
                                else
                                {
                                    checkResult.ValidResult = true;
                                    checkResult.ResultWorkInfo = BuildInVehicleEnrollInfo(enrolment, planTaskBatch, innerVehicle, lastPlanTaskBatchDetail);
                                }
                            }
                            else if (lastPlanTaskBatchDetail == null)
                            {
                                if (enrolment.inout_type == (int)InOutType.入库 && planTaskBatch.PlanTask.weight_mode == (int)WeightMode.先皮后毛)
                                {
                                    PlanTaskBatchDetail lastPlanTaskBatchDetailConfig = planTaskBatch.PlanTaskBatchDetails.Where(ptbd => ptbd.InnerVehicle.inner_vehicle_id == innerVehicle.inner_vehicle_id && ptbd.bill_status != 0).LastOrDefault();
                                    if (lastPlanTaskBatchDetailConfig != null &&
                                        (lastPlanTaskBatchDetailConfig.duty_confirm == null ||
                                        lastPlanTaskBatchDetailConfig.duty_confirm == 0))
                                    {
                                        checkResult.ValidResult = false;
                                        checkResult.InvalidReason = DBReason.NOTUNLOAD;
                                    }
                                    else
                                    {
                                        checkResult.ValidResult = true;
                                        checkResult.ResultWorkInfo = BuildInVehicleEnrollInfo(enrolment, planTaskBatch, innerVehicle, lastPlanTaskBatchDetailConfig);
                                        checkResult.EnrolmentInfo = enrolment;
                                        checkResult.PlantaskBatchInfo = planTaskBatch;
                                    }
                                }
                                else
                                {
                                    checkResult.ValidResult = true;
                                    checkResult.ResultWorkInfo = BuildInVehicleEnrollInfo(enrolment, planTaskBatch, innerVehicle, lastPlanTaskBatchDetail);
                                    checkResult.EnrolmentInfo = enrolment;
                                    checkResult.PlantaskBatchInfo = planTaskBatch;
                                }
                            }
                        }
                    }
                    else
                    {
                        checkResult.ValidResult = false;
                        checkResult.InvalidReason = DBReason.NOTPROGRESS;
                    }
                }
                #endregion

                #region 内部车辆 倒仓
                else if (enrolment == null && innerVehicle != null)
                {
                    //内部车
                    if (planTaskBatch.PlanTask.plan_status == (int)PlanTaskStatus.执行)
                    {
                        PlanTaskBatchDetail lastPlanTaskBatchDetail = planTaskBatch.PlanTaskBatchDetails.
                               Where(ptbd => ptbd.InnerVehicle.inner_vehicle_id == innerVehicle.inner_vehicle_id && ptbd.weight == null && ptbd.bill_status != 0).LastOrDefault();

                        if (planTaskBatch.PlanTaskBatchSiteScales != null && planTaskBatch.PlanTaskBatchSiteScales.Where(s => s.SiteScale.scale_id.ToString() == ScaleID).Count() == 0)
                        {
                            //与计划批次相关磅秤不为空，且当前传入磅秤号不在其中
                            //如果相关磅秤为空，则默认为所有磅秤皆可使用，所以不进行磅秤判断
                            checkResult.ValidResult = false;
                            checkResult.InvalidReason = DBReason.SCALEERROR;
                        }
                        else
                        {
                            checkResult.ValidResult = true;
                            checkResult.ResultWorkInfo = BuildInVehicleEnrollInfo(enrolment, planTaskBatch, innerVehicle, lastPlanTaskBatchDetail);
                            checkResult.EnrolmentInfo = enrolment;
                            checkResult.PlantaskBatchInfo = planTaskBatch;
                        }
                    }
                    else
                    {
                        checkResult.ValidResult = false;
                        checkResult.InvalidReason = DBReason.NOTPROGRESS;
                    }
                }
                #endregion
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("判断车辆上磅出错", ex);
            }
            return checkResult;
        }

        private WorkInfo BuildOutVehicleEnrollInfo(Enrolment enrolment, PlanTaskBatch planTaskBatch, PlanTask planTask)
        {
            WorkInfo workInfo = new WorkInfo();

            if (enrolment.carrier_name != null)
            {
                workInfo.VehicleDriver = enrolment.carrier_name;
            }
            else
            {
                workInfo.VehicleDriver = string.Empty;
            }

            if (planTask.is_outvehicle)
            {
                if (enrolment.plate_number != null)
                {
                    workInfo.VehiclePlate = enrolment.plate_number;
                }
                else
                {
                    workInfo.VehiclePlate = string.Empty;
                }
                workInfo.ShipPlate = string.Empty;
            }
            else
            {
                if (enrolment.plate_number != null)
                {
                    workInfo.ShipPlate = enrolment.plate_number;
                }
                else
                {
                    workInfo.ShipPlate = string.Empty;
                }
                workInfo.VehiclePlate = string.Empty;
            }

            //if (enrolment.RFIDTagIssues.FirstOrDefault() != null)
            //{
            //    workInfo.VehicleTag = enrolment.RFIDTagIssues.FirstOrDefault().RFIDTag.tag_main_id;
            //}
            //else
            //{
            //    vehicleInfo.VehicleTag = string.Empty;
            //}

            //vehicleInfo.VehicleType = (int)VehicleType.外部车;

            workInfo.ApprovedWeight = 0;

            if (planTask.PlanTaskType.warehouse_control_mode != null)
            {
                workInfo.BusinessType = planTask.PlanTaskType.warehouse_control_mode.Value;
            }

            if (planTask.plan_weight != null)
            {
                workInfo.CompletedWeight = planTask.plan_weight.Value;
            }

            if (planTaskBatch.unit_price != null)
            {
                workInfo.Price = (double)planTaskBatch.unit_price.Value;
            }

            workInfo.WarnFlag = 0;

            workInfo.WorkNumber = planTaskBatch.generate_id.ToString();

            PlanTaskBatchDetail lastPlanTaskBatchDetail = null;
            if (planTaskBatch.PlanTaskBatchDetails != null && planTaskBatch.PlanTaskBatchDetails.Count > 0)
            {
                IList<PlanTaskBatchDetail> ptbds = planTaskBatch.PlanTaskBatchDetails.Where(w => w.bill_status != 0).ToList();
                if (ptbds != null && ptbds.Count > 0)
                {
                    lastPlanTaskBatchDetail = ptbds.Last();
                }
                else
                {
                    lastPlanTaskBatchDetail = null;
                }
            }

            if (lastPlanTaskBatchDetail != null
                && ((lastPlanTaskBatchDetail.gross_time != null && lastPlanTaskBatchDetail.tare_time == null)
                    || (lastPlanTaskBatchDetail.gross_time == null && lastPlanTaskBatchDetail.tare_time != null)))
            {
                workInfo.WorkPlace = "结算处结算";
            }
            else if (planTaskBatch.PlanTaskBatchWorkPlaces != null && planTaskBatch.PlanTaskBatchWorkPlaces.Count > 0 &&
                planTaskBatch.PlanTaskBatchWorkPlaces.FirstOrDefault().WorkPlace.work_place_name != null &&
                planTaskBatch.PlanTaskBatchWorkPlaces.FirstOrDefault().WorkPlace.work_place_name.Trim().Length > 0)
            {
                workInfo.WorkPlace = planTaskBatch.PlanTaskBatchWorkPlaces.FirstOrDefault().WorkPlace.work_place_name;
            }
            else if (planTaskBatch.PlanTaskInWarehouses != null && planTaskBatch.PlanTaskInWarehouses.Count > 0 &&
                planTaskBatch.PlanTaskInWarehouses.ToList().FirstOrDefault().warehouseName != null &&
                planTaskBatch.PlanTaskInWarehouses.ToList().FirstOrDefault().warehouseName.Trim().Length > 0)
            {
                workInfo.WorkPlace = planTaskBatch.PlanTaskInWarehouses.ToList().FirstOrDefault().warehouseName.Trim();
            }
            else if (planTaskBatch.PlanTaskOutWarehouses != null && planTaskBatch.PlanTaskOutWarehouses.Count > 0 &&
            planTaskBatch.PlanTaskOutWarehouses.ToList().FirstOrDefault().warehouseName != null &&
            planTaskBatch.PlanTaskOutWarehouses.ToList().FirstOrDefault().warehouseName.Trim().Length > 0)
            {
                workInfo.WorkPlace = planTaskBatch.PlanTaskOutWarehouses.ToList().FirstOrDefault().warehouseName.Trim();
            }
            else
            {
                workInfo.WorkPlace = string.Empty;
            }

            if (enrolment.EnrolmentType.inout_type == (int)InOutType.入库)
            {
                workInfo.WarehouseInfo = planTaskBatch.PlanTaskInWarehouses.Last().Warehouse;
            }
            else
            {               
                workInfo.WarehouseInfo = planTaskBatch.PlanTaskOutWarehouses.Last().OutWarehouse;
            }

            //皮重记忆（0为不记忆，1为记忆）
            if (planTask.first_tare_con)
            {
                workInfo.RememberTareWeight = 1;
            }
            else
            {
                workInfo.RememberTareWeight = 0;
            }

            if (planTask.weight_mode == (int)WeightMode.先毛后皮)
            {
                //先毛后皮
                workInfo.WeighSequence = 1;

                if (lastPlanTaskBatchDetail == null)
                {
                    workInfo.WeighType = 1;
                }
                else if (lastPlanTaskBatchDetail != null && lastPlanTaskBatchDetail.gross_time != null && lastPlanTaskBatchDetail.tare_time == null)
                {
                    workInfo.WeighType = 0;
                }
                else
                {
                    workInfo.WeighType = 1;
                }
            }
            else if (planTask.weight_mode == (int)WeightMode.先皮后毛)
            {
                //先皮后毛
                workInfo.WeighSequence = 0;

                if (lastPlanTaskBatchDetail == null)
                {
                    workInfo.WeighType = 0;
                }
                else if (lastPlanTaskBatchDetail != null && lastPlanTaskBatchDetail.tare_time != null && lastPlanTaskBatchDetail.gross_time == null)
                {
                    workInfo.WeighType = 1;
                }
                else
                {
                    workInfo.WeighType = 0;
                }
            }
            else
            {
                workInfo.WeighType = 1;
            }
            //未值仓 = 0，正常值仓确认 = 1，值仓确认后需要换仓的 = 2，质量问题需要重新化验入仓 = 3 ,皮重=4,毛重=5,整车换仓=6
            if (lastPlanTaskBatchDetail != null && (lastPlanTaskBatchDetail.duty_confirm == null || lastPlanTaskBatchDetail.duty_confirm == 0))
            {
                workInfo.WorkNode = 0;
                if (planTask.weight_mode == (int)WeightMode.先毛后皮 && lastPlanTaskBatchDetail.gross_time != null && lastPlanTaskBatchDetail.tare_time == null)
                {
                    workInfo.WorkNode = 5;
                }
                else if (planTask.weight_mode == (int)WeightMode.先毛后皮 && lastPlanTaskBatchDetail.gross_time != null && lastPlanTaskBatchDetail.tare_time != null)
                {
                    workInfo.WorkNode = 4;
                }
                if (planTask.weight_mode == (int)WeightMode.先皮后毛 && lastPlanTaskBatchDetail.tare_time != null && lastPlanTaskBatchDetail.gross_time == null)
                {
                    workInfo.WorkNode = 4;
                }
                else if (planTask.weight_mode == (int)WeightMode.先皮后毛 && lastPlanTaskBatchDetail.tare_time != null && lastPlanTaskBatchDetail.gross_time != null)
                {
                    workInfo.WorkNode = 5;
                }
            }
            else if (lastPlanTaskBatchDetail != null && lastPlanTaskBatchDetail.duty_confirm != null)
            {
                workInfo.WorkNode = lastPlanTaskBatchDetail.duty_confirm.Value;
            }
            return workInfo;
        }

        private WorkInfo BuildInVehicleEnrollInfo(Enrolment enrolment, PlanTaskBatch planTaskBatch, InnerVehicle innerVehicle, PlanTaskBatchDetail planTaskBatchDetail)
        {
            WorkInfo workInfo = new WorkInfo();
            PlanTask planTask = planTaskBatch.PlanTask;

            if (innerVehicle.driver_name != null)
            {
                workInfo.VehicleDriver = innerVehicle.driver_name;
            }
            else
            {
                workInfo.VehicleDriver = string.Empty;
            }

            if (planTask.is_outvehicle)
            {
                if (enrolment.plate_number != null)
                {
                    workInfo.VehiclePlate = enrolment.plate_number;
                }
                else
                {
                    workInfo.VehiclePlate = string.Empty;
                }
                workInfo.ShipPlate = string.Empty;
            }
            else
            {
                if (enrolment == null)
                {
                    workInfo.ShipPlate = string.Empty;
                }
                else
                {
                    if (enrolment.plate_number != null)
                    {
                        workInfo.ShipPlate = enrolment.plate_number;
                    }
                    else
                    {
                        workInfo.ShipPlate = string.Empty;
                    }
                }
                workInfo.VehiclePlate = innerVehicle.inner_vehicle_plate;
            }

            //if (innerVehicle.inner_vehicle_id != null)
            //{
            //    workInfo.VehicleTag = innerVehicle.inner_vehicle_id;
            //}
            //else
            //{
            //    vehicleInfo.VehicleTag = string.Empty;
            //}
            //vehicleInfo.VehicleType = (int)VehicleType.内部车;

            workInfo.ApprovedWeight = 0;
            workInfo.BusinessType = planTask.PlanTaskType.warehouse_control_mode.Value;
            if (planTask.plan_weight != null)
            {
                workInfo.CompletedWeight = planTask.plan_weight.Value;
            }
            if (planTaskBatch.unit_price != null)
            {
                workInfo.Price = (double)planTaskBatch.unit_price.Value;
            }
            workInfo.WarnFlag = 0;

            workInfo.WorkNumber = planTaskBatch.generate_id.ToString();

            string strWorkPlace = string.Empty;
            string strWarehouse = string.Empty;
            string strWarehouseOut = string.Empty;
            string strWorkPlaceOther = string.Empty;
            if (planTaskBatch.PlanTask.PlanTaskType.warehouse_control_mode != (int)WarehouseControlMode.出入仓)
            {

                if (planTaskBatch.PlanTaskBatchWorkPlaces != null && planTaskBatch.PlanTaskBatchWorkPlaces.Count > 0 &&
                    planTaskBatch.PlanTaskBatchWorkPlaces.FirstOrDefault().WorkPlace.work_place_name != null &&
                    planTaskBatch.PlanTaskBatchWorkPlaces.FirstOrDefault().WorkPlace.work_place_name.Trim().Length > 0)
                {
                    strWorkPlace = planTaskBatch.PlanTaskBatchWorkPlaces.FirstOrDefault().WorkPlace.work_place_name;
                }

                if (planTaskBatch.PlanTaskInWarehouses != null && planTaskBatch.PlanTaskInWarehouses.Count > 0)
                {
                    PlanTaskInWarehouse planTaskInWarehouse = GetInWareHouseByBatchAndVichle(innerVehicle.inner_vehicle_id, planTaskBatch);
                    //strWarehouse = planTaskBatch.PlanTaskInWarehouses.Last().Warehouse.generate_id.ToString();
                    if (planTaskInWarehouse != null)
                    {
                        strWarehouse = planTaskInWarehouse.Warehouse.generate_id.ToString();
                        strWorkPlaceOther = planTaskInWarehouse.Warehouse.warehouse_name;
                    }
                    else if (planTaskBatch.PlanTask.PlanTaskType.warehouse_control_mode == (int)WarehouseControlMode.入仓)
                    {
                        strWarehouse = planTaskBatch.PlanTaskInWarehouses.Last().Warehouse.generate_id.ToString();
                        strWorkPlaceOther = planTaskBatch.PlanTaskInWarehouses.Last().Warehouse.warehouse_name;
                    }
                    else
                    {
                        strWarehouse = string.Empty;
                    }
                    strWarehouseOut = string.Empty;
                }

                if (planTaskBatch.PlanTaskOutWarehouses != null && planTaskBatch.PlanTaskOutWarehouses.Count > 0)
                {
                    //strWarehouse = planTaskBatch.PlanTaskOutWarehouses.Last().Warehouse.generate_id.ToString();
                    PlanTaskOutWarehouse planTaskOutWarehouse = GetOutWareHouseByBatchAndVichle(innerVehicle.inner_vehicle_id, planTaskBatch);
                    if (planTaskOutWarehouse != null)
                    {
                        strWarehouse = planTaskOutWarehouse.OutWarehouse.generate_id.ToString();
                        strWarehouseOut = planTaskOutWarehouse.OutWarehouse.generate_id.ToString();
                        strWorkPlaceOther = planTaskOutWarehouse.OutWarehouse.warehouse_name;
                    }
                    else if (planTaskBatch.PlanTask.PlanTaskType.warehouse_control_mode == (int)WarehouseControlMode.出仓)
                    {
                        strWarehouse = planTaskBatch.PlanTaskOutWarehouses.Last().OutWarehouse.generate_id.ToString();
                        strWarehouseOut = planTaskBatch.PlanTaskOutWarehouses.Last().OutWarehouse.generate_id.ToString();
                        strWorkPlaceOther = planTaskBatch.PlanTaskOutWarehouses.Last().OutWarehouse.warehouse_name;
                    }
                    else
                    {
                        strWarehouseOut = string.Empty;
                    }
                }
               
            }
            else
            {                

                PlanTaskInWarehouse planTaskInWarehouse = GetInWareHouseByBatchAndVichle(innerVehicle.inner_vehicle_id, planTaskBatch);
                PlanTaskOutWarehouse planTaskOutWarehouse = GetOutWareHouseByBatchAndVichle(innerVehicle.inner_vehicle_id, planTaskBatch);
                if (planTaskInWarehouse != null)
                {
                    strWarehouse = planTaskInWarehouse.Warehouse.generate_id.ToString();
                    strWorkPlace = planTaskInWarehouse.Warehouse.warehouse_name;
                }
                if (planTaskOutWarehouse != null)
                {
                    strWarehouseOut = planTaskOutWarehouse.OutWarehouse.generate_id.ToString();
                    strWorkPlaceOther = planTaskOutWarehouse.OutWarehouse.warehouse_name;
                }
            }
            if (planTaskBatch.PlanTask.PlanTaskType.warehouse_control_mode == (int)WarehouseControlMode.入仓)
            {
                if (planTask.weight_mode == (int)WeightMode.先毛后皮)
                {
                    if (!(planTaskBatchDetail != null && planTaskBatchDetail.weight_time == null))
                    {
                        //第一次
                        workInfo.WorkPlace = strWorkPlaceOther;
                    }
                    else
                    {
                        //第二次
                        workInfo.WorkPlace = strWorkPlace;
                    }
                }
                else if (planTask.weight_mode == (int)WeightMode.先皮后毛)
                {
                    if (!(planTaskBatchDetail != null && planTaskBatchDetail.weight_time == null))
                    {
                        //第一次
                        workInfo.WorkPlace = strWorkPlace;
                    }
                    else
                    {
                        //第二次
                        workInfo.WorkPlace = strWorkPlaceOther;
                    }

                }
            }
            else if (planTaskBatch.PlanTask.PlanTaskType.warehouse_control_mode == (int)WarehouseControlMode.出仓)
            {
                if (planTask.weight_mode == (int)WeightMode.先毛后皮)
                {
                    if (!(planTaskBatchDetail != null && planTaskBatchDetail.weight_time == null))
                    {
                        //第一次
                        workInfo.WorkPlace = strWorkPlace;
                    }
                    else
                    {
                        //第二次
                        workInfo.WorkPlace = strWorkPlaceOther;
                    }
                }
                else if (planTask.weight_mode == (int)WeightMode.先皮后毛)
                {
                    if (!(planTaskBatchDetail != null && planTaskBatchDetail.weight_time == null))
                    {
                        //第一次
                        workInfo.WorkPlace = strWorkPlaceOther;
                    }
                    else
                    {
                        //第二次
                        workInfo.WorkPlace = strWorkPlace;
                    }
                }
            }
            else if (planTaskBatch.PlanTask.PlanTaskType.warehouse_control_mode == (int)WarehouseControlMode.出入仓)
            {
                if (planTask.weight_mode == (int)WeightMode.先毛后皮)
                {
                    if (!(planTaskBatchDetail != null && planTaskBatchDetail.weight_time == null))
                    {
                        //第一次
                        workInfo.WorkPlace = strWorkPlaceOther;
                    }
                    else
                    {
                        //第二次
                        workInfo.WorkPlace = strWorkPlace;
                    }
                }
                else if (planTask.weight_mode == (int)WeightMode.先皮后毛)
                {
                    if (!(planTaskBatchDetail != null && planTaskBatchDetail.weight_time == null))
                    {
                        //第一次
                        workInfo.WorkPlace = strWorkPlace;
                    }
                    else
                    {
                        //第二次
                        workInfo.WorkPlace = strWorkPlaceOther;
                    }
                }
            }


            if (enrolment == null)
            {
                //workInfo.WarehouseID = planTaskBatch.PlanTaskInWarehouses.Last().Warehouse.generate_id.ToString();

                PlanTaskInWarehouse planTaskInWarehouse = GetInWareHouseByBatchAndVichle(innerVehicle.inner_vehicle_id, planTaskBatch);
                if (planTaskInWarehouse != null)
                {
                    workInfo.WarehouseInfo = planTaskInWarehouse.Warehouse;
                }
                else
                {
                    workInfo.WarehouseInfo = planTaskBatch.PlanTaskInWarehouses.Last().Warehouse;
                }
            }
            else
            {
                if (enrolment.EnrolmentType.inout_type == (int)InOutType.入库)
                {
                    PlanTaskInWarehouse planTaskInWarehouse = GetInWareHouseByBatchAndVichle(innerVehicle.inner_vehicle_id, planTaskBatch);
                    if (planTaskInWarehouse != null)
                    {
                        workInfo.WarehouseInfo = planTaskInWarehouse.Warehouse;
                    }
                    else
                    {
                        workInfo.WarehouseInfo = planTaskBatch.PlanTaskInWarehouses.Last().Warehouse;
                    }
                }
                else
                {
                    //workInfo.WarehouseID = planTaskBatch.PlanTaskOutWarehouses.Last().Warehouse.generate_id.ToString();
                    PlanTaskOutWarehouse planTaskOutWarehouse = GetOutWareHouseByBatchAndVichle(innerVehicle.inner_vehicle_id, planTaskBatch);
                    if (planTaskOutWarehouse != null)
                    {
                        workInfo.WarehouseInfo = planTaskOutWarehouse.OutWarehouse;
                    }
                    else
                    {
                        workInfo.WarehouseInfo = planTaskBatch.PlanTaskOutWarehouses.Last().OutWarehouse;
                    }
                }
            }

            if (planTask.weight_mode == (int)WeightMode.先毛后皮)
            {
                //先毛后皮
                workInfo.WeighSequence = 1;

                if (planTaskBatchDetail == null || planTaskBatchDetail.weight_time != null)
                {
                    workInfo.WeighType = 1;
                }
                else if (planTaskBatchDetail.gross_time != null && planTaskBatchDetail.tare_time == null)
                {
                    workInfo.WeighType = 0;
                }
                else
                {
                    workInfo.WeighType = 1;
                }
            }
            else if (planTask.weight_mode == (int)WeightMode.先皮后毛)
            {
                //先皮后毛
                workInfo.WeighSequence = 0;

                if (planTaskBatchDetail == null || planTaskBatchDetail.weight_time != null)
                {
                    workInfo.WeighType = 0;
                }
                else if (planTaskBatchDetail.tare_time != null && planTaskBatchDetail.gross_time == null)
                {
                    workInfo.WeighType = 1;
                }
                else
                {
                    workInfo.WeighType = 0;
                }
            }
            else
            {
                workInfo.WeighType = 1;
            }

            //未值仓 = 0，正常值仓确认 = 1，值仓确认后需要换仓的 = 2，质量问题需要重新化验入仓 = 3 ,皮重=4,毛重=5,整车换仓=6
            if (planTaskBatchDetail != null && (planTaskBatchDetail.duty_confirm == null || planTaskBatchDetail.duty_confirm == 0))
            {
                workInfo.WorkNode = 0;
                if (planTask.weight_mode == (int)WeightMode.先毛后皮 && planTaskBatchDetail.gross_time != null && planTaskBatchDetail.tare_time == null)
                {
                    workInfo.WorkNode = 5;
                }
                else if (planTask.weight_mode == (int)WeightMode.先毛后皮 && planTaskBatchDetail.gross_time != null && planTaskBatchDetail.tare_time != null)
                {
                    workInfo.WorkNode = 4;
                }
                if (planTask.weight_mode == (int)WeightMode.先皮后毛 && planTaskBatchDetail.tare_time != null && planTaskBatchDetail.gross_time == null)
                {
                    workInfo.WorkNode = 4;
                }
                else if (planTask.weight_mode == (int)WeightMode.先皮后毛 && planTaskBatchDetail.tare_time != null && planTaskBatchDetail.gross_time != null)
                {
                    workInfo.WorkNode = 5;
                }
            }
            else if (planTaskBatchDetail != null && planTaskBatchDetail.duty_confirm != null)
            {
                workInfo.WorkNode = planTaskBatchDetail.duty_confirm.Value;
            }
            return workInfo;
        }

        private QuantityRecordHead GetLastQuantityHeadByTagIdAndPlanNumber(string tagId, string plantaskBatchNumber)
        {
            //根据标签号和计划批次号查找是否存在还没有重量的数据
            RepositoryResultList<QuantityRecordHead> rrQuantityHead = _quantityHeadDal.Find(q => q.tag_id == tagId
                && q.weight_amount == 0
                && q.plantask_batch_detail_number.Contains(plantaskBatchNumber)
                && q.is_cancel == false
                );
            if (rrQuantityHead != null && rrQuantityHead.Entities != null && rrQuantityHead.Entities.Count() > 0)            
            {
                //存在此类数据，表示有前期称重记录，但是还没有产生净重，则返回改称重单头
                return rrQuantityHead.Entities.Last();
            }
            else
            {
                //不存在该类数据，表示还没有相应的产生或者已经有净重产生记录，都需要新增称重磅单
                return null;
            }
        }

        private UploadDataResult UploadVehicleWeighInfo(QuantityRecordDetail quantityDetail, int plantaskBatchId,ref string strTime)
        {
            string strErrorMessage = string.Empty;
            bool blResult = true;
            UploadDataResult uploadDataResult = new UploadDataResult();

            try
            {
                string tagId = quantityDetail.QuantityRecordHead.tag_id;
                RFIDTag rfidTagTemp = _rfidTagService.GetRFIDTagByMainId(tagId);
                strTime += "步骤二04：" + DateTime.Now.ToString() + ";                                   ";
                if (rfidTagTemp == null)
                {
                    rfidTagTemp = _rfidTagService.GetRFIDTagBySubId(tagId);
                }

                Enrolment enrolment = null;
                PlanTaskBatch planTaskBatch = null;
                InnerVehicle innerVehicle = null;
                if (rfidTagTemp != null)
                {
                    innerVehicle = _rfidTagIssueService.FindVehicleRFIDTagIssueByCode(rfidTagTemp.tag_main_id);
                    strTime += "步骤二05：" + DateTime.Now.ToString() + ";                                   ";
                    planTaskBatch = _planTaskBatchService.GetSinglePlanTaskBatchWithGenerateidOnlyBatch(plantaskBatchId);
                    strTime += "步骤二06：" + DateTime.Now.ToString() + ";                                   ";
                    enrolment = planTaskBatch.PlanTask.Enrolment;
                }
                if (planTaskBatch == null)
                {
                    uploadDataResult.ResponseResult = false;
                    uploadDataResult.FailedReason = "没有生成计划批次";
                    return uploadDataResult;
                }
                if (planTaskBatch.generate_id != plantaskBatchId)
                {
                    uploadDataResult.ResponseResult = false;
                    uploadDataResult.FailedReason = "计划批次不正确";
                    return uploadDataResult;
                }
                int outWarehouseId = 0;
                if(planTaskBatch.PlanTaskOutWarehouses != null && planTaskBatch.PlanTaskOutWarehouses.Count > 0)
                {
                    if (innerVehicle != null)
                    {
                        PlanTaskOutWarehouse planTaskOutWarehouse = GetOutWareHouseByBatchAndVichle(innerVehicle.inner_vehicle_id, planTaskBatch);
                        strTime += "步骤二07：" + DateTime.Now.ToString() + ";                                   ";
                        if (planTaskOutWarehouse != null)
                        {
                            outWarehouseId = planTaskOutWarehouse.OutWarehouse.generate_id;
                        }
                        else
                        {
                            outWarehouseId = planTaskBatch.PlanTaskOutWarehouses.Last().OutWarehouse.generate_id;
                        }
                    }
                    else
                    {
                        outWarehouseId = planTaskBatch.PlanTaskOutWarehouses.Last().OutWarehouse.generate_id;
                    }
                }
                if (enrolment == null)
                {
                    blResult = _planTaskService.WeightUpload(rfidTagTemp.tag_main_id, quantityDetail.QuantityRecordHead.id.ToString(),
                        quantityDetail.id.ToString(), quantityDetail.weight_type, quantityDetail.weight.ToString(),
                        quantityDetail.weight_time, 0, plantaskBatchId.ToString(), quantityDetail.QuantityRecordHead.warehouse_id,
                        outWarehouseId, ref strErrorMessage, ref strTime, quantityDetail);
                }
                else
                {
                    blResult = _planTaskService.WeightUpload(rfidTagTemp.tag_main_id, quantityDetail.QuantityRecordHead.id.ToString(),
                        quantityDetail.id.ToString(), quantityDetail.weight_type, quantityDetail.weight.ToString(),
                        quantityDetail.weight_time, enrolment.goods_kind, plantaskBatchId.ToString(), quantityDetail.QuantityRecordHead.warehouse_id,
                        outWarehouseId, ref strErrorMessage, ref strTime, quantityDetail);
                }
                strTime += "步骤二08：" + DateTime.Now.ToString() + ";                                   ";
            }
            catch (AisinoMesServiceException ex)
            {
                blResult = false;
            }
            catch (RepositoryException ex)
            {
                blResult = false;
            }
            catch (Exception ex)
            {
                blResult = false;
            }
            finally
            {
            }
            uploadDataResult.ResponseResult = blResult;
            uploadDataResult.FailedReason = strErrorMessage;
            return uploadDataResult;
        }
        #endregion


        public RepositoryResultList<QuantityRecordHead> GetQuantityHeadByConditions(DateTime starttime, DateTime endtime, Warehouse selectwarehouse)
        {
            var queryQuantity = PredicateBuilder.True<QuantityRecordHead>();
            if (endtime < starttime)
            {
                return null;
            }
            else
            {
                queryQuantity = queryQuantity.And(e => e.tare_time >= starttime && e.tare_time <= endtime);
            }

            if (selectwarehouse != null)
            {
                queryQuantity = queryQuantity.And(e => e.warehouse_id == selectwarehouse.generate_id);
            }
            return _quantityHeadDal.Find(queryQuantity);
        }

        private PlanTaskInWarehouse GetInWareHouseByBatchAndVichle(string strVichile, PlanTaskBatch planTaskBatch)
        {
            if (planTaskBatch == null || planTaskBatch.PlanTaskInWarehouses == null || planTaskBatch.PlanTaskInWarehouses.Count == 0)
            {
                return null;
            }
            foreach (PlanTaskInWarehouse planTaskInWarehouse in planTaskBatch.PlanTaskInWarehouses)
            {
                if (planTaskInWarehouse.vehicle_id == null || (planTaskInWarehouse.vehicle_id.Trim().Length == 0))
                {
                    continue;
                }
                if (planTaskInWarehouse.vehicle_id.Contains(strVichile))
                {
                    return planTaskInWarehouse;
                }
            }
            return null;
        }

        private PlanTaskOutWarehouse GetOutWareHouseByBatchAndVichle(string strVichile, PlanTaskBatch planTaskBatch)
        {
            if (planTaskBatch == null || planTaskBatch.PlanTaskOutWarehouses == null || planTaskBatch.PlanTaskOutWarehouses.Count == 0)
            {
                return null;
            }
            foreach (PlanTaskOutWarehouse planTaskOutWarehouse in planTaskBatch.PlanTaskOutWarehouses)
            {
                if (planTaskOutWarehouse.vehicle_id == null || (planTaskOutWarehouse.vehicle_id.Trim().Length == 0))
                {
                    continue;
                }
                if (planTaskOutWarehouse.vehicle_id.Contains(strVichile))
                {
                    return planTaskOutWarehouse;
                }
            }
            return null;
        }
    }
}
