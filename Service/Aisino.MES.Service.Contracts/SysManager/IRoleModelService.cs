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
    /// 角色服务接口
    /// </summary>
    public interface IRoleModelService
    {

        /// <summary>
        /// 查找所有角色
        /// </summary>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<RoleDisplayModel> GetAllRoles();

        /// <summary>
        /// 根据编号查找角色编辑对象
        /// </summary>
        /// <param name="code">角色编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        RoleEditModel GetRoleByCode(string code);

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="newRole">需要添加的角色</param>
       [OperationContract, ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        RoleEditModel Add(RoleEditModel newRole);

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="newRole">需要更新的角色对象</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        RoleEditModel Update(RoleEditModel newRole, List<string> userCodeList, List<string> rightCodeList);

        /// <summary>
        /// 删除角色对象
        /// </summary>
        /// <param name="deleteRole">需要删除的角色对象</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Delete(RoleEditModel deleteRole);

        /// <summary>
        /// 根据编号删除角色
        /// </summary>
        /// <param name="code">需要删除角色的编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteByCode(string code);
    }//end IRoleModelService

}//end namespace SysManager