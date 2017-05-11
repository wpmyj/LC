using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Service.ManuManager;
using Aisino.MES.Service.SysManager ;
using Aisino.MES.Service.BaseManager;
using Aisino.MES.DAL.Repository.Helpers;
using Aisino.MES.Service.StoreProcedures;
using Aisino.MES.Model.NewModels;

namespace Aisino.MES.Service.EnrolmentManager.Impl
{
    public class EnrolmentService : IEnrolmentService
    {
        private Repository<EnrolmentType> _enrolmentTypeDal;
        private Repository<Enrolment> _enrolmentDal;
        private Repository<EnrolmentRFIDReader> _enrolmentRFIDReaderDal;
        private UnitOfWork _unitOfWork;
        private IPlanTaskService _planTaskService;
        private ISysBillNoService _sysBillNoService;
        private IRFIDTagIssueService _rfidTagIssueService;
        SPGetSysDateTimeService _sPGetSysDateTimeService;

        public EnrolmentService(Repository<EnrolmentType> enrolmentTypeDal,
                                    Repository<Enrolment> enrolmentDal,
                                    Repository<EnrolmentRFIDReader> enrolmentRFIDReaderDal,
                                    UnitOfWork unitOfWork,
                                    IPlanTaskService planTaskService,
                                    ISysBillNoService sysBillNoService,
                                     IRFIDTagIssueService rfidTagIssueService,
            SPGetSysDateTimeService sPGetSysDateTimeService)
        {
            _enrolmentTypeDal = enrolmentTypeDal;
            _enrolmentDal = enrolmentDal;
            _enrolmentRFIDReaderDal = enrolmentRFIDReaderDal;
            _unitOfWork = unitOfWork;
            _planTaskService = planTaskService;
            _sysBillNoService = sysBillNoService;
            _rfidTagIssueService = rfidTagIssueService;
            _sPGetSysDateTimeService = sPGetSysDateTimeService;
        }
        #region EnrolmentType 服务

        public void AddEnrolmentType(EnrolmentType newEnrolmentType)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                if (newEnrolmentType.isdefault == true)
                {
                    EnrolmentType oldDefaultEnrolmentType = _enrolmentTypeDal.Single(t => t.isdefault == true).Entity;
                    if (oldDefaultEnrolmentType != null)
                    {
                        oldDefaultEnrolmentType.isdefault = false;
                        _unitOfWork.AddAction(oldDefaultEnrolmentType, DataActions.Update);
                    }
                }

