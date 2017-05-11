using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Service.StoreProcedures;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Service.SysManager;

namespace Aisino.MES.Service.WarehouseManager.Impl
{
    public class WarehouseStoreInfoService : IWarehouseStoreInfoService
    {
        #region unity依赖注入及构造方法
        private Repository<WarehouseStoreInfo> _warehouseStoreInfoDal;
        private WarehouseInOutRecordService _warehouseInOutRecordService;
        private ISysBillNoService _sysBillNoService;
        private SPGetSysDateTimeService _getSysDateTimeService;
        private Repository<Contract> _contractDal;
        private Repository<CustomerPlan> _customerPlanDal;
        private Repository<JiaKongManage> _jiaKongManagerDal;
        private Repository<CustomerShipingCertificate> _csCertificateDal;
        private Repository<GrainAttribute> _grainAttributeDal;
        private Repository<Warehouse> _warehouseDal;
        private ISysDepartmentUserService _sysDepartmentUserService;
        private UnitOfWork _unitOfWork;

        public WarehouseStoreInfoService(Repository<WarehouseStoreInfo> warehouseStoreInfoDal,
                                         WarehouseInOutRecordService warehouseInOutRecordService,
                                         ISysBillNoService sysBillNoService,
                                         SPGetSysDateTimeService sPGetSysDateTimeService,
                                         Repository<Contract> contractDal,
                                        Repository<CustomerPlan> customerPlanDal,
                                        Repository<JiaKongManage> jiaKongManagerDal,
                                        Repository<CustomerShipingCertificate> csCertificateDal,
                                        Repository<GrainAttribute> grainAttributeDal,
            Repository<Warehouse> warehouseDal,
            ISysDepartmentUserService sysDepartmentUserService,
                                         UnitOfWork unitOfWork)
        {
            this._warehouseStoreInfoDal = warehouseStoreInfoDal;
            this._warehouseInOutRecordService = warehouseInOutRecordService;
            this._sysBillNoService = sysBillNoService;
            this._getSysDateTimeService = sPGetSysDateTimeService;
            this._contractDal = contractDal;
            this._customerPlanDal = customerPlanDal;
            this._jiaKongManagerDal = jiaKongManagerDal;
            this._csCertificateDal = csCertificateDal;
            this._grainAttributeDal = grainAttributeDal;
            this._warehouseDal = warehouseDal;
            this._sysDepartmentUserService = sysDepartmentUserService;
            this._unitOfWork = unitOfWork;
        }
        #endregion

        #region 私有方法
        private WarehouseStoreInfo GetUsingWarehouseStoreInfoById(string warehouseId)
        {
            return _warehouseStoreInfoDal.Single(w => w.warehouse_id == warehouseId && w.store_status == (int)StoreStatus.在储保管帐).Entity;
        }

        private WarehouseStoreInfo GetUsingWarehouseStoreInfoById(string warehouseId,int goodsLocationId)
        {
            return _warehouseStoreInfoDal.Single(w => w.warehouse_id == warehouseId && w.store_status == (int)StoreStatus.在储保管帐 && w.goods_localtion == goodsLocationId).Entity;
        }

        private WarehouseStoreInfo CreateNewWarehouseStoreInfo(string warehouseId, PlanTaskBatchDetail planTaskBatchDetail)
        {
            WarehouseStoreInfo warehouseStoreInfo = new WarehouseStoreInfo();
            Warehouse warehouse = _warehouseDal.Single(w => w.warehouse_id == warehouseId).Entity;
            //初始化赋值，需要新增保管帐的只可能为空库存，所以肯定为创建入库
            int sysBillNoID = _sysBillNoService.GetBillNoID(MESSystem.whm, BillPrefix.wsi);
            string orgcode = string.Empty;
            if (warehouse.owner_org != null)
            {
                orgcode = _sysDepartmentUserService.GetSysDepartment(warehouse.owner_org.Value).code;
            }
            //warehouseStoreInfo.warehouse_store_id = AisinoMesServiceHelper.GetOriginalDept(planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.OrganizationEmployee) + _sysBillNoService.GetBillNo(sysBillNoID);
            warehouseStoreInfo.warehouse_store_id = orgcode + _sysBillNoService.GetBillNo(sysBillNoID);
            warehouseStoreInfo.expectation_in_count = 0;
            warehouseStoreInfo.expectation_out_count = 0;
            warehouseStoreInfo.final_statement_weight = 0;
            warehouseStoreInfo.store_out_count = 0;
            warehouseStoreInfo.in_begin_time = _getSysDateTimeService.GetSysDateTime().ToString("yyyyMMdd");
            warehouseStoreInfo.store_status = (int)StoreStatus.在储保管帐;
            warehouseStoreInfo.location_flag = planTaskBatchDetail.goods_location_id.HasValue;
            warehouseStoreInfo.store_count = planTaskBatchDetail.weight.Value;
            warehouseStoreInfo.store_in_count = planTaskBatchDetail.weight.Value;
            if (planTaskBatchDetail.goods_location_id == null)
            {
                warehouseStoreInfo.location_flag = false;
            }
            else
            {
                warehouseStoreInfo.location_flag = true;
                warehouseStoreInfo.goods_localtion = planTaskBatchDetail.goods_location_id.Value;
            }

            if (planTaskBatchDetail.outwarehouse != null)
            {
                //倒仓入库，带入为出库仓的信息
                //获取出库仓原有仓储保管信息
                WarehouseStoreInfo outWarehouseStoreInfo = null;
                if (planTaskBatchDetail.outgoods_location_id != null)
                {
                    outWarehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.outwarehouse,planTaskBatchDetail.outgoods_location_id.Value);
                }
                else
                {
                    outWarehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.outwarehouse);
                }
                warehouseStoreInfo.store_way = outWarehouseStoreInfo.store_way;
                warehouseStoreInfo.grain_attribute = outWarehouseStoreInfo.grain_attribute;
                warehouseStoreInfo.grain_annual = outWarehouseStoreInfo.grain_annual;
                warehouseStoreInfo.grain_kind = outWarehouseStoreInfo.grain_kind;
            }
            else
            {
                //正常入库，直接获取计划带入的原始信息
                if (planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.EnrolmentBasicTypePackage.type_name == "散装")
                {
                    warehouseStoreInfo.store_way = (int)StoreWay.散粮存放;
                }
                else
                {
                    warehouseStoreInfo.store_way = (int)StoreWay.包装粮存放;
                }
                var tempValue = _grainAttributeDal.Single(t => t.is_default_value == true);
                if (tempValue != null && tempValue.Entity != null && tempValue.Entity.grain_attribute_id != null)
                {
                    warehouseStoreInfo.grain_attribute = tempValue.Entity.grain_attribute_id;
                }
                warehouseStoreInfo.grain_annual = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.grain_annual;
                warehouseStoreInfo.grain_kind = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.goods_kind;
            }

