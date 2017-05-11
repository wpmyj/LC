using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Helpers;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.NewModels;


namespace Aisino.MES.Service.EnrolmentManager
{
    public interface IEnrolmentService
    {
        #region EnrolmentType 服务
        void AddEnrolmentType(EnrolmentType newEnrolmentType);
        void UpdEnrolmentType(EnrolmentType newEnrolmentType);
        void DelEnrolmentType(List<EnrolmentType> lstDelEnrolmentType);
        IEnumerable<EnrolmentType> GetAllEnromentType();
        IEnumerable<EnrolmentType> GetAllEnromentTypeBySQL(string sqlString);
        /// <summary>
        /// 获取默认报港单类别
        /// </summary>
        /// <returns>默认的报港单类别</returns>
        EnrolmentType GetDefaultEnrolmentType();
        EnrolmentType GetFastEnrolmentType();

        bool CheckEnrolmentTypeCode(string typeCode);
        bool CheckEnrolmentTypeName(string typeName);
        #endregion

        #region Enrolment   服务
        /// <summary>
        /// 创建报港单
        /// </summary>
        /// <param name="newEnrolment">需要新增的报港单</param>
        /// <param name="enrolmentType">报港单类型</param>
        /// <param name="OrgDepCode">所属机构代码</param>
        /// <param name="userName">当前操作用户名称</param>
        /// <param name="mainId">主标签id</param>
        /// <returns>新增完成的报港单</returns>
        Enrolment AddEnrolment(Enrolment newEnrolment, EnrolmentType enrolmentType, string OrgDepCode,string userName, string mainId);
        /// <summary>
        /// 更新报港单
        /// </summary>
        /// <param name="newEnrolment">需要更新的报港单</param>
        /// <param name="OrgDepCode">所属机构代码</param>
        /// <param name="userName">当前操作用户名称</param>
        /// <param name="mainId">主标签id</param>
        /// <returns>更新完成的报港单</returns>
        Enrolment UpdateEnrolment(Enrolment newEnrolment, string OrgDepCode, string userName, string mainId);
        Enrolment UpdateEnrolmentNew(Enrolment newEnrolment, string OrgDepCode, string userName, string mainId);
        Enrolment GetEnrolmentByRfidCode(string mainId);
        Enrolment UpdEnrolment(Enrolment newEnrolment);
        void DelEnrolment(Enrolment delEnrolment);
        IEnumerable<Enrolment> GetAllEnroment();
        IEnumerable<Enrolment> GetAllEnrolmentBySQL(string sqlString);
        Enrolment GetEnrolmentByCode(string enrolmentCode);
        Enrolment UpdEnrolmentwithunit(Enrolment newEnrolment, RFIDTagIssue newRFIDTagIssue);
        IEnumerable<Enrolment> GetPageEnrolmentBySQL(string strwhere, PagingCriteria paging);
        IEnumerable<Enrolment> GetCountEnrolmentBySQL(string strwhere);
        IEnumerable<Enrolment> GetEnrolmentByIDCard(string IDCard);
        Enrolment GetEnrolmentByRFID(string main_id);
        int GetEnrolmentCountByCondition(int status, string ownerName, string plateNumber, PagingCriteria paging);
        /// <summary>
        /// 根据条件分页查询符合的报港单信息
        /// </summary>
        /// <param name="status">报港单状态</param>
        /// <param name="ownerName">货主名称</param>
        /// <param name="plateNumber">车船号</param>
        /// <param name="paging">分页信息</param>
        /// <returns>符合条件的报港单列表</returns>
        RepositoryResultList<Enrolment> FindPageEnrolmentByCondition(int status, string ownerName, string plateNumber, PagingCriteria paging);

        RepositoryResultList<Enrolment> FindPageEnrolmentByCondition(int status,int traffic_type, string ownerName, string plateNumber, PagingCriteria paging);
        //bool CheckEnrolmentCode(string enrolmentCode);
        //bool CheckEnrolmentName(string enrolmentName);

        /// <summary>
        /// 退港
        /// </summary>
        /// <param name="enrolment">报港单</param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        bool FinishAssay(Enrolment enrolment, ref string strErrorMessage);

        /// <summary>
        /// 返回货主和货主身份证号
        /// </summary>
        /// <param name="enroment"></param>
        /// <returns></returns>
        List<OwernerAndID> GetOwernerAndID();

        /// <summary>
        /// 返回承运人和承运人身份证
        /// </summary>
        /// <returns></returns>
        List<CarryerAndID> GetCarryerAndID();
        #endregion

        #region EnrolmentRfid 服务
        void AddEnrolmentRfid(EnrolmentRFIDReader newEnrolmentRFIDReader);
        void UdpEnrolmentRfid(EnrolmentRFIDReader udpEnrolmentRFIDReader);
        void DelEnrolmentRfid(string strDelEnrolmentRifdReaderCodes);
        IEnumerable<EnrolmentRFIDReader> GetEnrolmentRFIDReaderBySql(string sqlString);
        IEnumerable<EnrolmentRFIDReader> GetAllEnrolmentRFIDReader();
        EnrolmentRFIDReader GetEnrolmentRFIDReaderDefault(string strMacAddress);
        #endregion

        void RefreshData();

        Enrolment UpenrolmentWithUnit(Enrolment upenrolment);

        void UpdateEnrolmentAndPlantask(Enrolment upEnrolment, PlanTask upPlantask, Assay myAssay);

        void UnitOfWorkActionClear();

        void UnitOfWorkActionSave();
    }
}
