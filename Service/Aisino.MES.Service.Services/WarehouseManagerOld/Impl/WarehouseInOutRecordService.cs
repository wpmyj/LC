using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Service.SysManager;
using Aisino.MES.Service.StoreProcedures;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.WarehouseManager.Impl
{
    public class WarehouseInOutRecordService : IWarehouseInOutRecordService
    {
        #region unity依赖注入及构造函数
        private Repository<WarehouseInOutRecord> _warehouseInOutRecordDal;
        private Repository<WarehouseInOutRecordDetail> _warehouseInOutRecordDetailDal;
        private ISysBillNoService _sysBillNoService;
        private SPGetSysDateTimeService _getSysDateTimeService;
        private UnitOfWork _unitOfWork;

        public WarehouseInOutRecordService(Repository<WarehouseInOutRecord> warehouseInOutRecordDal,
                                         Repository<WarehouseInOutRecordDetail> warehouseInOutRecordDetailDal,
                                         ISysBillNoService sysBillNoService,
                                         SPGetSysDateTimeService sPGetSysDateTimeService,
                                         UnitOfWork unitOfWork)
        {
            this._warehouseInOutRecordDal = warehouseInOutRecordDal;
            this._warehouseInOutRecordDetailDal = warehouseInOutRecordDetailDal;
            this._sysBillNoService = sysBillNoService;
            this._getSysDateTimeService = sPGetSysDateTimeService;
            this._unitOfWork = unitOfWork;
        }
        #endregion

        public void RefreshData()
        {
            _warehouseInOutRecordDal.RefreshData();
            _warehouseInOutRecordDetailDal.RefreshData();
        }

        public WarehouseInOutRecord UpdateWarehouseInOutRecordWithStoreInfoAndBatchDetail(WarehouseStoreInfo warehouseStoreInfo, WarehouseStoreInfo outWarehouseStoreInfo, PlanTaskBatchDetail planTaskBatchDetail,string orgCode)
        {
            PlanTaskBatch planTaskBatch = planTaskBatchDetail.PlanTaskBatch;
            //如果是倒仓，则有出库storeinfo判定
            if (outWarehouseStoreInfo != null)
            {
                WarehouseInOutRecord outWarehouseInOutRecord = planTaskBatch.WarehouseInOutRecords.Where(t => t.warehouse_id == outWarehouseStoreInfo.warehouse_id).FirstOrDefault();
                if (outWarehouseInOutRecord == null)
                {
                    outWarehouseInOutRecord = this.AddWarehouseInOutRecordWithStoreInfoAndBatchDetail(outWarehouseStoreInfo, planTaskBatchDetail,true,orgCode,true);
                }
                else
                {
                    outWarehouseInOutRecord.record_count += planTaskBatchDetail.weight;
                    outWarehouseInOutRecord.balance_count -= planTaskBatchDetail.weight;
                    outWarehouseInOutRecord.inout_datetime = planTaskBatchDetail.weight_time == null ? "" : planTaskBatchDetail.weight_time.Value.ToString("yyyyMMddHHmmss");
                    _unitOfWork.AddAction(outWarehouseInOutRecord, DAL.Enums.DataActions.Update);
                    AddWarehouseInOutRecordDetailWithInOutRecordAndBatchDetail(outWarehouseInOutRecord, planTaskBatchDetail);
                }
            }
            //是否需要判断批次已经完成，需要确认一下
            WarehouseInOutRecord warehouseInOutRecord = planTaskBatch.WarehouseInOutRecords.Where(t=>t.warehouse_id == warehouseStoreInfo.warehouse_id).FirstOrDefault();
            if (warehouseInOutRecord == null)
            {
                //如果当前新增的批次明细所属计划批次还没有对应的出入库记录，则新增出入库记录
                warehouseInOutRecord = this.AddWarehouseInOutRecordWithStoreInfoAndBatchDetail(warehouseStoreInfo,planTaskBatchDetail,false,orgCode,false);
            }
            else
            {
                //更新出入库记录表，以及增加出入库明细表
                warehouseInOutRecord.record_count += planTaskBatchDetail.weight;
                if (planTaskBatch.PlanTask.Enrolment == null)
                {
                    warehouseInOutRecord.balance_count += planTaskBatchDetail.weight;
                }
                else
                {
                    if (planTaskBatch.PlanTask.Enrolment.inout_type == (int)InOutType.入库)
                    {
                        warehouseInOutRecord.balance_count += planTaskBatchDetail.weight;
                    }
                    else
                    {
                        warehouseInOutRecord.balance_count -= planTaskBatchDetail.weight;
                    }
                }
                warehouseInOutRecord.inout_datetime = planTaskBatchDetail.weight_time == null ? "" : planTaskBatchDetail.weight_time.Value.ToString("yyyyMMddHHmmss");
                _unitOfWork.AddAction(warehouseInOutRecord, DAL.Enums.DataActions.Update); 
            }
            AddWarehouseInOutRecordDetailWithInOutRecordAndBatchDetail(warehouseInOutRecord, planTaskBatchDetail);
            return warehouseInOutRecord;
        }

        public WarehouseInOutRecord UpdateWarehouseInOutRecordWithStoreInfoAndBatchAdjust(WarehouseStoreInfo warehouseStoreInfo, PlanTaskBatchAdjust planTaskBatchAdjust,PlanTaskBatch plantaskBatch)
        {
            WarehouseInOutRecord warehouseInOutRecord = plantaskBatch.WarehouseInOutRecords.Where(t => t.warehouse_id == warehouseStoreInfo.warehouse_id).FirstOrDefault();

            if (plantaskBatch.PlanTask.PlanTaskType.warehouse_control_mode.Value == (int)WarehouseControlMode.入仓)
            {
                if (planTaskBatchAdjust.adjust_type == (int)plantaskahjusttype.增加)
                {
                    warehouseInOutRecord.record_count += planTaskBatchAdjust.adjust_count;
                    warehouseInOutRecord.balance_count += planTaskBatchAdjust.adjust_count;
                }
                else
                {
                    warehouseInOutRecord.record_count -= planTaskBatchAdjust.adjust_count;
                    warehouseInOutRecord.balance_count -= planTaskBatchAdjust.adjust_count;
                }
            }
            else
            {
                if (planTaskBatchAdjust.adjust_type == (int)plantaskahjusttype.增加)
                {
                    warehouseInOutRecord.record_count += planTaskBatchAdjust.adjust_count;
                    warehouseInOutRecord.balance_count -= planTaskBatchAdjust.adjust_count;
                }
                else
                {
                    warehouseInOutRecord.record_count -= planTaskBatchAdjust.adjust_count;
                    warehouseInOutRecord.balance_count += planTaskBatchAdjust.adjust_count;
                }
            }
            _unitOfWork.AddAction(warehouseInOutRecord, DataActions.Update);

            AddWarehouseInOutRecordDetailWithInOutRecordAndBatchAdjust(warehouseInOutRecord, planTaskBatchAdjust);
            return warehouseInOutRecord;
        }

        public WarehouseInOutRecord CancelWarehouseInOutRecordWithScaleBill(WarehouseStoreInfo warehouseStoreInfo, PlanTaskBatchDetail planTaskBatchDetail)
        {
            PlanTaskBatch planTaskBatch = planTaskBatchDetail.PlanTaskBatch;
            if (planTaskBatchDetail.outwarehouse != null)
            {
                //倒仓出入库记录
                //根据出仓点找到该计划批次明细对应的出入库记录
                WarehouseInOutRecord warehouseInOutRecordOut = planTaskBatch.WarehouseInOutRecords.Where(wior => wior.warehouse_id == planTaskBatchDetail.outwarehouse).FirstOrDefault();
                if (warehouseInOutRecordOut != null && planTaskBatchDetail.weight.HasValue)
                {
                    //有对应的出入库记录，则回滚出仓点信息
                    decimal dBatchCount = planTaskBatchDetail.weight.Value;
                    //对于倒仓出库，则回滚增加被扣除数量
                    warehouseInOutRecordOut.record_count -= dBatchCount;
                    warehouseInOutRecordOut.balance_count += dBatchCount;
                    _unitOfWork.AddAction(warehouseInOutRecordOut, DAL.Enums.DataActions.Update);
                }
            }
            //不论出入库还是倒仓，都需要更新对应plantaskbatchdetail中warehouse的出入库记录
            WarehouseInOutRecord warehouseInOutRecord = planTaskBatch.WarehouseInOutRecords.Where(wior => wior.warehouse_id == planTaskBatchDetail.warehouse_id).FirstOrDefault();
            if (warehouseInOutRecord != null && planTaskBatchDetail.weight.HasValue)
            {
                decimal dBatchCount = planTaskBatchDetail.weight.Value;
                warehouseInOutRecord.record_count -= dBatchCount;
                if (planTaskBatch.PlanTask.Enrolment != null)
                {
                    if (planTaskBatch.PlanTask.Enrolment.inout_type == (int)InOutType.入库)
                    {
                        warehouseInOutRecord.balance_count -= dBatchCount;
                    }
                    else
                    {
                        warehouseInOutRecord.balance_count += dBatchCount;
                    }
                }
                else
                {
                    if (planTaskBatch.PlanTask.PlanTaskType.warehouse_control_mode == (int)WarehouseControlMode.出入仓)
                    {
                        //倒仓情况下，对应的warehouse等于入仓点，回滚入库则扣除
                        warehouseInOutRecord.balance_count -= dBatchCount;
                    }
                }
                _unitOfWork.AddAction(warehouseInOutRecord, DAL.Enums.DataActions.Update);
            }
            
            CancelWarehouseInOutRecordDetailWithInOutRecordAndBatchDetail(planTaskBatchDetail);
            return warehouseInOutRecord; 
        }

        /// <summary>
        /// 仓库概况接口
        /// </summary>
        /// <param name="wareHouseID">仓库ID</param>
        /// <param name="searchFlag">查看入库还是出库逐日汇总：0表示入库，1表示出库，2表示出入库</param>
        /// <param name="searchLength">查询数据记录数，默认为4条，视手持机显示界面的空间定</param>
        /// <param name="page">页数(从0开始)</param>
        /// <returns></returns>
        public WarehouseInOutRecord GetWareHouseInfo(string wareHouseID, int searchFlag, int searchLength, int page)
        {
            //if()
            return null;
        }

        #region 私有方法
        /// <summary>
        /// 新建出入库记录
        /// </summary>
        /// <param name="warehouseStoreInfo">仓储保管帐信息</param>
        /// <param name="planTaskBatchDetail">计划批次明细</param>
        /// <param name="isOutWarehouseStoreInfo">是否倒仓出库标志</param>
        /// <returns>创建完成的出入库记录</returns>
        private WarehouseInOutRecord AddWarehouseInOutRecordWithStoreInfoAndBatchDetail(WarehouseStoreInfo warehouseStoreInfo, PlanTaskBatchDetail planTaskBatchDetail, bool isOutWarehouseStoreInfo, string orgCode, bool isOutInFlag)
        {
            WarehouseInOutRecord warehouseInOutRecord = null;
            PlanTaskBatch planTaskBatch = planTaskBatchDetail.PlanTaskBatch;
            warehouseInOutRecord = new WarehouseInOutRecord();
            int sysBillNoID = _sysBillNoService.GetBillNoID(MESSystem.whm, BillPrefix.ioh);
            //warehouseInOutRecord.record_number = AisinoMesServiceHelper.GetOriginalDept(planTaskBatch.PlanTask.Enrolment.OrganizationEmployee1) + _sysBillNoService.GetBillNo(sysBillNoID);
            if (isOutWarehouseStoreInfo)
            {
                //如果是倒仓出入，则前缀增加O
                warehouseInOutRecord.record_number = orgCode + "O" + _sysBillNoService.GetBillNo(sysBillNoID) + planTaskBatch.generate_id;
            }
            else
            {
                warehouseInOutRecord.record_number = orgCode + _sysBillNoService.GetBillNo(sysBillNoID) + planTaskBatch.generate_id;
            }
            warehouseInOutRecord.record_status = (int)RecordStatus.有效;
            warehouseInOutRecord.registe_type = (int)WarehouseInOutRecordRegisteType.系统登记;
            if (!isOutWarehouseStoreInfo)
            {
                //不是倒仓，则不论出入库都使用warehouse_id
                warehouseInOutRecord.warehouse_id = planTaskBatchDetail.warehouse_id;
                warehouseInOutRecord.grain_kind = planTaskBatch.PlanTask.goods_kind.Value;

                if (planTaskBatch.PlanTask.Enrolment == null)
                {
                    warehouseInOutRecord.record_type = (int)RecordType.入库;
                    if (warehouseInOutRecord.balance_count == null)
                    {
                        warehouseInOutRecord.balance_count = planTaskBatchDetail.weight;
                    }
                    else
                    {
                        warehouseInOutRecord.balance_count += planTaskBatchDetail.weight;
                    }
                }
                else
                {
                    if (planTaskBatch.PlanTask.Enrolment.inout_type == (int)InOutType.入库)
                    {
                        warehouseInOutRecord.record_type = (int)RecordType.入库;
                        if (warehouseInOutRecord.balance_count == null)
                        {
                            warehouseInOutRecord.balance_count = planTaskBatchDetail.weight;
                        }
                        else
                        {
                            warehouseInOutRecord.balance_count += planTaskBatchDetail.weight;
                        }
                    }
                    else
                    {
                        warehouseInOutRecord.record_type = (int)RecordType.出库;
                        warehouseInOutRecord.balance_count -= planTaskBatchDetail.weight;
                    }
                }
            }
            else
            {
                //记录倒仓仓号
                warehouseInOutRecord.warehouse_id = planTaskBatchDetail.outwarehouse;
                warehouseInOutRecord.grain_kind = warehouseStoreInfo.grain_kind;
                warehouseInOutRecord.record_type = (int)RecordType.出库;
                warehouseInOutRecord.balance_count -= planTaskBatchDetail.weight;
            }
            warehouseInOutRecord.warehouse_store_info = warehouseStoreInfo.warehouse_store_id;
            warehouseInOutRecord.work_plan = planTaskBatch.plantask_batch_number;
            warehouseInOutRecord.inout_datetime = planTaskBatchDetail.weight_time == null ? "" : planTaskBatchDetail.weight_time.Value.ToString("yyyyMMddHHmmss");
            warehouseInOutRecord.inout_finished = true;
            warehouseInOutRecord.location_flag = false;
            warehouseInOutRecord.manual_registe = false;
            warehouseInOutRecord.record_count = planTaskBatchDetail.weight;

            if (isOutInFlag)
            {
                if (isOutWarehouseStoreInfo)
                {
                    warehouseInOutRecord.record_type = (int)RecordType.倒仓出库;
                }
                else
                {
                    warehouseInOutRecord.record_type = (int)RecordType.倒仓入库;
                }
            }

            _unitOfWork.AddAction(warehouseInOutRecord, DAL.Enums.DataActions.Add);

            return warehouseInOutRecord;
        }

        /// <summary>
        /// 根据出入库计划和批次明细新增出入库明细
        /// </summary>
        /// <param name="warehouseInOutRecord">出入库记录单</param>
        /// <param name="planTaskBatchDetail">出入库明细</param>
        /// <returns></returns>
        private WarehouseInOutRecordDetail AddWarehouseInOutRecordDetailWithInOutRecordAndBatchDetail(WarehouseInOutRecord warehouseInOutRecord,PlanTaskBatchDetail planTaskBatchDetail)
        {
            WarehouseInOutRecordDetail winoutRecordDetail = new WarehouseInOutRecordDetail();
            winoutRecordDetail.create_time = planTaskBatchDetail.tare_time > planTaskBatchDetail.gross_time ? planTaskBatchDetail.tare_time.Value.ToString("yyyyMMdd") : planTaskBatchDetail.gross_time.Value.ToString("yyyyMMdd");
            winoutRecordDetail.inout_count = planTaskBatchDetail.weight.Value;
            winoutRecordDetail.record_number = warehouseInOutRecord.record_number;
            winoutRecordDetail.record_status = (int)RecordStatus.有效;
            winoutRecordDetail.reference_number = planTaskBatchDetail.scale_number;
            winoutRecordDetail.regist_type = (int)WarehouseInOutRecordDetailRegistType.从地磅自动称重系统采集而来;

            _unitOfWork.AddAction(winoutRecordDetail, DAL.Enums.DataActions.Add);
            return winoutRecordDetail;
        }

        /// <summary>
        /// 根据出入库冲补和记录，新增出入库条目
        /// </summary>
        /// <param name="warehouseInOutRecord">出入库记录</param>
        /// <param name="planTaskBatchAdjust">计划冲补单</param>
        /// <returns></returns>
        private WarehouseInOutRecordDetail AddWarehouseInOutRecordDetailWithInOutRecordAndBatchAdjust(WarehouseInOutRecord warehouseInOutRecord, PlanTaskBatchAdjust planTaskBatchAdjust)
        {
            WarehouseInOutRecordDetail winoutRecordDetail = new WarehouseInOutRecordDetail();
            winoutRecordDetail.create_time = planTaskBatchAdjust.adjust_time.ToString("yyyyMMdd");
            winoutRecordDetail.inout_count = planTaskBatchAdjust.adjust_count.Value;
            winoutRecordDetail.record_number = warehouseInOutRecord.record_number;
            winoutRecordDetail.record_status = (int)RecordStatus.有效;
            winoutRecordDetail.reference_number = planTaskBatchAdjust.plantask_batch_adjust_number;
            winoutRecordDetail.regist_type = (int)WarehouseInOutRecordDetailRegistType.通过冲补登记;

            _unitOfWork.AddAction(winoutRecordDetail, DAL.Enums.DataActions.Add);
            return winoutRecordDetail;
        }

        /// <summary>
        /// 根据称重榜单号废除出入库明细
        /// </summary>
        /// <param name="strScaleBillNumber">称重磅单号</param>
        /// <returns></returns>
        private WarehouseInOutRecordDetail CancelWarehouseInOutRecordDetailWithInOutRecordAndBatchDetail(PlanTaskBatchDetail planTaskBatchDetail)
        {
            var warehouseInOutRecordDetails = _warehouseInOutRecordDetailDal.Find(w => w.reference_number == planTaskBatchDetail.scale_number).Entities;
            WarehouseInOutRecordDetail winoutRecordDetail = null; 
            if (warehouseInOutRecordDetails != null && warehouseInOutRecordDetails.Count() > 0)
            {
                winoutRecordDetail = warehouseInOutRecordDetails.LastOrDefault();
                winoutRecordDetail.record_status = (int)RecordStatus.废除;
                _unitOfWork.AddAction(winoutRecordDetail, DAL.Enums.DataActions.Update);
                return winoutRecordDetail;
            }
            return null;           
        }
        #endregion


        public WarehouseInOutRecord AddWarehouseInOutRecordWithPlanTaskInHouse(WarehouseInOutRecord newWarehouseInOutRecord)
        {
            WarehouseInOutRecord returnWarehouseInOutRecord = null;
            try
            {
                _unitOfWork.AddAction(newWarehouseInOutRecord, DataActions.Add);
                returnWarehouseInOutRecord = newWarehouseInOutRecord;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnWarehouseInOutRecord;
        }
    }
}
