using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.WarehouseManager
{
    public interface IWarehouseUnitAndUnitTypeService
    {
        #region “计量单位”部分
        //计量单位的增删改查、验证
        IEnumerable<WarehouseUnit> SelectAllWarehUnit();
        IEnumerable<WarehouseUnit> SelectWarehUnit(int unitTypeID);

        WarehouseUnit AddWarehUnit(WarehouseUnit addWarehUnit);
        WarehouseUnit UpdWarehUnit(WarehouseUnit updWarehUnit);
        void DelWarehUnit(WarehouseUnit delWarehUnit);
        void DeleteWarehUnitList(List<WarehouseUnit> lstDelWarehouseUnit);
        void FullyDelWarehUnit(WarehouseUnit delWarehUnit);

        bool CheckWarehUnitCodeExist(string warehUnitCode);
        bool CheckWarehUnitCNameExist(string warehUnitName);
        bool CheckWarehUnitBaseExist(int unitTypeID);
        bool CheckWarehUnitUsed(WarehouseUnit warehUnit);
        #endregion

        #region “计量单位类别”部分
        //计量单位类别的增删改查、验证
        IEnumerable<WarehouseUnitType> SelectAllWarehUnitType();

        WarehouseUnitType AddWarehUnitType(WarehouseUnitType addWarehUnitType);
        WarehouseUnitType UpdWarehUnitType(WarehouseUnitType updWarehUnitType);
        void DelWarehUnitType(WarehouseUnitType delWarehUnitType);
        void DeleteWarehUnitTypeList(List<WarehouseUnitType> lstDelWarehUnitType);
        void FullyDelUnitType(WarehouseUnitType delWarehUnitType);

        bool CheckWarehUnitTypeCodeExist(string warehUnitTypeCode);
        bool CheckWarehUnitTypeNameExist(string warehUnitTypeName);

        #endregion
    }
}
