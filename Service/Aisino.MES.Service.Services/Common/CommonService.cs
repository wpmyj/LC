using LC.DAL.Enums;
using LC.DAL.Interfaces;
using LC.DAL.Repository.Repositories;
using LC.DAL.UnitOfWork;
using LC.Model;
using LC.Model.Business;
using LC.Service.Contracts.Common;
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LC.Service.Services.Common
{
    /// <summary>
	/// 系统通用服务
	/// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
	public class CommonService : ICommonService 
    {
        private UnitOfWork _unitOfWork;

        public CommonService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region 实现接口
        /// <summary>
		/// 登录
		/// 输入登录名、密码，查找对应的账号；没有则返回空
		/// </summary>
        /// <param name="loginName">登录名称</param>
        /// <param name="password">密码</param>
		public LoginModel Login(string loginName, string password)
        {
            try
            {
                LoginModel loginModel = new LoginModel();
                Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
                SysUser sysUser = sysUserDal.Single(us => us.LoginName == loginName).Entity;
                if (sysUser != null)
                {
                    if (sysUser.Stopped)
                    {
                        throw new FaultException<LCFault>(new LCFault("登录失败"), "该用户已停用");
                    }
                    if (sysUser.Password != password)
                    {
                        throw new FaultException<LCFault>(new LCFault("登录失败"), "密码错误");
                    }
                    loginModel = BuildModel(sysUser);
                    return loginModel;
                }
                else
                {
                    throw new FaultException<LCFault>(new LCFault("登录失败"), "该登录名不存在");
                }
            }
            catch (RepositoryException rex)
            {
                string msg = rex.Message;
                string reason = rex.StackTrace;
                throw new FaultException<LCFault>
                (new LCFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<LCFault>
                (new LCFault(msg), reason);
            }
        }

        /// <summary>
        /// 获取服务器时间
        /// 同时借助该方法测试服务通断
        /// </summary>
        /// <returns>系统时间</returns>
        public DateTime GetSystemDateTime()
        {
            try
            {
                var systemDateTime = this._unitOfWork.Context.Database.SqlQuery<DateTime>("EXEC GetSystemTime");
                return systemDateTime.First();
            }
            catch (RepositoryException rex)
            {
                string msg = rex.Message;
                string reason = rex.StackTrace;
                throw new FaultException<LCFault>
                (new LCFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<LCFault>
                (new LCFault(msg), reason);
            }
        }

        #endregion

        #region 私有方法
        private LoginModel BuildModel(SysUser sysUser)
        {
            if (sysUser == null)
            {
                return null;
            }
            else
            {
                LoginModel LoginModel = new LoginModel();
                LoginModel.LoginName = sysUser.LoginName;
                LoginModel.NeedChangePassword = sysUser.NeedChangePassword;
                LoginModel.Password = sysUser.Password;
                LoginModel.UserCode = sysUser.UserCode;
                LoginModel.UserName = sysUser.Name;
                LoginModel.LoginTime = DateTime.Now;
                LoginModel.IPAddress = this.GetClientIp();

                return LoginModel;
            }
        }

        private string GetClientIp()
        {
            try
            {
                OperationContext context = OperationContext.Current;
                MessageProperties properties = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                return endpoint.Address;
            }
            catch (Exception)
            {
                return "...";
            }
        }         
        #endregion

    }//end CommonService

}//end namespace Common
