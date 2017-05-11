using LC.Contracts.BaseManager;
using LC.DAL.Enums;
using LC.DAL.Interfaces;
using LC.DAL.Repository.Repositories;
using LC.DAL.UnitOfWork;
using LC.Model;
using LC.Model.Business;
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
    public class ConsultantService:IConsultantService
    {
        private UnitOfWork _unitOfWork;
        public ConsultantService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// 获得所有会籍顾问信息
        /// </summary>
        /// <returns></returns>
        public IList<ConsultantModel> GetAllConsultant()
        {
            try
            {
                Repository<consultant> consultantmoduleDal = _unitOfWork.GetRepository<consultant>();
                IEnumerable<consultant> consultants = consultantmoduleDal.GetAll().Entities;
                if (consultants != null)
                {
                    return BuildModelList(consultants.ToList());
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
        /// 创建会籍顾问信息
        /// </summary>
        /// <param name="newUserModel">需要创建的会籍顾问信息</param>
        public ConsultantModel Add(ConsultantModel newConsultantModel)
        {
            try
            {
                consultant consul = new consultant();

                consul.abbreviation = newConsultantModel.abbreviation;
                consul.name = newConsultantModel.Name;
                consul.commission_rate = newConsultantModel.CommissionRate;

                _unitOfWork.AddAction(consul, DataActions.Add);
                _unitOfWork.Save();

                newConsultantModel.Id = consul.consultant_id;

                return newConsultantModel;
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
        /// 更新会籍顾问信息
        /// </summary>
        /// <param name="newUserModel">需要更新的会籍顾问信息</param>
        public ConsultantModel Update(ConsultantModel newConsultantModel)
        {
            try
            {
                Repository<consultant> consultantDal = _unitOfWork.GetRepository<consultant>();
                consultant consul = consultantDal.GetObjectByKey(newConsultantModel.Id).Entity;
                if (consul != null)
                {
                    consul.abbreviation = newConsultantModel.abbreviation;
                    consul.name = newConsultantModel.Name;
                    consul.commission_rate = newConsultantModel.CommissionRate;
                }
                _unitOfWork.AddAction(consul, DataActions.Update);
                _unitOfWork.Save();

                return newConsultantModel;
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
        /// 删除会籍顾问信息
        /// </summary>
        /// <param name="deleteUserModel">删除会籍顾问信息</param>
        public bool Delete(ConsultantModel deleteConsultantModel)
        {
            try
            {
                return DeleteById(deleteConsultantModel.Id);
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
        /// 根据会籍顾问编号删除会籍顾问信息
        /// </summary>
        /// <param name="id">会籍顾问id</param>
        public bool DeleteById(int id)
        {
            bool res = true;
            try
            {
                Repository<consultant> consultantDal = _unitOfWork.GetRepository<consultant>();
                consultant consul = consultantDal.GetObjectByKey(id).Entity;
                if (consul != null)
                {
                    if (consul.consultant_check_record.Count > 0)
                    {
                        throw new FaultException<LCFault>(new LCFault("会籍顾问删除失败"), "该会籍顾问已存在结算信息，无法删除");
                    }
                    else if(consul.centers.Count > 0)
                    {
                        throw new FaultException<LCFault>(new LCFault("会籍顾问删除失败"), "该会籍顾问目前还存在需要管理的中心，无法删除");
                    }
                    else
                    {
                        _unitOfWork.AddAction(consul, DataActions.Delete);
                        _unitOfWork.Save();
                    }
                }
                else
                {
                    res = false;
                    throw new FaultException<LCFault>(new LCFault("会籍顾问删除失败"), "该会籍顾问不存在，无法删除");
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

        public IList<ConsultantCheckModel> GetCheckMonthMoney(int id)
        {
            IList<ConsultantCheckModel> consultantCheckModelList = new List<ConsultantCheckModel>();
            try
            {
                //根据助教id获取所有未结算上课记录,并计算所有课时需要结算金额总和
                Repository<class_record_detail> recordDal = _unitOfWork.GetRepository<class_record_detail>();
                IEnumerable<class_record_detail> records = recordDal.Find(r => r.consultants_id == id && r.is_checked == false && r.attendance_status == 1006).Entities.OrderBy(cr => cr.class_record.class_schedule.real_date);
                string month = "";
                foreach (class_record_detail cr in records)
                {
                    if (!month.Equals(cr.class_record.class_schedule.real_date.ToString("yyyyMM")))
                    {
                        //当前处理月份为新月份，需要增加对象
                        ConsultantCheckModel ccm = new ConsultantCheckModel();
                        ccm.month = cr.class_record.class_schedule.real_date.ToString("yyyyMM");
                        ccm.id = id;
                        ccm.studentNum = 0;
                        consultantCheckModelList.Add(ccm);
                        month = ccm.month;
                    }
                    consultantCheckModelList.Last().money += (cr.class_record.actual_amount.Value / cr.class_record.student_number) * cr.consultant_check_rate.Value;
                    consultantCheckModelList.Last().studentNum += 1;
                }
                
                return consultantCheckModelList;
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
        public bool CheckMonthMoney(int id, string month, decimal money, LoginModel loginModel)
        {
            bool res = true;
            try
            {
                //根据会籍顾问id获取所有未结算上课记录,并计算所有课时需要结算金额总和
                Repository<class_record_detail> recordDal = _unitOfWork.GetRepository<class_record_detail>();
                IEnumerable<class_record_detail> records = recordDal.Find(r => r.consultants_id == id && r.is_checked == false && r.attendance_status == 1006 && r.class_record.class_schedule.real_date.ToString("yyyyMM").Equals(month)).Entities;

                //新增会籍顾问结算信息
                consultant_check_record ccr = new consultant_check_record();
                ccr.check_time = DateTime.Now;
                ccr.check_user = loginModel.UserCode;
                ccr.consultant_id = id;
                ccr.total_money = money;
                ccr.check_month = month;
                _unitOfWork.AddAction(ccr, DataActions.Add);

                records.ToList().ForEach(r=> { r.consultant_check_record_id = ccr.consultant_check_record_id; r.is_checked = true; });
                

                //更新学生出勤记录对应结算单信息
                //foreach (class_record_detail cr in records)
                //{
                //    cr.consultant_check_record = ccr;
                //    cr.is_checked = true;
                //    _unitOfWork.AddAction(cr, DataActions.Update);
                //}

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

        public IList<ConsultantRecordDetailModel> FindConsultantRecordDetailByDate(DateTime startDate, DateTime endDate, int consultantId)
        {
            IList<ConsultantRecordDetailModel> consultantRecordDetailList = new List<ConsultantRecordDetailModel>();
            try
            {
                System.Data.SqlClient.SqlParameter[] parameters = {
                                                                      new System.Data.SqlClient.SqlParameter("@startDate",startDate),
                                                                      new System.Data.SqlClient.SqlParameter("@endDate",endDate),
                                                                      new System.Data.SqlClient.SqlParameter("@consultantId",consultantId)};
                consultantRecordDetailList = this._unitOfWork.Context.Database.SqlQuery<ConsultantRecordDetailModel>("exec ConsultantReportByDate @consultantId,@startDate,@endDate", parameters).ToList();
                return consultantRecordDetailList;
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
        private IList<ConsultantModel> BuildModelList(List<consultant> consultants)
        {
            if (consultants == null)
            {
                return null;
            }
            else
            {
                IList<ConsultantModel> moduledisplays = new List<ConsultantModel>();
                foreach (consultant consul in consultants)
                {
                    moduledisplays.Add(BuildModel(consul));
                }
                return moduledisplays;
            }
        }

        private ConsultantModel BuildModel(consultant consultantModule)
        {
            if (consultantModule == null)
            {
                return null;
            }
            else
            {
                ConsultantModel consultantmodel = new ConsultantModel();
                consultantmodel.Id = consultantModule.consultant_id;
                consultantmodel.Name = consultantModule.name;
                consultantmodel.abbreviation = consultantModule.abbreviation;
                consultantmodel.CommissionRate = consultantModule.commission_rate;

                return consultantmodel;
            }
        }
        #endregion
    }
}
