using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Helpers;

namespace Aisino.MES.Service.WarehouseManager
{
    public interface IWarehouseTransactionService
    {
        #region  库存交易记录单头

        WarehouseTransactionHead AddWarehTranHead(WarehouseTransactionHead newWarehTranHead);

        WarehouseTransactionHead UpdateWarehTranHead(WarehouseTransactionHead updateWarehTranHead);

        IEnumerable<WarehouseTransactionHead> GetWarehTranHeadList();
        int GetWarehTranHeadCount(string strWhere);
        IEnumerable<WarehouseTransactionHead> GetPagedWareTranHeadBySql(PagingCriteria paging, string strWhere);
        void ModifyWarehTranHead(WarehouseTransactionHead warehTranHead, int billNoID);
        void ModifyWarehTranDetail(WarehouseTransactionDetail warehTranDetail);
        void ConfirmWarehTranHead(WarehouseTransactionHead warehTranHead);

        IList<SysBillNo> GetMotifyTypes(string MESSystemString);
        #endregion

        #region 库存交易记录明细

        WarehouseTransactionDetail AddWarehTranDetail(WarehouseTransactionDetail newWarehTranDetail);

        WarehouseTransactionDetail UpdWarehTranDetail(WarehouseTransactionDetail updataWarehTranDetail);

        void DelWarehTranDetail(WarehouseTransactionDetail warehTranDetail);

        void DelWarehTranDetailList(List<WarehouseTransactionDetail> warehTranDetailList);

        #endregion
    }
}
