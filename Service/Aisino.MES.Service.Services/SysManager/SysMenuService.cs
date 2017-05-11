using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;
using System.Linq.Expressions;

namespace Aisino.MES.Service.SysManager.Impl
{
    public class SysMenuService : ISysMenuService
    {
        private Repository<SysMenu> _sysMenuDal;
        private UnitOfWork _unitOfWork;

        public SysMenuService(Repository<SysMenu> sysMenuDal,UnitOfWork unitOfWork)        {
            _sysMenuDal = sysMenuDal;
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// 获取根菜单
        /// </summary>
        /// <returns>根菜单实例</returns>
        public SysMenu GetRootSysMenu()
        {
            var rootSysMenu = _sysMenuDal.Single(s=>s.menu_parent_id == null);
            if (rootSysMenu.HasValue)
            {
                return rootSysMenu.Entity;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 根据id获取菜单
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <returns>菜单实例</returns>
        public SysMenu GetSysMenu(int id)
        {
            var rootSysMenu = _sysMenuDal.Single(s => s.id == id);
            if (rootSysMenu.HasValue)
            {
                return rootSysMenu.Entity;
            }
            else
            {                
                return null;
            }
        }

        /// <summary>
        /// 根据用户id获得所有菜单
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IEnumerable<SysMenu> GetSysMenuListByUserId(int userid)
        {
            string esql = string.Format("select m.* from SysMenu as m inner join " +
                          "(select distinct menu_id from SysRightMenu as rm inner join " +
                          "(select right_id from SysRoleRight as rr inner join " +
                          "SysRoleUser as ru on rr.role_id = ru.role_id and ru.acount_id = {0}" +
                          ") as r on rm.right_id = r.right_id) as srm on m.id = srm.menu_id", userid);

            return _sysMenuDal.QueryByESql(esql).Entities;

            //string strSysMenu = string.Empty;
            //foreach (SysRoleUser sysRoleUser in acount.SysRoleUsers)
            //{
            //    if (sysRoleUser.SysRole == null)
            //    {
            //        return;
            //    }
            //    SysRole sr = sysRoleUser.SysRole;

            //    foreach (SysRoleRight sysRoleRight in sr.SysRoleRights)
            //    {
            //        SysRight srr = sysRoleRight.SysRight;
            //        foreach (SysRightMenu sysRightMenu in srr.SysRightMenus)
            //        {
            //            if (strSysMenu.Trim().Length == 0)
            //            {
            //                strSysMenu = sysRightMenu.SysMenu.menu_code;
            //            }
            //            else
            //            {
            //                strSysMenu = strSysMenu + "," + sysRightMenu.SysMenu.menu_code;
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 根据sql获得所需要的菜单
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IEnumerable<SysMenu> QueryByESql(string strSql)
        {
            return _sysMenuDal.QueryByESql(strSql).Entities;
        }

        public IEnumerable<SysMenu> GetOpSysMenuListByUserIdAndParentMenuId(int userId, int parentId)
        {
            string esql = string.Format("select m.* from SysMenu as m inner join " +
                          "(select distinct menu_id from SysRightMenu as rm inner join " +
                          "(select right_id from SysRoleRight as rr inner join " +
                          "SysRoleUser as ru on rr.role_id = ru.role_id and ru.acount_id = {1}" +
                          ") as r on rm.right_id = r.right_id) as srm on m.id = srm.menu_id where m.menu_parent_id = {0}", parentId, userId);

            return _sysMenuDal.QueryByESql(esql).Entities;
        }

        /// <summary>
        /// 获取菜单总数
        /// </summary>
        /// <returns>菜单总数</returns>
        public int GetMenuCount()
        {
            return _sysMenuDal.GetCount();
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="menu">需要新增的菜单</param>
        /// <returns>新增后的菜单</returns>
        public SysMenu AddSysMenu(SysMenu newMenu)
        {
            try
            {
                _sysMenuDal.Add(newMenu);
                return newMenu;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="updateMenu">需要更新的菜单</param>
        /// <returns>更新后的菜单</returns>
        public SysMenu UpdateSysMenu(SysMenu updateMenu)
        {
            try
            {               
                _sysMenuDal.Update(updateMenu);
                return updateMenu;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="delMenu">需要删除的菜单</param>
        /// <returns>删除后的菜单</returns>
        public void DeleteSysMenu(SysMenu delMenu)
        {
            try
            {
                //删除子菜单
                DeleteSubMenus(delMenu);
                _unitOfWork.Save();

            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        private void DeleteSubMenus(SysMenu delMenu)
        {
            //有子菜单，则调用自身删除
            if (delMenu.SubMenus != null)
            {
                List<SysMenu> subMenuList = delMenu.SubMenus.ToList();
                foreach (SysMenu sm in subMenuList)
                {
                    DeleteSubMenus(sm);
                }
            }
            //删除自身
            _unitOfWork.AddAction(delMenu, DataActions.Delete);
        }

        public void DeleteSysMenuList(List<SysMenu> lstDelSysMenu)
        {
            try
            {
                foreach (SysMenu delMenu in lstDelSysMenu)
                {
                    //删除子菜单
                    DeleteSubMenus(delMenu);
                    //判断是否删除父菜单与权限关联
                    //删除菜单本身
                    _unitOfWork.AddAction(delMenu, DataActions.Delete);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除菜单信息失败！", ex);
            }
        }
        /// <summary>
        /// 删除父级菜单权限关联
        /// </summary>
        /// <param name="sysRight">权限</param>
        /// <param name="delMenu">菜单</param>
        private void DeleteRightParentMenu(SysRight sysRight, SysMenu delMenu)
        {
            //SysRightMenu sysRightMenu = new SysRightMenu();
            //bool canDeleteParent = true;
            //List<SysMenu> subMenuList = delMenu.ParentMenu.SubMenus.ToList();
            //foreach (SysMenu subMenu in subMenuList)
            //{
            //    if (subMenu.id == delMenu.id)
            //        continue;
            //    else
            //    {
            //        sysRightMenu = _sysRightMenuDal.Single(s => s.menu_id == subMenu.id && s.right_id == sysRight.id).Entity;
            //        if (sysRightMenu != null)
            //        {
            //            //存在该菜单父菜单的其他子菜单关联
            //            canDeleteParent = false;
            //            break;
            //        }
            //    }
            //}
            //if (canDeleteParent)
            //{
            //    //可以删除父菜单，则调用自身持续判断
            //    if (delMenu.ParentMenu.ParentMenu != null)
            //    {
            //        DeleteRightParentMenu(sysRight, delMenu.ParentMenu);
            //    }
            //    //退出后获得该关联项，加入删除
            //    sysRightMenu = _sysRightMenuDal.Single(s => s.menu_id == delMenu.ParentMenu.id && s.right_id == sysRight.id).Entity;
            //    _unitOfWork.AddAction(sysRightMenu, DataActions.Delete);
            //}
        }

        /// <summary>
        /// 检测菜单编号是否已存在
        /// </summary>
        /// <param name="menuCode">菜单编号</param>
        /// <returns>若存在，则返回true；若不存在，则返回false</returns>
        public bool CheckMenuCodeExist(string menuCode)
        {
            var sysMenu = _sysMenuDal.Single(s => s.menu_code == menuCode);
            if (sysMenu.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测菜单名称是否已存在
        /// </summary>
        /// <param name="menuCode">菜单名称</param>
        /// <returns>若存在，则返回true；若不存在，则返回false</returns>
        public bool CheckMenuNameExist(string menuName)
        {
            var sysMenu = _sysMenuDal.Single(s => s.menu_name == menuName);
            if (sysMenu.HasValue)
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
