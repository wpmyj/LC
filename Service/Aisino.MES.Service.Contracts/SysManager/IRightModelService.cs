using LC.Model;
using LC.Model.Business.SysManager;
using LC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Service.Contracts.SysManager
{
    [ServiceContract(Namespace = "LC.Service.SysManager")]
    /// <summary>
    /// 权限服务接口
    /// </summary>
    public interface IRightModelService
    {

        /// <summary>
        /// 查找所有权限信息
        /// </summary>
       [OperationContract,ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        IList<RightDisplayModel> GetAllRights();

        /// <summary>
        /// 根据权限编号获取编辑对象
        /// </summary>
        /// <param name="code">权限编号</param>
       [OperationContract,ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        RightEditModel GetRightByCode(string code);

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="newRight">需要添加的权限信息</param>
       [OperationContract,ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        RightEditModel Add(RightEditModel newRight);

        /// <summary>
        /// 更新权限信息
        /// </summary>
        /// <param name="newRight">需要更新的权限信息</param>
       [OperationContract,ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        RightEditModel Update(RightEditModel newRight);


        /// <summary>
        /// 更新权限信息
        /// </summary>
        /// <param name="newRole"></param>
        /// <param name="userCodeList"></param>
        /// <param name="rightCodeList"></param>
        /// <returns></returns>
       [OperationContract, ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
       RightEditModel SetRightMenus(RightEditModel rightEditModel, List<string> menuCodes);
        /// <summary>
        /// 删除权限信息
        /// </summary>
        /// <param name="deleteRight">需要删除的权限信息</param>
       [OperationContract,ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        bool Delete(RightEditModel deleteRight);

        /// <summary>
        /// 根据编号删除对应权限
        /// </summary>
        /// <param name="code">需要删除的权限编码</param>
        [OperationContract,ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteByCode(string code);
    }//end IRightModelService

}//end namespace SysManager