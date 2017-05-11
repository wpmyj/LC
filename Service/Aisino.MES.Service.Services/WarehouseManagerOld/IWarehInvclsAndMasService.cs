using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.WarehouseManager
{
    public interface IWarehInvclsAndMasService
    {
        #region 货品大类 部分
        //货品大类的增删改查，以及检验
        WarehouseInvcls SelectRootWarehInvcls();
        WarehouseInvcls SelectOneWarehInvcls(int warehInvclsID);
        IEnumerable<WarehouseInvcls> SelectExistInvmasWarehInvcls();
        IEnumerable<WarehouseInvcls> SelectConsumerWarehInvcls();
        IEnumerable<WarehouseInvcls> SelectEnableConsumerWarehInvcls();
        WarehouseInvcls AddWarehInvcls(WarehouseInvcls newWarehInvcls);
        WarehouseInvcls UpdateWarehInvcls(WarehouseInvcls updWarehInvcls);
        void DeleteWarehInvcls(WarehouseInvcls delWarehInvcls);
        void DelFullyWarehInvcls(WarehouseInvcls delWarehInvcls);
        void DeleteWarehInvclsList(List<WarehouseInvcls> lstDelWarehInvcls);

        bool CheckWarehInvclsCodeExist(string warehInvclsCode);
        bool CheckWarehInvclsNameExist(string warehInvclsName);
        #endregion

        #region 货品主档 部分
        //货品主档的增删改查，以及检验
        IEnumerable<WarehouseInvmas> SelectAllWarehInvmas();
        WarehouseInvmas SelectOneWarehInvmas(int warehInvmasID);
        WarehouseInvmas AddWarehInvmas(WarehouseInvmas newWarehInvmas);
        WarehouseInvmas UpdateWarehInvmas(WarehouseInvmas updWarehInvmas);
        void DelWarehInvmas(WarehouseInvmas delWarehInvmas);
        void DeleteWarehInvmasList(List<WarehouseInvmas> lstDelInvmas);

        bool CheckWarehInvmasCodeExist(string warehInvmasCode);
        bool CheckWarehInvmasNameExist(string warehInvmasName);
        bool CheckWarehInvmasUnitUsed(int unit_id);
  
        // 根据仓库ID查询货品主档
        IEnumerable<WarehouseInvmas> GetInvmasByWareid(int warehouse_id);
     

        #endregion
    }
}
