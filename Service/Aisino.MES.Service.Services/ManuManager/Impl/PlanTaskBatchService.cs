using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.Service.SysManager;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Repository.Helpers;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class PlanTaskBatchService : IPlanTaskBatchService
    {
        private Repository<PlanTaskBatch> _planTaskBatchDal;
        private Repository<PlanTaskBatchDetail> _planTaskBatchDetailDal;
        private UnitOfWork _unitOfWork;

        public PlanTaskBatchService(Repository<PlanTaskBatch> planTaskBatchDal,
                                    Repository<PlanTaskBatchDetail> planTaskBatchDetailDal,
                                    UnitOfWork unitOfWork)
        {

            _planTaskBatchDal = planTaskBatchDal;
            _planTaskBatchDetailDal = planTaskBatchDetailDal;

            _unitOfWork = unitOfWork;
        }

        #region 计划批次操作
        /// <summary>
        /// 根据计划创建计划批次
        /// 一般用于计划第一次创建的时候
        /// </summary>
        /// <param name="planTask">传入的计划</param>
        /// <returns></returns>
        public PlanTaskBatch AddPlanTaskBatchWithPlanTask(PlanTask planTask, PlanTaskType planTaskType)
        {
            PlanTaskBatch newPlanTaskBatch = null;
            try
            {
                //设置是否需要工艺路线，工艺路线上是否有计量，创建时间，是否值仓，是否需要内部车辆，是否经过作业点
                //复制计划编号
                //计划批次编号为计划编号+流水号，默认为第一流水号，补足三位
                newPlanTaskBatch = new PlanTaskBatch();
                newPlanTaskBatch.bor_weight = planTaskType.bor_weight;
                newPlanTaskBatch.create_time = planTask.create_time.Value;
                newPlanTaskBatch.need_bor = planTaskType.need_bor;
                newPlanTaskBatch.need_onduty = planTaskType.need_onduty;
                newPlanTaskBatch.need_vehicle = planTaskType.need_vehicle;
                newPlanTaskBatch.need_workplace = planTaskType.need_workplace;
                newPlanTaskBatch.plantask_batch_number = planTask.plantask_number + string.Format("{0:D3}", planTask.PlanTaskBatches.Count + 1);
                newPlanTaskBatch.plantask_number = planTask.plantask_number;
                if (planTask.Enrolment != null)
                {
                    newPlanTaskBatch.need_quality_test = planTask.Enrolment.EnrolmentType.need_quality_test;
                }
                else
                {
                    newPlanTaskBatch.need_quality_test = false;
                }
                if (planTask != null && planTask.unit_price != null)
                {
                    newPlanTaskBatch.unit_price = planTask.unit_price;
                }
                newPlanTaskBatch.is_settlemented = false;

                _unitOfWork.AddAction(newPlanTaskBatch, DAL.Enums.DataActions.Add);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return newPlanTaskBatch;
        }

        public PlanTaskBatch AddPlanTaskBatchWithCreate(PlanTask plantask, PlanTaskBatch newPlanTaskBatch)
        {
            try
            {
                //设置是否需要工艺路线，工艺路线上是否有计量，创建时间，是否值仓，是否需要内部车辆，是否经过作业点
                //复制计划编号
                //计划批次编号为计划编号+流水号，默认为第一流水号，补足三位
                newPlanTaskBatch.create_time = plantask.create_time.Value;
                newPlanTaskBatch.plantask_batch_number = plantask.plantask_number + string.Format("{0:D3}", 1);
                newPlanTaskBatch.plantask_number = plantask.plantask_number;

                newPlanTaskBatch.is_settlemented = false;

                _unitOfWork.AddAction(newPlanTaskBatch, DAL.Enums.DataActions.Add);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return newPlanTaskBatch;
        }

        /// <summary>
        /// 复制上一条批次，并根据此新建一批次
        /// 一般用于值仓时由于粮食质量原因引起的换仓操作，且新建批次必须需要检化验
        /// </summary>
        /// <param name="PlanTaskBatch">传入的上一条批次</param>
        /// <returns></returns>
        public PlanTaskBatch AddPlanTaskBatchWithPlanTaskBatch(PlanTaskBatch planTaskBatch)
        {
            PlanTaskBatch newPlanTaskBatch = null;
            try
            {
                //设置是否需要工艺路线，工艺路线上是否有计量，创建时间，是否值仓，是否需要内部车辆，是否经过作业点
                //复制计划编号
                //计划批次编号为计划编号+流水号，默认为第一流水号，补足三位
                newPlanTaskBatch = new PlanTaskBatch();
                newPlanTaskBatch.bor_weight = planTaskBatch.bor_weight;
                newPlanTaskBatch.create_time = planTaskBatch.create_time;
                newPlanTaskBatch.need_bor = planTaskBatch.need_bor;
                newPlanTaskBatch.need_onduty = planTaskBatch.need_onduty;
                newPlanTaskBatch.need_vehicle = planTaskBatch.need_vehicle;
                newPlanTaskBatch.need_workplace = planTaskBatch.need_workplace;
                newPlanTaskBatch.plantask_batch_number = planTaskBatch.PlanTask.plantask_number + string.Format("{0:D3}", planTaskBatch.PlanTask.PlanTaskBatches.Count + 1);
                newPlanTaskBatch.plantask_number = planTaskBatch.plantask_number;

                newPlanTaskBatch.need_quality_test = true;

                newPlanTaskBatch.is_settlemented = false;

                _unitOfWork.AddAction(newPlanTaskBatch, DAL.Enums.DataActions.Add);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return newPlanTaskBatch;
        }


        public PlanTaskBatch GetSinglePlanTaskBatchWithNumber(string batchNumber)
        {
            var ptBatch = _planTaskBatchDal.Single(ptb => ptb.plantask_batch_number == batchNumber);
            if (ptBatch.HasValue)
                return ptBatch.Entity;
            else
                return null;
        }

        public PlanTaskBatch GetSinglePlanTaskBatchWithGenerateid(int generateid)
        {
            var ptBatch = _planTaskBatchDal.Single(ptb => ptb.generate_id == generateid
                , new string[] { "PlanTaskBatchDetails", "PlanTask", "PlanTask.Enrolment", "BusinessDisposes", "BusinessDisposes.BusinessDisposeDetails" });
            if (ptBatch.HasValue)
                return ptBatch.Entity;
            else
                return null;
        }

        public List<PlanTaskBatch> GetRunningPlanTaskBatchOutWarehouseInnerVehicleWithWharf()
        {
            //查找作业计划为执行状态，且作业批次结束时间为空的作业批次（最后一笔）
            //在确认在这些作业批次中是否存在正在使用传入内部车辆id的作业批次
            List<PlanTaskBatch> planTaskBatchs = _planTaskBatchDal.Find(ptb => ptb.batch_finish_time == null
                && ptb.PlanTask.plan_status.Value == (int)PlanTaskStatus.执行 && ptb.PlanTask.Enrolment.inout_type == (int)InOutType.出库
                && ptb.PlanTaskBatchVehicles != null && ptb.PlanTaskBatchVehicles.Count > 0
                && ptb.PlanTaskBatchWorkPlaces != null && ptb.PlanTaskBatchWorkPlaces.Count > 0,
                new string[] { "PlanTaskBatchDetails", "PlanTaskBatchVehicles", "PlanTaskBatchSiteScales", "PlanTaskBatchWorkPlaces", "PlanTaskInWarehouses", "PlanTaskOutWarehouses" }).Entities.ToList();

            return planTaskBatchs;
        }

        public List<PlanTaskBatchDetail> GetPlanTaskBatchDetailWithOnlyGross(PlanTaskBatch planTaskBatch)
        {
            List<PlanTaskBatchDetail> planTaskBatchDetails = null;
            try
            {
                planTaskBatchDetails = _planTaskBatchDetailDal.Find(ptbd => ptbd.plantask_batch_number == planTaskBatch.plantask_batch_number
                && ptbd.gross_time != null
                && ptbd.tare_time == null,
                new string[] { "PlanTaskBatch.PlanTaskBatchDetails" }).Entities.ToList();//
            }
            catch
            {
                planTaskBatchDetails = null;
            }
            return planTaskBatchDetails;
        }

        public PlanTaskBatch GetRunningPlanTaskBatchByInnerVehicle(string innerVehicleId)
        {
            //查找作业计划为执行状态，且作业批次结束时间为空的作业批次（最后一笔）
            //在确认在这些作业批次中是否存在正在使用传入内部车辆id的作业批次
            List<PlanTaskBatch> planTaskBatchs = _planTaskBatchDal.Find(ptb => ptb.batch_finish_time == null && ptb.PlanTask.plan_status.Value == (int)PlanTaskStatus.执行, new string[] { "PlanTaskBatchDetails", "PlanTaskBatchVehicles", "PlanTaskBatchSiteScales", "PlanTaskBatchWorkPlaces", "PlanTaskInWarehouses", "PlanTaskOutWarehouses" }).Entities.ToList();
            foreach (PlanTaskBatch planTaskBatch in planTaskBatchs)
            {
                if (planTaskBatch.PlanTaskBatchVehicles != null && planTaskBatch.PlanTaskBatchVehicles.Count > 0)
                {
                    foreach (PlanTaskBatchVehicle planTaskBatchVehicle in planTaskBatch.PlanTaskBatchVehicles)
                    {
                        if (planTaskBatchVehicle.vehicle_id != null && planTaskBatchVehicle.vehicle_id == innerVehicleId)
                        {
                            return planTaskBatch;
                        }
                    }
                }

            }
            return null;

            //IEnumerable<PlanTaskBatch> planTaskBatchs = _planTaskBatchDal.Find(ptb => ptb.batch_finish_time == null && ptb.PlanTask.plan_status.Value == (int)PlanTaskStatus.执行).Entities
            //                            .Where(ptb => ptb.PlanTaskBatchVehicles.Count > 0 && ptb.PlanTaskBatchVehicles.Single(v => v.vehicle_id == innerVehicleId) != null);

            //if (planTaskBatchs != null && planTaskBatchs.Count() > 0)
            //{
            //    return planTaskBatchs.FirstOrDefault();
            //}
            //else
            //{
            //    return null;
            //}
        }

        public PlanTaskBatch GetRunningPlanTaskBatchByPlanTaskNumber(string plantaskNumber)
        {
            return _planTaskBatchDal.Find(pb => pb.plantask_number == plantaskNumber, new string[] { "PlanTaskBatchDetails", "PlanTaskBatchVehicles", "PlanTaskBatchSiteScales", "PlanTaskBatchWorkPlaces", "PlanTaskInWarehouses", "PlanTaskOutWarehouses" }).Entities.LastOrDefault();
        }

        public RepositoryResultList<PlanTaskBatch> SelectPlanTaskBatchWithNoAssay(int PageNumber, int PageSize)
        {
            return _planTaskBatchDal.Find(t => t.Samples.Count == 0, new PagingCriteria { PageNumber = PageNumber, PageSize = PageSize, SortBy = "plantask_batch_number", SortDirection = "asc" });
        }

        public RepositoryResultList<PlanTaskBatch> SelectPlanTaskBatchWithAssay(int userId, int PageNumber, int PageSize, string vehiclePlate)
        {
            return _planTaskBatchDal.Find(t => t.Samples.Count > 0 && t.PlanTask.Enrolment.plate_number == vehiclePlate && t.Samples.Where(d => d.sample_user == userId).Count() > 0, new PagingCriteria { PageNumber = PageNumber, PageSize = PageSize, SortBy = "plantask_batch_number", SortDirection = "asc" });
        }
        #endregion

        #region 计划批次明细操作
        public PlanTaskBatchDetail GetNotFinishBatchDetailByNumberAndVehicle(string planTaskBatchNumber, string vehicleId)
        {
            //_planTaskBatchDetailDal.Find(ptbd => ptbd.plantask_batch_number 
            return null;
        }

        /// <summary>
        /// 根据磅单号查询PlanTaskBatchDetail
        /// </summary>
        /// <param name="ScaleBillNumber">磅单号</param>
        /// <returns></returns>
        public PlanTaskBatchDetail GetPlanTaskBatchDetailByScaleBillNumber(string ScaleBillNumber)
        {
            //_planTaskBatchDetailDal.RefreshData();
            return _planTaskBatchDetailDal.Single(ptbd => ptbd.scale_number == ScaleBillNumber).Entity;
        }

        /// <summary>
        /// 根据称重号查询PlanTaskBatchDetail
        /// </summary>
        /// <param name="WeightNumber">称重号</param>
        /// <returns></returns>
        public PlanTaskBatchDetail GetPlanTaskBatchDetailByWeightNumber(string WeightNumber)
        {
            return _planTaskBatchDetailDal.Single(ptbd => ptbd.weight_number == WeightNumber).Entity;
        }

        /// <summary>
        /// 完成值仓确认
        /// </summary>
        /// <param name="planTaskBatchDetail"></param>
        public void UpdatePlanTaskBatchDetailByDutyConfirm(PlanTaskBatchDetail planTaskBatchDetail)
        {
            _planTaskBatchDetailDal.Update(planTaskBatchDetail);
        }

        public void RefreshData()
        {
            this._planTaskBatchDal.RefreshData();
            this._planTaskBatchDetailDal.RefreshData();
        }
        #endregion


        public PlanTaskBatch UpdatePlantaskBatch(PlanTaskBatch upplantaskbatch)
        {
            _planTaskBatchDal.Update(upplantaskbatch);
            return upplantaskbatch;
        }


        public PlanTaskBatch UpPlantaskBatchWithUnit(PlanTaskBatch upplantaskbatch)
        {
            _unitOfWork.AddAction(upplantaskbatch, DataActions.Update);
            return upplantaskbatch;
        }


        //先找到报港状态为执行中的，并且该报港使用了此标签卡
        public PlanTaskBatch GetSinglePlanTaskBatchWithRfid(string rfidNumber)
        {
            PlanTaskBatch theBatch = _planTaskBatchDal.Find(ptb => ptb.PlanTask.Enrolment.status == (int)EnrolmentStatue.执行中 && ptb.PlanTask.Enrolment.RFIDTagIssues.Any(rf => rf.RFIDTag.tag_main_id == rfidNumber), new string[] { "PlanTaskBatchDetails", "PlanTaskBatchVehicles", "PlanTaskBatchSiteScales", "PlanTaskBatchWorkPlaces", "PlanTaskInWarehouses", "PlanTaskOutWarehouses" }).Entities.LastOrDefault();
            //if (theBatch == null)
            //{
            //    return null;
            //}
            //else
            //{
            //    return theBatch.PlanTask.PlanTaskBatches.Last();
            //}
            return theBatch;
        }

        public IEnumerable<PlanTaskBatch> GetEnrolmentListIsRuning()
        {
            //return _planTaskBatchDal.Find(ptb => ptb.PlanTask.Enrolment.status == (int)EnrolmentStatue.确认).Entities;
            return _planTaskBatchDal.Find(ptb => ptb.PlanTask.plan_status != (int)PlanTaskStatus.完成
                && ptb.PlanTask.plan_status != (int)PlanTaskStatus.终止 && ptb.PlanTask.plan_status != (int)PlanTaskStatus.作废
                && ptb.PlanTask.PlanTaskType.warehouse_control_mode == (int)WarehouseControlMode.入仓,
                new string[] { "Samples", "PlanTask", "PlanTask.PlanTaskType", "PlanTask.Enrolment" }).Entities.Where(p => p.Samples.Count == 0);
        }

        public PlanTaskBatch GetPlanTaskBatchByCardID(string card_mainid)
        {
            var ptbatchList = _planTaskBatchDal.Find(ptb => ptb.PlanTask.Enrolment != null
                && ptb.PlanTask.Enrolment.RFIDTagIssues.Any(rf => rf.RFIDTag.tag_main_id == card_mainid)
                , new string[] { "PlanTask", "PlanTask.Enrolment", "BusinessDisposes", "Samples.Assays", "PlanTask.Enrolment.RFIDTagIssues.RFIDTag", "BusinessDisposes.BusinessDisposeDetails" });
            if (ptbatchList.Entities.Count() != 0)
            {
                return ptbatchList.Entities.OrderBy(pt => pt.create_time).Last();
            }
            else
            {
                return null;
            }
        }

        public PlanTaskBatch GetPlanTaskBatchByDiBangCode(string code)
        {
            var tempBTList = _planTaskBatchDal.Find(ptb => ptb.PlanTaskBatchDetails.First().scale_number == code);
            if (tempBTList.Entities.ToList().Count > 0)
            {
                return tempBTList.Entities.Last();
            }
            else
            {
                return null;
            }
        }


        public void UpdatePlantaskAndBatch(PlanTask thePT, PlanTaskBatch thePTBatch)
        {
            _unitOfWork.AddAction(thePT, DataActions.Update);
            _unitOfWork.AddAction(thePTBatch, DataActions.Update);
            _unitOfWork.Save();
        }

        public IList<Warehouse> FindPlanTaskBatchWarehousList(string plantaskBatchNum)
        {
            IList<Warehouse> warehouseList = new List<Warehouse>();
            RepositoryResultList<PlanTaskBatchDetail> plantaskBatchDetails = _planTaskBatchDetailDal.Find(d => d.plantask_batch_number == plantaskBatchNum, new string[] { "Warehouse", "GoodsLocation" });

            return plantaskBatchDetails.Entities.Select(p => p.Warehouse).Distinct().ToList();
        }

        public IList<GoodsLocation> FindPlanTaskBatchLocationList(string plantaskBatchNum, string warehouseId)
        {
            IList<Warehouse> warehouseList = new List<Warehouse>();
            RepositoryResultList<PlanTaskBatchDetail> plantaskBatchDetails = _planTaskBatchDetailDal.Find(d => d.plantask_batch_number == plantaskBatchNum && d.warehouse_id == warehouseId, new string[] { "Warehouse", "GoodsLocation" });

            return plantaskBatchDetails.Entities.Select(p => p.GoodsLocation).Distinct().ToList();
        }

        public List<PlanTaskBatch> GePlanTaskBatchsByPlanTaskBatchNumber(string[] plantaskbatchnumbers)
        {
            string strSql = "Select * from PlanTaskBatch where 1 = 1 ";
            string strWhere = string.Empty;
            foreach (string planTaskBatchNum in plantaskbatchnumbers)
            {
                if (strWhere.Trim().Length == 0)
                {
                    strWhere = "and (plantask_batch_number = '" + planTaskBatchNum + "'";
                    continue;
                }
                strWhere = strWhere + " or plantask_batch_number = '" + planTaskBatchNum + "'";
            }
            if (strWhere.Trim().Length > 0)
            {
                strWhere = strWhere + ")";
            }
            return _planTaskBatchDal.QueryByESql(strSql + strWhere).Entities.ToList();
        }

        public PlanTaskBatch GetSinglePlanTaskBatchWithGenerateidOnlyBatch(int generateid)
        {
            var ptBatch = _planTaskBatchDal.Single(ptb => ptb.generate_id == generateid);
            if (ptBatch.HasValue)
                return ptBatch.Entity;
            else
                return null;
        }

        public PlanTaskBatch GetSinglePlanTaskBatchWithGenerateidWithAll(int generateid)
        {
            var ptBatch = _planTaskBatchDal.Single(ptb => ptb.generate_id == generateid, "PlanTaskBatchDetails", "PlanTaskBatchVehicles", "PlanTaskBatchSiteScales", "PlanTaskBatchWorkPlaces", "PlanTaskInWarehouses", "PlanTaskOutWarehouses", "PlanTaskBatchVehicles", "PlanTask", "PlanTask.Enrolment", "PlanTask.Enrolment.Contract", "PlanTask.Enrolment.Contract.CustomerShipingCertificates");
            if (ptBatch.HasValue)
                return ptBatch.Entity;
            else
                return null;
        }
    }
}