            warehouseStoreInfo.warehouse_id = warehouse.warehouse_id;
            warehouse.warehouse_status = (int)WarehouseStatus.作业;
            _unitOfWork.AddAction(warehouse, DataActions.Update);
            _unitOfWork.AddAction(warehouseStoreInfo, DataActions.Add);

            return warehouseStoreInfo;
        }

        //private WarehouseStoreInfo CreateNewWarehouseStoreInfo(string warehouseId, PlanTaskBatchAdjust planTaskBatchAdjust)
        //{
        //    WarehouseStoreInfo warehouseStoreInfo = new WarehouseStoreInfo();
        //    //初始化赋值，需要新增保管帐的只可能为空库存，所以肯定为创建入库
        //    int sysBillNoID = _sysBillNoService.GetBillNoID(MESSystem.whm, BillPrefix.wsi);
        //    warehouseStoreInfo.warehouse_store_id = AisinoMesServiceHelper.GetOriginalDept(planTaskBatchAdjust.PlanTaskBatch.PlanTask.Enrolment.OrganizationEmployee) + _sysBillNoService.GetBillNo(sysBillNoID);
        //    warehouseStoreInfo.expectation_in_count = 0;
        //    warehouseStoreInfo.expectation_out_count = 0;
        //    warehouseStoreInfo.final_statement_weight = 0;
        //    warehouseStoreInfo.store_out_count = 0;
        //    warehouseStoreInfo.in_begin_time = _getSysDateTimeService.GetSysDateTime().ToString("yyyyMMdd");
        //    warehouseStoreInfo.store_status = (int)StoreStatus.在储保管帐;
        //    warehouseStoreInfo.location_flag = planTaskBatchAdjust.goods_location_id.HasValue;
        //    warehouseStoreInfo.store_count = planTaskBatchAdjust.adjust_count.Value;
        //    warehouseStoreInfo.store_in_count = planTaskBatchAdjust.adjust_count.Value;
        //    if (planTaskBatchAdjust.goods_location_id == null)
        //    {
        //        warehouseStoreInfo.location_flag = false;
        //    }
        //    else
        //    {
        //        warehouseStoreInfo.location_flag = true;
        //        warehouseStoreInfo.goods_localtion = planTaskBatchAdjust.goods_location_id.Value;
        //    }

        //    PlanTaskBatchDetail planTaskBatchDetail = planTaskBatchAdjust.PlanTaskBatch.PlanTaskBatchDetails.LastOrDefault();
        //    if (planTaskBatchDetail.outwarehouse != null && planTaskBatchDetail.outwarehouse != planTaskBatchAdjust.warehouse_id)
        //    {
        //        //倒仓入库，带入为出库仓的信息,当前冲补条目的仓不是倒仓出仓，表明冲补入仓
        //        //则需要获取出库仓原有仓储保管信息
        //        WarehouseStoreInfo outWarehouseStoreInfo = null;
        //        if (planTaskBatchDetail.outgoods_location_id != null)
        //        {
        //            outWarehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.outwarehouse, planTaskBatchDetail.outgoods_location_id.Value);
        //        }
        //        else
        //        {
        //            outWarehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.outwarehouse);
        //        }
        //        warehouseStoreInfo.store_way = outWarehouseStoreInfo.store_way;
        //        warehouseStoreInfo.grain_attribute = outWarehouseStoreInfo.grain_attribute;
        //        warehouseStoreInfo.grain_annual = outWarehouseStoreInfo.grain_annual;
        //        warehouseStoreInfo.grain_kind = outWarehouseStoreInfo.grain_kind;
        //    }
        //    else
        //    {
        //        //正常入库，直接获取计划带入的原始信息
        //        warehouseStoreInfo.store_way = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.package_type;
        //        warehouseStoreInfo.grain_attribute = _grainAttributeDal.Single(t => t.is_default_value == true).Entity.grain_attribute_id;
        //        warehouseStoreInfo.grain_annual = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.grain_annual;
        //        warehouseStoreInfo.grain_kind = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.goods_kind;
        //    }

        //    _unitOfWork.AddAction(warehouseStoreInfo, DataActions.Add);

        //    return warehouseStoreInfo;
        //}
        #endregion

        #region 实现接口方法

        public void RefreshData()
        {
            this._warehouseStoreInfoDal.RefreshData();
            this._contractDal.RefreshData();
            this._customerPlanDal.RefreshData();
            this._jiaKongManagerDal.RefreshData();
            this._csCertificateDal.RefreshData();
        }

        public IEnumerable<WarehouseStoreInfo> FindUsingWarehouseStoreInfoByGoodsKind(int goodsKindId)
        {
            return _warehouseStoreInfoDal.Find(w => w.grain_kind == goodsKindId).Entities;
        }

        public WarehouseStoreInfo UpdateWarehouseStoreInfoWithPlanTaskBatchDetail(PlanTaskBatchDetail planTaskBatchDetail)
        {
            string warehouseId = "";
            int goodsLocationId = 0;
            //倒仓入库记录在warehouse_id
            warehouseId = planTaskBatchDetail.warehouse_id;
            goodsLocationId = planTaskBatchDetail.goods_location_id == null ? 0 : planTaskBatchDetail.goods_location_id.Value;
            WarehouseStoreInfo warehouseStoreInfo = null;

            if (goodsLocationId == 0)
            {
                //不管理货位
                warehouseStoreInfo = GetUsingWarehouseStoreInfoById(warehouseId);
            }
            else
            {
                warehouseStoreInfo = GetUsingWarehouseStoreInfoById(warehouseId, goodsLocationId);
            }



            WarehouseStoreInfo outWarehouseStoreInfo = null;
            #region 更新倒仓出库
            if (planTaskBatchDetail.outwarehouse != null)
            {
                //是倒仓的明细，需要更新倒仓出库
                if (planTaskBatchDetail.goods_location_id == null)
                {
                    outWarehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.outwarehouse);
                }
                else
                {
                    outWarehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.outwarehouse, planTaskBatchDetail.goods_location_id.Value);
                }
                if (outWarehouseStoreInfo.store_count == null)
                {
                    outWarehouseStoreInfo.store_count = 0 - planTaskBatchDetail.weight.Value;
                }
                else
                {
                    outWarehouseStoreInfo.store_count -= planTaskBatchDetail.weight.Value;
                }
                if (outWarehouseStoreInfo.store_out_count == null)
                {
                    outWarehouseStoreInfo.store_out_count = planTaskBatchDetail.weight.Value;
                }
                else
                {
                    outWarehouseStoreInfo.store_out_count += planTaskBatchDetail.weight.Value;
                }
                PlanTaskOutWarehouse ptow = planTaskBatchDetail.PlanTaskBatch.PlanTaskOutWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.plantask_out_amount != 0).FirstOrDefault();
                if (ptow != null)
                {
                    //有预排出仓数量，则原有出库数量扣除本次出库数量
                    if (outWarehouseStoreInfo.expectation_in_count != null)
                    {
                        outWarehouseStoreInfo.expectation_in_count -= ptow.plantask_out_amount;
                    }
                }
                _unitOfWork.AddAction(outWarehouseStoreInfo, DataActions.Update);

                //倒仓情况下，可能遇到不做仓储损益的虚拟仓（如烘干仓等）
                //需要更新该仓房的仓储信息中粮食品种为对应出仓的粮食品种，便于后期该虚拟仓出库使用
                if (warehouseStoreInfo != null)
                {
                    warehouseStoreInfo.grain_kind = outWarehouseStoreInfo.grain_kind;
                }
            }
            #endregion

            //var wareHouseTemp = _warehouseDal.Find(wh => wh.warehouse_id == warehouseId).Entities;
            Warehouse warehouse = null;
            var wareHouseTemp = _warehouseDal.Single(w => w.warehouse_id == warehouseId);
            if (wareHouseTemp.HasValue)
            {
                warehouse = wareHouseTemp.Entity;
            }
            if (warehouse != null && warehouse.warehouse_status != null &&
                warehouse.warehouse_status == (int)WarehouseStatus.空仓)
            {
                warehouseStoreInfo = this.CreateNewWarehouseStoreInfo(warehouseId, planTaskBatchDetail, outWarehouseStoreInfo);
            }
            else if (warehouseStoreInfo == null)
            {
                //出库的仓房肯定有仓储保管帐，所以只有在入库时候才会发生此类情况
                //如果没有该入库仓房的在储仓储保管帐，则说明新开仓或重新开仓，需要创建纪录
                warehouseStoreInfo = this.CreateNewWarehouseStoreInfo(warehouseId, planTaskBatchDetail, outWarehouseStoreInfo);
            }
            else
            {
                //如果入库仓不需要重新开仓
                if (planTaskBatchDetail.inout_type == (int)InOutType.入库)
                {
                    if (warehouseStoreInfo.store_count == null)
                    {
                        warehouseStoreInfo.store_count = planTaskBatchDetail.weight.Value;
                    }
                    else
                    {
                        warehouseStoreInfo.store_count += planTaskBatchDetail.weight.Value;
                    }

                    if (warehouseStoreInfo.store_in_count == null)
                    {
                        warehouseStoreInfo.store_in_count = planTaskBatchDetail.weight.Value;
                    }
                    else
                    {
                        warehouseStoreInfo.store_in_count += planTaskBatchDetail.weight.Value;
                    }
                    PlanTaskInWarehouse ptiw = null;
                    if (planTaskBatchDetail.goods_location_id == null)
                    {
                        ptiw = planTaskBatchDetail.PlanTaskBatch.PlanTaskInWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.plantask_in_amount != 0).LastOrDefault();
                    }
                    else
                    {
                        ptiw = planTaskBatchDetail.PlanTaskBatch.PlanTaskInWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.goods_location_id == planTaskBatchDetail.goods_location_id && w.plantask_in_amount != 0).LastOrDefault();
                    }
                    if (ptiw != null)
                    {
                        //有预排入仓数量，则原有预入库数量扣除本次入库数量
                        if (warehouseStoreInfo.expectation_in_count == null)
                        {
                            //warehouseStoreInfo.expectation_in_count = 0 - ptiw.plantask_in_amount;
                        }
                        else
                        {
                            warehouseStoreInfo.expectation_in_count -= ptiw.plantask_in_amount;
                        }
                    }
                }
                else
                {
                    if (warehouseStoreInfo.store_count == null)
                    {
                        //warehouseStoreInfo.store_count = 0 - planTaskBatchDetail.weight.Value;
                    }
                    else
                    {
                        warehouseStoreInfo.store_count -= planTaskBatchDetail.weight.Value;
                    }

                    if (warehouseStoreInfo.store_out_count == null)
                    {
                        warehouseStoreInfo.store_out_count = planTaskBatchDetail.weight.Value;
                    }
                    else
                    {
                        warehouseStoreInfo.store_out_count += planTaskBatchDetail.weight.Value;
                    }
                    PlanTaskOutWarehouse ptow = null;
                    if (planTaskBatchDetail.goods_location_id == null)
                    {
                        ptow = planTaskBatchDetail.PlanTaskBatch.PlanTaskOutWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.plantask_out_amount != 0).LastOrDefault();
                    }
                    else
                    {
                        ptow = planTaskBatchDetail.PlanTaskBatch.PlanTaskOutWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.goods_location_id == planTaskBatchDetail.goods_location_id && w.plantask_out_amount != 0).LastOrDefault();
                    }
                    if (ptow != null)
                    {
                        //有预排出仓数量，则原有出库数量扣除本次出库数量
                        if (warehouseStoreInfo.expectation_in_count == null)
                        {
                            //warehouseStoreInfo.expectation_in_count = 0 - ptow.plantask_out_amount;
                        }
                        else
                        {
                            warehouseStoreInfo.expectation_in_count -= ptow.plantask_out_amount;
                        }
                    }
                }

                _unitOfWork.AddAction(warehouseStoreInfo, DataActions.Update);
            }
            //Warehouse warehouse = _warehouseDal.Single(w => w.warehouse_id == warehouseId).Entity;
            if (planTaskBatchDetail.outwarehouse != null)
            {
                //倒仓
                Warehouse outWarehouse = _warehouseDal.Single(w => w.warehouse_id == planTaskBatchDetail.outwarehouse).Entity;
                if (outWarehouse.warehouse_status.Value == (int)WarehouseStatus.封仓)
                {
                    outWarehouse.warehouse_status = (int)WarehouseStatus.作业;
                    _unitOfWork.AddAction(outWarehouse, DataActions.Update);
                }
            }
            else if (planTaskBatchDetail.inout_type == (int)InOutType.出库)
            {
                //出库的时候才会在更新仓容的时候改变仓房状态
                if (warehouse.warehouse_status.Value == (int)WarehouseStatus.封仓)
                {
                    warehouse.warehouse_status = (int)WarehouseStatus.作业;
                    _unitOfWork.AddAction(warehouse, DataActions.Update);
                }
            }
            string orgcode = _sysDepartmentUserService.GetSysDepartment(warehouse.owner_org.Value).code;
            //_warehouseInOutRecordService.RefreshData();
            _warehouseInOutRecordService.UpdateWarehouseInOutRecordWithStoreInfoAndBatchDetail(warehouseStoreInfo, outWarehouseStoreInfo, planTaskBatchDetail, orgcode);

            return warehouseStoreInfo;
        }

        public WarehouseStoreInfo UpdateWarehouseStoreInfoWithPlanTaskBatchAdjust(PlanTaskBatchAdjust planTaskBatchAdjust,PlanTaskBatch plantaskBatch)
        {
            WarehouseStoreInfo warehouseStoreInfo = null;
            try
            {
                int inoutType = plantaskBatch.PlanTask.PlanTaskType.warehouse_control_mode.Value;
                string warehouseId = planTaskBatchAdjust.warehouse_id;
                int goodsLocationId = planTaskBatchAdjust.goods_location_id.HasValue ? planTaskBatchAdjust.goods_location_id.Value : 0;
                if (goodsLocationId == 0)
                {
                    //不管理货位
                    warehouseStoreInfo = GetUsingWarehouseStoreInfoById(warehouseId);
                }
                else
                {
                    warehouseStoreInfo = GetUsingWarehouseStoreInfoById(warehouseId, goodsLocationId);
                }

                if (warehouseStoreInfo != null)
                {
                    if (planTaskBatchAdjust.adjust_type == (int)plantaskahjusttype.增加)
                    {
                        if (inoutType == (int)WarehouseControlMode.入仓)
                        {
                            //入库增加冲补量，则增加当前仓容以及当前入仓总重量
                            warehouseStoreInfo.store_count += planTaskBatchAdjust.adjust_count.Value;
                            warehouseStoreInfo.store_in_count += planTaskBatchAdjust.adjust_count.Value;
                        }
                        else
                        {
                            //出库增加充不量，则扣除当前仓容，同时增加当前出入总重量
                            warehouseStoreInfo.store_count -= planTaskBatchAdjust.adjust_count.Value;
                            warehouseStoreInfo.store_out_count += planTaskBatchAdjust.adjust_count.Value;
                        }
                    }
                    else
                    {
                        if (inoutType == (int)WarehouseControlMode.入仓)
                        {
                            //入仓减量，则减去当前仓容及入库总量
                            warehouseStoreInfo.store_count -= planTaskBatchAdjust.adjust_count.Value;
                            warehouseStoreInfo.store_in_count -= planTaskBatchAdjust.adjust_count.Value;
                        }
                        else
                        {
                            //出库减量，则增加当前仓容，并扣除当前出库总量
                            warehouseStoreInfo.store_count += planTaskBatchAdjust.adjust_count.Value;
                            warehouseStoreInfo.store_out_count -= planTaskBatchAdjust.adjust_count.Value;
                        }
                    }
                    _unitOfWork.AddAction(warehouseStoreInfo, DataActions.Update);

                    _warehouseInOutRecordService.UpdateWarehouseInOutRecordWithStoreInfoAndBatchAdjust(warehouseStoreInfo, planTaskBatchAdjust, plantaskBatch);
                }
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            
            return warehouseStoreInfo;
        }

        public WarehouseStoreInfo CancelWarehouseStoreInfoWithScaleBill(PlanTaskBatchDetail planTaskBatchDetail)
        {
            WarehouseStoreInfo warehouseStoreInfo = null;
            if (planTaskBatchDetail.goods_location_id == null)
            {
                warehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.warehouse_id);
            }
            else
            {
                warehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.warehouse_id,planTaskBatchDetail.goods_location_id.Value);
            }
            if (warehouseStoreInfo == null)
            {
                return null;
            }
            else
            {
                if (planTaskBatchDetail.weight.HasValue)
                {
                    
                    if (planTaskBatchDetail.inout_type == (int)InOutType.入库)
                    {
                        warehouseStoreInfo.store_count -= planTaskBatchDetail.weight.Value;
                        warehouseStoreInfo.store_in_count -= planTaskBatchDetail.weight.Value;
                        PlanTaskInWarehouse ptiw = null;
                        if (planTaskBatchDetail.goods_location_id == null)
                        {
                            ptiw = planTaskBatchDetail.PlanTaskBatch.PlanTaskInWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.plantask_in_amount != 0).LastOrDefault();
                        }
                        else
                        {
                            ptiw = planTaskBatchDetail.PlanTaskBatch.PlanTaskInWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.goods_location_id == planTaskBatchDetail.goods_location_id && w.plantask_in_amount != 0).LastOrDefault();
                        }
                        if (ptiw != null)
                        {
                            //有预排入仓数量，则原有预入库数量扣除本次入库数量
                            warehouseStoreInfo.expectation_in_count += ptiw.plantask_in_amount;
                        }
                    }
                    else
                    {
                        warehouseStoreInfo.store_count += planTaskBatchDetail.weight.Value;
                        warehouseStoreInfo.store_out_count -= planTaskBatchDetail.weight.Value;
                        PlanTaskOutWarehouse ptow = null;
                        if (planTaskBatchDetail.goods_location_id == null)
                        {
                            ptow = planTaskBatchDetail.PlanTaskBatch.PlanTaskOutWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.plantask_out_amount != 0).LastOrDefault();
                        }
                        else
                        {
                            ptow = planTaskBatchDetail.PlanTaskBatch.PlanTaskOutWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.goods_location_id == planTaskBatchDetail.goods_location_id && w.plantask_out_amount != 0).LastOrDefault();
                        }
                        if (ptow != null)
                        {
                            //有预排出仓数量，则原有出库数量扣除本次出库数量
                            warehouseStoreInfo.expectation_in_count += ptow.plantask_out_amount;
                        }
                    }
                    _unitOfWork.AddAction(warehouseStoreInfo, DataActions.Update);
                }
            }
            _warehouseInOutRecordService.CancelWarehouseInOutRecordWithScaleBill(warehouseStoreInfo, planTaskBatchDetail);
            return warehouseStoreInfo;
        }

        public WarehouseStoreInfo CancelWarehouseStoreInfoWithWeightNumber(PlanTaskBatchDetail planTaskBatchDetail)
        {
            WarehouseStoreInfo warehouseStoreInfo = null;
            if (planTaskBatchDetail.goods_location_id == null)
            {
                warehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.warehouse_id);
            }
            else
            {
                warehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.warehouse_id, planTaskBatchDetail.goods_location_id.Value);
            }
            if (warehouseStoreInfo == null)
            {
                return null;
            }
            else
            {
                decimal dWeight = 0;
                if (planTaskBatchDetail.weight != null)
                {
                    dWeight = planTaskBatchDetail.weight.Value;
                }
                
                if (planTaskBatchDetail.inout_type == (int)InOutType.入库)
                {
                    warehouseStoreInfo.store_count -= dWeight;
                    warehouseStoreInfo.store_in_count -= dWeight;
                    PlanTaskInWarehouse ptiw = null;
                    if (planTaskBatchDetail.goods_location_id == null)
                    {
                        ptiw = planTaskBatchDetail.PlanTaskBatch.PlanTaskInWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.plantask_in_amount != 0).LastOrDefault();
                    }
                    else
                    {
                        ptiw = planTaskBatchDetail.PlanTaskBatch.PlanTaskInWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.goods_location_id == planTaskBatchDetail.goods_location_id && w.plantask_in_amount != 0).LastOrDefault();
                    }
                    if (ptiw != null)
                    {
                        //有预排入仓数量，则原有预入库数量扣除本次入库数量
                        warehouseStoreInfo.expectation_in_count += ptiw.plantask_in_amount;
                    }
                }
                else
                {
                    warehouseStoreInfo.store_count += dWeight;
                    warehouseStoreInfo.store_out_count -= dWeight;
                    PlanTaskOutWarehouse ptow = null;
                    if (planTaskBatchDetail.goods_location_id == null)
                    {
                        ptow = planTaskBatchDetail.PlanTaskBatch.PlanTaskOutWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.plantask_out_amount != 0).LastOrDefault();
                    }
                    else
                    {
                        ptow = planTaskBatchDetail.PlanTaskBatch.PlanTaskOutWarehouses.Where(w => w.warehouse_id == planTaskBatchDetail.warehouse_id && w.goods_location_id == planTaskBatchDetail.goods_location_id && w.plantask_out_amount != 0).LastOrDefault();
                    }
                    if (ptow != null)
                    {
                        //有预排出仓数量，则原有出库数量扣除本次出库数量
                        warehouseStoreInfo.expectation_in_count += ptow.plantask_out_amount;
                    }
                }
                _unitOfWork.AddAction(warehouseStoreInfo, DataActions.Update);
            }
            _warehouseInOutRecordService.CancelWarehouseInOutRecordWithScaleBill(warehouseStoreInfo, planTaskBatchDetail);
            return warehouseStoreInfo;
        }
        #endregion


        public WarehouseStoreInfo AddWarehouseStoreWithPlanTaskInOutHouse(WarehouseStoreInfo newWarehouseStoreInfo)
        {
            WarehouseStoreInfo returnWarehouseStoreInfo = null;
            try
            {
                _unitOfWork.AddAction(newWarehouseStoreInfo, DataActions.Add);
                returnWarehouseStoreInfo = newWarehouseStoreInfo;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnWarehouseStoreInfo;
        }


        public WarehouseStoreInfo UpWarehouseStoreInfoWithPlanTaskInOutHouse(WarehouseStoreInfo newWarehouseStoreInfo)
        {
            WarehouseStoreInfo rthouseinfor = null;
            try
            {
                _unitOfWork.AddAction(newWarehouseStoreInfo, DataActions.Update);
                rthouseinfor = newWarehouseStoreInfo;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rthouseinfor;
        }

        public void UpDateCertificate(PlanTaskBatchDetail planTaskBatchDetail)
        {
            if (planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment == null)
            {
                return;
            }
            //更新凭证
            CustomerShipingCertificate csCertificate = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.CShipingCertificate;
            if (csCertificate != null)
            {
                csCertificate.finished_count += planTaskBatchDetail.weight;
                _unitOfWork.AddAction(csCertificate, DataActions.Update);
            }
        }

        public void CancelDateCertificate(PlanTaskBatchDetail planTaskBatchDetail)
        {
            //更新凭证
            CustomerShipingCertificate csCertificate = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.CShipingCertificate;
            if (csCertificate != null)
            {
                csCertificate.finished_count -= planTaskBatchDetail.PlanTaskBatch.PlanTask.plan_weight;
                _unitOfWork.AddAction(csCertificate, DataActions.Update);
            }
        }

        public void UpDateBusinessWeight(PlanTask planTask)
        {
            //更新合同
            if (planTask.Enrolment != null)
            {
                //执行倒仓的时候没有报港单
                Contract contract = planTask.Enrolment.Contract;
                if (contract != null)
                {
                    if (contract.finished_quantity == null)
                    {
                        contract.finished_quantity = planTask.plan_weight;
                    }
                    else
                    {
                        contract.finished_quantity += planTask.plan_weight;
                    }
                    _unitOfWork.AddAction(contract, DataActions.Update);
                    //更新客户计划
                    CustomerPlan customerPlan = contract.CustomerPlan;
                    if (customerPlan == null)
                    {
                        return;
                    }
                    if (planTask.Enrolment.inout_type == (int)InOutType.入库)
                    {
                        if (customerPlan.instore_count == null)
                        {
                            customerPlan.instore_count = planTask.plan_weight;
                        }
                        else
                        {
                            customerPlan.instore_count += planTask.plan_weight;
                        }
                    }
                    else
                    {
                        if (customerPlan.outstore_count == null)
                        {
                            customerPlan.outstore_count = planTask.plan_weight;
                        }
                        else
                        {
                            customerPlan.outstore_count += planTask.plan_weight;
                        }
                    }
                    _unitOfWork.AddAction(customerPlan, DataActions.Update);
                    //更新架空
                    JiaKongManage jiaKongManager = null;
                    DateTime curDate = _getSysDateTimeService.GetSysDateTime();
                    //先根据日期，品种，性质查找
                    jiaKongManager = customerPlan.JiaKongManages.Where(t => t.goods_kind == customerPlan.grain_kind && t.grain_attribute == customerPlan.grain_attribute && t.start_date.Substring(0, 8) == curDate.ToString("yyyyMMdd")).LastOrDefault();
                    if (jiaKongManager == null)
                    {
                        //如果根据品种，日期，性质找不到，则去掉日期后查找
                        jiaKongManager = customerPlan.JiaKongManages.Where(t => t.goods_kind == customerPlan.grain_kind && t.grain_attribute == customerPlan.grain_attribute).LastOrDefault();
                        //存在就更新并新增
                        if (jiaKongManager != null)
                        {
                            JiaKongManage newJiaKongManager = new JiaKongManage();
                            newJiaKongManager.customer_plan = jiaKongManager.customer_plan;
                            newJiaKongManager.goods_kind = jiaKongManager.goods_kind;
                            newJiaKongManager.grain_attribute = jiaKongManager.grain_attribute;
                            newJiaKongManager.in_out_bill_number = planTask.plantask_number;
                            newJiaKongManager.in_out_bill_type = planTask.Enrolment.inout_type == (int)InOutType.入库 ? (int)JiaKongBillType.入库 : (int)JiaKongBillType.出库;
                            newJiaKongManager.owner_org = jiaKongManager.owner_org;
                            if (newJiaKongManager.in_out_bill_type == (int)JiaKongBillType.入库)
                            {
                                newJiaKongManager.jiakong_quantity = jiaKongManager.jiakong_quantity - planTask.plan_weight.Value;
                            }
                            else
                            {
                                newJiaKongManager.jiakong_quantity = jiaKongManager.jiakong_quantity + planTask.plan_weight.Value;
                            }
                            newJiaKongManager.start_date = _getSysDateTimeService.GetSysDateTime().ToString("yyyyMMddHHmmss");

                            jiaKongManager.end_date = _getSysDateTimeService.GetSysDateTime().ToString("yyyyMMddHHmmss");
                            jiaKongManager.time_duration = 0;
                            _unitOfWork.AddAction(newJiaKongManager, DataActions.Add);
                            _unitOfWork.AddAction(jiaKongManager, DataActions.Update);
                        }
                    }
                    else
                    {
                        //更新原有即可
                        if (jiaKongManager.jiakong_quantity == null)
                        {
                            jiaKongManager.jiakong_quantity = planTask.plan_weight.Value;
                        }
                        else
                        {
                            jiaKongManager.jiakong_quantity += planTask.plan_weight.Value;
                        }
                        _unitOfWork.AddAction(jiaKongManager, DataActions.Update);
                    }
                }
            }
        }

        public void UpDateBusinessWeight(PlanTaskBatchDetail planTaskBatchDetail, decimal dFinishedCount)
        {
            PlanTask planTask = planTaskBatchDetail.PlanTaskBatch.PlanTask;
            //更新合同
            if (planTask.Enrolment != null)
            {
                //执行倒仓的时候没有报港单
                Contract contract = planTask.Enrolment.Contract;
                if (contract != null)
                {
                    if (contract.finished_quantity == null)
                    {
                        contract.finished_quantity = dFinishedCount;
                        if (planTask.unit_price != null && planTask.unit_price != 0)
                        {
                            contract.money_of_finished_quantity = contract.finished_quantity * planTask.unit_price;
                        }
                        else
                        {
                            contract.money_of_finished_quantity = 0;
                        }
                    }
                    else
                    {
                        contract.finished_quantity += dFinishedCount;

                        if (planTask.unit_price == null || planTask.unit_price == 0)
                        {
                            contract.money_of_finished_quantity = contract.finished_quantity * (contract.grain_price / 1000);
                        }
                        else
                        {
                            contract.money_of_finished_quantity = contract.finished_quantity * planTask.unit_price;
                        }
                    }
                    _unitOfWork.AddAction(contract, DataActions.Update);
                    //更新客户计划
                    CustomerPlan customerPlan = contract.CustomerPlan;
                    if (customerPlan == null)
                    {
                        return;
                    }
                    if (planTask.Enrolment.inout_type == (int)InOutType.入库)
                    {
                        if (customerPlan.instore_count == null)
                        {
                            customerPlan.instore_count = planTaskBatchDetail.weight;
                        }
                        else
                        {
                            customerPlan.instore_count += planTaskBatchDetail.weight;
                        }
                    }
                    else
                    {
                        if (customerPlan.outstore_count == null)
                        {
                            customerPlan.outstore_count = planTaskBatchDetail.weight;
                        }
                        else
                        {
                            customerPlan.outstore_count += planTaskBatchDetail.weight;
                        }
                    }
                    _unitOfWork.AddAction(customerPlan, DataActions.Update);
                    //更新架空
                    JiaKongManage jiaKongManager = null;
                    DateTime curDate = _getSysDateTimeService.GetSysDateTime();
                    //先根据日期，品种，性质查找
                    jiaKongManager = customerPlan.JiaKongManages.Where(t => t.goods_kind == customerPlan.grain_kind && t.grain_attribute == customerPlan.grain_attribute && t.start_date.Substring(0, 8) == curDate.ToString("yyyyMMdd")).LastOrDefault();
                    if (jiaKongManager == null)
                    {
                        //如果根据品种，日期，性质找不到，则去掉日期后查找
                        jiaKongManager = customerPlan.JiaKongManages.Where(t => t.goods_kind == customerPlan.grain_kind && t.grain_attribute == customerPlan.grain_attribute).LastOrDefault();
                        //存在就更新并新增
                        if (jiaKongManager != null)
                        {
                            JiaKongManage newJiaKongManager = new JiaKongManage();
                            newJiaKongManager.customer_plan = jiaKongManager.customer_plan;
                            newJiaKongManager.goods_kind = jiaKongManager.goods_kind;
                            newJiaKongManager.grain_attribute = jiaKongManager.grain_attribute;
                            newJiaKongManager.in_out_bill_number = planTask.plantask_number;
                            newJiaKongManager.in_out_bill_type = planTask.Enrolment.inout_type == (int)InOutType.入库 ? (int)JiaKongBillType.入库 : (int)JiaKongBillType.出库;
                            newJiaKongManager.owner_org = jiaKongManager.owner_org;
                            if (newJiaKongManager.in_out_bill_type == (int)JiaKongBillType.入库)
                            {
                                newJiaKongManager.jiakong_quantity = jiaKongManager.jiakong_quantity - planTaskBatchDetail.weight.Value;
                            }
                            else
                            {
                                newJiaKongManager.jiakong_quantity = jiaKongManager.jiakong_quantity + planTaskBatchDetail.weight.Value;
                            }
                            newJiaKongManager.start_date = _getSysDateTimeService.GetSysDateTime().ToString("yyyyMMddHHmmss");

                            jiaKongManager.end_date = _getSysDateTimeService.GetSysDateTime().ToString("yyyyMMddHHmmss");
                            jiaKongManager.time_duration = 0;
                            _unitOfWork.AddAction(newJiaKongManager, DataActions.Add);
                            _unitOfWork.AddAction(jiaKongManager, DataActions.Update);
                        }
                    }
                    else
                    {
                        //更新原有即可
                        if (jiaKongManager.jiakong_quantity == null)
                        {
                            jiaKongManager.jiakong_quantity = planTask.plan_weight.Value;
                        }
                        else
                        {
                            jiaKongManager.jiakong_quantity += planTask.plan_weight.Value;
                        }
                        _unitOfWork.AddAction(jiaKongManager, DataActions.Update);
                    }
                }
            }
        }

        public void UdDateContractWeight(PlanTaskBatchDetail planTaskBatchDetail)
        {
            //更新合同
            if (planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment != null)
            {
                //执行倒仓的时候没有报港单
                Contract contract = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.Contract;
                if (contract != null)
                {
                    if (contract.finished_quantity == null)
                    {
                        contract.finished_quantity = planTaskBatchDetail.weight;
                    }
                    else
                    {
                        contract.finished_quantity += planTaskBatchDetail.weight;
                    }
                    _unitOfWork.AddAction(contract, DataActions.Update);
                    //更新客户计划
                    CustomerPlan customerPlan = contract.CustomerPlan;
                    if (customerPlan == null)
                    {
                        return;
                    }
                    if (planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.inout_type == (int)InOutType.入库)
                    {
                        if (customerPlan.instore_count == null)
                        {
                            customerPlan.instore_count = planTaskBatchDetail.weight;
                        }
                        else
                        {
                            customerPlan.instore_count += planTaskBatchDetail.weight;
                        }
                    }
                    else
                    {
                        if (customerPlan.outstore_count == null)
                        {
                            customerPlan.outstore_count = planTaskBatchDetail.weight;
                        }
                        else
                        {
                            customerPlan.outstore_count += planTaskBatchDetail.weight;
                        }
                    }
                    _unitOfWork.AddAction(customerPlan, DataActions.Update);

                    //更新架空
                    JiaKongManage jiaKongManager = null;
                    DateTime curDate = _getSysDateTimeService.GetSysDateTime();
                    //先根据日期，品种，性质查找
                    jiaKongManager = customerPlan.JiaKongManages.Where(t => t.goods_kind == customerPlan.grain_kind && t.grain_attribute == customerPlan.grain_attribute && t.start_date.Substring(0, 8) == curDate.ToString("yyyyMMdd")).LastOrDefault();
                    if (jiaKongManager == null)
                    {
                        //如果根据品种，日期，性质找不到，则去掉日期后查找
                        jiaKongManager = customerPlan.JiaKongManages.Where(t => t.goods_kind == customerPlan.grain_kind && t.grain_attribute == customerPlan.grain_attribute).LastOrDefault();
                        //存在就更新并新增
                        if (jiaKongManager != null)
                        {
                            JiaKongManage newJiaKongManager = new JiaKongManage();
                            newJiaKongManager.customer_plan = jiaKongManager.customer_plan;
                            newJiaKongManager.goods_kind = jiaKongManager.goods_kind;
                            newJiaKongManager.grain_attribute = jiaKongManager.grain_attribute;
                            newJiaKongManager.in_out_bill_number = planTaskBatchDetail.PlanTaskBatch.PlanTask.plantask_number;
                            newJiaKongManager.in_out_bill_type = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.inout_type == (int)InOutType.入库 ? (int)JiaKongBillType.入库 : (int)JiaKongBillType.出库;
                            newJiaKongManager.owner_org = jiaKongManager.owner_org;
                            if (newJiaKongManager.in_out_bill_type == (int)JiaKongBillType.入库)
                            {
                                newJiaKongManager.jiakong_quantity = jiaKongManager.jiakong_quantity - planTaskBatchDetail.weight.Value;
                            }
                            else
                            {
                                newJiaKongManager.jiakong_quantity = jiaKongManager.jiakong_quantity + planTaskBatchDetail.weight.Value;
                            }
                            newJiaKongManager.start_date = _getSysDateTimeService.GetSysDateTime().ToString("yyyyMMddHHmmss");

                            jiaKongManager.end_date = _getSysDateTimeService.GetSysDateTime().ToString("yyyyMMddHHmmss");
                            jiaKongManager.time_duration = 0;
                            _unitOfWork.AddAction(newJiaKongManager, DataActions.Add);
                            _unitOfWork.AddAction(jiaKongManager, DataActions.Update);
                        }
                    }
                    else
                    {
                        //更新原有即可
                        if (jiaKongManager.jiakong_quantity == null)
                        {
                            jiaKongManager.jiakong_quantity = planTaskBatchDetail.PlanTaskBatch.PlanTask.plan_weight.Value;
                        }
                        else
                        {
                            jiaKongManager.jiakong_quantity += planTaskBatchDetail.PlanTaskBatch.PlanTask.plan_weight.Value;
                        }
                        _unitOfWork.AddAction(jiaKongManager, DataActions.Update);
                    }
                }
            }
        }

        private WarehouseStoreInfo CreateNewWarehouseStoreInfo(string warehouseId, PlanTaskBatchDetail planTaskBatchDetail, WarehouseStoreInfo outWarehouseStoreInfo)
        {
            WarehouseStoreInfo warehouseStoreInfo = new WarehouseStoreInfo();
            Warehouse warehouse = _warehouseDal.Single(w => w.warehouse_id == warehouseId).Entity;
            //初始化赋值，需要新增保管帐的只可能为空库存，所以肯定为创建入库
            int sysBillNoID = _sysBillNoService.GetBillNoID(MESSystem.whm, BillPrefix.wsi);
            string orgcode = string.Empty;
            if (warehouse.owner_org != null)
            {
                orgcode = _sysDepartmentUserService.GetSysDepartment(warehouse.owner_org.Value).code;
            }
            //warehouseStoreInfo.warehouse_store_id = AisinoMesServiceHelper.GetOriginalDept(planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.OrganizationEmployee) + _sysBillNoService.GetBillNo(sysBillNoID);
            warehouseStoreInfo.warehouse_store_id = orgcode + _sysBillNoService.GetBillNo(sysBillNoID);
            warehouseStoreInfo.expectation_in_count = 0;
            warehouseStoreInfo.expectation_out_count = 0;
            warehouseStoreInfo.final_statement_weight = 0;
            warehouseStoreInfo.store_out_count = 0;
            warehouseStoreInfo.in_begin_time = _getSysDateTimeService.GetSysDateTime().ToString("yyyyMMdd");
            warehouseStoreInfo.store_status = (int)StoreStatus.在储保管帐;
            warehouseStoreInfo.location_flag = planTaskBatchDetail.goods_location_id.HasValue;
            warehouseStoreInfo.store_count = planTaskBatchDetail.weight.Value;
            warehouseStoreInfo.store_in_count = planTaskBatchDetail.weight.Value;
            if (planTaskBatchDetail.goods_location_id == null)
            {
                warehouseStoreInfo.location_flag = false;
            }
            else
            {
                warehouseStoreInfo.location_flag = true;
                warehouseStoreInfo.goods_localtion = planTaskBatchDetail.goods_location_id.Value;
            }

            if (planTaskBatchDetail.outwarehouse != null && outWarehouseStoreInfo != null)
            {
                //倒仓入库，带入为出库仓的信息
                //获取出库仓原有仓储保管信息
                //WarehouseStoreInfo outWarehouseStoreInfo = null;
                //if (planTaskBatchDetail.outgoods_location_id != null)
                //{
                //    outWarehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.outwarehouse,planTaskBatchDetail.outgoods_location_id.Value);
                //}
                //else
                //{
                //    outWarehouseStoreInfo = GetUsingWarehouseStoreInfoById(planTaskBatchDetail.outwarehouse);
                //}
                warehouseStoreInfo.store_way = outWarehouseStoreInfo.store_way;
                warehouseStoreInfo.grain_attribute = outWarehouseStoreInfo.grain_attribute;
                warehouseStoreInfo.grain_annual = outWarehouseStoreInfo.grain_annual;
                warehouseStoreInfo.grain_kind = outWarehouseStoreInfo.grain_kind;
            }
            else
            {
                //正常入库，直接获取计划带入的原始信息
                if (planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.EnrolmentBasicTypePackage.type_name == "散装")
                {
                    warehouseStoreInfo.store_way = (int)StoreWay.散粮存放;
                }
                else
                {
                    warehouseStoreInfo.store_way = (int)StoreWay.包装粮存放;
                }
                var tempValue = _grainAttributeDal.Single(t => t.is_default_value == true);
                if (tempValue != null && tempValue.Entity != null && tempValue.Entity.grain_attribute_id != null)
                {
                    warehouseStoreInfo.grain_attribute = tempValue.Entity.grain_attribute_id;
                }
                warehouseStoreInfo.grain_annual = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.grain_annual;
                warehouseStoreInfo.grain_kind = planTaskBatchDetail.PlanTaskBatch.PlanTask.Enrolment.goods_kind;
            }

            warehouseStoreInfo.warehouse_id = warehouse.warehouse_id;
            warehouse.warehouse_status = (int)WarehouseStatus.作业;
            _unitOfWork.AddAction(warehouse, DataActions.Update);
            _unitOfWork.AddAction(warehouseStoreInfo, DataActions.Add);

            return warehouseStoreInfo;
        }

    }
}
