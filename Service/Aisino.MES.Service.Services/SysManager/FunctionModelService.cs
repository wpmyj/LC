using LC.DAL.UnitOfWork;
using LC.Model.Business.SysManager;
using LC.Service.Contracts.SysManager;
using LC.DAL.Repository.Repositories;
using LC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using LC.DAL.Enums;
using LC.DAL.Interfaces;
using LC.Model.Entity.Models;


namespace LC.Service.Services.SysManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    /// <summary>
    /// 方法服务实现
    /// </summary>
    public class FunctionModelService : IFunctionModelService
    {

        private UnitOfWork _unitOfWork;

        public FunctionModelService(UnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        ~FunctionModelService()
        {

        }

        /// <summary>
        /// 添加方法
        /// </summary>
        /// <param name="newFunction">需要添加的方法</param>
        public FunctionEditModel Add(FunctionEditModel newFunction)
        {

            try
            {
                if (CheckCodeExists(newFunction.FunctionCode))
                {
                    throw new FaultException<LCFault>(new LCFault("方法添加失败"), "该方法编号已存在，不能重复添加");
                }
                Repository<SysFunction> sysFunctionEiditModelDal = _unitOfWork.GetRepository<SysFunction>();

                SysFunction sysFunction = new SysFunction();
                sysFunction.FunctionCode = newFunction.FunctionCode;
                sysFunction.Name = newFunction.Name;
                sysFunction.Remark = newFunction.Remark;
                sysFunction.Type = newFunction.Type;
                sysFunction.Assembly = newFunction.Assembly;
                sysFunction.ClassName = newFunction.ClassName;
                sysFunction.OperationCode = newFunction.OperationCode;
                sysFunction.OperationName = newFunction.OperationName;
                sysFunction.Params = newFunction.Params;
                sysFunction.ModuleCode = newFunction.ModuleCode;

                _unitOfWork.AddAction(sysFunction, DataActions.Add);
                _unitOfWork.Save();

                return newFunction;
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
        /// 删除方法
        /// </summary>
        /// <param name="deleteFunction">需要删除的方法对象</param>
        public bool Delete(FunctionEditModel deleteFunction)
        {

            try
            {
                return DeleteByCode(deleteFunction.FunctionCode);
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
        /// 根据编号删除方法
        /// </summary>
        /// <param name="code">需要删除的方法编号</param>
        public bool DeleteByCode(string functionCode)
        {

            bool res = true;
            try
            {
                Repository<SysFunction> sysFunctionEditModelDal = _unitOfWork.GetRepository<SysFunction>();
                SysFunction sysFunctionEiditModel = sysFunctionEditModelDal.GetObjectByKey(functionCode).Entity;
                if (sysFunctionEditModelDal != null)
                {
                    _unitOfWork.AddAction(sysFunctionEiditModel, DataActions.Delete);
                    _unitOfWork.Save();
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("编号删除失败"), "该编号不存在，无法删除");
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
        /// 根据模块编号查找所有包含的方法
        /// </summary>
        /// <param name="moduleCode">模块编号</param>
        public IList<FunctionDisplayModel> FindFunctionsByModuleCode(string moduleCode)
        {

            try
            {
                Repository<SysFunction> sysFunctionDal = _unitOfWork.GetRepository<SysFunction>();
                IEnumerable<SysFunction> sysFunction = sysFunctionDal.GetAll().Entities;
                if (sysFunction != null)
                {
                    return BuildModels(sysFunction.ToList());
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
        /// 根据编号查找方法编辑对象
        /// </summary>
        /// <param name="code">方法编号</param>
        public FunctionEditModel GetFunctionByCode(string functionCode)
        {

            try
            {
                FunctionEditModel roleEditModel = new FunctionEditModel();
                Repository<SysFunction> sysFunctionDal = _unitOfWork.GetRepository<SysFunction>();
                SysFunction sysFunction = sysFunctionDal.GetObjectByKey(functionCode).Entity;
                if (sysFunction != null)
                {
                    roleEditModel.InitEditModel(sysFunction);
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
        /// 更新方法
        /// </summary>
        /// <param name="newFunction">需要更新的方法</param>
        public FunctionEditModel Update(FunctionEditModel newFunction)
        {
            try
            {
                Repository<SysFunction> sysFunctionDal = _unitOfWork.GetRepository<SysFunction>();
                SysFunction sysFunction = sysFunctionDal.GetObjectByKey(newFunction.FunctionCode).Entity;
                if (sysFunction != null)
                {
                    sysFunction.FunctionCode = newFunction.FunctionCode;
                    sysFunction.Name = newFunction.Name;
                    sysFunction.Remark = newFunction.Remark;
                    sysFunction.Type = newFunction.Type;
                    sysFunction.Assembly = newFunction.Assembly;
                    sysFunction.ClassName = newFunction.ClassName;
                    sysFunction.OperationCode = newFunction.OperationCode;
                    sysFunction.OperationName = newFunction.OperationName;
                    sysFunction.Params = newFunction.Params;
                    sysFunction.ModuleCode = newFunction.ModuleCode;
                    //sysFunction.ParentModule = newFunction.Function.ParentModule;
                    //sysFunction.SysMenus = newFunction.Function.SysMenus;
                }
                _unitOfWork.AddAction(sysFunction, DataActions.Update);
                _unitOfWork.Save();

                return newFunction;
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
        /// 方法显示对象
        /// </summary>
        /// <param name="sysFunction">需要转换的方法实体</param>
        private FunctionDisplayModel BuildModel(SysFunction sysFunction)
        {

            if (sysFunction == null)
            {
                return null;
            }
            else
            {
                FunctionDisplayModel functionDisplayMode = new FunctionDisplayModel();
                functionDisplayMode.Code = sysFunction.FunctionCode;
                functionDisplayMode.Name = sysFunction.Name;
                functionDisplayMode.Type = sysFunction.Type.ToString();
                functionDisplayMode.Remark = sysFunction.Remark;

                GetFunctionByCode(sysFunction.FunctionCode);

                return functionDisplayMode;
            }
        }

        /// <summary>
        /// 转换多个方法实体
        /// </summary>
        /// <param name="sysFunctions">需要转换的多个方法实体</param>
        private List<FunctionDisplayModel> BuildModels(List<SysFunction> sysFunctions)
        {

            if (sysFunctions == null)
            {
                return null;
            }
            else
            {
                List<FunctionDisplayModel> functionDisplayModels = new List<FunctionDisplayModel>();
                foreach (SysFunction sysFunction in sysFunctions)
                {
                    functionDisplayModels.Add(BuildModel(sysFunction));
                }
                return functionDisplayModels;
            }
        }

        /// <summary>
        /// 判断方法编号是否存在
        /// </summary>
        /// <param name="code">方法编号</param>
        private bool CheckCodeExists(string code)
        {
            try
            {
                Repository<SysFunction> FunctionDal = _unitOfWork.GetRepository<SysFunction>();
                var sysRole = FunctionDal.GetObjectByKey(code);
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
        /// 判断方法名称是否存在
        /// </summary>
        /// <param name="name">方法名称</param>
        private bool CheckNameExists(string functionName)
        {
            try
            {
                Repository<SysFunction> sysFunctionDal = _unitOfWork.GetRepository<SysFunction>();
                var sysDepartment = sysFunctionDal.Single(sd => sd.Name == functionName);
                if (sysDepartment.HasValue)
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

    }//end FunctionModelService

}//end namespace SysManager