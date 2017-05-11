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
    public interface ISubSystemModelService
    {
        /// <summary>
        /// 根据用户编号返回对应的子系统显示业务对象列表
        /// </summary>
        /// <param name="userCode">用户对象</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<SubSystemDisplayModel> FindSubSystemByUserCode(string userCode);

        /// <summary>
        /// 添加子系统
        /// </summary>
        /// <param name="newSubSystem">需要添加的子系统</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        SubSystemEditModel Add(SubSystemEditModel newSubSystem);

        /// <summary>
        /// 根据子系统编号删除子系统
        /// 全局系统设置子系统不可删除
        /// </summary>
        /// <param name="code">需要删除的子系统编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteByCode(string code);

        /// <summary>
        /// 根据编号获取子系统编辑对象
        /// </summary>
        /// <param name="code">需要查找的子系统编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        SubSystemEditModel GetSubSystemByCode(string code);

        /// <summary>
        /// 更新子系统
        /// </summary>
        /// <param name="newSubSystem">需要更新的子系统</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        SubSystemEditModel Update(SubSystemEditModel newSubSystem);

        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        SubSystemEditModel SetSubSystemMenu(SubSystemEditModel newSubSystem, List<string> menuCodes);

        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<SubSystemDisplayModel> GetAllSubMenu();
    }
}
