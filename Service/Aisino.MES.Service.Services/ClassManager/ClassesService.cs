using LC.Contracts.ClassManager;
using LC.DAL.Enums;
using LC.DAL.Interfaces;
using LC.DAL.Repository.Repositories;
using LC.DAL.UnitOfWork;
using LC.Model;
using LC.Model.Business.ClassModel;
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Service.Services.ClassManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ClassesService : IClassesService
    {
        private UnitOfWork _unitOfWork;
        public ClassesService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// 根据班级类型获得所有班级信息
        /// </summary>
        /// <returns></returns>
        public IList<ClassDisplayModel> FindClassByClassType(int classTypeId)
        {
            try
            {
                Repository<classes> classTypeModuleDal = _unitOfWork.GetRepository<classes>();
                IEnumerable<classes> classTypes = classTypeModuleDal.Find(c => c.class_type == classTypeId).Entities;
                if (classTypes != null)
                {
                    return BuildModelList(classTypes.ToList());
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

        public IList<ClassRecordDisplayModel> FindClassRecordByTeacher(int teacherId)
        {
            //根据教师id获取所有未结算上课记录,并计算所有课时需要结算金额总和
            try
            {
                Repository<class_record> recordDal = _unitOfWork.GetRepository<class_record>();
                IEnumerable<class_record> records = recordDal.Find(r => r.teacher_id == teacherId).Entities;
                if (records != null)
                {
                    return BuildModelList(records.ToList());
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

        public IList<ClassDisplayModel> GetAllClasses()
        {
            try
            {
                Repository<classes> classTypeModuleDal = _unitOfWork.GetRepository<classes>();
                IEnumerable<classes> classTypes = classTypeModuleDal.GetAll().Entities;
                if (classTypes != null)
                {
                    return BuildModelList(classTypes.ToList());
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

        public IList<ClassDisplayModel> GetAllActiveClasses()
        {
            try
            {
                Repository<classes> classModuleDal = _unitOfWork.GetRepository<classes>();
                IEnumerable<classes> classes = classModuleDal.Find(c=>c.is_active == true).Entities;
                if (classes != null)
                {
                    return BuildModelList(classes.ToList());
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

        public ClassEditModel GetClassById(int id)
        {
            try
            {
                ClassEditModel classEditModel = new ClassEditModel();
                Repository<classes> classesDal = _unitOfWork.GetRepository<classes>();
                classes classEntity = classesDal.GetObjectByKey(id).Entity;
                if (classEntity != null)
                {
                    classEditModel.InitEditModel(classEntity);
                }
                return classEditModel;
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
        /// 创建班级信息
        /// </summary>
        /// <param name="newClassEditModel">需要创建的班级信息</param>
        public ClassEditModel Add(ClassEditModel newClassEditModel)
        {
            try
            {
                classes classentity = new classes();

                classentity.class_name = newClassEditModel.Name;
                classentity.class_type = newClassEditModel.TypeId;
                classentity.end_date = newClassEditModel.EndDate;
                classentity.last_count = newClassEditModel.LastCount;
                classentity.start_date = newClassEditModel.StartDate;
                classentity.is_active = newClassEditModel.IsActive;

                _unitOfWork.AddAction(classentity, DataActions.Add);
                _unitOfWork.Save();
                newClassEditModel.Id = classentity.class_id;

                return newClassEditModel;
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
        /// 更新班级信息
        /// </summary>
        /// <param name="newClassEditModel">需要更新的班级信息</param>
        public ClassEditModel Update(ClassEditModel newClassEditModel)
        {
            try
            {
                Repository<classes> classesDal = _unitOfWork.GetRepository<classes>();
                classes classentity = classesDal.GetObjectByKey(newClassEditModel.Id).Entity;
                if (classentity != null)
                {
                    classentity.class_name = newClassEditModel.Name;
                    classentity.class_type = newClassEditModel.TypeId;
                    classentity.end_date = newClassEditModel.EndDate;
                    classentity.last_count = newClassEditModel.LastCount;
                    classentity.start_date = newClassEditModel.StartDate;
                    classentity.is_active = newClassEditModel.IsActive;
                }
                _unitOfWork.AddAction(classentity, DataActions.Update);
                _unitOfWork.Save();

                return newClassEditModel;
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

        public ClassEditModel UpdateClassStudents(int classId, List<int> studentIds)
        {
            try
            {
                Repository<classes> classesDal = _unitOfWork.GetRepository<classes>();
                classes classentity = classesDal.GetObjectByKey(classId).Entity;
                if (classentity != null)
                {
                    classentity.students.Clear();
                    Repository<student> studentDal = _unitOfWork.GetRepository<student>();
                    foreach(int studentId in studentIds)
                    {
                        student studententity = studentDal.GetObjectByKey(studentId).Entity;
                        classentity.students.Add(studententity);
                    }
                }
                _unitOfWork.AddAction(classentity, DataActions.Update);
                _unitOfWork.Save();

                return GetClassById(classentity.class_id);
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
        /// 删除班级信息
        /// </summary>
        /// <param name="deleteUserModel">删除班级信息</param>
        public bool Delete(ClassEditModel deleteClassEditModel)
        {
            try
            {
                return DeleteById(deleteClassEditModel.Id);
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
        /// 根据班级类型编号删除班级信息
        /// </summary>
        /// <param name="userCode">班级编号</param>
        public bool DeleteById(int id)
        {
            bool res = true;
            try
            {
                Repository<classes> classesDal = _unitOfWork.GetRepository<classes>();
                classes classentity = classesDal.GetObjectByKey(id).Entity;
                if (classentity != null)
                {
                    if (classentity.class_schedule.Count > 0)
                    {
                        throw new FaultException<LCFault>(new LCFault("删除班级失败"), "该班级已经编制课程表，无法删除");
                    }
                    else
                    {
                        _unitOfWork.AddAction(classentity, DataActions.Delete);
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("删除班级失败"), "该班级类型不存在，无法删除");
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
        private IList<ClassDisplayModel> BuildModelList(List<classes> classEntities)
        {
            if (classEntities == null)
            {
                return null;
            }
            else
            {
                IList<ClassDisplayModel> classdisplays = new List<ClassDisplayModel>();
                foreach (classes classEntity in classEntities)
                {
                    classdisplays.Add(BuildModel(classEntity));
                }
                return classdisplays;
            }
        }

        private ClassDisplayModel BuildModel(classes classEntity)
        {
            if (classEntity == null)
            {
                return null;
            }
            else
            {
                ClassDisplayModel classmodel = new ClassDisplayModel();
                classmodel.Id = classEntity.class_id;
                classmodel.Name = classEntity.class_name;
                classmodel.LastCount = classEntity.last_count;
                classmodel.TypeId = classEntity.class_type;
                classmodel.TypeName = classEntity.parentClassTypes.name;
                if (classEntity.start_date.HasValue)
                {
                    classmodel.StartDate = classEntity.start_date.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    classmodel.StartDate = "1900-01-01";
                }
                if (classEntity.end_date.HasValue)
                {
                    classmodel.EndDate = classEntity.end_date.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    classmodel.EndDate = "1900-01-01";
                }
                classmodel.IsActive = classEntity.is_active;

                return classmodel;
            }
        }

        private IList<ClassRecordDisplayModel> BuildModelList(List<class_record> recordEntities)
        {
            if (recordEntities == null)
            {
                return null;
            }
            else
            {
                IList<ClassRecordDisplayModel> classrecorddisplays = new List<ClassRecordDisplayModel>();
                foreach (class_record recordEntity in recordEntities)
                {
                    classrecorddisplays.Add(BuildModel(recordEntity));
                }
                return classrecorddisplays;
            }

        }
        private ClassRecordDisplayModel BuildModel(class_record recordEntity)
        {
            if (recordEntity == null)
            {
                return null;
            }
            else
            {
                ClassRecordDisplayModel recordmodel = new ClassRecordDisplayModel();
                recordmodel.ClassName = recordEntity.class_schedule.wclass.class_name;
                recordmodel.IsChecked = recordEntity.is_checked.Value == 0?false:true;
                recordmodel.Money = recordEntity.amount_receivable.Value * recordEntity.teacher_check_rate;
                recordmodel.ScheduleDate = recordEntity.class_schedule.real_date;
                recordmodel.StudentLimit = recordEntity.student_limit.Value;
                recordmodel.StudentNum = recordEntity.student_number;

                return recordmodel;
            }
        }
        #endregion
    }
}
