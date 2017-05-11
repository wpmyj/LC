using LC.Model;
using LC.Model.Business;
using LC.Model.Business.TeacherModel;
using LC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Contracts.TeacherManager
{
    [ServiceContract(Namespace = "LC.Service.TeacherManager")]
    public interface ITeacherService
    {
        /// <summary>
        /// 获得所有教师信息
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<TeacherModel> GetAllTeacher();

        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        TeacherModel FindTeacherByUserCode(string userCode);

        /// <summary>
        /// 创建教师信息
        /// </summary>
        /// <param name="newUserModel">需要创建的教师信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        TeacherModel Add(TeacherModel newTeacherModel);

        /// <summary>
        /// 更新教师信息
        /// </summary>
        /// <param name="newUserModel">需要更新的教师信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        TeacherModel Update(TeacherModel newTeacherModel);

        /// <summary>
        /// 教师结算
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool CheckMoney(int id,decimal money, LoginModel loginModel);

        /// <summary>
        /// 根据月份结算教师工资
        /// </summary>
        /// <param name="id"></param>
        /// <param name="month"></param>
        /// <param name="money"></param>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool CheckMonthMoney(int id,string month, decimal money, LoginModel loginModel);

        /// <summary>
        /// 根据教师编号获取可供结算金额
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        decimal GetCheckMoney(int id);

        /// <summary>
        /// 根据教师编号获取需要结算的月份（按月结算）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<TeacherCheckModel> GetCheckMonthMoney(int id);

        /// <summary>
        /// 根据教师编号以及日期段，查找该教师在该段时间内的所有上课记录明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<TeacherClassRecordDetailModel> FindTeacherClassRecordDetailByDate(DateTime startDate, DateTime endDate, int teacherId);

        /// <summary>
        /// 删除教师信息
        /// </summary>
        /// <param name="deleteUserModel">删除教师信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Delete(TeacherModel deleteTeacherModel);

        /// <summary>
        /// 根据教师编号删除教学中心信息
        /// </summary>
        /// <param name="userCode">教师编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteById(int id);
    }
}
