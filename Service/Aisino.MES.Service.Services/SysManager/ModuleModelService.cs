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
    /// 模块服务实现
    /// </summary>
    public class ModuleModelService : IModuleModelService
    {
        private UnitOfWork _unitOfWork;
        public ModuleModelService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 根据编号获取模块信息
        /// </summary>
        /// <param name="code">模块信息编号</param>
        /// <returns>获取的模块信息实例</returns>
        public ModuleEditModel GetModuleByCode(string modulecode)
        {
            try
            {
                ModuleEditModel moduleEditModel = new ModuleEditModel(); ;
                Repository<SysModule> sysModuleDal = _unitOfWork.GetRepository<SysModule>();
                SysModule sysModule = sysModuleDal.GetObjectByKey(modulecode).Entity;
                if (sysModule != null)
                {
                    moduleEditModel.InitEditModel(sysModule);
                    moduleEditModel.ModuleCode = sysModule.ModuleCode;
                    moduleEditModel.Name = sysModule.Name;
                    moduleEditModel.Remark = sysModule.Remark;
                    moduleEditModel.Stopped = sysModule.Stopped;
                }
                return moduleEditModel;
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
        /// 添加模块
        /// </summary>
        /// <param name="newModule">需要添加的模块信息</param>
        public ModuleEditModel Add(ModuleEditModel newModule)
        {
            try
            {
                if (CheckCodeExists(newModule.ModuleCode))
                {
                    throw new FaultException<LCFault>(new LCFault("模块添加失败"), "该模块编号已存在，不能重复添加");
                }
                SysModule sysModule = new SysModule();
                sysModule.ModuleCode = newModule.ModuleCode;
                sysModule.Name = newModule.Name;
                sysModule.Remark = newModule.Name;
                sysModule.Stopped = newModule.Stopped;
                _unitOfWork.AddAction(sysModule, DataActions.Add);
                _unitOfWork.Save();

                return newModule;
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
        /// 删除模块信息
        /// </summary>
        /// <param name="deleteModule">需要删除的模块信息</param>
        public bool Delete(ModuleEditModel deleteModule)
        {
            try
            {
                return DeleteByCode(deleteModule.ModuleCode);
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
        /// 根据编号删除模块信息
        /// </summary>
        /// <param name="code">需要删除的模块信息编号</param>
        public bool DeleteByCode(string code)
        {
            bool res = false;
            try
            {
                Repository<SysModule> sysmoduleDal = _unitOfWork.GetRepository<SysModule>();
                SysModule sysmodule = sysmoduleDal.GetObjectByKey(code).Entity;
                if (sysmodule != null)
                {
                    if (sysmodule.SysMenus!=null&&sysmodule.SysMenus.Count>0)
                    {
                         throw new FaultException<LCFault>(new LCFault("模块删除失败"), "该模块下存在菜单，无法删除");
                    }
                    if (sysmodule.SysFunctions != null && sysmodule.SysFunctions.Count > 0)
                    {
                        throw new FaultException<LCFault>(new LCFault("模块删除失败"), "该模块下存在方法，无法删除");
                    }
                    _unitOfWork.AddAction(sysmodule, DataActions.Delete);
                    _unitOfWork.Save();
                    res = true;
                }
                else
                {
                    throw new FaultException<LCFault>(new LCFault("模块删除失败"), "该模块不存在，无法删除");
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
        /// 获取所有模块信息
        /// </summary>
        public IList<ModuleDisplayModel> GetAllModules()
        {
            try
            {
                Repository<SysModule> sysmoduleDal = _unitOfWork.GetRepository<SysModule>();
                IEnumerable<SysModule> sysmodules = sysmoduleDal.GetAll().Entities;
                if (sysmodules != null)
                {
                    return BuildModelList(sysmodules.ToList());
                }
                else
                {
                    return null;
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

        private IList<ModuleDisplayModel> BuildModelList(List<SysModule> sysmodulelist)
        {
            if (sysmodulelist == null)
            {
                return null;
            }
            else
            {
                IList<ModuleDisplayModel> moduledisplays = new List<ModuleDisplayModel>();
                foreach (SysModule sysmodule in sysmodulelist)
                {
                    moduledisplays.Add(BuildModel(sysmodule));
                }
                return moduledisplays;
            }
        }

        /// <summary>
        /// 更新模块信息
        /// </summary>
        /// <param name="newModule">需要更新的模块信息</param>
        public ModuleEditModel Update(ModuleEditModel newModule)
        {
            try
            {
                Repository<SysModule> sysModuleDal = _unitOfWork.GetRepository<SysModule>();
                SysModule sysModule = sysModuleDal.GetObjectByKey(newModule.ModuleCode).Entity;
                if (sysModule != null)
                {
                    sysModule.Remark = newModule.Name;
                    sysModule.Stopped = newModule.Stopped;
                }
                _unitOfWork.AddAction(sysModule, DataActions.Update);
                _unitOfWork.Save();

                return newModule;
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
        /// 转换实体对象到显示对象
        /// </summary>
        /// <param name="sysModule">需要转换的实体对象</param>
        private ModuleDisplayModel BuildModel(SysModule sysModule)
        {
            if (sysModule == null)
            {
                return null;
            }
            else
            {
                ModuleDisplayModel moduledisplay = new ModuleDisplayModel();
                moduledisplay.Code = sysModule.ModuleCode;
                moduledisplay.Name = sysModule.Name;
                moduledisplay.Remark = sysModule.Remark;
                moduledisplay.Stopped = sysModule.Stopped;

                return moduledisplay;
            }
        }

        /// <summary>
        /// 转换多个实体对象到显示对象
        /// </summary>
        /// <param name="sysModules">需要转换的模块实体</param>
        private List<ModuleDisplayModel> BuildModels(List<SysModule> sysModules)
        {

            return null;
        }

        /// <summary>
        /// 判断编号是否存在
        /// </summary>
        /// <param name="code">模块编号</param>
        private bool CheckCodeExists(string code)
        {
            try
            {
                Repository<SysModule> sysModuleDal = _unitOfWork.GetRepository<SysModule>();
                var sysmodule = sysModuleDal.GetObjectByKey(code);
                if (sysmodule.HasValue)
                {
                    return true;
                }
                else
                {
                    return false;
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
        /// 判断模块名称是否存在
        /// </summary>
        /// <param name="name">模块名称</param>
        private bool CheckNameExists(string name)
        {

            return false;
        }

    }//end ModuleModelService

}//end namespace SysManager