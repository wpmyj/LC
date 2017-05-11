using LC.DAL.Enums;
using LC.DAL.Interfaces;
using LC.DAL.Repository.Repositories;
using LC.DAL.UnitOfWork;
using LC.Model;
using LC.Model.Business.SysManager;
using LC.Service.Contracts.SysManager;
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LC.Service.Services.SysManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    /// <summary>
    /// 用户领域模型实现类
    /// </summary>
    public class UserModelService :IUserModelService, IDisposable
    {
        //private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(SysDepartmentUserService));
        private UnitOfWork _unitOfWork;

        public UserModelService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckUserRole(string userCode, string roleCode)
        {
            Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
            var sysUser = sysUserDal.GetObjectByKey(userCode);
            bool res = false;
            if (sysUser.HasValue)
            {
                foreach(SysRole sr in sysUser.Entity.SysRoles)
                {
                    if(sr.RoleCode == roleCode)
                    {
                        res = true;
                        break;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 根据部门编号，查找所有该部门用户
        /// </summary>
        /// <param name="departmentCode">部门编号</param>
        public IList<UserDisplayModel> FindUsersByDepartmentCode(string departmentCode)
        {
            try
            {
                UserEditModel userEditModel = new UserEditModel();
                Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
                IEnumerable<SysUser> sysUsers = null;
                if (sysUsers != null)
                {
                    return BuildModelList(sysUsers.ToList());
                }
                return null;
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

        public IList<UserDisplayModel> GetAllUser()
        {
            try
            {
                Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
                IEnumerable<SysUser> sysUsers = sysUserDal.GetAll().Entities;
                if (sysUsers!=null)
                {
                    return BuildModelList(sysUsers.ToList());
                }
                return null;
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
        /// 根据用户编号获取用户编辑领域对象
        /// </summary>
        /// <param name="userCode">用户编号</param>
        public UserEditModel GetUserByCode(string userCode)
        {
            try
            {
                UserEditModel userEditModel = new UserEditModel();
                Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
                SysUser sysUser = sysUserDal.GetObjectByKey(userCode).Entity;
                if (sysUser != null)
                {
                    userEditModel.InitEditModel(sysUser);
                }
                return userEditModel;
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
        /// 更新用户对象
        /// </summary>
        /// <param name="newUserModel">需要更新的用户领域对象</param>
        public UserEditModel Update(UserEditModel newUserModel)
        {
            try
            {
                Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
                SysUser sysUser = sysUserDal.GetObjectByKey(newUserModel.UserCode).Entity;
                if (sysUser != null)
                {
                    sysUser.Birthday = newUserModel.Birthday;
                    sysUser.Email = newUserModel.Email;
                    sysUser.IsLeader = newUserModel.IsLeader;
                    sysUser.LoginName = newUserModel.LoginName;
                    sysUser.Mobile = newUserModel.Mobile;
                    sysUser.Name = newUserModel.Name;
                    sysUser.NeedChangePassword = newUserModel.NeedChangePassword;
                    sysUser.OfficialPhone = newUserModel.OfficialPhone;
                    sysUser.Password = newUserModel.Password;
                    sysUser.Position = newUserModel.Position;
                    sysUser.Remark = newUserModel.Remark;
                    sysUser.Sex = newUserModel.Sex;
                    sysUser.Stopped = newUserModel.Stopped;
                }
                _unitOfWork.AddAction(sysUser, DataActions.Update);
                _unitOfWork.Save();

                return newUserModel;
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
        /// 修改密码
        /// </summary>
        /// <param name="userCode">用户编号</param>
        /// <param name="oldPassword">原始密码</param>
        /// <param name="newPassword">新密码</param>
        public UserEditModel ChangePassword(string userCode, string oldPassword, string newPassword)
        {
            try
            {
                UserEditModel userEditModel = new UserEditModel();
                Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
                SysUser sysUser = sysUserDal.GetObjectByKey(userCode).Entity;
                if (sysUser == null)
                {
                    throw new FaultException<LCFault>(new LCFault("密码修改失败"), "该用户不存在");
                }
                else
                {
                    if (sysUser.Password != oldPassword)
                    {
                        throw new FaultException<LCFault>(new LCFault("密码修改失败"), "原始密码错误");
                    }
                    else
                    {
                        sysUser.Password = newPassword;
                        _unitOfWork.AddAction(sysUser, DataActions.Update);
                        _unitOfWork.Save();
                        userEditModel.InitEditModel(sysUser);
                    }
                }

                return userEditModel;
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
        /// 删除用户信息
        /// </summary>
        /// <param name="deleteUserModel">删除用户信息</param>
        public bool Delete(UserEditModel deleteUserModel)
        {
            try
            {
                return DeleteByCode(deleteUserModel.UserCode);
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
        /// 根据用户编号删除用户信息
        /// </summary>
        /// <param name="userCode">用户编号</param>
        public bool DeleteByCode(string userCode)
        {
            bool res = true;
            try
            {
                Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
                SysUser sysUser = sysUserDal.GetObjectByKey(userCode).Entity;
                if (sysUser != null)
                {
                    _unitOfWork.AddAction(sysUser, DataActions.Delete);
                    _unitOfWork.Save();
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("用户删除失败"), "该用户不存在，无法删除");
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
            return res;
        }

        /// <summary>
        /// 创建用户信息
        /// </summary>
        /// <param name="newUserModel">需要创建的用户信息</param>
        public UserEditModel Add(UserEditModel newUserModel)
        {
            try
            {
                if (CheckCodeExists(newUserModel.UserCode))
                {
                    throw new FaultException<LCFault>(new LCFault("用户添加失败"), "该用户编号已存在，不能重复添加");
                }
                if (CheckLoginNameExists(newUserModel.LoginName))
                {
                    throw new FaultException<LCFault>(new LCFault("用户添加失败"), "该用户登录名已存在，不能重复添加");
                }

                SysUser sysUser = new SysUser();

                sysUser.Birthday = newUserModel.Birthday;
                sysUser.Email = newUserModel.Email;
                sysUser.IsOnline = false;
                sysUser.IsLeader = newUserModel.IsLeader;
                sysUser.LoginName = newUserModel.LoginName;
                sysUser.Mobile = newUserModel.Mobile;
                sysUser.Name = newUserModel.Name;
                sysUser.NeedChangePassword = newUserModel.NeedChangePassword;
                sysUser.OfficialPhone = newUserModel.OfficialPhone;
                sysUser.Password = newUserModel.Password;
                sysUser.Position = newUserModel.Position;
                sysUser.Remark = newUserModel.Remark;
                sysUser.Sex = newUserModel.Sex;
                sysUser.Stopped = newUserModel.Stopped;
                sysUser.UserCode = newUserModel.UserCode;

                _unitOfWork.AddAction(sysUser, DataActions.Add);
                _unitOfWork.Save();

                return newUserModel;
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
        /// 判断该用户编码是否已经使用
        /// </summary>
        /// <param name="userCode">用户编码</param>
        private bool CheckCodeExists(string userCode)
        {
            Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
            var sysUser = sysUserDal.GetObjectByKey(userCode);
            if (sysUser.HasValue)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断登录名是否已经存在
        /// </summary>
        /// <param name="loginName">登录名</param>
        private bool CheckLoginNameExists(string loginName)
        {
            Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
            var sysUser = sysUserDal.Single(su => su.LoginName == loginName);
            if (sysUser.HasValue)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将用户实体对象转换成用户显示对象
        /// </summary>
        /// <param name="sysUser">用户实体对象</param>
        private UserDisplayModel BuildModel(SysUser sysUser)
        {
            if (sysUser == null)
            {
                return null;
            }
            else
            {
                UserDisplayModel userDisplayMode = new UserDisplayModel();
                userDisplayMode.Code = sysUser.UserCode;
                userDisplayMode.IsLeader = sysUser.IsLeader;
                userDisplayMode.IsOnline = sysUser.IsOnline;
                userDisplayMode.Name = sysUser.Name;
                userDisplayMode.Phone = sysUser.Mobile ?? sysUser.OfficialPhone ?? "";
                userDisplayMode.Position = sysUser.Position;
                userDisplayMode.Remark = sysUser.Remark;
                userDisplayMode.Sex = sysUser.Sex.ToString();
                userDisplayMode.Stopped = sysUser.Stopped;

                return userDisplayMode;
            }
        }

        /// <summary>
        /// 将用户实体列表转换成用户显示模型列表
        /// </summary>
        /// <param name="sysUsers">用户列表</param>
        private List<UserDisplayModel> BuildModelList(List<SysUser> sysUsers)
        {
            if (sysUsers == null)
            {
                return null;
            }
            else
            {
                List<UserDisplayModel> userDisplayModels = new List<UserDisplayModel>();
                foreach (SysUser sysUser in sysUsers)
                {
                    userDisplayModels.Add(BuildModel(sysUser));
                }
                return userDisplayModels;
            }
        }

        public void Dispose()
        {
            //log.Info(String.Format("Service dispose", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
            this._unitOfWork.Dispose();
        }

    }//end UserModelService

}//end namespace SysManager