using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Service.SysManager;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;
using Aisino.MES.DAL.Repository.Helpers;
using Aisino.MES.Common;

namespace Aisino.MES.Service.WarehouseManager.Impl
{
    public class WarehouseTransactionService :IWarehouseTransactionService
    {
        private Repository<WarehouseTransactionHead> _warehTransactionHeadDal;
        private Repository<WarehouseTransactionDetail> _warehTransactionDetailDal;
        private ISysBillNoService _sysBillNoService;
        private UnitOfWork _unitOfWork;

        public WarehouseTransactionService(
            Repository<WarehouseTransactionHead> warehTransactionHeadDal,
            Repository<WarehouseTransactionDetail> warehTransactionDetailDal,
            ISysBillNoService sysBillNoService,
            UnitOfWork unitOfWork)
        {
            _warehTransactionHeadDal = warehTransactionHeadDal;
            _warehTransactionDetailDal = warehTransactionDetailDal;
            _sysBillNoService = sysBillNoService;
            _unitOfWork = unitOfWork;
        }

        #region 库存交易记录单头

        public WarehouseTransactionHead AddWarehTranHead(WarehouseTransactionHead newWarehTranHead)
        {
            WarehouseTransactionHead returnWarehTranHead = null;
            try
            {
                _warehTransactionHeadDal.Add(newWarehTranHead);
                returnWarehTranHead = newWarehTranHead;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnWarehTranHead;
        }

        public WarehouseTransactionHead UpdateWarehTranHead(WarehouseTransactionHead updateWarehTranHead)
        {
            WarehouseTransactionHead returnWarehTranHead = null;
            try
            {
                _warehTransactionHeadDal.Update(updateWarehTranHead);
                returnWarehTranHead = updateWarehTranHead;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnWarehTranHead;
        }

        public IEnumerable<WarehouseTransactionHead> GetWarehTranHeadList()
        {
            return _warehTransactionHeadDal.GetAll().Entities;
        }

        #endregion

        #region 库存交易记录明细

        public WarehouseTransactionDetail AddWarehTranDetail(WarehouseTransactionDetail newWarehTranDetail)
        {
            WarehouseTransactionDetail returnWarehTranDetail = null;
            try
            {
                _warehTransactionDetailDal.Add(newWarehTranDetail);
                returnWarehTranDetail = newWarehTranDetail;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnWarehTranDetail;
        }

        public WarehouseTransactionDetail UpdWarehTranDetail(WarehouseTransactionDetail updataWarehTranDetail)
        {
            WarehouseTransactionDetail returnWarehTranDetail = null;
            try
            {
                _warehTransactionDetailDal.Update(updataWarehTranDetail);
                returnWarehTranDetail = updataWarehTranDetail;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnWarehTranDetail;
        }

        public void DelWarehTranDetail(WarehouseTransactionDetail warehTranDetail)
        {
            try
            {
                _warehTransactionDetailDal.Delete(warehTranDetail);
            }
            catch (AisinoMesServiceException ex)
            {
                throw ex;
            }
        }

        public void DelWarehTranDetailList(List<WarehouseTransactionDetail> warehTranDetailList)
        {
            _unitOfWork.Actions.Clear();
            foreach (WarehouseTransactionDetail warehTranDetail in warehTranDetailList)
            {
                _unitOfWork.AddAction(warehTranDetail, DataActions.Delete);
            }
            _unitOfWork.Save();
        }
        #endregion
        
        public IList<SysBillNo> GetMotifyTypes(string MESSystemString)
        {
            return _sysBillNoService.GetMotifyTypes(MESSystemString);
        }
        
        public void ModifyWarehTranHead(WarehouseTransactionHead warehTranHead, int billNoID)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                //获取billNoID
                warehTranHead.bill_no = _sysBillNoService.GetBillNo(billNoID);
                _unitOfWork.AddAction(warehTranHead, DataActions.Add);

                _unitOfWork.Save();
            }
            catch (AisinoMesServiceException ex)
            {
                throw ex;
            }
        }

        public void ModifyWarehTranDetail(WarehouseTransactionDetail warehTranDetail)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                _unitOfWork.AddAction(warehTranDetail, DataActions.Add);

                _unitOfWork.Save();
            }
            catch (AisinoMesServiceException ex)
            {
                throw ex;
            }
        }
        
