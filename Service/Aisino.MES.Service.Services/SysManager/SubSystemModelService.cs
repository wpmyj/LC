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
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Service.Services.SysManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SubSystemModelService : ISubSystemModelService
    {
        private UnitOfWork _unitOfWork;

        public SubSystemModelService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region 实现接口
        public IList<SubSystemDisplayModel> FindSubSystemByUserCode(string userCode)
        {
            try
            {
                List<SysSubSystem> sysSubSystemList = this._unitOfWork.Context.Set<SysSubSystem>().SqlQuery("EXEC FindSubSystemByUserCode @usercode", new SqlParameter("usercode", userCode)).ToList();
                return BuildModels(sysSubSystemList);
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

        public SubSystemEditModel Add(SubSystemEditModel newSubSystem)
        {
            try
            {
                if (CheckCodeExists(newSubSystem.SubSystemCode))
                {
                    throw new FaultException<LCFault>(new LCFault("子菜单添加失败"), "该子菜单编号已存在，不能重复添加");
                }
                if (CheckNameExists(newSubSystem.SubSystemName))
                {
                    throw new FaultException<LCFault>(new LCFault("子菜单添加失败"), "该子菜单编号已存在，不能重复添加");
                }

                SysSubSystem sysSubSystem = new SysSubSystem();
                sysSubSystem.IconString = newSubSystem.IconString;
                sysSubSystem.MetroBackColor = newSubSystem.MetroBackColor;
                sysSubSystem.MetroForeColor = newSubSystem.MetroForeColor;
                sysSubSystem.MetroType = newSubSystem.MetroType;
                sysSubSystem.Remark = newSubSystem.Remark;
                sysSubSystem.SubSystemCode = newSubSystem.SubSystemCode;
                sysSubSystem.SubSystemName = newSubSystem.SubSystemName;

                this._unitOfWork.AddAction(sysSubSystem, DataActions.Add);
                this._unitOfWork.Save();
                return newSubSystem;
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

        public bool DeleteByCode(string code)
        {
            try
            {
                bool res = true;
                Repository<SysSubSystem> sysSubSystemDal = this._unitOfWork.GetRepository<SysSubSystem>();
                SysSubSystem sysSubSystem = sysSubSystemDal.GetObjectByKey(code).Entity;
                if (sysSubSystem != null)
                {
                    if (sysSubSystem.SysMenus!=null&&sysSubSystem.SysMenus.Count>0)
                    {
                        sysSubSystem.SysMenus.Clear();
                    }
                    this._unitOfWork.AddAction(sysSubSystem, DataActions.Delete);
                    this._unitOfWork.Save();
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("子菜单删除失败"), "该子菜单编号不存在，无法删除");
                }
                return res;
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

        public SubSystemEditModel GetSubSystemByCode(string code)
        {
            try
            {
                SubSystemEditModel subSystemEditModel = new SubSystemEditModel();
                Repository<SysSubSystem> sysSubSystemDal = this._unitOfWork.GetRepository<SysSubSystem>();
                SysSubSystem sysSubSystem = sysSubSystemDal.GetObjectByKey(code).Entity;
                if (sysSubSystem != null)
                {
                    subSystemEditModel.InitEditModel(sysSubSystem);
                }
                return subSystemEditModel;
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

        public SubSystemEditModel Update(SubSystemEditModel newSubSystem)
        {
            try
            {
                if (newSubSystem == null)
                {
                    return null;
                }
                Repository<SysSubSystem> sysSubSystemDal = this._unitOfWork.GetRepository<SysSubSystem>();
                SysSubSystem sysSubSystem = sysSubSystemDal.GetObjectByKey(newSubSystem.SubSystemCode).Entity;
                if (sysSubSystem != null)
                {
                    sysSubSystem.SubSystemCode = newSubSystem.SubSystemCode;
                    sysSubSystem.SubSystemName = newSubSystem.SubSystemName;
                    sysSubSystem.Remark = newSubSystem.Remark;
                    sysSubSystem.MetroBackColor = newSubSystem.MetroBackColor;
                    sysSubSystem.MetroForeColor = newSubSystem.MetroForeColor;
                    sysSubSystem.MetroType = newSubSystem.MetroType;
                    sysSubSystem.IconString = newSubSystem.IconString;
                }
                this._unitOfWork.AddAction(sysSubSystem, DataActions.Update);
                this._unitOfWork.Save();
                return newSubSystem;
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

        public SubSystemEditModel SetSubSystemMenu(SubSystemEditModel newSubSystem, List<string> menuCodes)
        {
            try
            {
                Repository<SysSubSystem> sysSubSystemDal = this._unitOfWork.GetRepository<SysSubSystem>();
                SysSubSystem sysSubSystem = sysSubSystemDal.GetObjectByKey(newSubSystem.SubSystemCode).Entity;
                if (sysSubSystem != null)
                {
                    sysSubSystem.Remark = newSubSystem.Remark;
                    sysSubSystem.MetroType = newSubSystem.MetroType;
                    sysSubSystem.MetroBackColor = newSubSystem.MetroBackColor;
                    sysSubSystem.MetroForeColor = newSubSystem.MetroForeColor;
                    sysSubSystem.IconString = newSubSystem.IconString;
                    if (sysSubSystem.SysMenus != null && sysSubSystem.SysMenus.Count > 0)
                    {
                        sysSubSystem.SysMenus.Clear();
                    }

                    if (menuCodes != null && menuCodes.Count > 0)
                    {
                        List<SysMenu> sysMenuListTemp = new List<SysMenu>();
                        Repository<SysMenu> sysMenuDal = _unitOfWork.GetRepository<SysMenu>();
                        foreach (string menuCode in menuCodes)
                        {
                            SysMenu sysMenu = sysMenuDal.GetObjectByKey(menuCode).Entity;
                            sysMenuListTemp.Add(sysMenu);
                        }
                        sysSubSystem.SysMenus = sysMenuListTemp;
                    }
                }
                _unitOfWork.AddAction(sysSubSystem, DataActions.Update);
                _unitOfWork.Save();
                return newSubSystem;
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

        public IList<SubSystemDisplayModel> GetAllSubMenu()
        {
            try
            {
                Repository<SysSubSystem> sysSubSystemDal = this._unitOfWork.GetRepository<SysSubSystem>();
                IEnumerable<SysSubSystem> sysSubSystems = sysSubSystemDal.GetAll().Entities;
                if (sysSubSystems != null)
                {
                    return this.BuildModels(sysSubSystems.ToList());
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

        #endregion

        #region 私有方法
        private List<SubSystemDisplayModel> BuildModels(List<SysSubSystem> sysSubSystemList)
        {
            List<SubSystemDisplayModel> menuDisplayModelList = new List<SubSystemDisplayModel>();
            if (sysSubSystemList != null && sysSubSystemList.Count > 0)
            {
                foreach (SysSubSystem sysSubSystem in sysSubSystemList)
                {
                    SubSystemDisplayModel ssdm = new SubSystemDisplayModel();
                    ssdm.SubSystemCode = sysSubSystem.SubSystemCode;
                    ssdm.SubSystemName = sysSubSystem.SubSystemName;
                    ssdm.MetroType = sysSubSystem.MetroType.ToString();
                    ssdm.BackColor = sysSubSystem.MetroBackColor;
                    ssdm.Remark = sysSubSystem.Remark;
                    ssdm.IconString = sysSubSystem.IconString;
                    ssdm.ForeColor = sysSubSystem.MetroForeColor;
                    menuDisplayModelList.Add(ssdm);
                }
                return menuDisplayModelList;
            }
            return null;
        }

        private bool CheckCodeExists(string code)
        {
            try
            {
                Repository<SysSubSystem> SysSubSystemDal = this._unitOfWork.GetRepository<SysSubSystem>();
                var sysSubSystem = SysSubSystemDal.GetObjectByKey(code);
                if (sysSubSystem.HasValue)
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

        private bool CheckNameExists(string name)
        {
            try
            {
                Repository<SysSubSystem> SysSubSystemDal = this._unitOfWork.GetRepository<SysSubSystem>();
                var sysSubSystem = SysSubSystemDal.Single(n => n.SubSystemName == name);
                if (sysSubSystem.HasValue)
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

        #endregion
    }
}
