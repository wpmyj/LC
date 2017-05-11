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
using System.Threading.Tasks;

namespace LC.Service.Services.SysManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    /// <summary>
    /// 角色服务
    /// </summary>
    public class RoleModelService : IRoleModelService
    {

        private UnitOfWork _unitOfWork;

        public RoleModelService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="newRole">需要添加的角色</param>
        public RoleEditModel Add(RoleEditModel newRole)
        {
            try
            {
                if (CheckCodeExists(newRole.RoleCode))
                {
                    throw new FaultException<LCFault>(new LCFault("角色添加失败"), "该角色编号已存在，不能重复添加");
                }
                if (CheckNameExists(newRole.Name))
                {
                    throw new FaultException<LCFault>(new LCFault("角色添加失败"), "该角色名称已存在，不能重复添加");
                }

                SysRole sysRole = new SysRole();
                sysRole.Name = newRole.Name;
                sysRole.Remark = newRole.Remark;
                sysRole.RoleCode = newRole.RoleCode;
                sysRole.Stopped = newRole.Stopped;

                //添加用户
                if(newRole.SysUsers != null && newRole.SysUsers.Count > 0)
                {
                    List<SysUser> sysUserList = new List<SysUser>();
                    Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
                    foreach (UserEditModel sysUserTemp in newRole.SysUsers)
                    {
                        SysUser sysUser = sysUserDal.GetObjectByKey(sysUserTemp.UserCode).Entity;
                        sysRole.SysUsers.Add(sysUser);
                    }
                }

                //添加权限
                if (newRole.SysRights != null && newRole.SysRights.Count > 0)
                {
                    List<SysRight> sysRightList = new List<SysRight>();
                    Repository<SysRight> sysRightDal = _unitOfWork.GetRepository<SysRight>();
                    foreach (RightEditModel sysRightTemp in newRole.SysRights)
                    {
                        SysRight sysRight = sysRightDal.GetObjectByKey(sysRightTemp.RightCode).Entity;
                        sysRole.SysRights.Add(sysRight);
                    }
                }

                //保存
                _unitOfWork.AddAction(sysRole, DataActions.Add);
                _unitOfWork.Save();

                return newRole;
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
        /// 删除角色对象
        /// </summary>
        /// <param name="deleteRole">需要删除的角色对象</param>
        public bool Delete(RoleEditModel deleteRole)
        {
            try
            {
                return DeleteByCode(deleteRole.RoleCode);
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
        /// 根据编号删除角色
        /// </summary>
        /// <param name="code">需要删除角色的编号</param>
        public bool DeleteByCode(string rolecode)
        {
            bool res = true;
            try
            {
                Repository<SysRole> sysRoleDal = _unitOfWork.GetRepository<SysRole>();
                SysRole sysRole = sysRoleDal.GetObjectByKey(rolecode).Entity;
                if (sysRole != null)
                {
                    sysRole.SysUsers.Clear();
                    sysRole.SysRights.Clear();
                    _unitOfWork.AddAction(sysRole, DataActions.Delete);
                    _unitOfWork.Save();
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("角色删除失败"), "该角色不存在，无法删除");
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
        /// 查找所有角色
        /// </summary>
        public IList<RoleDisplayModel> GetAllRoles()
        {
            try
            {
                Repository<SysRole> sysRoleDal = _unitOfWork.GetRepository<SysRole>();
                IEnumerable<SysRole> sysRoles = sysRoleDal.GetAll().Entities;
                if (sysRoles!=null)
                {
                    return BuildModelList(sysRoles.ToList());
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

        private IList<RoleDisplayModel> BuildModelList(List<SysRole> sysRoles)
        {
            if (sysRoles == null)
            {
                return null;
            }
            else
            {
                List<RoleDisplayModel> roledisplays = new List<RoleDisplayModel>();
                foreach (SysRole sysrole in sysRoles)
                {
                    roledisplays.Add(BuildModel(sysrole));
                }
                return roledisplays;
            }
        }

        /// <summary>
        /// 根据编号查找角色编辑对象
        /// </summary>
        /// <param name="code">角色编号</param>
        public RoleEditModel GetRoleByCode(string rolecode)
        {
            try
            {
                RoleEditModel roleEditModel = new RoleEditModel();
                Repository<SysRole> sysRoleDal = _unitOfWork.GetRepository<SysRole>();
                SysRole sysRole = sysRoleDal.GetObjectByKey(rolecode).Entity;
                if (sysRole != null)
                {
                    roleEditModel.InitEditModel(sysRole);
                }
                return roleEditModel;
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
        /// 更新角色
        /// </summary>
        /// <param name="newRole">需要更新的角色对象</param>
        public RoleEditModel Update(RoleEditModel newRole, List<string> userCodeList, List<string> rightCodeList)
        {
            try
            {
                Repository<SysRole> sysRoleDal = _unitOfWork.GetRepository<SysRole>();
                SysRole sysRole = sysRoleDal.GetObjectByKey(newRole.RoleCode).Entity;
                if (sysRole != null)
                {
                    sysRole.Name = newRole.Name;
                    sysRole.Remark = newRole.Remark;
                    sysRole.Stopped = newRole.Stopped;
                }

                //修改用户
                sysRole.SysUsers.Clear();
                if (userCodeList != null && userCodeList.Count > 0)
                {
                    List<SysUser> sysUserListTemp = new List<SysUser>();
                    Repository<SysUser> sysUserDal = _unitOfWork.GetRepository<SysUser>();
                    foreach (string userCodeTemp in userCodeList)
                    {
                        SysUser sysUser = sysUserDal.GetObjectByKey(userCodeTemp).Entity;
                        sysUserListTemp.Add(sysUser);
                    }

                    sysRole.SysUsers = sysUserListTemp;
                }

                //修改权限
                sysRole.SysRights.Clear();
                if (rightCodeList != null && rightCodeList.Count > 0)
                {
                    List<SysRight> sysRightListTemp = new List<SysRight>();
                    Repository<SysRight> sysRightDal = _unitOfWork.GetRepository<SysRight>();
                    foreach (string rightCodeTemp in rightCodeList)
                    {
                        SysRight sysRight = sysRightDal.GetObjectByKey(rightCodeTemp).Entity;
                        sysRightListTemp.Add(sysRight);
                    }

                    sysRole.SysRights = sysRightListTemp;
                }

                _unitOfWork.AddAction(sysRole, DataActions.Update);
                _unitOfWork.Save();

                return newRole;
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
        /// 转换角色实体对象成显示对象
        /// </summary>
        /// <param name="sysRole">需要转换的角色</param>
        private RoleDisplayModel BuildModel(SysRole sysRole)
        {
            if (sysRole == null)
            {
                return null;
            }
            else
            {
                RoleDisplayModel roledisplay = new RoleDisplayModel();
                roledisplay.Code = sysRole.RoleCode;
                roledisplay.Name = sysRole.Name;
                roledisplay.Remark = sysRole.Remark;
                roledisplay.Stopped = (bool)sysRole.Stopped;

                return roledisplay;
            }
        }

        /// <summary>
        /// 转换多个角色实体对象
        /// </summary>
        /// <param name="sysRoles">需要转换的角色列表</param>
        private List<RoleDisplayModel> BuildModels(List<SysRole> sysRoles)
        {
            if (sysRoles == null || sysRoles.Count == 0)
            {
                return null;
            }
            List<RoleDisplayModel> roleDisplayModelList = new List<RoleDisplayModel>();
            foreach (SysRole sysRoleTemp in sysRoles)
            {
                RoleDisplayModel roleDisplayModel = new RoleDisplayModel();
                roleDisplayModel.Code = sysRoleTemp.RoleCode;
                roleDisplayModel.Name = sysRoleTemp.Name;
                roleDisplayModel.Stopped = sysRoleTemp.Stopped.Value;
                roleDisplayModel.Remark = sysRoleTemp.Remark;
            }
            return null;
        }

        /// <summary>
        /// 判断角色编号是否存在
        /// </summary>
        /// <param name="code">角色编号</param>
        private bool CheckCodeExists(string rolecode)
        {
            try
            {
                Repository<SysRole> RoleDal = _unitOfWork.GetRepository<SysRole>();
                var sysRole = RoleDal.GetObjectByKey(rolecode);
                if (sysRole.HasValue)
                {
                    return true;
                }
                return false;
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
        /// 判断角色名称是否存在
        /// </summary>
        /// <param name="name">角色名称</param>
        private bool CheckNameExists(string name)
        {
            try
            {
                Repository<SysRole> sysRoleDal = _unitOfWork.GetRepository<SysRole>();
                var sysRole = sysRoleDal.Single(sr => sr.Name == name);
                if (sysRole.HasValue)
                {
                    return true;
                }
                return false;
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

    }//end RoleModelService

}//end namespace SysManager