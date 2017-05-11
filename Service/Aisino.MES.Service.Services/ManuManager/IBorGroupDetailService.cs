using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IBorGroupDetailService
    {
        IEnumerable<BorGroupDetail> SelectAllBorGroupDetail();
        IEnumerable<BorGroupDetail> GetBorGroupDetailIEnGroup(int borGroup_ID);
        IEnumerable<BorGroupDetail> GetBorGroupDetailIEnInvmas(int invmas_ID);
        BorGroupDetail GetBorGroupDetail(int borGroup_ID, int invmas_ID);
        void UpdateBorGroupDetail(int borGroupId, List<BorGroupDetail> lstBorGroupDetail);
    }
}
