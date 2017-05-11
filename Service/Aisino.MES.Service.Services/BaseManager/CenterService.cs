using LC.Contracts.BaseManager;
using LC.DAL.Enums;
using LC.DAL.Interfaces;
using LC.DAL.Repository.Repositories;
using LC.DAL.UnitOfWork;
using LC.Model;
using LC.Model.Business.BaseModel;
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Service.Services.BaseManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class CenterService :ICenterService
    {
        private UnitOfWork _unitOfWork;
        public CenterService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获得所有中心信息
        /// </summary>
        /// <returns></returns>
        public IList<CenterModel> GetAllCenter()
        {
            try
            {
                Repository<center> centermoduleDal = _unitOfWork.GetRepository<center>();
                IEnumerable<center> centers = centermoduleDal.GetAll().Entities;
                if (centers != null)
                {
                    return BuildModelList(centers.ToList());
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

        public CenterModel Add(CenterModel newCenterModel)
        {
            try
            {
                center cen = new center();

                cen.address = newCenterModel.Address;
                cen.name = newCenterModel.Name;
                cen.phone = newCenterModel.Phone;
                if (newCenterModel.ConsultantId != 0)
                    cen.consultant_id = newCenterModel.ConsultantId;

                _unitOfWork.AddAction(cen, DataActions.Add);
                _unitOfWork.Save();
                newCenterModel.Id = cen.center_id;

                return newCenterModel;
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

        public CenterModel Update(CenterModel newCenterModel)
        {
            try
            {
                Repository<center> centerDal = _unitOfWork.GetRepository<center>();
                center cen = centerDal.GetObjectByKey(newCenterModel.Id).Entity;
                if (cen != null)
                {
                    cen.address = newCenterModel.Address;
                    cen.name = newCenterModel.Name;
                    cen.phone = newCenterModel.Phone;
                    if (newCenterModel.ConsultantId != 0)
                        cen.consultant_id = newCenterModel.ConsultantId;
                }
                _unitOfWork.AddAction(cen, DataActions.Update);
                _unitOfWork.Save();

                return newCenterModel;
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

        public bool Delete(CenterModel deleteCenterModel)
        {
            try
            {
                return DeleteById(deleteCenterModel.Id);
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

        public bool DeleteById(int id)
        {
            bool res = true;
            try
            {
                Repository<center> centerDal = _unitOfWork.GetRepository<center>();
                center cen = centerDal.GetObjectByKey(id).Entity;
                if (cen != null)
                {
                    if (cen.subclassrooms.Count > 0)
                    {
                        throw new FaultException<LCFault>(new LCFault("用户中心失败"), "该中心存在子班级，无法删除");
                    }
                    else
                    {
                        _unitOfWork.AddAction(cen, DataActions.Delete);
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("用户中心失败"), "该中心不存在，无法删除");
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

        #region 私有方法
        private IList<CenterModel> BuildModelList(List<center> centers)
        {
            if (centers == null)
            {
                return null;
            }
            else
            {
                IList<CenterModel> moduledisplays = new List<CenterModel>();
                foreach (center centermodule in centers)
                {
                    moduledisplays.Add(BuildModel(centermodule));
                }
                return moduledisplays;
            }
        }

        private CenterModel BuildModel(center centerModule)
        {
            if (centerModule == null)
            {
                return null;
            }
            else
            {
                CenterModel centermodel = new CenterModel();
                centermodel.Id = centerModule.center_id;
                centermodel.Name = centerModule.name;
                centermodel.Phone = centerModule.phone;
                centermodel.Address = centerModule.address;
                if(centerModule.consultant_id.HasValue)
                {
                    centermodel.ConsultantId = centerModule.consultant_id.Value;
                    centermodel.ConsultantName = centerModule.consultants.name;
                }
                else
                {
                    centermodel.ConsultantId = 0;
                    centermodel.ConsultantName = "";
                }

                return centermodel;
            }
        }
        #endregion
    }
}
