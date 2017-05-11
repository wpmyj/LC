using LC.Model;
using LC.Model.Business;
using LC.Model.Business.ClassModel;
using LC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Contracts.ClassManager
{
    [ServiceContract(Namespace = "LC.Service.ClassManager")]
    public interface IScheduleService
    {
        /// <summary>
        /// 根据月份查找相应的排课计划
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ScheduleDisplayModel> FindScheduleByMonth(string month);

        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ScheduleDisplayModel> FindScheduleByMonthAndTeacher(string month,int teacherId);

        /// <summary>
        /// 根据排课计划id获取编辑对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ScheduleEditModel GetScheduleById(int id);

        /// <summary>
        /// 创建排课信息
        /// </summary>
        /// <param name="newClassEditModel">需要创建的排课信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ScheduleEditModel Add(ScheduleEditModel newScheduleEditModel);

        /// <summary>
        /// 更新排课信息
        /// </summary>
        /// <param name="newClassEditModel">需要更新的排课信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ScheduleEditModel Update(ScheduleEditModel newScheduleEditModel);

        /// <summary>
        /// 根据课程表编号删除课程信息
        /// </summary>
        /// <param name="userCode">课程编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteById(int id);

        /// <summary>
        /// 登记学生上课信息
        /// </summary>
        /// <param name="newClassEditModel">需要更新的排课信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool CheckStudent(int scheduleId,List<int> studentIds,LoginModel loginModel);

        /// <summary>
        /// 撤销学员签到信息
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="studentId"></param>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool UnCheckStudent(int scheduleId, int studentId, LoginModel loginModel);
    }
}
