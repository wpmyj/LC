using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;

namespace Aisino.MES.Service.SysManager.Impl
{
    public class SysFunctionService : ISysFunctionService
    {
        private Repository<SysFunction> _sysFunction;

        public SysFunctionService(Repository<SysFunction> sysFunctionDal)
        {
            _sysFunction = sysFunctionDal; 
        }

        public IList<SysFunction> SelectAllSysFunction()
        {
            return _sysFunction.GetAll().Entities.ToList();
        }

        public SysFunction GetSysFunction(int id)
        {
            var sysFunctionDal = _sysFunction.Single(s => s.id == id);
            if (sysFunctionDal.HasValue)
            {
                return sysFunctionDal.Entity;
            }
            else
            {
                return null;
            }
        }

        public SysFunction AddSysFunction(SysFunction newFunction)
        {           
            SysFunction returnFunction = null;
            try
            {
                _sysFunction.Add(newFunction);
                returnFunction = newFunction;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnFunction;
        }

        public SysFunction UpdateSysFunction(SysFunction updateFunction)
        {            
            SysFunction returnFunction = null;
            try
            {
                _sysFunction.Update(updateFunction);
                returnFunction = updateFunction;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnFunction;
        }

        public SysFunction DeleteSysFunction(SysFunction deleteFunction)
        {
            SysFunction returnFunction = null;
            try
            {
                _sysFunction.Delete(deleteFunction);
                returnFunction = deleteFunction;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnFunction;
        }

        public void DeleteSysFunctionList(List<SysFunction> lstDelSysFunction)
        {
            try
            {
                foreach (SysFunction sysFunction in lstDelSysFunction)
                {
                    _sysFunction.Delete(sysFunction);
                }
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除方法信息失败！", ex);
            }
        }

        public bool CheckFunctionCodeExist(string functionCode)
        {
            var sysFunction = _sysFunction.Single(s => s.function_code == functionCode);
            if (sysFunction.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckFunctionNameExist(string functionName)
        {
            var sysFunction = _sysFunction.Single(s => s.function_name == functionName);
            if (sysFunction.HasValue)
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
