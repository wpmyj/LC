using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.SysManager.Impl
{
    public class SysRoleService : ISysRoleService
    {
        private Repository<SysRole> _sysRoleDal;
        private UnitOfWork _unitOfWork;

        public SysRoleService(Repository<SysRole> sysRoleDal, UnitOfWork unitOfWork)
        {
            _sysRoleDal = sysRoleDal;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 查询所有角色
        /// </summary>
        /// <returns>返回所有角色列表</returns>
        public IList<SysRole> SelectAllRoles()
        {
            return _sysRoleDal.GetAll().Entities.ToList();
        }
        
        /// <summary>
        /// 新增一个角色
        /// </summary>
        /// <param name="newSysRole">要添加的角色</param>
        /// <returns>新增后的角色</returns>
        public SysRole AddSysRole(SysRole newSysRole)
        {
            SysRole returnRole = null;
            try
            {
                _sysRoleDal.Add(newSysRole);
                returnRole = newSysRole;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnRole;
        }
            
        /// <summary>
        /// 更新一个角色
        /// </summary>
        /// <param name="upSysRole">要更新的角色</param>
        /// <returns>更新后的角色</returns>
        public SysRole UpdateSysRole(SysRole upSysRole)
        {
            SysRole returnRole = null;
            try
            {
                _sysRoleDal.Update(upSysRole);
                returnRole = upSysRole;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnRole;
        }

        /// <summary>
        /// 删除一个角色
        /// </summary>
        /// <param name="delSysRole">要删除的角色</param>
        public void DelSysRole(SysRole delSysRole)
        {            
            try
            {
                delSysRole.role_deleted = true;
                _unitOfWork.AddAction(delSysRole, DataActions.Update);
                ////删除所有该角色与权限的关系
                //IList<SysRoleRight> sysRoleRightList = _sysRoleRightDal.Find(s => s.role_id == delSysRole.id).Entities.ToList();
                //foreach (SysRoleRight srr in sysRoleRightList)
                //{
                //    _unitOfWork.AddAction(srr, DataActions.Delete);
                //}
                ////删除所有该角色与用户的关系
                //IList<SysRoleUser> sysRoleUserList = _sysRoleUserDal.Find(s => s.role_id == delSysRole.id).Entities.ToList();
                //foreach (SysRoleUser sru in sysRoleUserList)
                //{
                //    _unitOfWork.AddAction(sru, DataActions.Delete);
                //}
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量删除角色
        /// </summary>
        /// <param name="lstDelSysRole"></param>
        public void DeleteSysRoleList(List<SysRole> lstDelSysRole)
        {
            try
            {
                foreach (SysRole delSysRole in lstDelSysRole)
                {
                    if (delSysRole.role_deleted == true)
                    {
                        continue;
                    }
                    delSysRole.role_deleted = true;
                    _unitOfWork.AddAction(delSysRole, DataActions.Update);
                    //删除所有该角色与权限的关系
                    //IList<SysRoleRight> sysRoleRightList = _sysRoleRightDal.Find(s => s.role_id == delSysRole.id).Entities.ToList();
                    //foreach (SysRoleRight srr in sysRoleRightList)
                    //{
                    //    _unitOfWork.AddAction(srr, DataActions.Delete);
                    //}
                    ////删除所有该角色与用户的关系
                    //IList<SysRoleUser> sysRoleUserList = _sysRoleUserDal.Find(s => s.role_id == delSysRole.id).Entities.ToList();
                    //foreach (SysRoleUser sru in sysRoleUserList)
                    //{
                    //    _unitOfWork.AddAction(sru, DataActions.Delete);
                    //}
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除角色信息失败！", ex);
            }
        }

        /// <summary>
        /// 检测角色编号是否已存在
        /// </summary>
        /// <param name="roleCode">角色编号</param>
        /// <returns>存在返回true，不存在返回false</returns>
        public bool CheckRoleCodeExist(string roleCode)
        {
            var sysRole = _sysRoleDal.Single(role => role.role_code == roleCode);
            if (sysRole.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测角色名称是否已存在
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns>存在返回true，不存在返回false</returns>
        public bool CheckRoleNameExist(string roleName)
        {
            var sysRole = _sysRoleDal.Single(role => role.role_name == roleName);
            if (sysRole.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
