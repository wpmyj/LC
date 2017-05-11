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
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Service.Services.SysManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    /// <summary>
    /// 菜单方法实现类
    /// </summary>
    public class MenuModelService : IMenuModelService
    {
         private UnitOfWork _unitOfWork;

        public MenuModelService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region 接口实现
        /// <summary>
        /// 根据编号获取菜单编辑对象
        /// </summary>
        /// <param name="code">菜单编号</param>
        /// <returns>查询的菜单编辑实例</returns>
        public MenuEditModel GetMenuByCode(string code)
        {
            try
            {
                MenuEditModel menuEditModel = new MenuEditModel();
                Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                SysMenu sysMenu = sysMenuDal.GetObjectByKey(code).Entity;
                if (sysMenu != null)
                {
                    menuEditModel.InitEditModel(sysMenu);
                }
                return menuEditModel;
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
        /// 添加菜单
        /// </summary>
        /// <param name="newMenu">需要添加的菜单业务模型</param>
        /// <param name="isRoot">是否新增根菜单
        /// 默认为不是根菜单，则校验是否已经赋值根菜单；同时在该根菜单下的节点最右侧增加
        /// 如果是根菜单，则要判定系统是否已经存在根菜单</param>
        public MenuEditModel Add(MenuEditModel newMenu, bool isRoot)
        {
            try
            {
                if (CheckCodeExists(newMenu.MenuCode))
                {
                    throw new FaultException<LCFault>(new LCFault("菜单添加失败"), "该菜单编号已存在，不能重复添加");
                }
                if (CheckNameExists(newMenu.Name))
                {
                    throw new FaultException<LCFault>(new LCFault("菜单添加失败"), "该菜单名称已存在，不能重复添加");
                }
                int layer = this.GetMenuByCode(newMenu.ParentCode).Layer.Value;

                SysMenu sysMenu = new SysMenu();

                sysMenu.MenuCode = newMenu.MenuCode;
                sysMenu.Name = newMenu.Name;
                sysMenu.DisplayName = newMenu.DisplayName;
                sysMenu.Remark = newMenu.Remark;
                sysMenu.ShowIndex = newMenu.ShowIndex;
                sysMenu.ModuleCode = newMenu.ModuleCode;
                sysMenu.ParentCode = newMenu.ParentCode;
                sysMenu.FunctionCode = newMenu.FunctionCode;
                sysMenu.Type = newMenu.Type;
                sysMenu.Layer = layer + 1;

                _unitOfWork.AddAction(sysMenu, DataActions.Add);
                _unitOfWork.Save();
                return newMenu;
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
        /// 删除菜单实体
        /// </summary>
        /// <param name="deleteMenu">需要删除的菜单</param>
        public bool Delete(MenuEditModel deleteMenu)
        {
            bool res = true;
            try
            {
                if (deleteMenu.HasSubMenu())
                {
                    //存在子部门
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("菜单删除失败"), "该部门存在子部门，不能删除");
                }
                res = this.DeleteByCode(deleteMenu.MenuCode);
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
        /// 根据菜单编号删除菜单
        /// </summary>
        /// <param name="menuCode">菜单编号</param>
        public bool DeleteByCode(string menuCode)
        {
            bool res = true;
            try
            {
                MenuEditModel menuEditModel = new MenuEditModel();
                Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                SysMenu sysMenu = sysMenuDal.GetObjectByKey(menuCode).Entity;
                if (sysMenu != null)
                {
                    menuEditModel.InitEditModel(sysMenu);
                    if (menuEditModel.HasSubMenu())
                    {
                        //存在子菜单
                        res = false;
                        throw new FaultException<LCFault>(new LCFault("菜单删除失败"), "该菜单存在子菜单，不能删除");
                    }
                    if (sysMenu.SysRights!=null&&sysMenu.SysRights.Count>0)
                    {
                        sysMenu.SysRights.Clear();
                    }
                    _unitOfWork.AddAction(sysMenu, DataActions.Delete);
                    _unitOfWork.Save();
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("菜单删除失败"), "该菜单不存在，无法删除");
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
        /// 根据用户编号返回对应的菜单显示业务对象列表
        /// </summary>
        /// <param name="userCode">用户对象</param>
        public IList<MainMenuModel> FindMenusByUserCode(string userCode)
        {
            try
            {
                List<SysMenu> sysmenuList = this._unitOfWork.Context.Set<SysMenu>().SqlQuery("EXEC FindMenuByUserCode @usercode", new SqlParameter("usercode", userCode)).ToList();
                return BuildModels(sysmenuList);
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

        public IList<MainMenuModel> FindMenusByUserCodeAndSubSystemCode(string userCode,string subSystemCode)
        {
            try
            {
                SqlParameter[] sps = new SqlParameter[2];
                sps[0] = new SqlParameter("usercode", userCode);
                sps[1] = new SqlParameter("subsystemcode", subSystemCode);
                List<SysMenu> sysmenuList = this._unitOfWork.Context.Set<SysMenu>().SqlQuery("EXEC FindMenuByUserCodeAndSubSystemCode @usercode,@subsystemcode", sps).ToList();
                return BuildModels(sysmenuList);
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
        /// 获取根菜单
        /// </summary>
        public MenuDisplayModel GetRootMenu()
        {
            try
            {
                Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                var sysMenu = sysMenuDal.Single(sm => sm.ParentCode == null);
                if (sysMenu.HasValue)
                {
                    return BuildModel(sysMenu.Entity);
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
        /// 更新菜单
        /// </summary>
        /// <param name="newMenu">需要更新的菜单</param>
        public MenuEditModel Update(MenuEditModel newMenu)
        {
            try
            {
                Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                SysMenu sysMenu = sysMenuDal.GetObjectByKey(newMenu.MenuCode).Entity;
                if (sysMenu!= null)
                {
                    sysMenu.MenuCode = newMenu.MenuCode;
                    sysMenu.Name = newMenu.Name;
                    sysMenu.DisplayName = newMenu.DisplayName;
                    sysMenu.Remark = newMenu.Remark;
                    sysMenu.ShowIndex = newMenu.ShowIndex;
                    sysMenu.ModuleCode = newMenu.ModuleCode;
                    sysMenu.ParentCode = newMenu.ParentCode;
                    sysMenu.FunctionCode = newMenu.FunctionCode;
                    sysMenu.Type = newMenu.Type;
                    sysMenu.Layer = newMenu.Layer;
                    sysMenu.BigImage = newMenu.BigImage;
                    sysMenu.ControlType = newMenu.ControlType;
                    sysMenu.ImagePosition = newMenu.ImagePosition;
                    sysMenu.ShowImage = newMenu.ShowImage;
                    sysMenu.ShowText = newMenu.ShowText;
                    sysMenu.SmallImage = newMenu.SmallImage;
                }
                _unitOfWork.AddAction(sysMenu, DataActions.Update);
                _unitOfWork.Save();

                return newMenu;
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
        /// <summary>
        /// 转换实体对象成显示对象
        /// </summary>
        /// <param name="sysMenu">菜单实体对象</param>
        private MenuDisplayModel BuildModel(SysMenu sysMenu, bool needSub = true)
        {
            if (sysMenu == null)
            {
                return null;
            }
            else
            {
                MenuDisplayModel menuDisplayMode = new MenuDisplayModel();
                menuDisplayMode.Code = sysMenu.MenuCode;
                menuDisplayMode.Name = sysMenu.Name;
                menuDisplayMode.Remark = sysMenu.Remark;
                if (needSub)
                {
                    menuDisplayMode.SubMenus = GetSubMenus(sysMenu.MenuCode);
                }

                return menuDisplayMode;
            }
        }

        /// <summary>
        /// 转换多个实体对象成显示对象列表
        /// </summary>
        /// <param name="sysMenus">多个系统菜单实体</param>
        private List<MenuDisplayModel> BuildModels(List<SysMenu> sysMenus,bool needSub=true)
        {
            List<MenuDisplayModel> menuDisplayModelList = new List<MenuDisplayModel>();
            if (sysMenus != null && sysMenus.Count > 0)
            {
                foreach (SysMenu sysMenu in sysMenus)
                {
                    MenuDisplayModel menuDisplayModel = BuildModel(sysMenu);
                    if (menuDisplayModel != null)
                    {
                        menuDisplayModelList.Add(menuDisplayModel);
                    }
                }
                return menuDisplayModelList;
            }
            return null;
        }

        private List<MainMenuModel> BuildModels(List<SysMenu> sysMenus)
        {
            List<MainMenuModel> mainMenuModelList = new List<MainMenuModel>();
            if (sysMenus != null && sysMenus.Count > 0)
            {
                foreach (SysMenu sysMenu in sysMenus)
                {
                    MainMenuModel mainMenuModel = new MainMenuModel();
                    mainMenuModel.Code = sysMenu.MenuCode;
                    mainMenuModel.ParentCode = sysMenu.ParentCode;
                    mainMenuModel.Name = sysMenu.Name;
                    mainMenuModel.DisplayName = sysMenu.DisplayName;
                    mainMenuModel.Layer = sysMenu.Layer.HasValue ? sysMenu.Layer.Value : 0;
                    if (sysMenu.ParentFunction != null)
                    {
                        mainMenuModel.Assembly = sysMenu.ParentFunction.Assembly;
                        mainMenuModel.ClassName = sysMenu.ParentFunction.ClassName;
                    }
                    mainMenuModelList.Add(mainMenuModel);
                }
                return mainMenuModelList;
            }
            return null;
        }

        /// <summary>
        /// 判断菜单编号是否存在
        /// </summary>
        /// <param name="code">菜单编号</param>
        private bool CheckCodeExists(string code)
        {
            try
            {
                Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                SysMenu sysMenu = sysMenuDal.GetObjectByKey(code).Entity;
                if (sysMenu != null)
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
        /// 判断菜单名称是否存在
        /// </summary>
        /// <param name="name">菜单名称</param>
        private bool CheckNameExists(string name)
        {
            try
            {
                Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                var sysMenu = sysMenuDal.Single(sm => sm.Name == name);
                if (sysMenu.HasValue)
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
        /// 判断根菜单是否存在
        /// </summary>
        private bool CheckRootMenuExists()
        {
            try
            {
                Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                var sysMenu = sysMenuDal.Single(sm => sm.ParentCode == null);
                if (sysMenu.HasValue)
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

        private List<MenuDisplayModel> GetSubMenus(string menuCode)
        {
            try
            {
                if (menuCode == null || menuCode.Trim().Length == 0)
                {
                    return null;
                }
                Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                var sysMenuListTemp = sysMenuDal.Find(sm => sm.ParentCode == menuCode).Entities;
                List<MenuDisplayModel> menuDisplayModelList = new List<MenuDisplayModel>();
                if (sysMenuListTemp != null && sysMenuListTemp.Count() > 0)
                {
                    foreach (SysMenu sysMenu in sysMenuListTemp.ToList())
                    {
                        MenuDisplayModel menuDisplayModel = new MenuDisplayModel();
                        menuDisplayModel.Code = sysMenu.MenuCode;
                        menuDisplayModel.Name = sysMenu.Name;
                        menuDisplayModel.Remark = sysMenu.Remark;
                        menuDisplayModel.SubMenus = GetSubMenus(menuDisplayModel.Code);
                        menuDisplayModelList.Add(menuDisplayModel);
                    }
                }
                return menuDisplayModelList;
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

        public void Dispose()
        {
            this._unitOfWork.Dispose();
        }

    }//end MenuModelService

}//end namespace SysManager