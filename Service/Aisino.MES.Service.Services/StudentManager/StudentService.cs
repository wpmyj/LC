using LC.Contracts.StudentManager;
using LC.DAL.Enums;
using LC.DAL.Interfaces;
using LC.DAL.Repository.Repositories;
using LC.DAL.UnitOfWork;
using LC.Model;
using LC.Model.Business;
using LC.Model.Business.StudentModel;
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Service.Services.StudentManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class StudentService : IStudentService
    {
        private UnitOfWork _unitOfWork;
        public StudentService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// 获取所有学生信息
        /// </summary>
        /// <returns></returns>
        public IList<StudentDisplayModel> GetAll()
        {
            try
            {
                Repository<student> studentModuleDal = _unitOfWork.GetRepository<student>();
                IEnumerable<student> students = studentModuleDal.GetAll().Entities;
                if (students != null)
                {
                    return BuildModelList(students.ToList());
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

        public IList<StudentDisplayModel> FindStudentsByClassId(int classId)
        {
            try
            {
                Repository<classes> classDal = _unitOfWork.GetRepository<classes>();
                classes cl = classDal.GetObjectByKey(classId).Entity;
                if (cl != null && cl.students != null && cl.students.Count >0)
                {
                    return BuildModelList(cl.students.ToList());
                }
                else
                {
                    return new List<StudentDisplayModel>();
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

        public IList<StudentDisplayModel> FindStudentsByScheduleId(int scheduleId)
        {
            try
            {
                Repository<class_schedule> scheduleDal = _unitOfWork.GetRepository<class_schedule>();
                class_schedule cs = scheduleDal.GetObjectByKey(scheduleId).Entity;
                if (cs != null && cs.classrecords != null && cs.classrecords.Count > 0)
                {
                    Repository<student> studentModuleDal = _unitOfWork.GetRepository<student>();
                    
                    List<student> ls = new List<student>();
                    foreach (class_record_detail crd in cs.classrecords.First().class_record_detail)
                    {
                        ls.Add(studentModuleDal.GetObjectByKey(crd.student_id).Entity);
                    }
                    return BuildModelList(ls);
                }
                else
                {
                    return new List<StudentDisplayModel>();
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
        /// 根据学员id获取编辑对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StudentEditModel GetStudentById(int id)
        {
            try
            {
                StudentEditModel studentEditModel = new StudentEditModel();
                Repository<student> studentDal = _unitOfWork.GetRepository<student>();
                student studentEntity = studentDal.GetObjectByKey(id).Entity;
                if (studentEntity != null)
                {
                    studentEditModel.InitEditModel(studentEntity);
                }
                return studentEditModel;
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
        /// 创建学员信息
        /// </summary>
        /// <param name="newClassEditModel">需要创建的学员信息</param>
        public StudentEditModel Add(StudentEditModel newStudentEditModel)
        {
            try
            {
                student studententity = new student();
                studententity.address = newStudentEditModel.Address;
                studententity.center_id = newStudentEditModel.CenterId;
                studententity.dads_name = newStudentEditModel.Dadsname;
                studententity.dads_phone = newStudentEditModel.Dadsphone;
                studententity.email = newStudentEditModel.Email;
                studententity.extra_info = newStudentEditModel.ExtraInfo;
                studententity.google_contacts_id = newStudentEditModel.GoogleContactsId;
                studententity.grade = newStudentEditModel.Grade;
                studententity.moms_name = newStudentEditModel.Momsname;
                studententity.moms_phone = newStudentEditModel.Momsphone;
                studententity.original_class = newStudentEditModel.OriginalClass;
                studententity.relationship = newStudentEditModel.RelationShip;
                studententity.remaining_balance = newStudentEditModel.RemainingBalance;
                studententity.rfid_tag = newStudentEditModel.RfidTag;
                studententity.school = newStudentEditModel.School;
                studententity.students_birthdate = newStudentEditModel.Birthdate;
                studententity.students_name = newStudentEditModel.Name;
                studententity.students_nickname = newStudentEditModel.Nickname;
                studententity.status = newStudentEditModel.StatusId;

                if (newStudentEditModel.ConsultantId != 0)
                {
                    Repository<consultant> consultantModuleDal = _unitOfWork.GetRepository<consultant>();
                    consultant consultantEntity = consultantModuleDal.GetObjectByKey(newStudentEditModel.ConsultantId).Entity;
                    studententity.consultants.Add(consultantEntity);
                }
                if(newStudentEditModel.ClassesId != null)
                {
                    Repository<classes> classesDal = _unitOfWork.GetRepository<classes>();
                    foreach (int classid in newStudentEditModel.ClassesId)
                    {
                        classes cls = classesDal.GetObjectByKey(classid).Entity;
                        studententity.classess.Add(cls);
                    }
                }
                if(newStudentEditModel.ConsultantRate!=0)
                {
                    studententity.consultant_check_rate = newStudentEditModel.ConsultantRate;
                }
                
                _unitOfWork.AddAction(studententity, DataActions.Add);
                _unitOfWork.Save();
                newStudentEditModel.Id = studententity.student_id;

                return newStudentEditModel;
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
        /// 更新学员信息
        /// </summary>
        /// <param name="newClassEditModel">需要更新的学员信息</param>
        public StudentEditModel Update(StudentEditModel newStudentEditModel)
        {
            try
            {
                Repository<student> studentModuleDal = _unitOfWork.GetRepository<student>();
                student studententity = studentModuleDal.GetObjectByKey(newStudentEditModel.Id).Entity;
                if (studententity != null)
                {
                    studententity.address = newStudentEditModel.Address;
                    studententity.center_id = newStudentEditModel.CenterId;
                    studententity.dads_name = newStudentEditModel.Dadsname;
                    studententity.dads_phone = newStudentEditModel.Dadsphone;
                    studententity.email = newStudentEditModel.Email;
                    studententity.extra_info = newStudentEditModel.ExtraInfo;
                    studententity.google_contacts_id = newStudentEditModel.GoogleContactsId;
                    studententity.grade = newStudentEditModel.Grade;
                    studententity.moms_name = newStudentEditModel.Momsname;
                    studententity.moms_phone = newStudentEditModel.Momsphone;
                    studententity.original_class = newStudentEditModel.OriginalClass;
                    studententity.relationship = newStudentEditModel.RelationShip;
                    studententity.remaining_balance = newStudentEditModel.RemainingBalance;
                    studententity.rfid_tag = newStudentEditModel.RfidTag;
                    studententity.school = newStudentEditModel.School;
                    studententity.students_birthdate = newStudentEditModel.Birthdate;
                    studententity.students_name = newStudentEditModel.Name;
                    studententity.students_nickname = newStudentEditModel.Nickname;
                    studententity.student_id = newStudentEditModel.Id;
                    studententity.status = newStudentEditModel.StatusId;

                    if(studententity.consultants.Count>0)
                    {
                        studententity.consultants.Clear();
                    }

                    if (newStudentEditModel.ConsultantId != 0)
                    {
                        Repository<consultant> consultantModuleDal = _unitOfWork.GetRepository<consultant>();
                        consultant consultantEntity = consultantModuleDal.GetObjectByKey(newStudentEditModel.ConsultantId).Entity;
                        studententity.consultants.Add(consultantEntity);
                    }

                    if (newStudentEditModel.ConsultantRate != 0)
                    {
                        studententity.consultant_check_rate = newStudentEditModel.ConsultantRate;
                    }

                    studententity.classess.Clear();
                    if (newStudentEditModel.ClassesId != null)
                    {
                        Repository<classes> classesDal = _unitOfWork.GetRepository<classes>();
                        foreach (int classid in newStudentEditModel.ClassesId)
                        {
                            classes cls = classesDal.GetObjectByKey(classid).Entity;
                            studententity.classess.Add(cls);
                        }
                    }
                }

                _unitOfWork.AddAction(studententity, DataActions.Update);
                _unitOfWork.Save();

                return newStudentEditModel;
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

        public bool Recharge(int id,int money,LoginModel loginModel)
        {
            bool res = true;
            try
            {
                Repository<student> studentModuleDal = _unitOfWork.GetRepository<student>();
                Repository<student_recharge_detail> rechargeDal = _unitOfWork.GetRepository<student_recharge_detail>();
                student studententity = studentModuleDal.GetObjectByKey(id).Entity;
                if (studententity != null)
                {
                    //更新
                    studententity.remaining_balance = studententity.remaining_balance + money;
                    _unitOfWork.AddAction(studententity, DataActions.Update);

                    //新增学员费用交易流水
                    student_recharge_detail srd = new student_recharge_detail();
                    srd.student_id = id;
                    srd.amount = money;
                    srd.inout_type = 1;
                    srd.incur_time = DateTime.Now;
                    srd.recharge_user = loginModel.UserCode;

                    _unitOfWork.AddAction(srd, DataActions.Add);                    
                    _unitOfWork.Save();
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("学员充值失败"), "该学员不存在，无法充值");
                }
                return res;
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
        /// 删除学员信息
        /// </summary>
        /// <param name="deleteUserModel">删除学员信息</param>
        public bool Delete(StudentEditModel deleteStudentEditModel)
        {
            try
            {
                return DeleteById(deleteStudentEditModel.Id);
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
        /// 根据学员编号删除学员信息
        /// </summary>
        /// <param name="userCode">学员编号</param>
        public bool DeleteById(int id)
        {
            bool res = true;
            try
            {
                Repository<student> studentModuleDal = _unitOfWork.GetRepository<student>();
                Repository<class_record_detail> crdDal = _unitOfWork.GetRepository<class_record_detail>();
                student studententity = studentModuleDal.GetObjectByKey(id).Entity;
                if (studententity != null)
                {
                    if (studententity.student_recharge_detail.Count > 0)
                    {
                        throw new FaultException<LCFault>(new LCFault("删除学员失败"), "该学员已经参与过充值、上课等内容，无法删除");
                    }
                    else
                    {
                        if(studententity.consultants != null && studententity.consultants.Count>0)
                        {
                            //如果该学员设置了会籍顾问，需要先删除会籍顾问
                            studententity.consultants.Clear();
                        }
                        if(studententity.classess != null && studententity.classess.Count > 0)
                        {
                            //如果给这个学员分配过班级，则先需要删除学员班级信息
                            studententity.classess.Clear();
                        }

                        //删除未签到的上课记录
                        IEnumerable<class_record_detail> crds = crdDal.Find(cr => cr.student_id == studententity.student_id).Entities;
                        foreach(class_record_detail crd in crds)
                        {
                            _unitOfWork.AddAction(crd, DataActions.Delete);
                        }

                        _unitOfWork.AddAction(studententity, DataActions.Delete);
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("删除学员失败"), "该学员不存在，无法删除");
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
        private IList<StudentDisplayModel> BuildModelList(List<student> students)
        {
            if (students == null)
            {
                return null;
            }
            else
            {
                IList<StudentDisplayModel> moduledisplays = new List<StudentDisplayModel>();
                foreach (student studentEntity in students)
                {
                    moduledisplays.Add(BuildModel(studentEntity));
                }
                return moduledisplays;
            }
        }

        private StudentDisplayModel BuildModel(student studentEntity)
        {
            if (studentEntity == null)
            {
                return null;
            }
            else
            {
                StudentDisplayModel studentModel = new StudentDisplayModel();
                studentModel.Address = studentEntity.address;
                studentModel.Birthdate = studentEntity.students_birthdate;
                studentModel.Dadsname = studentEntity.dads_name;
                studentModel.Dadsphone = studentEntity.dads_phone;
                studentModel.Email = studentEntity.email;
                studentModel.ExtraInfo = studentEntity.extra_info;
                studentModel.Grade = studentEntity.grade;
                studentModel.Id = studentEntity.student_id;
                studentModel.Momsname = studentEntity.moms_name;
                studentModel.Momsphone = studentEntity.moms_phone;
                studentModel.Name = studentEntity.students_name;
                studentModel.Nickname = studentEntity.students_nickname;
                studentModel.OriginalClass = studentEntity.original_class;
                studentModel.RemainingBalance = studentEntity.remaining_balance.HasValue ? studentEntity.remaining_balance.Value : 0;
                studentModel.School = studentEntity.school;
                studentModel.Status = studentEntity.status1.description;

                return studentModel;
            }
        }
        #endregion
    }
}
