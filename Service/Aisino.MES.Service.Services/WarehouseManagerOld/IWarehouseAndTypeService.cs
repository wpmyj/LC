using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.WarehouseManager
{
    public interface IWarehouseAndTypeService
    {
        #region 仓库类别
        //仓库类别的增删改查
        IList<WarehouseType> SelectAllWarehType();

        WarehouseType AddWarehType(WarehouseType newWarehType);
        WarehouseType UpdateWarehType(WarehouseType updWarehType);
        //void DelWarehType(WarehouseType delWarehType);
        //void DelFullyWarehType(WarehouseType delWarehType);

        bool CheckWarehTypeCodeExist(int WarehTypeCode);
        bool CheckWarehTypeNameExist(string WarehTypeName);

        #endregion

        #region 仓库
        //仓库的增删改查
        IEnumerable<Warehouse> SelectAllWarehouse();
        IEnumerable<Warehouse> SelectTypeWarehouse(int TypeID);

        Warehouse GetWarehouse(string id);

        Warehouse AddWarehouse(Warehouse newWarehouse);
        Warehouse UpdateWarehouse(Warehouse udpWarehouse);
        //void DelWarehouse(Warehouse delWarehouse);
        //void DeleteWarehouseList(List<Warehouse> lstDelWarehouse);
        //void DeleteWarehTypeList(List<WarehouseType> lstDelWarehType);

        bool CheckWarehouseCodeExist(string WarehouseCode);
        bool CheckWarehouseNameExist(string WarehouseName);
        bool CheckWarehouseIdExist(string WarehouseId);

        Warehouse SetWarehouseDuty(Warehouse warehouse, List<OrganizationEmployee> dutyUsers);
        //bool CheckWarehouseUnitExist(int unit_id);

        #endregion

        #region 仓库位置方法
        /// <summary>
        /// 获取所有仓库位置
        /// </summary>
        /// <returns></returns>
        IEnumerable<WarehouseLocation> FindAllWarehouseLocation();

        /// <summary>
        /// 保存位置信息
        /// </summary>
        void SaveWarehouseLocations(List<WarehouseLocation> warehouseLocationList);
        #endregion
    }
}
