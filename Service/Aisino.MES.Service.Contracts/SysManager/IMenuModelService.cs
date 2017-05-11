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
    /// 菜单业务对象服务
    /// </summary>
    public interface IMenuModelService
    {

        /// <summary>
        /// 获取根菜单
        /// </summary>
        [OperationContract,ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        MenuDisplayModel GetRootMenu();

        /// <summary>
        /// 根据编号获取菜单编辑对象
        /// </summary>
        /// <param name="code">菜单编号</param>
        /// <returns>查询的菜单编辑实例</returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        MenuEditModel GetMenuByCode(string code);

        /// <summary>
        /// 根据用户编号返回对应的菜单显示业务对象列表
        /// </summary>
        /// <param name="userCode">用户对象</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<MainMenuModel> FindMenusByUserCode(string userCode);

        /// <summary>
        /// 根据用户编号返回对应的菜单显示业务对象列表
        /// </summary>
        /// <param name="userCode">用户对象</param>
        /// <param name="subSystemCode">子系统编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<MainMenuModel> FindMenusByUserCodeAndSubSystemCode(string userCode,string subSystemCode);

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="newMenu">需要添加的菜单业务模型</param>
        /// <param name="isRoot">是否新增根菜单
        /// 默认为不是根菜单，则校验是否已经赋值根菜单；同时在该根菜单下的节点最右侧增加
        /// 如果是根菜单，则要判定系统是否已经存在根菜单</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        MenuEditModel Add(MenuEditModel newMenu, bool isRoot = false);

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="newMenu">需要更新的菜单</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        MenuEditModel Update(MenuEditModel newMenu);

        /// <summary>
        /// 删除菜单实体
        /// </summary>
        /// <param name="deleteMenu">需要删除的菜单</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Delete(MenuEditModel deleteMenu);

        /// <summary>
        /// 根据菜单编号删除菜单
        /// </summary>
        /// <param name="menuCode">菜单编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteByCode(string menuCode);
    }//end IMenuModelService

}//end namespace SysManager