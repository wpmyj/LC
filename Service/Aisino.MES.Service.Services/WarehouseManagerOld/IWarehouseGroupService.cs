using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.WarehouseManager
{
    public interface IWarehouseGroupService
    {
        //仓库组的增删改查
        IList<WarehouseGroup> SelectAllWarehGroup();
        IList<WarehouseGroup> GetRootWarehGroup();

        WarehouseGroup AddWarehGroup(WarehouseGroup newWarehGroup);
        WarehouseGroup UpdateWarehGroup(WarehouseGroup updWarehGroup);
        void DeleteWarehGroup(WarehouseGroup delWarehGroup);
        void DeleteWarehGroupList(List<WarehouseGroup> lstDelWarehGroup);
        void DeleteFullyWarehGroup(WarehouseGroup delWarehGroup);

        //与总部数据同步
        void UpdateAsHQ(IEnumerable<Aisino.MES.DataCenter.Models.BusinessModel.WarehouseGroup> warehGroupHQ);

        bool CheckWarehGroupCodeExist(string WarehGroupCode);
        bool CheckWarehGroupNameExist(string WarehGroupName);
    }
}
