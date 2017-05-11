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
    public class ClassTypeService : IClassTypeService
    {
        private UnitOfWork _unitOfWork;
        public ClassTypeService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// 获得所有班级类型信息
        /// </summary>
        /// <returns></returns>
        public IList<ClassTypeModel> GetAllClassType()
        {
            try
            {
                Repository<class_types> classTypeModuleDal = _unitOfWork.GetRepository<class_types>();
                IEnumerable<class_types> classTypes = classTypeModuleDal.GetAll().Entities;
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

        public IList<ClassTypeModel> GetAllClassTypeWithClasses()
        {
            try
            {
                Repository<class_types> classTypeModuleDal = _unitOfWork.GetRepository<class_types>();
                IEnumerable<class_types> classTypes = classTypeModuleDal.GetAll().Entities;
                if (classTypes != null)
                {
                    return BuildModelList(classTypes.ToList(),true);
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

        public ClassTypeModel GetClassTypeById(int id)
        {
            try
            {
                Repository<class_types> classTypeModuleDal = _unitOfWork.GetRepository<class_types>();
                class_types classType = classTypeModuleDal.GetObjectByKey(id).Entity;
                if (classType != null)
                {
                    return BuildModel(classType);
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
        /// 创建班级类型信息
        /// </summary>
        /// <param name="newUserModel">需要创建的班级类型信息</param>
        public ClassTypeModel Add(ClassTypeModel newClassTypeModel)
        {
            try
            {
                class_types classtypes = new class_types();

                classtypes.name = newClassTypeModel.Name;
                classtypes.commission_rate_consultant = Convert.ToDecimal(newClassTypeModel.ConsultantRate);
                classtypes.commission_rate_assistant = Convert.ToDecimal(newClassTypeModel.AssistantRate);
                classtypes.commission_rate_teacher = Convert.ToDecimal(newClassTypeModel.TeacherRate);
                classtypes.description = newClassTypeModel.Des;
                classtypes.is_active = newClassTypeModel.IsActive;
                classtypes.student_limit = newClassTypeModel.StudentLimit;
                classtypes.total_lessons = newClassTypeModel.TotalLessons;
                classtypes.unit_price = newClassTypeModel.UnitPrice;

                _unitOfWork.AddAction(classtypes, DataActions.Add);
                _unitOfWork.Save();
                newClassTypeModel.Id = classtypes.id;

                return newClassTypeModel;
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
        /// 更新班级类型信息
        /// </summary>
        /// <param name="newUserModel">需要更新的班级类型信息</param>
        public ClassTypeModel Update(ClassTypeModel newClassTypeModel)
        {
            try
            {
                Repository<class_types> classtypesDal = _unitOfWork.GetRepository<class_types>();
                class_types classtypes = classtypesDal.GetObjectByKey(newClassTypeModel.Id).Entity;
                if (classtypes != null)
                {
                    classtypes.name = newClassTypeModel.Name;
                    classtypes.commission_rate_consultant = Convert.ToDecimal(newClassTypeModel.ConsultantRate);
                    classtypes.commission_rate_assistant = Convert.ToDecimal(newClassTypeModel.AssistantRate);
                    classtypes.commission_rate_teacher = Convert.ToDecimal(newClassTypeModel.TeacherRate);
                    classtypes.description = newClassTypeModel.Des;
                    classtypes.is_active = newClassTypeModel.IsActive;
                    classtypes.student_limit = newClassTypeModel.StudentLimit;
                    classtypes.total_lessons = newClassTypeModel.TotalLessons;
                    classtypes.unit_price = newClassTypeModel.UnitPrice;
                }
                _unitOfWork.AddAction(classtypes, DataActions.Update);
                _unitOfWork.Save();

                return newClassTypeModel;
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
        /// 删除班级类型信息
        /// </summary>
        /// <param name="deleteUserModel">删除班级类型信息</param>
        public bool Delete(ClassTypeModel deleteClassTypeModel)
        {
            try
            {
                return DeleteById(deleteClassTypeModel.Id);
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
        /// 根据班级类型编号删除班级类型信息
        /// </summary>
        /// <param name="userCode">班级类型编号</param>
        public bool DeleteById(int id)
        {
            bool res = true;
            try
            {
                Repository<class_types> classtypesDal = _unitOfWork.GetRepository<class_types>();
                class_types classtypes = classtypesDal.GetObjectByKey(id).Entity;
                if (classtypes != null)
                {
                    if (classtypes.subclasses.Count > 0)
                    {
                        throw new FaultException<LCFault>(new LCFault("删除班级类型失败"), "该班级类型还存在下属班级信息，无法删除");
                    }
                    else
                    {
                        _unitOfWork.AddAction(classtypes, DataActions.Delete);
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("删除班级类型失败"), "该班级类型不存在，无法删除");
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

        #region schemas
        public IList<SchemasEditModel> FindSchemasByClassType(int typeId)
        {
            try
            {
                Repository<lesson_schemas> schemasModuleDal = _unitOfWork.GetRepository<lesson_schemas>();
                IEnumerable<lesson_schemas> schemas = schemasModuleDal.Find(c => c.class_type_id == typeId).Entities;
                if (schemas != null)
                {
                    return BuildModelList(schemas.ToList());
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

        public SchemasEditModel AddSchemas(SchemasEditModel newSchemasEditModel)
        {
            try
            {
                lesson_schemas schemas = new lesson_schemas();

                schemas.class_type_id = newSchemasEditModel.TypeId;
                schemas.lesson_name = newSchemasEditModel.LessonName;
                schemas.level_name = newSchemasEditModel.LevelName;
                schemas.sequence_num = newSchemasEditModel.Seq;

                _unitOfWork.AddAction(schemas, DataActions.Add);
                _unitOfWork.Save();
                newSchemasEditModel.Id = schemas.lesson_schemas_id;

                return newSchemasEditModel;
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

        public int GetMaxSeqByClassType(int typeId)
        {
            try
            {
                Repository<lesson_schemas> schemasModuleDal = _unitOfWork.GetRepository<lesson_schemas>();
                IEnumerable<lesson_schemas> schemas = schemasModuleDal.Find(c => c.class_type_id == typeId).Entities.OrderByDescending(c=>c.sequence_num);
                if (schemas != null)
                {
                    return schemas.First().sequence_num.Value;
                }
                else
                {
                    return 0;
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
        #endregion

        #region 私有方法
        private IList<ClassTypeModel> BuildModelList(List<class_types> classTypes,bool withClasses = false)
        {
            if (classTypes == null)
            {
                return null;
            }
            else
            {
                IList<ClassTypeModel> moduledisplays = new List<ClassTypeModel>();
                foreach (class_types classtypes in classTypes)
                {
                    ClassTypeModel moduledisplay = BuildModel(classtypes);
                    if (withClasses)
                    {
                        moduledisplay.classDisplayModels = new List<ClassDisplayModel>();
                        foreach (classes cs in classtypes.subclasses)
                        {
                            if (cs.is_active)
                            {
                                ClassDisplayModel cdm = new ClassDisplayModel();
                                cdm.Id = cs.class_id;
                                cdm.Name = cs.class_name;
                                cdm.LastCount = cs.last_count;
                                moduledisplay.classDisplayModels.Add(cdm);
                            }
                        }
                    }

                    moduledisplays.Add(moduledisplay);
                }
                return moduledisplays;
            }
        }

        private ClassTypeModel BuildModel(class_types classtypesModule)
        {
            if (classtypesModule == null)
            {
                return null;
            }
            else
            {
                ClassTypeModel classtypemodel = new ClassTypeModel();
                classtypemodel.Id = classtypesModule.id;
                classtypemodel.Name = classtypesModule.name;
                classtypemodel.AssistantRate = Convert.ToDouble(classtypesModule.commission_rate_assistant);
                classtypemodel.ConsultantRate = Convert.ToDouble(classtypesModule.commission_rate_consultant);
                classtypemodel.Des = classtypesModule.description;
                classtypemodel.IsActive = classtypesModule.is_active;
                classtypemodel.StudentLimit = classtypesModule.student_limit;
                classtypemodel.TeacherRate = Convert.ToDouble(classtypesModule.commission_rate_teacher);
                classtypemodel.TotalLessons = classtypesModule.total_lessons;
                classtypemodel.UnitPrice = classtypesModule.unit_price;

                return classtypemodel;
            }
        }

        private IList<SchemasEditModel> BuildModelList(List<lesson_schemas> schemas)
        {
            if (schemas == null)
            {
                return null;
            }
            else
            {
                IList<SchemasEditModel> moduledisplays = new List<SchemasEditModel>();
                foreach (lesson_schemas s in schemas)
                {
                    SchemasEditModel moduledisplay = BuildModel(s);
                    moduledisplays.Add(moduledisplay);
                }
                return moduledisplays;
            }
        }

        private SchemasEditModel BuildModel(lesson_schemas schemas)
        {
            if (schemas == null)
            {
                return null;
            }
            else
            {
                SchemasEditModel schemasModel = new SchemasEditModel();
                schemasModel.TypeId = schemas.class_type_id;
                schemasModel.Id = schemas.lesson_schemas_id;
                schemasModel.LessonName = schemas.lesson_name;
                schemasModel.LevelName = schemas.level_name;
                schemasModel.Seq = schemas.sequence_num.Value;

                return schemasModel;
            }
        }
        #endregion
    }
}
