using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;

namespace Aisino.MES.Service.SysManager.Impl
{
    public class SysModuleService : ISysModuleService
    {
        private Repository<SysModule> _sysModuleDal;

        public SysModuleService(Repository<SysModule> sysModuleDal)
        {
            _sysModuleDal = sysModuleDal;
        }

        public IList<SysModule> SelectAllSysModule()
        {
            return _sysModuleDal.GetAll().Entities.ToList();
        }

        public SysModule GetSysModule(int id)
        {
            var sysModuleDal = _sysModuleDal.Single(s => s.id == id);
            if (sysModuleDal.HasValue)
            {
                return sysModuleDal.Entity;
            }
            else
            {
                return null;
            }
        }

        public SysModule AddSysModule(SysModule newModule)
        {
            SysModule returnModule = null;
            try
            {
                _sysModuleDal.Add(newModule);
                returnModule = newModule;
            }
            catch (RepositoryException ex)
            {
                throw ex; 
            }
            return returnModule;
        }

        public SysModule UpdateSysModule(SysModule updateModule)
        {            
            SysModule returnModule = null;
            try
            {
                _sysModuleDal.Update(updateModule);
                returnModule = updateModule;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnModule;
        }

        public SysModule DeleteSysModule(SysModule deleteModule)
        {
            SysModule returnModule = null;
            try
            {
                deleteModule.module_deleted = true;
                _sysModuleDal.Update(deleteModule);
                returnModule = deleteModule;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除模块信息失败！", ex);
            }
            return returnModule;
        }

        public void DeleteSysModuleList(List<SysModule> lstDelSysModule)
        {
            try
            {
                foreach (SysModule deleteModule in lstDelSysModule)
                {
                    if (deleteModule.module_deleted == true)
                    {
                        continue;
                    }
                    deleteModule.module_deleted = true;
                    _sysModuleDal.Update(deleteModule);
                }
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除模块信息失败！", ex);
            }
        }

        public bool CheckModuleCodeExist(string moduleCode)
        {
            var sysModule = _sysModuleDal.Single(s => s.module_code == moduleCode);
            if (sysModule.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        public bool CheckModuleNameExist(string moduleName)
        {

            var sysModule = _sysModuleDal.Single(s => s.module_name == moduleName);
            if (sysModule.HasValue)
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
