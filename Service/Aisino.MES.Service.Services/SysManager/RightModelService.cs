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
    /// 权限服务
    /// </summary>
    public class RightModelService : IRightModelService
    {
        private UnitOfWork _unitOfWork;

        public RightModelService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region 接口实现

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="newRight">需要添加的权限信息</param>
        public RightEditModel Add(RightEditModel newRight)
        {
            try
            {
                if (CheckCodeExists(newRight.RightCode))
                {
                    throw new FaultException<LCFault>(new LCFault("权限添加失败"), "该权限编号已存在，不能重复添加");
                }
                if (CheckNameExists(newRight.Name))
                {
                    throw new FaultException<LCFault>(new LCFault("权限添加失败"), "该权限名称已存在，不能重复添加");
                }

                SysRight sysRight = new SysRight();
                sysRight.Name = newRight.Name;
                sysRight.Remark = newRight.Remark;
                sysRight.RightCode = newRight.RightCode;
                sysRight.Stopped = newRight.Stopped;

                _unitOfWork.AddAction(sysRight, DataActions.Add);
                _unitOfWork.Save();

                return newRight;
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
        /// 删除权限信息
        /// </summary>
        /// <param name="deleteRight">需要删除的权限信息</param>
        public bool Delete(RightEditModel deleteRight)
        {
            bool res = true;
            try
            {
                res = this.DeleteByCode(deleteRight.RightCode);
            }
            catch (Exception ex)
            {
                res = false;
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<LCFault>
                (new LCFault(msg), reason);
            }
            return res;
        }

        /// <summary>
        /// 根据编号删除对应权限
        /// </summary>
        /// <param name="code">需要删除的权限编码</param>
        public bool DeleteByCode(string code)
        {
            bool res = true;
            try
            {
                RightEditModel rightEditModel = new RightEditModel();
                Repository<SysRight> sysRightDal = _unitOfWork.GetRepository<SysRight>();
                SysRight sysRight = sysRightDal.GetObjectByKey(code).Entity;
                if (sysRight != null)
                {
                    //此处不提示直接删除
                    if (sysRight.SysRoles!=null&&sysRight.SysRoles.Count >0)
                    {
                        sysRight.SysRoles.Clear();
                    }
                    if (sysRight.SysMenus != null && sysRight.SysMenus.Count > 0)
                    {
                        sysRight.SysMenus.Clear();
                    }
                    _unitOfWork.AddAction(sysRight, DataActions.Delete);
                    _unitOfWork.Save();
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("权限删除失败"), "该权限不存在，无法删除");
                }
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
        /// 查找所有权限信息
        /// </summary>
        public IList<RightDisplayModel> GetAllRights()
        {
            try
            {
                List<RightDisplayModel> rightDisplayModelList = new List<RightDisplayModel>();
                Repository<SysRight> sysRightDal = _unitOfWork.GetRepository<SysRight>();
                List<SysRight> sysRightList = sysRightDal.GetAll().Entities.ToList();

                if (sysRightList == null || sysRightList.Count() == 0)
                {
                    return null;
                }

                rightDisplayModelList = BuildModels(sysRightList);
                return rightDisplayModelList;
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
        /// 根据权限编号获取编辑对象
        /// </summary>
        /// <param name="code">权限编号</param>
        public RightEditModel GetRightByCode(string code)
        {
            try
            {
                RightEditModel rightEditModel = new RightEditModel();
                Repository<SysRight> sysRightDal = _unitOfWork.GetRepository<SysRight>();
                SysRight sysRight = sysRightDal.GetObjectByKey(code).Entity;
                if (sysRight != null)
                {
                    rightEditModel.InitEditModel(sysRight);
                }
                return rightEditModel;
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
        /// 更新权限信息
        /// </summary>
        /// <param name="newRight">需要更新的权限信息</param>
        public RightEditModel Update(RightEditModel newRight)
        {
            try
            {
                Repository<SysRight> sysRightDal = _unitOfWork.GetRepository<SysRight>();
                SysRight sysRight = sysRightDal.GetObjectByKey(newRight.RightCode).Entity;
                if (sysRight != null)
                {
                    sysRight.Name = newRight.Name;
                    sysRight.Remark = newRight.Remark;
                    sysRight.Stopped = newRight.Stopped;
                    sysRight.SysMenus.Clear();
                    if (newRight.SysMenus != null && newRight.SysMenus.Count > 0)
                    {
                        Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                        List<SysMenu> sysMenuList = sysMenuDal.GetAll().Entities.ToList();
                        foreach (MenuEditModel sysMenuTemp in newRight.SysMenus)
                        {
                            SysMenu sysMenu = sysMenuList.Single(sm => sm.MenuCode == sysMenuTemp.MenuCode);
                            sysRight.SysMenus.Add(sysMenu);
                        }
                    }
                    if (newRight.SysRoles != null && newRight.SysRoles.Count > 0)
                    {
                        sysRight.SysRoles.Clear();
                        Repository<SysRole> sysRoleDal = _unitOfWork.GetRepository<SysRole>();
                        List<SysRole> sysRoleList = sysRoleDal.GetAll().Entities.ToList();
                        foreach (RoleEditModel sysRoleTemp in newRight.SysRoles)
                        {
                            SysRole sysRole = sysRoleList.Single(sr => sr.RoleCode == sysRoleTemp.RoleCode);
                            sysRight.SysRoles.Add(sysRole);
                        }
                    }
                }
                _unitOfWork.AddAction(sysRight, DataActions.Update);
                _unitOfWork.Save();
                return newRight;
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

        public RightEditModel SetRightMenus(RightEditModel rightEditModel, List<string> menuCodes)
        {
            try
            {
                Repository<SysRight> sysRightDal = _unitOfWork.GetRepository<SysRight>();
                SysRight sysRight = sysRightDal.GetObjectByKey(rightEditModel.RightCode).Entity;
                if (sysRight != null)
                {
                    sysRight.Name = rightEditModel.Name;
                    sysRight.Remark = rightEditModel.Remark;
                    sysRight.Stopped = rightEditModel.Stopped;
                }

                //修改菜单
                sysRight.SysMenus.Clear();
                if (menuCodes != null && menuCodes.Count > 0)
                {
                    List<SysMenu> sysMenuListTemp = new List<SysMenu>();
                    Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                    foreach (string menuCode in menuCodes)
                    {
                        SysMenu sysMenu = sysMenuDal.GetObjectByKey(menuCode).Entity;
                        sysMenuListTemp.Add(sysMenu);
                    }

                    sysRight.SysMenus = sysMenuListTemp;
                }
                _unitOfWork.AddAction(sysRight, DataActions.Update);
                _unitOfWork.Save();

                return rightEditModel;
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
        /// 根据实体对象转换成显示对象
        /// </summary>
        /// <param name="sysRight">需要转换的权限实体对象</param>
        private RightDisplayModel BuildModel(SysRight sysRight)
        {
            if (sysRight == null)
            {
                return null;
            }

            RightDisplayModel rightDisplayModel = new RightDisplayModel();
            rightDisplayModel.Code = sysRight.RightCode;
            rightDisplayModel.Name = sysRight.Name;
            rightDisplayModel.Remark = sysRight.Remark;
            rightDisplayModel.Stopped = sysRight.Stopped.Value;
            return rightDisplayModel;
        }

        /// <summary>
        /// 转换多个实体对象到显示对象
        /// </summary>
        /// <param name="sysRights">需要转换的权限列表</param>
        private List<RightDisplayModel> BuildModels(List<SysRight> sysRights)
        {
            if (sysRights == null || sysRights.Count() == 0)
            {
                return null;
            }
            List<RightDisplayModel> rightDisplayModelList = new List<RightDisplayModel>();
            foreach (SysRight sysRightTemp in sysRights)
            {
                RightDisplayModel rightDisplayModel = BuildModel(sysRightTemp);
                if (rightDisplayModel == null)
                {
                    continue;
                }
                rightDisplayModelList.Add(rightDisplayModel);
            }
            return rightDisplayModelList;
        }

        /// <summary>
        /// 判断编号是否存在
        /// </summary>
        /// <param name="code">权限编号</param>
        private bool CheckCodeExists(string code)
        {
            Repository<SysRight> sysRightDal = _unitOfWork.GetRepository<SysRight>();
            SysRight sysRight = sysRightDal.GetObjectByKey(code).Entity;
            if (sysRight != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断权限名称是否存在
        /// </summary>
        /// <param name="name">权限名称</param>
        private bool CheckNameExists(string name)
        {
            Repository<SysRight> sysRightDal = _unitOfWork.GetRepository<SysRight>();
            var sysRight = sysRightDal.Single(sr => sr.Name == name);
            if (sysRight.HasValue)
            {
                return true;
            }
            return false;
        }
        #endregion

        public void Dispose()
        {
            this._unitOfWork.Dispose();
        }

    }//end RightModelService

}//end namespace SysManager