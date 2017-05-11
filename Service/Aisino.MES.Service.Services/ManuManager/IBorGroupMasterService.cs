using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IBorGroupMasterService
    {
        IEnumerable<BorGroupMaster> SelectAllBorGroupMaster();
        BorGroupMaster GetBorGroupMaster(int id);
        BorGroupMaster AddBorGroupMaster(BorGroupMaster newBorGroupMaster);
        BorGroupMaster UpdateBorGroupMaster(BorGroupMaster updBorGroupMaster);
        BorGroupMaster DeleteBorGroupMaster(BorGroupMaster delBorGroupMaster);
        void DeleteBorGroupMasterList(List<BorGroupMaster> lstDelBorGroupMaster);
        bool CheckBorGroupMasterCodeExist(string borGroupMasterCode);
        bool CheckBorGroupMasterNameExist(string borGroupMasterName);
    }
}