        public int GetWarehTranHeadCount(string strWhere)
        {
            string esql = "select *  from  WarehouseTransactionHead ";
            esql += strWhere;

            IEnumerable<WarehouseTransactionHead> wareTranHeadList = _warehTransactionHeadDal.QueryByESql(esql).Entities;

            return wareTranHeadList.Count();
        }
        
        public IEnumerable<WarehouseTransactionHead> GetPagedWareTranHeadBySql(PagingCriteria paging, string strWhere)
        {
            string strSql = "select *  from  WarehouseTransactionHead ";
            strSql += strWhere;
            return _warehTransactionHeadDal.QueryByESql(strSql, paging).Entities;
        }
        
        public void ConfirmWarehTranHead(WarehouseTransactionHead warehTranHead)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                //表单头更新
                _unitOfWork.AddAction(warehTranHead, DataActions.Update);
                //如果是期初入库单，没有Amount,Invmas;
                if (warehTranHead.bill_type_id ==
                    GetMotifyTypes(MESSystem.whm).Single(b => b.bill_name == "期初入库单" || b.prefix == "WOI").id)
                {
                    //表单详细更新
                    foreach (WarehouseTransactionDetail wtDetail in warehTranHead.WarehouseTransactionDetails)
                    {
                        //更新WarehouseAmount
                        WarehouseAmount amount = new WarehouseAmount();
                        amount.warehouse_id = wtDetail.warehouse_id.Value;
                        amount.current_amount = wtDetail.tran_quantity.Value;
                        amount.invmas_id = wtDetail.invmas_id.Value;
                        amount.expectation_in_amount = 0;
                        amount.expectation_out_amount = 0;
                        _unitOfWork.AddAction(amount, DataActions.Add);

                        //更新WarehouseAmountChangeDetail
                        WarehouseAmountChangeDetail amountDetail = new WarehouseAmountChangeDetail();
                        amountDetail.invmas_id = wtDetail.invmas_id;
                        amountDetail.tran_date = wtDetail.tran_time;
                        //amountDetail.tran_detail_id =
                        amountDetail.tran_quantity = wtDetail.tran_quantity;
                        amountDetail.trans_type = 1;
                        amountDetail.trno = warehTranHead.bill_no;
                        amountDetail.trseq = wtDetail.tran_seq;
                        //amountDetail.trtype = 
                        amountDetail.warehouse_id = wtDetail.warehouse_id;
                        _unitOfWork.AddAction(amountDetail, DataActions.Add);

                    }
                }
                else
                { 
                    //表单详细更新
                    foreach (WarehouseTransactionDetail wtDetail in warehTranHead.WarehouseTransactionDetails)
                    {
                        //更新WarehouseAmount
                        WarehouseAmount amount = wtDetail.Warehouse.WarehouseAmounts.ToList()[0];
                        amount.current_amount = wtDetail.tran_quantity.Value;
                        _unitOfWork.AddAction(amount, DataActions.Update);

                        //更新WarehouseAmountChangeDetail
                        WarehouseAmountChangeDetail amountDetail = new WarehouseAmountChangeDetail();
                        amountDetail.invmas_id = wtDetail.invmas_id;
                        amountDetail.tran_date = wtDetail.tran_time;
                        //amountDetail.tran_detail_id =
                        amountDetail.tran_quantity = wtDetail.tran_quantity;
                        amountDetail.trans_type = 1;
                        amountDetail.trno = warehTranHead.bill_no;
                        amountDetail.trseq = wtDetail.tran_seq;
                        // amountDetail.trtype = 
                        amountDetail.warehouse_id = wtDetail.warehouse_id;
                        _unitOfWork.AddAction(amountDetail, DataActions.Add);

                    }
 
                }

                _unitOfWork.Save();
            }
            catch(AisinoMesServiceException ex)
            {
                throw ex; 
            }
        }
    }
}
