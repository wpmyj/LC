using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;

namespace Aisino.MES.Service.ExternalManager
{
   public interface ICustomerService
    {
       IEnumerable<CustomerProfile> GetAllCustomer();
       IEnumerable<CustomerProfile> FindCustomerByCondition(string txtname);
    }

    
}
