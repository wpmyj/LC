using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Service.SysManager;

namespace Aisino.MES.Service.WarehouseManager.Impl
{
    public class WarehouseAmountService : IWarehouseAmountService
    {
        private Repository<Warehouse> _warehouseDal;
        //private Repository<WarehouseAmount> _warehouseAmountDal;
        private UnitOfWork _unitOfWork;

        public WarehouseAmountService(Repository<Warehouse> warehouseDal,
            //Repository<WarehouseAmount> warehouseAmountDal,
            UnitOfWork unitOfWork)
        {
            _warehouseDal = warehouseDal;
            //_warehouseAmountDal = warehouseAmountDal;
            _unitOfWork = unitOfWork;
        }

    //    #region 仓库
    //    /// <summary>
    //    /// 查询所有的仓库货物
    //    /// </summary>
    //    /// <returns>返回所有仓库货物列表</returns>
    //    public IEnumerable<WarehouseAmount> GetAllWarehouseAmount()
    //    {
    //        return _warehouseAmountDal.GetAll().Entities;
    //    }

    //    /// <summary>
    //    /// 查询该id的仓库货物
    //    /// </summary>
    //    /// <param name="id">仓库货物ID</param>
    //    /// <returns>返回该该id的仓库货物</returns>
    //    public WarehouseAmount GetWarehouseAmount(int id)
    //    {
    //        return _warehouseAmountDal.Single(w => w.id == id).Entity;
    //    }



    //    // 查询对应warehouse_id的仓库货物
    //    public WarehouseAmount GetWarehouseAmountByWarehouseId(int warehouse_id)
    //    {
    //        return _warehouseAmountDal.Single(w => w.warehouse_id == warehouse_id).Entity;

    //    }

    //    // 查询对应invmas_id,warehouse_id的仓库货物
    //    public WarehouseAmount GetWarehouseAmount(int invmas_id, int warehouse_id)
    //    {
    //        return _warehouseAmountDal.Single(w => w.invmas_id == invmas_id && w.warehouse_id == warehouse_id).Entity;

    //    }

    //    // 查询对应invmas_id,warehouse_id的仓库货物
    //    public WarehouseAmount GetWarehouseAmount_NoTracking(int invmas_id, int warehouse_id)
    //    {
    //        return _warehouseAmountDal.SingleAsNoTracking(w => w.invmas_id == invmas_id && w.warehouse_id == warehouse_id).Entity;

    //    }


    //    // 查询对应invmas_id的待出库的仓库货物
    //    // 并且current_amount - expectation_out_amount > 0
    //    public IEnumerable<WarehouseAmount> GetWarehouseAmount_Out(int invmas_id)
    //    {
    //        string esql = "select *"
    //                     + " from WarehouseAmount "
    //                     + " where current_amount - expectation_out_amount > 0"
    //                     + " and   invmas_id = " + invmas_id.ToString();


    //        return _warehouseAmountDal.QueryByESql(esql).Entities;
    //    }



    //    public IEnumerable<Warehouse> GetWarehouse_NoInvmas()
    //    {
    //        //WarehouseAmount 不存在的货物的仓库, 
    //        string esql = "select *"
    //                    + " from Warehouse"
    //                    + " where  NOT EXISTS "
    //                    + " (select warehouse_id from WarehouseAmount where warehouse_id = Warehouse.id)";

    //        return _warehouseDal.QueryByESql(esql).Entities;
    //    }

    //    public IEnumerable<WarehouseAmount> GetWarehouseAmount_In(int invmas_id)
    //    {
    //        //WarehouseAmount 存在的， 但是invmas_id 是相同的内容(m.invmas_id = 1); 可用容量 > 0
    //        string esql = "select *"
    //                    + " from WarehouseAmount m, Warehouse h "
    //                    + " where m.warehouse_id = h.id"
    //                    + " and   h.warehouse_capacity - m.current_amount - m.expectation_in_amount > 0"
    //                    + " and   m.invmas_id = " + invmas_id.ToString();


    //        return _warehouseAmountDal.QueryByESql(esql).Entities;
    //    }


    //    //获得可用的入库容量
    //    public decimal GetAvailableCapacity_In(int warehouse_id)
    //    {
    //        decimal availableAmount;

