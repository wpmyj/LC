using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Repository.Helpers;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;
using Aisino.MES.DAL.Interfaces;

namespace Aisino.MES.Service.WarehouseManager.Impl
{
    public class WarehouseBaseService : IWarehouseBaseService
    {
        private Repository<Warehouse> _warehouseDal;
        private Repository<DayDefaultWarehouse> _dayDefaultWarehouseDal;
        private UnitOfWork _unitOfWork;

        public WarehouseBaseService(Repository<Warehouse> warehouseDal,Repository<DayDefaultWarehouse> dayDefaultWarehouseDal, UnitOfWork unitOfWork)
        {
            _warehouseDal = warehouseDal;
            _dayDefaultWarehouseDal = dayDefaultWarehouseDal;
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 查询所有仓房信息
        /// </summary>
        /// <returns>返回所有仓房列表</returns>
        public IEnumerable<Warehouse> GetAllWarehouse()
        {
            var ss = _warehouseDal.GetAll().Entities;
            return ss;
        }

        /// <summary>
        /// 查询所有有效仓房信息
        /// </summary>
        /// <returns>返回所有有效仓房列表</returns>
        public IEnumerable<Warehouse> GetAllEnabledWarehouse()
        {
            return _warehouseDal.Find(w => w.is_available == true).Entities.ToList(); 
            //return _warehouseDal.GetAll().Entities.ToList(); 
        }

        /// <summary>
        /// 根据Id获取仓房实例
        /// </summary>
        /// <param name="warehouse_id"></param>
        /// <returns>返回仓房实例</returns>
        public Warehouse GetWarehouseById(string warehouse_id)
        {
            var warehouse = _warehouseDal.Single(w => w.warehouse_id == warehouse_id);
            if (warehouse.HasValue)
            {
                return warehouse.Entity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据Id(int 自增)获取仓房实例
        /// </summary>
        /// <param name="generate_id"></param>
        /// <returns>返回仓房实例</returns>
        public Warehouse GetWarehouseByGenerateid(int generate_id)
        {
            var warehouse = _warehouseDal.Single(w => w.generate_id == generate_id);
            if (warehouse.HasValue)
            {
                return warehouse.Entity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据仓库保管员查询所有有效仓房信息
        /// </summary>
        /// <param name="iKeeper"></param>
        /// <returns></returns>
        public IEnumerable<Warehouse> GetWarehouseByKeeper(int iKeeper)
        {
            return _warehouseDal.Find(w => w.is_available == true && w.keeper == iKeeper).Entities.ToList();
        }

        /// <summary>
        /// 根据仓库标识更新仓库状态
        /// </summary>
        /// <param name="generate_id">仓库标识(int型自增)</param>
        /// <param name="warehouse_status">仓库状态</param>
        /// <returns></returns>
        public Warehouse UpdateWareHouseStatus(string generate_id, int warehouse_status)
        {
            Warehouse warehouse = GetWarehouseByGenerateid(int.Parse(generate_id));
            warehouse.warehouse_status = warehouse_status;
            _warehouseDal.Update(warehouse);
            return warehouse;
        }

        public void RefreshData()
        {
            this._warehouseDal.RefreshData();
        }

        

        #region 默认仓设置
        public void UpdateDayDefaultWarehouse(List<DayDefaultWarehouse> dayDefaultWarehouseList, DateTime selectDate, int inoutType)
        {
            try
            {
                IEnumerable<DayDefaultWarehouse> oldDefaultWarehouse = _dayDefaultWarehouseDal.Find(rm => rm.default_day == selectDate && rm.inout_type == inoutType).Entities;

                if (oldDefaultWarehouse != null)
                {
                    foreach (DayDefaultWarehouse dayDefaultWarehouse in dayDefaultWarehouseList)
                    {
                        //查找选择的仓库是否已经存在
                        if (!oldDefaultWarehouse.Any(d => d.default_warehouse == dayDefaultWarehouse.default_warehouse))
                        {
                            _unitOfWork.AddAction(dayDefaultWarehouse, DataActions.Add);
                        }
                    }
                }
                else
                {
                    foreach (DayDefaultWarehouse dayDefaultWarehouse in dayDefaultWarehouseList)
                    {
                        _unitOfWork.AddAction(dayDefaultWarehouse, DataActions.Add);
                    }
                }

                //查找之前选择的仓库是否已删除
                if (oldDefaultWarehouse != null)
                {
                    //原有单头所含明细不为空，则需要判断是否有删除项
                    foreach (DayDefaultWarehouse dayDefaultWarehouse in oldDefaultWarehouse.Where(x => !dayDefaultWarehouseList.Any(u => u.default_warehouse == x.default_warehouse && u.default_day == x.default_day)).ToList())
                    {
                        _unitOfWork.AddAction(dayDefaultWarehouse, DataActions.Delete);
                    }

                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("配置选择日默认入库仓失败！", ex);
            }
        }

        public IEnumerable<DayDefaultWarehouse> FindDayDefaultWarehouse(DateTime selectDate, int inoutType)
        {
            return _dayDefaultWarehouseDal.Find(d=>d.default_day == selectDate && d.inout_type == inoutType).Entities;
        }

        public IEnumerable<DayDefaultWarehouse> FindDayDefaultWarehouse(int inoutType)
        {
            return _dayDefaultWarehouseDal.Find(d => d.inout_type == inoutType).Entities;
        }
        #endregion
    }
}
