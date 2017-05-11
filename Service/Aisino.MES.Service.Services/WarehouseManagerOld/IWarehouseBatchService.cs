using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.WarehouseManager
{
    public interface IWarehouseBatchService
    {
        #region 库存批次记录单头

        WarehouseBatchHead AddBatchHead(WarehouseBatchHead newWarehTranHead);


        WarehouseBatchHead UpdateBatchHead(WarehouseBatchHead updateWarehTranHead);
 

        IEnumerable<WarehouseBatchHead> GetBatchHeadList();


        #endregion

        #region 库存批次记录明细

        WarehouseBatchDetail AddBatchDetail(WarehouseBatchDetail newBatchDetail);


        WarehouseBatchDetail UpdateBatchDetail(WarehouseBatchDetail updateBatchDetail);
 

        void DelBatchDetail(WarehouseBatchDetail warehouseBatchDetail);


        #endregion


    }
}
