using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.WarehouseManager
{
    public interface IWarehouseAmountService
    {

        //#region 仓容

        ///// <summary>
        ///// 查询所有的仓库货物
        ///// </summary>
        ///// <returns>返回所有仓库货物列表</returns>
        //IEnumerable<WarehouseAmount> GetAllWarehouseAmount();
 
        ///// <summary>
        ///// 查询该id的仓库货物
        ///// </summary>
        ///// <param name="id">仓库货物ID</param>
        ///// <returns>返回该该id的仓库货物</returns>
        //WarehouseAmount GetWarehouseAmount(int id);


        //// 查询对应warehouse_id的仓库货物
        //WarehouseAmount GetWarehouseAmountByWarehouseId(int warehouse_id);


        //// 查询对应invmas_id,warehouse_id的仓库货物
        //WarehouseAmount GetWarehouseAmount(int invmas_id, int warehouse_id);

        //WarehouseAmount GetWarehouseAmount_NoTracking(int invmas_id, int warehouse_id);
   
        //// 查询对应invmas_id的仓库货物
        //IEnumerable<WarehouseAmount> GetWarehouseAmount_Out(int invmas_id);
   
        //IEnumerable<Warehouse> GetWarehouse_NoInvmas();

        //IEnumerable<WarehouseAmount> GetWarehouseAmount_In(int invmas_id);

        ////获得可用的入库容量
        //decimal GetAvailableCapacity_In(int warehouse_id);

        ////获得可用的出库容量
        //decimal GetAvailableCapacity_Out(int warehouse_id);


        ///// 
        ///// <param name="newWarehouseAmount"></param>
        //WarehouseAmount AddWarehouseAmount(WarehouseAmount newWarehouseAmount);
 


        ///// 
        ///// <param name="updateWarehouseAmount"></param>
        //WarehouseAmount UpdateWarehouseAmount(WarehouseAmount updateWarehouseAmount);

 
        ///// 
        ///// <param name="newWarehouseAmount"></param>
        //WarehouseAmount AddWarehouseAmount_Unitwork(WarehouseAmount newWarehouseAmount);
 

        ///// 
        ///// <param name="updateWarehouseAmount"></param>
        //WarehouseAmount UpdateWarehouseAmount_Unitwork(WarehouseAmount updateWarehouseAmount);

        ///// <summary>
        ///// 根据仓库ID查询仓容
        ///// </summary>
        ///// <param name="warehInvmasID">仓库ID</param>
        ///// <returns>仓容</returns>
        //IEnumerable<WarehouseAmount> GetAmountByWareid(int warehouse_id);

        //#endregion

    }
}
