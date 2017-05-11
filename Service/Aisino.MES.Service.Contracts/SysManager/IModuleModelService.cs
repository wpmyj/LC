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
    /// 模块管理服务接口
    /// </summary>
    public interface IModuleModelService
    {

        /// <summary>
        /// 获取所有模块信息
        /// </summary>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ModuleDisplayModel> GetAllModules();

        /// <summary>
        /// 根据编号获取模块信息
        /// </summary>
        /// <param name="code">模块信息编号</param>
        /// <returns>获取的模块信息实例</returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ModuleEditModel GetModuleByCode(string code);

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="newModule">需要添加的模块信息</param>
       [OperationContract, ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        ModuleEditModel Add(ModuleEditModel newModule);

        /// <summary>
        /// 更新模块信息
        /// </summary>
        /// <param name="newModule">需要更新的模块信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ModuleEditModel Update(ModuleEditModel newModule);

        /// <summary>
        /// 删除模块信息
        /// </summary>
        /// <param name="deleteModule">需要删除的模块信息</param>
       [OperationContract, ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        bool Delete(ModuleEditModel deleteModule);

        /// <summary>
        /// 根据编号删除模块信息
        /// </summary>
        /// <param name="code">需要删除的模块信息编号</param>
       [OperationContract, ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        bool DeleteByCode(string code);
    }//end IModuleModelService

}//end namespace SysManager