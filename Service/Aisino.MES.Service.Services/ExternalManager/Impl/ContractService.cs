using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ExternalManager.Impl
{
    public class ContractService : IContractService
    {
        private Repository<Contract> _contractDal;

        public ContractService(Repository<Contract> contractDal)
        {
            _contractDal = contractDal;
        }

        public IEnumerable<Contract> GetAllContract()
        {
            return _contractDal.GetAll().Entities;
        }

        public IEnumerable<Contract> FindContractByGoodsKindAndCustomerAndInoutType(GoodsKind goodsKind, int customerId, int inoutType)
        {
            //根据客户id，出入库类型，查找已经审批结束，状态为已经提交（未终止或结束）的合同清单
            List<int> goodsKindIds = new List<int>();
            do
            {
                goodsKindIds.Add(goodsKind.goods_kind_id);
                goodsKind = goodsKind.ParentGoodsKind;
            }while(goodsKind != null);
            return _contractDal.Find(c=>c.customer_id == customerId && goodsKindIds.Contains(c.grain_kind.Value) && c.contract_type == (int)inoutType && c.contract_status == (int)ContractStatus.审批通过 && (c.original_contract == null || c.original_contract == string.Empty)).Entities;
        }


        public IEnumerable<Contract> FindContractByContractName(string ContractName)
        {
            if (ContractName != string.Empty)
            {
                return _contractDal.Find(c => c.customer_name.Contains(ContractName) && c.contract_status.HasValue && c.contract_status.Value == (int)ContractStatus.审批通过,
                    new string[] { "Enrolments", "Enrolments.PlanTasks", "Enrolments.PlanTasks.PlanTaskBatches", "Enrolments.PlanTasks.PlanTaskBatches.BusinessDisposes" }).Entities;
            }
            else
            {
                return _contractDal.Find(c => c.contract_status.HasValue && c.contract_status.Value == (int)ContractStatus.审批通过,
                    new string[] { "Enrolments", "Enrolments.PlanTasks", "Enrolments.PlanTasks.PlanTaskBatches", "Enrolments.PlanTasks.PlanTaskBatches.BusinessDisposes" }).Entities;
            }
        }
    }
}