    //        //获得对应仓库
    //        Warehouse warehouse = _warehouseDal.Single(w => w.id == warehouse_id).Entity;
    //        availableAmount = warehouse.warehouse_capacity.Value;

    //        //W获得对应仓库容量 > 0
    //        WarehouseAmount warehouseAmount = _warehouseAmountDal.Single(w => w.warehouse_id == warehouse_id).Entity;

    //        if (warehouseAmount != null)
    //        {
    //            availableAmount = availableAmount - warehouseAmount.current_amount - warehouseAmount.expectation_in_amount;
    //        }

    //        return availableAmount;
    //    }

    //    //获得可用的出库容量
    //    public decimal GetAvailableCapacity_Out(int warehouse_id)
    //    {
    //        decimal availableAmount = 0;


    //        //W获得对应仓库容量 > 0
    //        WarehouseAmount warehouseAmount = _warehouseAmountDal.Single(w => w.warehouse_id == warehouse_id).Entity;

    //        if (warehouseAmount != null)
    //        {
    //            availableAmount = warehouseAmount.current_amount - warehouseAmount.expectation_out_amount;
    //        }

    //        return availableAmount;
    //    }


    //    /// 
    //    /// <param name="newWarehouseAmount"></param>
    //    public WarehouseAmount AddWarehouseAmount(WarehouseAmount newWarehouseAmount)
    //    {
    //        WarehouseAmount returnWarehouseAmount = null;
    //        try
    //        {
    //            _warehouseAmountDal.Add(newWarehouseAmount);
    //            returnWarehouseAmount = newWarehouseAmount;
    //        }
    //        catch (RepositoryException ex)
    //        {
    //            throw ex;
    //        }
    //        return returnWarehouseAmount;
    //    }

    //    /// 
    //    /// <param name="updateWarehouseAmount"></param>
    //    public WarehouseAmount UpdateWarehouseAmount(WarehouseAmount updateWarehouseAmount)
    //    {
    //        WarehouseAmount returnWarehouseAmount = null;
    //        try
    //        {
    //            _warehouseAmountDal.Update(updateWarehouseAmount);
    //            returnWarehouseAmount = updateWarehouseAmount;
    //        }
    //        catch (RepositoryException ex)
    //        {
    //            throw ex;
    //        }
    //        return returnWarehouseAmount;

    //    }

    //    /// 
    //    /// <param name="newWarehouseAmount"></param>
    //    public WarehouseAmount AddWarehouseAmount_Unitwork(WarehouseAmount newWarehouseAmount)
    //    {
    //        WarehouseAmount returnWarehouseAmount = null;
    //        try
    //        {
    //            _unitOfWork.AddAction(newWarehouseAmount, DataActions.Add);

    //            returnWarehouseAmount = newWarehouseAmount;
    //        }
    //        catch (RepositoryException ex)
    //        {
    //            throw ex;
    //        }
    //        return returnWarehouseAmount;
    //    }



    //    /// 
    //    /// <param name="updateWarehouseAmount"></param>
    //    public WarehouseAmount UpdateWarehouseAmount_Unitwork(WarehouseAmount updateWarehouseAmount)
    //    {
    //        WarehouseAmount returnWarehouseAmount = null;
    //        try
    //        {
    //            _unitOfWork.AddAction(updateWarehouseAmount, DataActions.Update);

    //            returnWarehouseAmount = updateWarehouseAmount;
    //        }
    //        catch (RepositoryException ex)
    //        {
    //            throw ex;
    //        }
    //        return returnWarehouseAmount;

    //    }



    //    /// <summary>
    //    /// 根据仓库ID查询仓容
    //    /// </summary>
    //    /// <param name="warehInvmasID">仓库ID</param>
    //    /// <returns>仓容</returns>
    //    public IEnumerable<WarehouseAmount> GetAmountByWareid(int warehouse_id)
    //    {

    //        string esql = "select amt.*"
    //        + " from  WarehouseAmount amt, WarehouseBatchHead head"
    //        + " where head.warehouse_id = amt.warehouse_id"
    //        + " and head.warehouse_id = " + warehouse_id.ToString();

    //        return _warehouseAmountDal.QueryByESql(esql).Entities;
    //    }

    //    #endregion
    }
}
