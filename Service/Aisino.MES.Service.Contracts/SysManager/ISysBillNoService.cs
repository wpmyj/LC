using Aisino.MES.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aisino.MES.Service.SysManager
{
    public interface ISysBillNoService
    {
        string GetBillNoTemp(int billNoId);

        string GetBillNo(int billNoId);

        string GetBillNo(int billNoId, string OriginalDept);

        //根据系统system 和前缀 prefix 取得单据编号ID
        int GetBillNoID(string billNoSystem, string billNoPrefix);

        // 根据system获取单据类别列表
        IEnumerable<SysBillNo> SelectAllBillNo(string system);

        IList<SysBillNo> GetMotifyTypes(string MESSystemString);

        void RefreshData();
    }
}
