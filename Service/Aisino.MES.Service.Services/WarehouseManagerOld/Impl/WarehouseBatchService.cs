using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Helpers;

namespace Aisino.MES.Service.WarehouseManager.Impl
{
    public class WarehouseBatchService : IWarehouseBatchService
    {

        private Repository<WarehouseBatchHead> _warehouseBatchHeadDal;
        private Repository<WarehouseBatchDetail> _warehouseBatchDetail;
        private UnitOfWork _unitOfWork;

        public WarehouseBatchService(
            Repository<WarehouseBatchHead> warehouseBatchHeadDal,
            Repository<WarehouseBatchDetail> warehouseBatchDetail,
            UnitOfWork unitOfWork)
        {
            _warehouseBatchHeadDal = warehouseBatchHeadDal;
            _warehouseBatchDetail = warehouseBatchDetail;
            _unitOfWork = unitOfWork;
        }

        #region 库存批次记录单头

        public WarehouseBatchHead AddBatchHead(WarehouseBatchHead newWarehTranHead)
        {
            WarehouseBatchHead returnWarehTranHead = null;
            try
            {
                _warehouseBatchHeadDal.Add(newWarehTranHead);
                returnWarehTranHead = newWarehTranHead;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnWarehTranHead;
        }

        public WarehouseBatchHead UpdateBatchHead(WarehouseBatchHead updateWarehTranHead)
        {
            WarehouseBatchHead returnWarehTranHead = null;
            try
            {
                _warehouseBatchHeadDal.Update(updateWarehTranHead);
                returnWarehTranHead = updateWarehTranHead;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnWarehTranHead;
        }

        public IEnumerable<WarehouseBatchHead> GetBatchHeadList()
        {
            return _warehouseBatchHeadDal.GetAll().Entities;
        }

        #endregion

        #region 库存批次记录明细

        public WarehouseBatchDetail AddBatchDetail(WarehouseBatchDetail newBatchDetail)
        {
            WarehouseBatchDetail returnBatchDetail = null;
            try
            {
                _warehouseBatchDetail.Add(newBatchDetail);
                returnBatchDetail = newBatchDetail;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnBatchDetail;
        }

        public WarehouseBatchDetail UpdateBatchDetail(WarehouseBatchDetail updateBatchDetail)
        {
            WarehouseBatchDetail returnBatchDetail = null;
            try
            {
                _warehouseBatchDetail.Update(updateBatchDetail);
                returnBatchDetail = updateBatchDetail;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnBatchDetail;
        }

        public void DelBatchDetail(WarehouseBatchDetail warehouseBatchDetail)
        {
            try
            {
                _warehouseBatchDetail.Delete(warehouseBatchDetail);
            }
            catch (AisinoMesServiceException ex)
            {
                throw ex;
            }
        }

  
        #endregion



    }
}