                _unitOfWork.AddAction(newEnrolmentType, DataActions.Add);

                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void UpdEnrolmentType(EnrolmentType newEnrolmentType)
        {

            try
            {
                _unitOfWork.Actions.Clear();
                if (newEnrolmentType.isdefault == true)
                {
                    EnrolmentType oldDefaultEnrolmentType = _enrolmentTypeDal.Single(t => t.isdefault == true).Entity;
                    if (oldDefaultEnrolmentType != null)
                    {
                        oldDefaultEnrolmentType.isdefault = false;
                        _unitOfWork.AddAction(oldDefaultEnrolmentType, DataActions.Update);
                    }
                }

                _unitOfWork.AddAction(newEnrolmentType, DataActions.Update);

                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DelEnrolmentType(List<EnrolmentType> lstDelEnrolmentType)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                foreach (EnrolmentType delEnrolmentType in lstDelEnrolmentType)
                {

                    _unitOfWork.AddAction(delEnrolmentType, DataActions.Delete);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除设备信息失败！", ex);
            }
        }

        public IEnumerable<EnrolmentType> GetAllEnromentType()
        {
            return _enrolmentTypeDal.GetAll().Entities;
        }

        public IEnumerable<EnrolmentType> GetAllEnromentTypeBySQL(string sqlString)
        {
            return _enrolmentTypeDal.QueryByESql(sqlString).Entities;
        }

        public EnrolmentType GetDefaultEnrolmentType()
        {
            return _enrolmentTypeDal.Single(et => et.isdefault).Entity;
        }

        public EnrolmentType GetFastEnrolmentType()
        {
            return _enrolmentTypeDal.Single(et => et.faster.HasValue && et.faster == true).Entity;
        }

        public bool CheckEnrolmentTypeCode(string typeCode)
        {
            var rtEnromentType = _enrolmentTypeDal.Single(s => s.enrolment_type_code == typeCode);
            if (rtEnromentType.HasValue)
                return true;
            else
                return false;
        }

        public bool CheckEnrolmentTypeName(string typeName)
        {
            var rtEnrolmentType = _enrolmentTypeDal.Single(s => s.enrolment_type_name == typeName);
            if (rtEnrolmentType.HasValue)
                return true;
            else
                return false;
        }
        #endregion

        #region Enrolment服务
        public Enrolment AddEnrolment(Enrolment newEnrolment, EnrolmentType enrolmentType, string OrgDepCode, string userName, string mainId)
        {
            Enrolment rtEnrolment = null;
            try
            {
                newEnrolment.enrolment_number = _sysBillNoService.GetBillNo(enrolmentType.bill_number_id.Value, OrgDepCode);
                newEnrolment.enrolment_date = _sPGetSysDateTimeService.GetSysDateTime();
                newEnrolment.EnrolmentType = enrolmentType;
                if (newEnrolment.status == (int)EnrolmentStatue.确认)
                {
                    //如果确认状态，则可直接赋值confirmdate
                    newEnrolment.confirm_date = newEnrolment.enrolment_date;
                }
                _unitOfWork.AddAction(newEnrolment, DataActions.Add);
                if (mainId != "")
                {
                    //有标签发行
                    _rfidTagIssueService.AddRFIDTagIssueWithEnrolment(newEnrolment, mainId, userName);
                }

                if (newEnrolment.status != (int)EnrolmentStatue.新建)
                {
                    //新建状态不创建计划
                    _planTaskService.AddPlanTaskWithEnrolment(newEnrolment, enrolmentType.PlanTaskType, OrgDepCode);
                }

                _unitOfWork.Save();
                rtEnrolment = newEnrolment;
            }
            catch (DataMisalignedException ex)
            {
                throw ex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rtEnrolment;
        }

        public Enrolment UpdateEnrolment(Enrolment newEnrolment, string OrgDepCode, string userName, string mainId)
        {
            if (newEnrolment.status == (int)EnrolmentStatue.确认)
            {
                //如果确认状态，则可直接赋值confirmdate
                newEnrolment.confirm_date = newEnrolment.enrolment_date;
            }
            _unitOfWork.AddAction(newEnrolment, DataActions.Update);
            if (newEnrolment.RFIDTagIssues == null || newEnrolment.RFIDTagIssues.Count == 0)
            {
                if (mainId != "")
                {
                    //有标签发行
                    _rfidTagIssueService.AddRFIDTagIssueWithEnrolment(newEnrolment, mainId, userName);
                }
            }
            if (newEnrolment.status != (int)EnrolmentStatue.新建)
            {
                //新建状态不创建计划
                _planTaskService.AddPlanTaskWithEnrolment(newEnrolment, newEnrolment.EnrolmentType.PlanTaskType,OrgDepCode);
            }

            _unitOfWork.Save();
            return newEnrolment;
        }

        public Enrolment UpdateEnrolmentNew(Enrolment newEnrolment, string OrgDepCode, string userName, string mainId)
        {
            if (newEnrolment.status == (int)EnrolmentStatue.确认)
            {
                //如果确认状态，则可直接赋值confirmdate
                newEnrolment.confirm_date = newEnrolment.enrolment_date;
            }
            _unitOfWork.AddAction(newEnrolment, DataActions.Update);
            if (newEnrolment.RFIDTagIssues == null || newEnrolment.RFIDTagIssues.Count == 0)
            {
                if (mainId != "")
                {
                    //有标签发行
                    _rfidTagIssueService.AddRFIDTagIssueWithEnrolment(newEnrolment, mainId, userName);
                }
            }
            _unitOfWork.Save();
            return newEnrolment;
        }

        public Enrolment GetEnrolmentByRfidCode(string mainId)
        {
            return _enrolmentDal.Single(e => e.RFIDTagIssues.FirstOrDefault().RFIDTag.tag_main_id == mainId && e.RFIDTagIssues.FirstOrDefault().issue_status == (int)RFIDIssueStatus.正常).Entity;
        }

        public Enrolment UpdEnrolment(Enrolment newEnrolment)
        {
            Enrolment rtEnrolment = null;
            try
            {
                _enrolmentDal.Update(newEnrolment);
                rtEnrolment = newEnrolment;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rtEnrolment;
        }

        public Enrolment UpdEnrolmentwithunit(Enrolment newEnrolment, RFIDTagIssue newRFIDTagIssue)
        {
            Enrolment rtEnrolment = null;
            _unitOfWork.Actions.Clear();
            try
            {
                newRFIDTagIssue.enrolment = newEnrolment.enrolment_number;
                _unitOfWork.AddAction(newEnrolment, DataActions.Update);
                _rfidTagIssueService.AddRFIDTagIssue_Unitwork(newRFIDTagIssue);
                _unitOfWork.Save();
                rtEnrolment = newEnrolment;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rtEnrolment;
        }

        public void DelEnrolment(Enrolment delEnrolment)
        {
            try
            {
                _enrolmentDal.Delete(delEnrolment);
            }
            catch (RepositoryException ex)
            { throw ex; }
        }

        public IEnumerable<Enrolment> GetAllEnroment()
        {
            return _enrolmentDal.GetAll().Entities;
        }

        public IEnumerable<Enrolment> GetPageEnrolmentBySQL(string strwhere, PagingCriteria paging)
        {
            string esql = "select *  from  Enrolment where 1=1";
            esql += strwhere;

            return _enrolmentDal.QueryByESql(esql, paging).Entities;
        }

        public IEnumerable<Enrolment> GetEnrolmentByIDCard(string IDCard)
        {
            return _enrolmentDal.Find(el => el.carrier_id == IDCard || el.owner_id == IDCard).Entities;
        }

        public int GetEnrolmentCountByCondition(int status, string ownerName, string plateNumber, PagingCriteria paging)
        {
            var queryEnrolment = PredicateBuilder.True<Enrolment>();
            if (status > 0)
                queryEnrolment = queryEnrolment.And(e => e.status == status);
            if (ownerName != "")
                queryEnrolment = queryEnrolment.And(e => e.owner_name == ownerName);
            if (plateNumber != "")
                queryEnrolment = queryEnrolment.And(e => e.plate_number == plateNumber);

            return _enrolmentDal.Find(queryEnrolment, paging).PagedMetadata.TotalItemCount;
        }

        public RepositoryResultList<Enrolment> FindPageEnrolmentByCondition(int status, string ownerName, string plateNumber, PagingCriteria paging)
        {
            var queryEnrolment = PredicateBuilder.True<Enrolment>();
            if(status > 0)
                queryEnrolment = queryEnrolment.And(e => e.status == status);
            if (ownerName != "")
                queryEnrolment = queryEnrolment.And(e => e.owner_name.Contains(ownerName));
            if (plateNumber != "")
                queryEnrolment = queryEnrolment.And(e => e.plate_number.Contains(plateNumber));

            return _enrolmentDal.Find(queryEnrolment, paging);

        }

        public RepositoryResultList<Enrolment> FindPageEnrolmentByCondition(int status,int traffic_type, string ownerName, string plateNumber, PagingCriteria paging)
        {
            var queryEnrolment = PredicateBuilder.True<Enrolment>();
            if (status > 0)
                queryEnrolment = queryEnrolment.And(e => e.status == status);
            if (traffic_type >= 0)
            {
                //如果选择全部则传入-1
                if (traffic_type == 0)
                {
                    //所有外部车辆全部传入0
                    queryEnrolment = queryEnrolment.And(e=>e.EnrolmentBasicTypeTraffic.isoutcar);
                }
                else
                {
                    //非外部车辆，使用原始值
                    queryEnrolment = queryEnrolment.And(e => e.traffic_type == traffic_type);
                }
            }
            if (ownerName != "")
                queryEnrolment = queryEnrolment.And(e => e.owner_name.Contains(ownerName));
            if (plateNumber != "")
                queryEnrolment = queryEnrolment.And(e => e.plate_number.Contains(plateNumber));

            return _enrolmentDal.Find(queryEnrolment, paging);

        }

        public IEnumerable<Enrolment> GetCountEnrolmentBySQL(string strwhere)
        {
            string esql = "select *  from  Enrolment where 1=1";
            esql += strwhere;

            return _enrolmentDal.QueryByESql(esql).Entities;
        }
        //public IEnumerable<Enrolment> GetAllEnromentBySQL(string sqlString)
        //{
        //    return _enrolmentDal.QueryByESql(sqlString).Entities;
        //}

        public bool FinishAssay(Enrolment enrolment,ref string strErrorMessage)
        {            
            try
            {
                _unitOfWork.Actions.Clear();

                RFIDTagIssue rfidTagIssue = enrolment.RFIDTagIssues.FirstOrDefault();
                rfidTagIssue.issue_status = (int)RFIDIssueStatus.注销;
                _unitOfWork.AddAction(rfidTagIssue, DataActions.Update);

                enrolment.status = (int)EnrolmentStatue.完成;
                _unitOfWork.AddAction(enrolment, DataActions.Update);
                //_planTaskService.FinshPlanTask(enrolment.PlanTasks.LastOrDefault());
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                strErrorMessage = ex.Message;
                return false;
            }
            return true;
        }
        #endregion

        #region EnrolmentRfid 服务


        public void AddEnrolmentRfid(EnrolmentRFIDReader newEnrolmentRFIDReader)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                //非默认
                if (!newEnrolmentRFIDReader.enrolment_reader_default)
                {
                    _unitOfWork.AddAction(newEnrolmentRFIDReader, DataActions.Add);
                    _unitOfWork.Save();
                    return;
                }
                //默认
                string strSearchSql = "Select * from EnrolmentRFIDReader where enrolment_reader_phyip = '" + newEnrolmentRFIDReader.enrolment_reader_phyip+ "'";
                IList<EnrolmentRFIDReader> oldEnrolmentReader = _enrolmentRFIDReaderDal.QueryByESql(strSearchSql).Entities.ToList();

                if (oldEnrolmentReader != null || oldEnrolmentReader.Count > 0)
                {
                    foreach (EnrolmentRFIDReader srr in oldEnrolmentReader)
                    {
                        srr.enrolment_reader_default = false;
                        _unitOfWork.AddAction(srr, DataActions.Update);
                    }
                }
                _unitOfWork.AddAction(newEnrolmentRFIDReader, DataActions.Add);               
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
       }

        public void UdpEnrolmentRfid(EnrolmentRFIDReader udpEnrolmentRFIDReader)
        {
            try
            {
                if (udpEnrolmentRFIDReader.enrolment_reader_default)
                {
                    EnrolmentRFIDReader old = _enrolmentRFIDReaderDal.Single(r => r.enrolment_reader_phyip == udpEnrolmentRFIDReader.enrolment_reader_phyip && r.enrolment_reader_code != udpEnrolmentRFIDReader.enrolment_reader_code && r.enrolment_reader_default == true).Entity;
                    if (old != null)
                    {
                        old.enrolment_reader_default = false;
                        _unitOfWork.AddAction(old, DataActions.Update);
                    }
                }
                _unitOfWork.AddAction(udpEnrolmentRFIDReader, DataActions.Update);                         
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DelEnrolmentRfid(string strDelEnrolmentRifdReaderCodes)
        {
            try
            {
                _unitOfWork.Actions.Clear();

                string strSearchSql = "Select * from EnrolmentRFIDReader where 1 = 1 ";
                string[] strCodeList = strDelEnrolmentRifdReaderCodes.Split(',');
                for(int i = 0 ; i < strCodeList.Length ; i++)
                {
                    if(i == 0)
                    {
                        strSearchSql += " and (enrolment_reader_code = '"+strCodeList[i] +"' ";
                    }
                    else
                    {
                        strSearchSql += " or enrolment_reader_code = '"+strCodeList[i] +"' ";
                    }

                    if(i == strCodeList.Length - 1)
                    {
                        strSearchSql += " ) ";
                    }
                }
                IList<EnrolmentRFIDReader> oldEnrolmentReader = _enrolmentRFIDReaderDal.QueryByESql(strSearchSql).Entities.ToList();

                if (oldEnrolmentReader != null || oldEnrolmentReader.Count > 0)
                {
                    foreach (EnrolmentRFIDReader srr in oldEnrolmentReader)
                    {
                        srr.enrolment_reader_default = false;
                        _unitOfWork.AddAction(srr, DataActions.Delete);
                    }
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public IEnumerable<EnrolmentRFIDReader> GetAllEnrolmentRFIDReader()
        {
            return _enrolmentRFIDReaderDal.GetAll().Entities;
        }

        public IEnumerable<EnrolmentRFIDReader> GetEnrolmentRFIDReaderBySql(string sqlString)
        {
            return _enrolmentRFIDReaderDal.QueryByESql(sqlString).Entities;
        }

        public EnrolmentRFIDReader GetEnrolmentRFIDReaderDefault(string strMacAddress)
        {
            var rfidReaderDefault = _enrolmentRFIDReaderDal.Single(r => r.enrolment_reader_phyip == strMacAddress && r.enrolment_reader_default == true);
            if (rfidReaderDefault.HasValue)
            {
                return rfidReaderDefault.Entity;
            }
            else
            {
                return null;
            }
        }
        #endregion


        public IEnumerable<Enrolment> GetAllEnrolmentBySQL(string sqlString)
        {
            return _enrolmentDal.QueryByESql(sqlString).Entities;
        }


        public Enrolment GetEnrolmentByCode(string enrolmentCode)
        {
            var enrolment = _enrolmentDal.Single(d => d.enrolment_number == enrolmentCode);
            if (enrolment.HasValue)
            {
                return enrolment.Entity;
            }
            else
            {
                return null;
            }
        }

        public void RefreshData()
        {
            this._enrolmentDal.RefreshData();
            this._enrolmentRFIDReaderDal.RefreshData();
            this._enrolmentTypeDal.RefreshData();
        }




        public Enrolment UpenrolmentWithUnit(Enrolment upenrolment)
        {
            _unitOfWork.AddAction(upenrolment, DataActions.Update);
            return upenrolment;
        }


        public Enrolment GetEnrolmentByRFID(string main_id)
        {
            IEnumerable<Enrolment> reEnrolments = _enrolmentDal.Find(em => em.RFIDTagIssues.Any(rf => rf.RFIDTag.tag_main_id == main_id)
                , new string[] { "RFIDTagIssues.RFIDTag", "PlanTasks", "Contract", "PlanTasks.BusinessDisposes" }).Entities;
            if (reEnrolments.Count() == 0)
            {
                return null;
            }
            else
            {
                return reEnrolments.Last();
            }
        }


        public List<OwernerAndID> GetOwernerAndID()
        {
            List<OwernerAndID> owernerandids = new List<OwernerAndID>();
            List<Enrolment> enrolments = _enrolmentDal.GetAll().Entities.ToList();
            
            if (enrolments.Count > 0)
            {
                for (int i = 0; i < enrolments.Count; i++)
                {
                    OwernerAndID newowernerid = new OwernerAndID();
                    newowernerid.OwernerName = enrolments[i].owner_name;
                    newowernerid.OwernerId = enrolments[i].owner_id;
                    newowernerid.Address = enrolments[i].owner_address;
                    newowernerid.Mobile = enrolments[i].owner_mobile;
                    owernerandids.Add(newowernerid);
                }
            }
            else
            {
                owernerandids = null;
            }
            return owernerandids;
        }


        public List<CarryerAndID> GetCarryerAndID()
        {
            List<CarryerAndID> carryerandids = new List<CarryerAndID>();
            List<Enrolment> enrolments = _enrolmentDal.GetAll().Entities.ToList();

            if (enrolments.Count > 0)
            {
                for (int i = 0; i < enrolments.Count; i++)
                {
                    CarryerAndID newcarryid = new CarryerAndID();
                    newcarryid.CarryerName = enrolments[i].carrier_name;
                    newcarryid.CarryerId = enrolments[i].carrier_id;
                    carryerandids.Add(newcarryid);
                }

            }
            else
            {
                carryerandids = null;
            }
            return carryerandids;
        }

        public void UpdateEnrolmentAndPlantask(Enrolment upEnrolment, PlanTask upPlantask, Assay myAssay)
        {
            _unitOfWork.AddAction(myAssay, DataActions.Update);
            _unitOfWork.AddAction(upEnrolment, DataActions.Update);
            _unitOfWork.AddAction(upPlantask, DataActions.Update);
            _unitOfWork.Save();
        }

        public void UnitOfWorkActionClear()
        {
            _unitOfWork.Actions.Clear();            
        }

        public void UnitOfWorkActionSave()
        {
            _unitOfWork.Save();
        }
    }
}
