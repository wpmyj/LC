using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;

namespace Aisino.MES.Service.WarehouseManager
{
    public interface IWarehouseBaseService
    {
        /// <summary>
        /// 查询所有仓房
        /// </summary>
        /// <returns>返回所有仓房列表</returns>
        IEnumerable<Warehouse> GetAllWarehouse();

        /// <summary>
        /// 根据Id获得仓房实例
        /// </summary>
        /// <param name="warehouse_id"></param>
        /// <returns>返回仓房的实例</returns>
        Warehouse GetWarehouseById(string warehouse_id);

        /// <summary>
        /// 根据Id(int 自增)获取仓房实例
        /// </summary>
        /// <param name="generate_id"></param>
        /// <returns>返回仓房实例</returns>
        Warehouse GetWarehouseByGenerateid(int generate_id);

        /// <summary>
        /// 查询所有有效仓房信息
        /// </summary>
        /// <returns>返回所有有效仓房列表</returns>
        IEnumerable<Warehouse> GetAllEnabledWarehouse();

        /// <summary>
        /// 根据仓库保管员查询所有有效仓房信息
        /// </summary>
        /// <param name="iKeeper"></param>
        /// <returns></returns>
        IEnumerable<Warehouse> GetWarehouseByKeeper(int iKeeper);

        /// <summary>
        /// 根据仓库标识更新仓库状态
        /// </summary>
        /// <param name="generate_id">仓库标识(int型自增)</param>
        /// <param name="warehouse_status">仓库状态</param>
        /// <returns></returns>
        Warehouse UpdateWareHouseStatus(string generate_id, int warehouse_status);


        /// <summary>
        /// 刷新数据
        /// </summary>
        void RefreshData();

        #region 默认首选仓操作
        void UpdateDayDefaultWarehouse(List<DayDefaultWarehouse> dayDefaultWarehouseList,DateTime selectDate,int inoutType);

        IEnumerable<DayDefaultWarehouse> FindDayDefaultWarehouse(DateTime selectDate, int inoutType);

        IEnumerable<DayDefaultWarehouse> FindDayDefaultWarehouse(int inoutType);
        #endregion
    }
}
