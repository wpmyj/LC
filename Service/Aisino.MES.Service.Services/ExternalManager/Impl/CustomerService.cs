using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Repository.Helpers;

namespace Aisino.MES.Service.ExternalManager.Impl
{
    public class CustomerService : ICustomerService
    {
        private Repository<CustomerProfile> _customerDal;

        public CustomerService(Repository<CustomerProfile> customerDal)
        {
            _customerDal = customerDal;
        }

        public IEnumerable<CustomerProfile> GetAllCustomer()
        {
            return _customerDal.GetAll().Entities;
        }

        public IEnumerable<CustomerProfile> FindCustomerByCondition(string txtname)
        {
            //var queryCustomerprofile = PredicateBuilder.True<CustomerProfile>();
            if (txtname != null)
            {
                return _customerDal.Find(a => a.name.Contains(txtname)).Entities;
            }
            else
            {
                return null;
            }
        }
    }
}
