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
    public interface ICenterService
    {
        /// <summary>
        /// 获得所有中心信息
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<CenterModel> GetAllCenter();

        /// <summary>
        /// 创建教学中心信息
        /// </summary>
        /// <param name="newUserModel">需要创建的教学中心信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        CenterModel Add(CenterModel newCenterModel);

        /// <summary>
        /// 更新教学中心信息
        /// </summary>
        /// <param name="newUserModel">需要更新的教学中心信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        CenterModel Update(CenterModel newCenterModel);

        /// <summary>
        /// 删除教学中心信息
        /// </summary>
        /// <param name="deleteUserModel">删除教学中心信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Delete(CenterModel deleteCenterModel);

        /// <summary>
        /// 根据教学中心编号删除教学中心信息
        /// </summary>
        /// <param name="userCode">教学中心编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteById(int id);
    }
}
