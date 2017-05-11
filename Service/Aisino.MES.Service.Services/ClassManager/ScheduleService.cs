using LC.Contracts.ClassManager;
using LC.DAL.Enums;
using LC.DAL.Interfaces;
using LC.DAL.Repository.Repositories;
using LC.DAL.UnitOfWork;
using LC.Model;
using LC.Model.Business;
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
    public class ScheduleService:IScheduleService
    {
        private UnitOfWork _unitOfWork;
        public ScheduleService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 根据月份查找相应的排课计划
        /// </summary>
        /// <returns></returns>
        public IList<ScheduleDisplayModel> FindScheduleByMonth(string month)
        {
            try
            {
                Repository<class_schedule> scheduleModuleDal = _unitOfWork.GetRepository<class_schedule>();
                IEnumerable<class_schedule> schedules = scheduleModuleDal.Find(s => s.real_date.ToString("yyyyMM").Equals(month) || s.real_date.AddMonths(1).ToString("yyyyMM").Equals(month) || s.real_date.AddMonths(-1).ToString("yyyyMM").Equals(month)).Entities;
                if (schedules != null)
                {
                    return BuildModelList(schedules.ToList());
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

        public IList<ScheduleDisplayModel> FindScheduleByMonthAndTeacher(string month,int teacherId)
        {
            try
            {
                Repository<class_schedule> scheduleModuleDal = _unitOfWork.GetRepository<class_schedule>();
                IEnumerable<class_schedule> schedules = scheduleModuleDal.Find(s =>s.teacher_id == teacherId && (s.real_date.ToString("yyyyMM").Equals(month) || s.real_date.AddMonths(1).ToString("yyyyMM").Equals(month) || s.real_date.AddMonths(-1).ToString("yyyyMM").Equals(month))).Entities;
                if (schedules != null)
                {
                    return BuildModelList(schedules.ToList());
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
        /// 根据排课计划id获取编辑对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ScheduleEditModel GetScheduleById(int id)
        {
            try
            {
                ScheduleEditModel scheduleEditModel = new ScheduleEditModel();
                Repository<class_schedule> scheduleDal = _unitOfWork.GetRepository<class_schedule>();
                class_schedule scheduleEntity = scheduleDal.GetObjectByKey(id).Entity;
                if (scheduleEntity != null)
                {
                    Repository<status> statusDal = _unitOfWork.GetRepository<status>();
                    IEnumerable<status> attendanceStatus = statusDal.Find(s => s.cat == "attendance").Entities;
                    int attendedStatus = attendanceStatus.Single(s => s.description == "Attended").id;
                    scheduleEditModel.InitEditModel(scheduleEntity, attendedStatus);
                }
                return scheduleEditModel;
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

        public ScheduleEditModel Add(ScheduleEditModel newScheduleEditModel)
        {
            try
            {
                class_schedule scheduleentity = new class_schedule();

                scheduleentity.class_id = newScheduleEditModel.ClassId;
                scheduleentity.classroom_id = newScheduleEditModel.ClassroomId;
                scheduleentity.real_date = newScheduleEditModel.RealDate;
                scheduleentity.end_date = newScheduleEditModel.EndTime;
                scheduleentity.start_time = newScheduleEditModel.StartTime;
                scheduleentity.status = newScheduleEditModel.Status;
                scheduleentity.teacher_id = newScheduleEditModel.TeacherId;
                scheduleentity.lesson_schemas_text = newScheduleEditModel.SchemasText;
                scheduleentity.note = newScheduleEditModel.Note;
                if(newScheduleEditModel.AssistantId != 0)
                {
                    scheduleentity.assistant_id = newScheduleEditModel.AssistantId;
                }

                _unitOfWork.AddAction(scheduleentity, DataActions.Add);
                _unitOfWork.Save();

                Repository<status> statusDal = _unitOfWork.GetRepository<status>();
                newScheduleEditModel.StatusDes = statusDal.GetObjectByKey(scheduleentity.status).Entity.description;
                newScheduleEditModel.ScheduleId = scheduleentity.schedule_id;

                return newScheduleEditModel;
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

        public ScheduleEditModel Update(ScheduleEditModel newScheduleEditModel)
        {
            try
            {
                Repository<class_schedule> scheduleDal = _unitOfWork.GetRepository<class_schedule>();
                class_schedule scheduleentity = scheduleDal.GetObjectByKey(newScheduleEditModel.ScheduleId).Entity;
                if (scheduleentity != null)
                {
                    scheduleentity.classroom_id = newScheduleEditModel.ClassroomId;
                    scheduleentity.real_date = newScheduleEditModel.RealDate;
                    scheduleentity.end_date = newScheduleEditModel.EndTime;
                    scheduleentity.start_time = newScheduleEditModel.StartTime;
                    scheduleentity.status = newScheduleEditModel.Status;
                    scheduleentity.teacher_id = newScheduleEditModel.TeacherId;
                    scheduleentity.lesson_schemas_text = newScheduleEditModel.LessonName;
                    scheduleentity.note = newScheduleEditModel.Note;
                    if (newScheduleEditModel.AssistantId != 0)
                    {
                        scheduleentity.assistant_id = newScheduleEditModel.AssistantId;
                    }
                }

                _unitOfWork.AddAction(scheduleentity, DataActions.Update);
                _unitOfWork.Save();

                Repository<status> statusDal = _unitOfWork.GetRepository<status>();
                newScheduleEditModel.StatusDes = statusDal.GetObjectByKey(scheduleentity.status).Entity.description;

                return newScheduleEditModel;
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

        public bool CheckStudent(int scheduleId, List<int> studentIds, LoginModel loginModel)
        {
            bool res = true;
            try
            {                
                //获取学生签到状态
                Repository<status> statusDal = _unitOfWork.GetRepository<status>();
                IEnumerable<status> attendanceStatus = statusDal.Find(s => s.cat == "attendance").Entities;
                int attendedStatus = attendanceStatus.Single(s => s.description == "Attended").id;
                int excusedStatus = attendanceStatus.Single(s => s.description == "Excused absence").id;
                //获取排课信息
                Repository<class_schedule> scheduleDal = _unitOfWork.GetRepository<class_schedule>();
                class_schedule scheduleentity = scheduleDal.GetObjectByKey(scheduleId).Entity;
                int price = scheduleentity.wclass.parentClassTypes.unit_price;
                classes wclass = scheduleentity.wclass;

                if (scheduleentity != null)
                {
                    if(scheduleentity.classrecords == null || scheduleentity.classrecords.Count == 0)
                    {
                        //尚未有上课记录，说明还没有上课学生信息
                        Repository<class_record> recordDal = _unitOfWork.GetRepository<class_record>();
                        class_record cr = new class_record();
                        cr.schedule_id = scheduleId;
                        //设置学生下限计算值，用于教师提成金额核算
                        cr.student_limit = wclass.parentClassTypes.student_limit;
                        //设置教师信息和教师提成比例
                        cr.teacher_id = scheduleentity.teacher_id;
                        cr.teacher_check_rate = wclass.parentClassTypes.commission_rate_teacher;
                        //设置学生实际点到数量
                        cr.student_number = studentIds.Count;
                        cr.is_checked = 0;
                        //如果有助教设置，则设置助教信息和助教提成比例
                        if(scheduleentity.assistant_id.HasValue && scheduleentity.assistant_id.Value != 0)
                        {
                            cr.assistant_id = scheduleentity.assistant_id.Value;
                            cr.assistant_check_rate = wclass.parentClassTypes.commission_rate_assistant;
                            cr.assistant_is_checked = 0;
                        }
                        //设置实际收费综合，根据实际到课人数计算
                        cr.actual_amount = studentIds.Count * price;
                        //设置提成金额基数，如果实际到课人数低于计算下限人数，则按照下限人数计算
                        if (studentIds.Count < cr.student_limit)
                        {
                            cr.amount_receivable = cr.student_limit * price;
                        }
                        else
                        {
                            cr.amount_receivable = studentIds.Count * price;
                        }                        

                        foreach(student st in scheduleentity.wclass.students)
                        {
                            //班级所有学员建立上课明细，默认为没有参加课程
                            class_record_detail crd = new class_record_detail();
                            //上课明细记录中学生点到状态设置为没有参加
                            crd.attendance_status = excusedStatus;
                            //设置该学员会籍顾问提成比例，如果有会籍顾问，则设置会籍顾问相关信息
                            if (st.consultant_check_rate == 0)
                                crd.consultant_check_rate = wclass.parentClassTypes.commission_rate_consultant;
                            else
                                crd.consultant_check_rate = st.consultant_check_rate;
                            if (st.consultants.Count > 0)
                            {
                                crd.consultants_id = st.consultants.First().consultant_id;
                            }
                            crd.student_id = st.student_id;

                            if(studentIds.Contains(st.student_id))
                            {
                                //如果该学员在签到列表中，则需要更新状态为参加课程状态
                                crd.attendance_status = attendedStatus;
                                //学员上课，需要扣减学员剩余金额
                                st.remaining_balance = st.remaining_balance - price;
                                _unitOfWork.AddAction(st, DataActions.Update);

                                //更行学员充值消费记录流水信息
                                student_recharge_detail srd = new student_recharge_detail();
                                srd.student_id = st.student_id;
                                srd.amount = price;
                                srd.inout_type = -1;
                                srd.incur_time = DateTime.Now;
                                srd.recharge_user = loginModel.UserCode;
                                crd.student_recharge_detail.Add(srd);
                            }
                            cr.class_record_detail.Add(crd);
                        }
                        //该班级课程数量扣减一次，如果班级课程数量为0，则该班级进入锁定状态
                        wclass.last_count = wclass.last_count - 1;
                        if(wclass.last_count == 0)
                        {
                            wclass.is_active = false;
                        }
                        _unitOfWork.AddAction(wclass, DataActions.Update);
                        _unitOfWork.AddAction(cr, DataActions.Add);
                    }
                    else
                    {
                        Repository<student> studentDal = _unitOfWork.GetRepository<student>();
                        Repository<class_record_detail> classrecordDal = _unitOfWork.GetRepository<class_record_detail>();

                        //有上课信息，则需要更新上课学生信息
                        class_record cr = scheduleentity.classrecords.First();
                        cr.actual_amount = studentIds.Count * price;
                        cr.student_number = studentIds.Count;
                        if (cr.student_number < cr.student_limit)
                        {
                            //如果上课总数小于下限，则按照下限设定收费金额
                            cr.amount_receivable = cr.student_limit * price;
                        }
                        List<int> crdstids = new List<int>();
                        foreach (class_record_detail crd in cr.class_record_detail)
                        {
                            crdstids.Add(crd.student_id);
                            //处理所有已经在列表但是未check的学员
                            if (studentIds.Contains(crd.student_id) && crd.attendance_status != attendedStatus)
                            {
                                if (cr.student_number > cr.student_limit)
                                {
                                    //增加一个学员收费进入该堂课程总额
                                    cr.amount_receivable += price;
                                }
                                //签到学生原有明细记录为没有上课，标明是后来点到，则需要更新学员信息
                                student st = studentDal.GetObjectByKey(crd.student_id).Entity;
                                st.remaining_balance = st.remaining_balance - price;
                                _unitOfWork.AddAction(st, DataActions.Update);
                                class_record_detail crdd = classrecordDal.GetObjectByKey(crd.class_record_detail_id).Entity;
                                crdd.attendance_status = attendedStatus;
                                
                                //同时需要更新学员充值消费记录流水信息
                                student_recharge_detail srd = new student_recharge_detail();
                                srd.student_id = st.student_id;
                                srd.amount = price;
                                srd.inout_type = -1;
                                srd.incur_time = DateTime.Now;
                                srd.recharge_user = loginModel.UserCode;
                                crdd.student_recharge_detail.Add(srd);
                                _unitOfWork.AddAction(crdd, DataActions.Update);
                            }
                        }
                        //if (studentIds.Count > crdstids.Count)
                        //{
                            foreach (int studentId in studentIds)
                            {
                                if(!crdstids.Contains(studentId))
                                {
                                    //原理记录不包含学员id，则需要新增记录
                                    class_record_detail crd = new class_record_detail();
                                    
                                    crd.attendance_status = attendedStatus;

                                    student st = studentDal.GetObjectByKey(studentId).Entity;
                                    st.remaining_balance = st.remaining_balance - price;
                                    //设置该学员会籍顾问提成比例，如果有会籍顾问，则设置会籍顾问相关信息
                                    if (st.consultant_check_rate == 0)
                                        crd.consultant_check_rate = wclass.parentClassTypes.commission_rate_consultant;
                                    else
                                        crd.consultant_check_rate = st.consultant_check_rate;

                                    _unitOfWork.AddAction(st, DataActions.Update);

                                    //同时需要更新学员充值消费记录流水信息
                                    student_recharge_detail srd = new student_recharge_detail();
                                    srd.student_id = st.student_id;
                                    srd.amount = price;
                                    srd.inout_type = -1;
                                    srd.incur_time = DateTime.Now;
                                    srd.recharge_user = loginModel.UserCode;

                                    crd.student_recharge_detail.Add(srd);

                                    if (st.consultants.Count > 0)
                                    {
                                        crd.consultants_id = st.consultants.First().consultant_id;
                                    }
                                    crd.student_id = st.student_id;

                                    if (cr.student_number > cr.student_limit)
                                    {
                                        //增加一个学员收费进入该堂课程总额
                                        cr.amount_receivable += price;
                                    }

                                    cr.class_record_detail.Add(crd);
                                }
                            }
                        //}
                        _unitOfWork.AddAction(cr, DataActions.Update);
                    }

                    _unitOfWork.Save();
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

        public bool UnCheckStudent(int scheduleId, int studentId, LoginModel loginModel)
        {
            bool res = true;
            try
            {
                //获取学生签到状态
                Repository<status> statusDal = _unitOfWork.GetRepository<status>();
                IEnumerable<status> attendanceStatus = statusDal.Find(s => s.cat == "attendance").Entities;
                int attendedStatus = attendanceStatus.Single(s => s.description == "Attended").id;
                int excusedStatus = attendanceStatus.Single(s => s.description == "Excused absence").id;
                //获取排课信息
                Repository<class_schedule> scheduleDal = _unitOfWork.GetRepository<class_schedule>();
                class_schedule scheduleentity = scheduleDal.GetObjectByKey(scheduleId).Entity;
                int price = scheduleentity.wclass.parentClassTypes.unit_price;
                //获取学生信息
                Repository<student> studentDal = _unitOfWork.GetRepository<student>();
                Repository<student_recharge_detail> srdDal = _unitOfWork.GetRepository<student_recharge_detail>();
                student st = studentDal.GetObjectByKey(studentId).Entity;
                
                //学生剩余金额返还
                st.remaining_balance = st.remaining_balance + price;
                _unitOfWork.AddAction(st, DataActions.Update);
                //更新课程记录明细状态该学生未参加
                List<class_record_detail> crds = scheduleentity.classrecords.First().class_record_detail.ToList();
                int crdid = 0;
                foreach (class_record_detail crd in crds)
                {
                    if (crd.attendance_status == 1006 && crd.student_id == studentId)
                    {
                        crd.attendance_status = excusedStatus; 
                        crdid = crd.class_record_detail_id;
                        _unitOfWork.AddAction(crd, DataActions.Update);
                        break;
                    }
                }
                //课程记录减少一个实际参加学生人数
                class_record cr = scheduleentity.classrecords.First();
                //设置实际收费综合，根据实际到课人数计算
                cr.student_number = cr.student_number - 1;
                cr.actual_amount = cr.student_number * price;
                //设置提成金额基数，如果实际到课人数低于计算下限人数，则按照下限人数计算
                if (cr.student_number < cr.student_limit)
                {
                    cr.amount_receivable = cr.student_limit * price;
                }
                else
                {
                    cr.amount_receivable = cr.student_number * price;
                }
                _unitOfWork.AddAction(cr, DataActions.Update);
                
                //删除学生扣款记录
                //根据学生编号以及上课记录明细找到对应记录
                student_recharge_detail srd = srdDal.FindEntity(sr => sr.student_id == studentId && sr.class_record_detail_id == crdid).FirstOrDefault();
                _unitOfWork.AddAction(srd, DataActions.Delete);

                _unitOfWork.Save();
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

        public bool DeleteById(int id)
        {
            bool res = true;
            try
            {
                Repository<class_schedule> scheduleDal = _unitOfWork.GetRepository<class_schedule>();
                class_schedule scheduleEntity = scheduleDal.GetObjectByKey(id).Entity;
                if (scheduleEntity != null)
                {
                    if (scheduleEntity.classrecords.Count > 0 && scheduleEntity.classrecords.First().is_checked == 1)
                    {
                        throw new FaultException<LCFault>(new LCFault("删除课程失败"), "该课程已上课并结算，无法删除");
                    }
                    else
                    {
                        if (scheduleEntity.classrecords.Count > 0)
                        {
                            Repository<student> studentDal = _unitOfWork.GetRepository<student>();
                            Repository<student_recharge_detail> srdDal = _unitOfWork.GetRepository<student_recharge_detail>();
                            Repository<class_record_detail> crdDal = _unitOfWork.GetRepository<class_record_detail>();
                            List<class_record_detail> crds = crdDal.Find(cr => cr.class_record_id == scheduleEntity.classrecords.First().class_record_id).Entities.ToList();
                            //先将本来签到的学生金额回滚
                            foreach (class_record_detail crd in crds)
                            {
                                if (crd.attendance_status == 1006)
                                {
                                    //删除该学员扣款记录并回滚金额
                                    student_recharge_detail srd = srdDal.Single(ss => ss.class_record_detail_id == crd.class_record_detail_id).Entity;
                                    student s = studentDal.GetObjectByKey(crd.student_id).Entity;
                                    s.remaining_balance = s.remaining_balance + crd.class_record.class_schedule.wclass.parentClassTypes.unit_price;
                                    _unitOfWork.AddAction(srd, DataActions.Delete);
                                    _unitOfWork.AddAction(s, DataActions.Update);
                                }
                                //删除上课明细
                                _unitOfWork.AddAction(crd, DataActions.Delete);
                            }
                        }

                        _unitOfWork.AddAction(scheduleEntity, DataActions.Delete);
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("删除课程失败"), "该课程不存在，无法删除");
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
        private IList<ScheduleDisplayModel> BuildModelList(List<class_schedule> scheduleEntities)
        {
            if (scheduleEntities == null)
            {
                return null;
            }
            else
            {
                IList<ScheduleDisplayModel> scheduledisplays = new List<ScheduleDisplayModel>();
                foreach (class_schedule scheduleEntity in scheduleEntities)
                {
                    scheduledisplays.Add(BuildModel(scheduleEntity));
                }
                return scheduledisplays;
            }
        }

        private ScheduleDisplayModel BuildModel(class_schedule scheduleEntity)
        {
            if (scheduleEntity == null)
            {
                return null;
            }
            else
            {
                ScheduleDisplayModel schedule = new ScheduleDisplayModel();
                schedule.ClassName = scheduleEntity.wclass.class_name;
                schedule.ClassId = scheduleEntity.class_id;
                schedule.EndTime = scheduleEntity.end_date;
                schedule.Id = scheduleEntity.schedule_id;
                schedule.RealDate = scheduleEntity.real_date;
                schedule.StartTime = scheduleEntity.start_time;
                schedule.Status = scheduleEntity.schedulestatus.description;
                schedule.TeacherName = scheduleEntity.teacher.name;
                schedule.ClassroomName = scheduleEntity.classroom.classroom_name;
                schedule.SchemasText = scheduleEntity.lesson_schemas_text;
                schedule.Note = scheduleEntity.note;
                if (scheduleEntity.assistant != null)
                {
                    schedule.AssistantName = scheduleEntity.assistant.name;
                }

                return schedule;
            }
        }
        #endregion
    }
}
