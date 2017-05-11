using LC.Contracts.BaseManager;
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
    public class StatusService : IStatusService
    {
        private UnitOfWork _unitOfWork;
        public StatusService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 根据类型查询对应的状态
        /// </summary>
        /// <param name="centerId"></param>
        /// <returns></returns>
        public IList<StatusModel> FindStatusByCat(string cat)
        {
            try
            {
                Repository<status> statusDal = _unitOfWork.GetRepository<status>();
                IEnumerable<status> statusEntities = statusDal.Find(cr => cr.cat == cat).Entities;
                if (statusEntities != null)
                {
                    return BuildModelList(statusEntities.ToList());
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

        #region 私有方法
        private IList<StatusModel> BuildModelList(List<status> statusEntities)
        {
            if (statusEntities == null)
            {
                return null;
            }
            else
            {
                IList<StatusModel> moduledisplays = new List<StatusModel>();
                foreach (status statusEntity in statusEntities)
                {
                    moduledisplays.Add(BuildModel(statusEntity));
                }
                return moduledisplays;
            }
        }

        private StatusModel BuildModel(status statusEntity)
        {
            if (statusEntity == null)
            {
                return null;
            }
            else
            {
                StatusModel statusModel = new StatusModel();
                statusModel.Id = statusEntity.id;
                statusModel.Cat = statusEntity.cat;
                statusModel.Des = statusEntity.description;

                return statusModel;
            }
        }
        #endregion
    }


}
