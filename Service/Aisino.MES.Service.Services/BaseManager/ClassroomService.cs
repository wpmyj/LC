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
    public class ClassroomService : IClassroomService
    {
        private UnitOfWork _unitOfWork;
        public ClassroomService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// 获得所有教室信息
        /// </summary>
        /// <returns></returns>
        public IList<ClassroomModel> GetAllClassroom()
        {
            try
            {
                Repository<center_classrooms> classroommoduleDal = _unitOfWork.GetRepository<center_classrooms>();
                IEnumerable<center_classrooms> classrooms = classroommoduleDal.GetAll().Entities;
                if (classrooms != null)
                {
                    return BuildModelList(classrooms.ToList());
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

        /// <summary>
        /// 根据中心id查找所有所属教室
        /// </summary>
        /// <param name="centerId"></param>
        /// <returns></returns>
        public IList<ClassroomModel> FindClassroomByCenter(int centerId)
        {
            try
            {
                ClassroomModel classroomModel = new ClassroomModel();
                Repository<center_classrooms> classroommoduleDal = _unitOfWork.GetRepository<center_classrooms>();
                IEnumerable<center_classrooms> classrooms = classroommoduleDal.Find(cr => cr.center_id == centerId).Entities;
                if (classrooms != null)
                {
                    return BuildModelList(classrooms.ToList());
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
        /// 创建教室信息
        /// </summary>
        /// <param name="newUserModel">需要创建的教室信息</param>
        public ClassroomModel Add(ClassroomModel newClassroomModel)
        {
            try
            {
                center_classrooms classroom = new center_classrooms();

                classroom.center_id = newClassroomModel.CenterId;
                classroom.classroom_name = newClassroomModel.Name;
                classroom.upper_limit = newClassroomModel.UpperLimit;

                _unitOfWork.AddAction(classroom, DataActions.Add);
                _unitOfWork.Save();
                newClassroomModel.Id = classroom.classroom_id;

                return newClassroomModel;
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
        /// 更新教室信息
        /// </summary>
        /// <param name="newUserModel">需要更新的教室信息</param>
        public ClassroomModel Update(ClassroomModel newClassroomModel)
        {
            try
            {
                Repository<center_classrooms> classroommoduleDal = _unitOfWork.GetRepository<center_classrooms>();
                center_classrooms classroom = classroommoduleDal.GetObjectByKey(newClassroomModel.Id).Entity;
                if (classroom != null)
                {
                    classroom.center_id = newClassroomModel.CenterId;
                    classroom.classroom_name = newClassroomModel.Name;
                    classroom.upper_limit = newClassroomModel.UpperLimit;
                }
                _unitOfWork.AddAction(classroom, DataActions.Update);
                _unitOfWork.Save();

                return newClassroomModel;
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
        /// 删除教室信息
        /// </summary>
        /// <param name="deleteUserModel">删除教室信息</param>
        public bool Delete(ClassroomModel deleteClassroomModel)
        {
            try
            {
                return DeleteById(deleteClassroomModel.Id);
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
        /// 根据教室编号删除教室中心信息
        /// </summary>
        /// <param name="userCode">教学中心编号</param>
        public bool DeleteById(int id)
        {
            bool res = true;
            try
            {
                Repository<center_classrooms> classroomDal = _unitOfWork.GetRepository<center_classrooms>();
                center_classrooms classroom = classroomDal.GetObjectByKey(id).Entity;
                if (classroom != null)
                {
                    if (classroom.schedule.Count > 0)
                    {
                        throw new FaultException<LCFault>(new LCFault("教室信息删除失败"), "该教室目前正在被相关课程使用，无法删除");
                    }
                    else
                    {
                        _unitOfWork.AddAction(classroom, DataActions.Delete);
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("用户教室失败"), "该教室不存在，无法删除");
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
        private IList<ClassroomModel> BuildModelList(List<center_classrooms> classrooms)
        {
            if (classrooms == null)
            {
                return null;
            }
            else
            {
                IList<ClassroomModel> moduledisplays = new List<ClassroomModel>();
                foreach (center_classrooms classroom in classrooms)
                {
                    moduledisplays.Add(BuildModel(classroom));
                }
                return moduledisplays;
            }
        }

        private ClassroomModel BuildModel(center_classrooms classroomEntity)
        {
            if (classroomEntity == null)
            {
                return null;
            }
            else
            {
                ClassroomModel classroommodel = new ClassroomModel();
                classroommodel.Id = classroomEntity.classroom_id;
                classroommodel.CenterId = classroomEntity.center_id;
                classroommodel.Name = classroomEntity.classroom_name;
                if(!classroomEntity.upper_limit.HasValue)
                {
                    classroommodel.UpperLimit =0;
                }
                else
                {
                    classroommodel.UpperLimit = classroomEntity.upper_limit.Value;
                }

                return classroommodel;
            }
        }
        #endregion
    }
}
