using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;

namespace Aisino.MES.Service.ExternalManager.Impl
{
    class CertificateService : ICertificateService
    {
        private Repository<CustomerShipingCertificate> _certificateDal;

        public CertificateService(Repository<CustomerShipingCertificate> certificateDal)
        {
            _certificateDal = certificateDal;
        }

        public IEnumerable<CustomerShipingCertificate> GetAllCertificate()
        {
            return _certificateDal.GetAll().Entities;
        }

        public IEnumerable<CustomerShipingCertificate> FindCertificateByGoodsKindAndCustomer(GoodsKind goodsKind, int customerId, int inoutType)
        {
            List<int> goodsKindIds = new List<int>();
            do
            {
                goodsKindIds.Add(goodsKind.goods_kind_id);
                goodsKind = goodsKind.ParentGoodsKind;
            } while (goodsKind != null);
            return _certificateDal.Find(c => c.delivery_customer == customerId && goodsKindIds.Contains(c.goods_kind) && c.bill_type == (int)inoutType && c.bill_status == (int)Aisino.MES.Model.Models.CustomerShipingCertificate.CertificateStatus.审批通过).Entities;
        }
    }
}
