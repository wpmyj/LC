using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business
{
    public class AisinoMesBaseCode : ModelBase
    {
        public override string ToString()
        {

            return "";
        }

        #region 系统管理
        //全局系统管理子系统，删除时需要判断非该子系统才能删除
        public const string QJXTGL = "QJXTGL";
        #endregion

        #region 仓储交易单据
        public const string WAREHOUSEINITIAL = "期初入库单";
        public const string WAREHOUSELOSS = "盘盈入库单";
        public const string WAREHOUSEINCOME = "盘亏出库单";
        public const string WAREHOUSEIN = "仓储入库单";
        public const string WAREHOUSEOUT = "仓储出库单";
        public const string WAREHOUSEMANUIN = "生产入库单";
        public const string WAREHOUSEMANUOUT = "生产出库单";
        public const string WAREHOUSERECEIEVE = "领用出库单";
        public const string WAREHOUSEPURIN = "采购入库单";
        public const string WAREHOUSESALEOUT = "销售出库单";
        public const string WAREHOUSEALLOCATE = "调拨单";
        public const string WAREHOUSEWASTAGE = "清仓损耗单";

        public const string WAREHOUSEINITIALPREFIX = "WII";
        public const string WAREHOUSELOSSPREFIX = "WCI";
        public const string WAREHOUSEINCOMEPREFIX = "WCO";
        public const string WAREHOUSEINPREFIX = "WTI";
        public const string WAREHOUSEOUTPREFIX = "WTO";
        public const string WAREHOUSEMANUINPREFIX = "WMI";
        public const string WAREHOUSEMANUOUTPREFIX = "WMO";
        public const string WAREHOUSERECEIEVEPREFIX = "WRO";
        public const string WAREHOUSEPURINPREFIX = "WPI";
        public const string WAREHOUSESALEOUTPREFIX = "WSO";
        public const string WAREHOUSEALLOCATEPREFIX = "WEX";
        public const string WAREHOUSEWASTAGEPREFIX = "WWO";
        #endregion

        #region 生产计划单据
        public const string MANUPLANTASK = "生产计划单";
        public const string VEHICLEINPLANTASK = "车运入库计划单";
        public const string VEHICLEOUTPLANTASK = "车运出库计划单";
        public const string SHIPINPLANTASK = "船运入库计划单";
        public const string SHIPOUTPLANTASK = "船运出库计划单";
        public const string EXCHANGEPLANTASK = "倒仓计划单";

        public const string MANUPLANTASKPREFIX = "MPN";
        public const string VEHICLEINPLANTASKPREFIX = "MVI";
        public const string VEHICLEOUTPLANTASKPREFIX = "MVO";
        public const string SHIPINPLANTASKPREFIX = "MSI";
        public const string SHIPOUTPLANTASKPREFIX = "MSO";
        public const string EXCHANGEPLANTASKPREFIX = "MEX";
        #endregion

        public static string[] warehouseTransBill = new string[] { WAREHOUSEINITIAL, WAREHOUSELOSS,WAREHOUSEINCOME,WAREHOUSEIN,
            WAREHOUSEOUT,WAREHOUSEMANUIN,WAREHOUSEMANUOUT,WAREHOUSERECEIEVE,WAREHOUSEPURIN,WAREHOUSESALEOUT,WAREHOUSEALLOCATE,WAREHOUSEWASTAGE};

        public static string[] warehouseTransBillPrefix = new string[] { WAREHOUSEINITIALPREFIX, WAREHOUSELOSSPREFIX,WAREHOUSEINCOMEPREFIX,WAREHOUSEINPREFIX,
            WAREHOUSEOUTPREFIX,WAREHOUSEMANUINPREFIX,WAREHOUSEMANUOUTPREFIX,WAREHOUSERECEIEVEPREFIX,WAREHOUSEPURINPREFIX,WAREHOUSESALEOUTPREFIX,WAREHOUSEALLOCATEPREFIX,WAREHOUSEWASTAGEPREFIX};

        public static string[] plantaskBill = new string[] { MANUPLANTASK, VEHICLEINPLANTASK,VEHICLEOUTPLANTASK,SHIPINPLANTASK,
            SHIPOUTPLANTASK,EXCHANGEPLANTASK};

        public static string[] plantaskBillPrefix = new string[] { MANUPLANTASKPREFIX, VEHICLEINPLANTASKPREFIX,VEHICLEOUTPLANTASKPREFIX,SHIPINPLANTASKPREFIX,
            SHIPOUTPLANTASKPREFIX,EXCHANGEPLANTASKPREFIX};
    }
}
