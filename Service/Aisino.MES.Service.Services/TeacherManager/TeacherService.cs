using LC.Contracts.TeacherManager;
using LC.DAL.Enums;
using LC.DAL.Interfaces;
using LC.DAL.Repository.Repositories;
using LC.DAL.UnitOfWork;
using LC.Model;
using LC.Model.Business;
using LC.Model.Business.TeacherModel;
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Service.Services.TeacherManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TeacherService :ITeacherService
    {
        private UnitOfWork _unitOfWork;
        public TeacherService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// 获得所有教师信息
        /// </summary>
        /// <returns></returns>
        public IList<TeacherModel> GetAllTeacher()
        {
            try
            {
                Repository<teacher> teachermoduleDal = _unitOfWork.GetRepository<teacher>();
                IEnumerable<teacher> teachers = teachermoduleDal.GetAll().Entities;
                if (teachers != null)
                {
                    return BuildModelList(teachers.ToList());
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

        public TeacherModel FindTeacherByUserCode(string userCode)
        {
            try
            {
                Repository<teacher> teachermoduleDal = _unitOfWork.GetRepository<teacher>();
                IEnumerable<teacher> te = teachermoduleDal.FindEntity(t => t.UserCode == userCode);
                if (te != null && te.Count()>0)
                {
                    return BuildModel(te.First());
                }
                else
                {
                    return new TeacherModel();
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
        /// 创建教师信息
        /// </summary>
        /// <param name="newUserModel">需要创建的教师信息</param>
        public TeacherModel Add(TeacherModel newTeacherModel)
        {
            try
            {
                teacher teacherEntity = new teacher();

                teacherEntity.mobile = newTeacherModel.Mobile;
                teacherEntity.name = newTeacherModel.Name;
                teacherEntity.status = newTeacherModel.StatusId;
                teacherEntity.UserCode = newTeacherModel.UserCode;

                _unitOfWork.AddAction(teacherEntity, DataActions.Add);
                _unitOfWork.Save();
                newTeacherModel.Id = teacherEntity.teacher_id;

                return newTeacherModel;
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
        /// 更新教师信息
        /// </summary>
        /// <param name="newUserModel">需要更新的教师信息</param>
        public TeacherModel Update(TeacherModel newTeacherModel)
        {
            try
            {
                Repository<teacher> teachermoduleDal = _unitOfWork.GetRepository<teacher>();
                teacher teacherEntity = teachermoduleDal.GetObjectByKey(newTeacherModel.Id).Entity;
                if (teacherEntity != null)
                {
                    teacherEntity.mobile = newTeacherModel.Mobile;
                    teacherEntity.name = newTeacherModel.Name;
                    teacherEntity.status = newTeacherModel.StatusId;
                    teacherEntity.UserCode = newTeacherModel.UserCode;
                }
                _unitOfWork.AddAction(teacherEntity, DataActions.Update);
                _unitOfWork.Save();

                return newTeacherModel;
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

        public decimal GetCheckMoney(int id)
        {
            decimal money = 0;
            try
            {
                //根据教师id获取所有未结算上课记录,并计算所有课时需要结算金额总和
                Repository<class_record> recordDal = _unitOfWork.GetRepository<class_record>();
                IEnumerable<class_record> records = recordDal.Find(r => r.teacher_id == id && r.is_checked == 0).Entities;
                IEnumerable<class_record> assistantRecords = recordDal.Find(r => r.assistant_id == id && r.assistant_is_checked == 0).Entities;

                foreach (class_record cr in records)
                {
                    money += cr.amount_receivable.Value * cr.teacher_check_rate;
                }

                foreach (class_record cr in assistantRecords)
                {
                    money += cr.amount_receivable.Value * cr.assistant_check_rate.Value;
                }

                return money;
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

        public IList<TeacherCheckModel> GetCheckMonthMoney(int id)
        {
            IList<TeacherCheckModel> teacherCheckModelList = new List<TeacherCheckModel>();
            try
            {
                //根据教师id获取所有未结算上课记录,并计算所有课时需要结算金额总和
                Repository<class_record> recordDal = _unitOfWork.GetRepository<class_record>();
                IEnumerable<class_record> records = recordDal.Find(r => r.teacher_id == id && r.is_checked == 0).Entities.OrderBy(cr=>cr.class_schedule.real_date);
                IEnumerable<class_record> assistantRecords = recordDal.Find(r => r.assistant_id == id && r.assistant_is_checked == 0).Entities.OrderBy(cr => cr.class_schedule.real_date);
                string month = "";
                foreach (class_record cr in records)
                {
                    if (!month.Equals(cr.class_schedule.real_date.ToString("yyyyMM")))
                    {
                        //当前处理月份为新月份，需要增加对象
                        TeacherCheckModel tcm = new TeacherCheckModel();
                        tcm.month = cr.class_schedule.real_date.ToString("yyyyMM");
                        teacherCheckModelList.Add(tcm);
                        month = cr.class_schedule.real_date.ToString("yyyyMM");
                    }
                    teacherCheckModelList.Last().money += cr.amount_receivable.Value * cr.teacher_check_rate;
                }

                foreach (class_record cr in assistantRecords)
                {
                    bool hascheckmodel = false;
                    for(int i = 0;i < teacherCheckModelList.Count;i++)
                    {
                        if(teacherCheckModelList[i].month.Equals(cr.class_schedule.real_date.ToString("yyyyMM")))
                        {
                            hascheckmodel = true;
                            teacherCheckModelList[i].money += cr.amount_receivable.Value * cr.assistant_check_rate.Value;
                            break;
                        }
                    }
                    if(hascheckmodel==false)
                    {
                        //该月没有获得数据，则需要新增
                        TeacherCheckModel tcm = new TeacherCheckModel();
                        tcm.month = cr.class_schedule.real_date.ToString("yyyyMM");
                        tcm.money += cr.amount_receivable.Value * cr.assistant_check_rate.Value;
                        teacherCheckModelList.Add(tcm);
                    }
                }

                return teacherCheckModelList;
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

        public IList<TeacherClassRecordDetailModel> FindTeacherClassRecordDetailByDate(DateTime startDate, DateTime endDate, int teacherId)
        {
            IList<TeacherClassRecordDetailModel> teacherClassRecordDetailList = new List<TeacherClassRecordDetailModel>();
            try
            {
                System.Data.SqlClient.SqlParameter[] parameters = { 
                                                                      new System.Data.SqlClient.SqlParameter("@startDate",startDate),
                                                                      new System.Data.SqlClient.SqlParameter("@endDate",endDate),
                                                                      new System.Data.SqlClient.SqlParameter("@teacherId",teacherId)};
                teacherClassRecordDetailList = this._unitOfWork.Context.Database.SqlQuery<TeacherClassRecordDetailModel>("exec TeacherLessonsReportByDate @teacherId,@startDate,@endDate", parameters).ToList();
                return teacherClassRecordDetailList;
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

        public bool CheckMoney(int id,decimal money, LoginModel loginModel)
        {
            bool res = true;
            try
            {
                //根据教师id获取所有未结算上课记录,并计算所有课时需要结算金额总和
                Repository<class_record> recordDal = _unitOfWork.GetRepository<class_record>();
                IEnumerable<class_record> records = recordDal.Find(r => r.teacher_id == id && r.is_checked == 0).Entities;
                IEnumerable<class_record> assistantRecords = recordDal.Find(r => r.assistant_id == id && r.assistant_is_checked == 0).Entities;
                
                //新增教师结算信息
                teachers_check_record tcr = new teachers_check_record();
                tcr.check_time = DateTime.Now;
                tcr.check_user = loginModel.UserCode;
                tcr.teacher_id = id;
                tcr.total_money = money;
                _unitOfWork.AddAction(tcr, DataActions.Add);

                //更新上课记录对应结算单信息
                foreach (class_record cr in records)
                {
                    cr.teacher_check_record = tcr;
                    cr.is_checked = 1;
                    _unitOfWork.AddAction(cr, DataActions.Update);
                }
                foreach(class_record cr in assistantRecords)
                {
                    cr.assistant_check_record = tcr;
                    cr.assistant_is_checked = 1;
                    _unitOfWork.AddAction(cr, DataActions.Update);
                }

                _unitOfWork.Save();

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

        public bool CheckMonthMoney(int id,string month, decimal money, LoginModel loginModel)
        {
            bool res = true;
            try
            {
                //根据教师id获取所有未结算上课记录,并计算所有课时需要结算金额总和
                Repository<class_record> recordDal = _unitOfWork.GetRepository<class_record>();
                IEnumerable<class_record> records = recordDal.Find(r => r.teacher_id == id && r.is_checked == 0 && r.class_schedule.real_date.ToString("yyyyMM").Equals(month)).Entities;
                IEnumerable<class_record> assistantRecords = recordDal.Find(r => r.assistant_id == id && r.assistant_is_checked == 0 && r.class_schedule.real_date.ToString("yyyyMM").Equals(month)).Entities;

                //新增教师结算信息
                teachers_check_record tcr = new teachers_check_record();
                tcr.check_time = DateTime.Now;
                tcr.check_user = loginModel.UserCode;
                tcr.teacher_id = id;
                tcr.total_money = money;
                tcr.check_month = month;
                _unitOfWork.AddAction(tcr, DataActions.Add);

                //更新上课记录对应结算单信息
                foreach (class_record cr in records)
                {
                    cr.teacher_check_record = tcr;
                    cr.is_checked = 1;
                    _unitOfWork.AddAction(cr, DataActions.Update);
                }
                foreach (class_record cr in assistantRecords)
                {
                    cr.assistant_check_record = tcr;
                    cr.assistant_is_checked = 1;
                    _unitOfWork.AddAction(cr, DataActions.Update);
                }

                _unitOfWork.Save();

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
        /// 删除教师信息
        /// </summary>
        /// <param name="deleteUserModel">删除教师信息</param>
        public bool Delete(TeacherModel deleteTeacherModel)
        {
            try
            {
                return DeleteById(deleteTeacherModel.Id);
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
        /// 根据教师编号删除教学中心信息
        /// </summary>
        /// <param name="userCode">教师编号</param>
        public bool DeleteById(int id)
        {
            bool res = true;
            try
            {
                Repository<teacher> teachermoduleDal = _unitOfWork.GetRepository<teacher>();
                teacher teacherEntity = teachermoduleDal.GetObjectByKey(id).Entity;
                if (teacherEntity != null)
                {
                    if (teacherEntity.class_schedule.Count > 0 || teacherEntity.class_schedule1.Count > 0 || teacherEntity.teachers_check_record.Count > 0)
                    {
                        throw new FaultException<LCFault>(new LCFault("删除教师信息失败"), "该教师已有上课记录或结算记录，无法删除");
                    }
                    else
                    {
                        _unitOfWork.AddAction(teacherEntity, DataActions.Delete);
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("删除教师信息失败"), "该教师不存在，无法删除");
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
        private IList<TeacherModel> BuildModelList(List<teacher> teachers)
        {
            if (teachers == null)
            {
                return null;
            }
            else
            {
                IList<TeacherModel> moduledisplays = new List<TeacherModel>();
                foreach (teacher teachermodule in teachers)
                {
                    moduledisplays.Add(BuildModel(teachermodule));
                }
                return moduledisplays;
            }
        }

        private TeacherModel BuildModel(teacher teacherEntity)
        {
            if (teacherEntity == null)
            {
                return null;
            }
            else
            {
                TeacherModel teachermodel = new TeacherModel();
                teachermodel.Id = teacherEntity.teacher_id;
                teachermodel.Name = teacherEntity.name;
                teachermodel.Mobile = teacherEntity.mobile;
                teachermodel.StatusId = teacherEntity.status ;
                teachermodel.Status = teacherEntity.status1.description;
                teachermodel.UserCode = teacherEntity.UserCode;

                return teachermodel;
            }
        }
        #endregion
    }
}
