
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
	/// 用户领域模型操作接口
	/// </summary>
	public interface IUserModelService  
    {

		/// <summary>
		/// 根据部门编号，查找所有该部门用户
		/// </summary>
		/// <param name="departmentCode">部门编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
		IList<UserDisplayModel> FindUsersByDepartmentCode(string departmentCode);

		/// <summary>
		/// 根据用户编号获取用户信息
		/// </summary>
		/// <param name="userCode">用户编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
		UserEditModel GetUserByCode(string userCode);

        /// <summary>
        /// 获得所有用户
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<UserDisplayModel> GetAllUser();
		/// <summary>
		/// 更新用户信息
		/// </summary>
		/// <param name="newUserModel">需要更新的用户信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
		UserEditModel Update(UserEditModel newUserModel);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userCode">用户编号</param>
        /// <param name="oldPassword">原始密码</param>
        /// <param name="newPassword">新密码</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        UserEditModel ChangePassword(string userCode,string oldPassword,string newPassword);

		/// <summary>
		/// 创建用户信息
		/// </summary>
		/// <param name="newUserModel">需要创建的用户信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
		UserEditModel Add(UserEditModel newUserModel);

		/// <summary>
		/// 删除用户信息
		/// </summary>
		/// <param name="deleteUserModel">删除用户信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
		bool Delete(UserEditModel deleteUserModel);

		/// <summary>
		/// 根据用户编号删除用户信息
		/// </summary>
		/// <param name="userCode">用户编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
		bool DeleteByCode(string userCode);

        /// <summary>
		/// 判断用户是否属于某一角色
		/// </summary>
		/// <param name="userCode">用户编号</param>
        /// <param name="roleCode">角色编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool CheckUserRole(string userCode, string roleCode);
	}//end IUserModelService

}//end namespace SysManager
