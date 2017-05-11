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
    /// 方法服务接口
    /// </summary>
    public interface IFunctionModelService
    {

        /// <summary>
        /// 根据模块编号查找所有包含的方法
        /// </summary>
        /// <param name="moduleCode">模块编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<FunctionDisplayModel> FindFunctionsByModuleCode(string moduleCode);

        /// <summary>
        /// 根据编号查找方法编辑对象
        /// </summary>
        /// <param name="code">方法编号</param>
       [OperationContract, ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        FunctionEditModel GetFunctionByCode(string code);

        /// <summary>
        /// 添加方法
        /// </summary>
        /// <param name="newFunction">需要添加的方法</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        FunctionEditModel Add(FunctionEditModel newFunction);

        /// <summary>
        /// 更新方法
        /// </summary>
        /// <param name="newFunction">需要更新的方法</param>
       [OperationContract, ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        FunctionEditModel Update(FunctionEditModel newFunction);

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="deleteFunction">需要删除的方法对象</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Delete(FunctionEditModel deleteFunction);

        /// <summary>
        /// 根据编号删除方法
        /// </summary>
        /// <param name="code">需要删除的方法编号</param>
       [OperationContract, ApplyDataContractResolver]
       [FaultContractAttribute(typeof(LCFault))]
        bool DeleteByCode(string code);
    }//end IFunctionModelService

}//end namespace SysManager