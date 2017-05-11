using Aisino.MES.DAL.Enums;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.Model;
using Aisino.MES.Model.Business;
using Aisino.MES.Model.Entities;
using Aisino.MES.Service.Contracts.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Aisino.MES.Service.Services.Common
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]

    public class SysCodeService : ISysCodeService
    {
        public UnitOfWork _unitOfWork;
        public SysCodeService(UnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #region 公有方法
        public Model.Business.SysCodeEditModel Add(Model.Business.SysCodeEditModel newSysCodeModel)
        {
            try
            {
                Repository<SysCode> sysCodeDal = this._unitOfWork.GetRepository<SysCode>();
                this._unitOfWork.AddAction(newSysCodeModel.SysCodeEdit, DataActions.Add);
                this._unitOfWork.Save();
                return newSysCodeModel;
            }
            catch (RepositoryException rex)
            {
                string msg = rex.Message;
                string reason = rex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
        }

        public Model.Business.SysCodeEditModel Update(Model.Business.SysCodeEditModel newSysCodeModel)
        {
            try
            {
                Repository<SysCode> sysCodeDal = this._unitOfWork.GetRepository<SysCode>();
                SysCode sysCode = sysCodeDal.GetObjectByKey(newSysCodeModel.SysCodeEdit.Code).Entity;
                if (sysCode != null)
                {
                    sysCode.Stopped = newSysCodeModel.SysCodeEdit.Stopped;
                    sysCode.CodeType = newSysCodeModel.SysCodeEdit.CodeType;
                    this._unitOfWork.AddAction(sysCode, DataActions.Update);
                    this._unitOfWork.Save();
                    return newSysCodeModel;
                }
                else
                    return null;
            }
            catch (RepositoryException rex)
            {
                string msg = rex.Message;
                string reason = rex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }

        }

        public bool DeleteById(int id)
        {
            try
            {
                Repository<SysCode> sysCodeDal = this._unitOfWork.GetRepository<SysCode>();
                SysCode sysCode = sysCodeDal.GetObjectByKey(id).Entity;
                if (sysCode != null)
                {
                    this._unitOfWork.AddAction(sysCode, DataActions.Delete);
                    this._unitOfWork.Save();
                    return true;
                }
                return false;
            }
            catch (RepositoryException rex)
            {
                string msg = rex.Message;
                string reason = rex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
        }

        public bool Delete(Model.Business.SysCodeEditModel delSysCodeModel)
        {
            try
            {
                return this.DeleteById(delSysCodeModel.SysCodeEdit.Id);
            }
            catch (RepositoryException rex)
            {
                string msg = rex.Message;
                string reason = rex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
        }

        public IList<Model.Business.SysCodeDisplayModel> GetAllSysCodes()
        {
            try
            {
                Repository<SysCode> sysCodeDal = this._unitOfWork.GetRepository<SysCode>();
                IEnumerable<SysCode> sysCodeList = sysCodeDal.GetAll().Entities;
                if (sysCodeList != null)
                {
                    return this.BuildModelList(sysCodeList.ToList());
                }
                else
                    return null;
            }
            catch (RepositoryException rex)
            {
                string msg = rex.Message;
                string reason = rex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
        }

        public Model.Business.SysCodeEditModel GetSysCodeById(string id)
        {
            try
            {
                Repository<SysCode> sysCodeDal = this._unitOfWork.GetRepository<SysCode>();
                SysCode sysCode = sysCodeDal.GetObjectByKey(id).Entity;
                SysCodeEditModel sysCodeEditModel = new SysCodeEditModel();
                sysCodeEditModel.SysCodeEdit = sysCode;
                return sysCodeEditModel;
            }
            catch (RepositoryException rex)
            {
                string msg = rex.Message;
                string reason = rex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
        }
        #endregion


        #region 私有方法
        private bool CheckCodeExist(string code)
        {
            Repository<SysCode> sysCodeDal = this._unitOfWork.GetRepository<SysCode>();
            var sysCode = sysCodeDal.Single(sd => sd.Code == code);
            if (sysCode != null)
            {
                return true;
            }
            return false;
        }

        private bool CheckNameExist(string Name)
        {
            Repository<SysCode> sysCodeDal = this._unitOfWork.GetRepository<SysCode>();
            var sysCode = sysCodeDal.Single(sd => sd.Name == Name);
            if (sysCode != null)
            {
                return true;
            }
            return false;
        }

        private SysCodeDisplayModel BuildModel(SysCode sysCode)
        {
            try
            {
                if (sysCode == null)
                {
                    return null;
                }
                SysCodeDisplayModel sysCodeDisplay = new SysCodeDisplayModel();
                sysCodeDisplay.CodeType = sysCode.CodeType;
                sysCodeDisplay.Name = sysCode.Name;
                sysCodeDisplay.Stopped = sysCode.Stopped;
                return sysCodeDisplay;
            }
            catch (RepositoryException rex)
            {
                string msg = rex.Message;
                string reason = rex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
        }

        private IList<SysCodeDisplayModel> BuildModelList(List<SysCode> sysCodeList)
        {
            try
            {
                if (sysCodeList == null)
                {
                    return null;
                }
                else
                {
                    IList<SysCodeDisplayModel> CodeModelLists = new List<SysCodeDisplayModel>();
                    foreach (var item in sysCodeList)
                    {
                        CodeModelLists.Add(this.BuildModel(item));
                    }
                    return CodeModelLists;
                }
            }
            catch (RepositoryException rex)
            {
                string msg = rex.Message;
                string reason = rex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = ex.StackTrace;
                throw new FaultException<AisinoMesFault>
                (new AisinoMesFault(msg), reason);
            }
        }
        #endregion
    }
}
