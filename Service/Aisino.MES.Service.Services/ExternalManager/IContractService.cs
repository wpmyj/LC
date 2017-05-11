using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ExternalManager
{
   public interface IContractService
    {
       IEnumerable<Contract> GetAllContract();

       /// <summary>
       /// 根据粮食品种，出入库类型和客户查找对应的合同
       /// </summary>
       /// <param name="goodsKind">粮食品种</param>
       /// <param name="customerId">客户信息</param>
       /// <param name="inoutType">出入库类型</param>
       /// <returns>符合条件的合同</returns>
       IEnumerable<Contract> FindContractByGoodsKindAndCustomerAndInoutType(GoodsKind goodsKind, int customerId,int inoutType);
       IEnumerable<Contract> FindContractByContractName(string ContractName);
    }
}
