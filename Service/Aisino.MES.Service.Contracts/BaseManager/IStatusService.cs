using LC.Model;
using LC.Model.Business.BaseModel;
using LC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Contracts.BaseManager
{
    [ServiceContract(Namespace = "LC.Service.BaseManager")]
    public interface IStatusService
    {
        /// <summary>
        /// 根据类型查询对应的状态
        /// </summary>
        /// <param name="centerId"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<StatusModel> FindStatusByCat(string cat);
    }
}
