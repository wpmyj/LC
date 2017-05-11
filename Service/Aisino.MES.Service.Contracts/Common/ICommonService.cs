using LC.Model;
using LC.Model.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Service.Contracts.Common
{
    /// <summary>
	/// 系统通用服务
	/// </summary>
    [ServiceContract(Namespace = "LC.Service.Common")]
	public interface ICommonService  
    {
		/// <summary>
		/// 登录
		/// 输入登录名、密码，查找对应的账号；没有则返回空
		/// </summary>
		/// <param name="LoginName">登录名称</param>
		/// <param name="Password">密码</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContract(typeof(LCFault))]
		LoginModel Login(string LoginName, string Password);

        /// <summary>
        /// 获取服务器时间
        /// 同时借助该方法测试服务通断
        /// </summary>
        /// <returns>系统时间</returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContract(typeof(LCFault))]
        DateTime GetSystemDateTime();
	}//end ICommonService

}//end namespace Common
