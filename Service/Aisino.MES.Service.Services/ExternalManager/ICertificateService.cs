using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ExternalManager
{
   public interface ICertificateService
    {
       IEnumerable<CustomerShipingCertificate> GetAllCertificate();

       /// <summary>
       /// 根据粮食品种和客户查找对应的凭证
       /// </summary>
       /// <param name="goodsKind">粮食品种</param>
       /// <param name="customerId">客户信息</param>
       /// <param name="inoutType">出入库类型</param>
       /// <returns>符合条件的凭证</returns>
       IEnumerable<CustomerShipingCertificate> FindCertificateByGoodsKindAndCustomer(GoodsKind goodsKind, int customerId, int inoutType);
    }
}
