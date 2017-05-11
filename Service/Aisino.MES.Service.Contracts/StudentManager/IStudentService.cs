using LC.Model;
using LC.Model.Business;
using LC.Model.Business.StudentModel;
using LC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Contracts.StudentManager
{
    [ServiceContract(Namespace = "LC.Service.StudentManager")]
    public interface IStudentService
    {
        /// <summary>
        /// 获取所有学生信息
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<StudentDisplayModel> GetAll();

        /// <summary>
        /// 根据班级查找该班级所含所有学生列表
        /// </summary>
        /// <param name="classId">班级信息</param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<StudentDisplayModel> FindStudentsByClassId(int classId);

        /// <summary>
        /// 根据课程表查找该班级所含所有学生列表
        /// </summary>
        /// <param name="classId">班级信息</param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<StudentDisplayModel> FindStudentsByScheduleId(int scheduleId);

        /// <summary>
        /// 根据学员id获取编辑对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        StudentEditModel GetStudentById(int id);

        /// <summary>
        /// 创建学员信息
        /// </summary>
        /// <param name="newClassEditModel">需要创建的学员信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        StudentEditModel Add(StudentEditModel newStudentEditModel);

        /// <summary>
        /// 更新学员信息
        /// </summary>
        /// <param name="newClassEditModel">需要更新的学员信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        StudentEditModel Update(StudentEditModel newStudentEditModel);

        /// <summary>
        /// 学员充值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Recharge(int id,int money,LoginModel loginModel);

        /// <summary>
        /// 删除学员信息
        /// </summary>
        /// <param name="deleteStudentEditModel">删除学员信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Delete(StudentEditModel deleteStudentEditModel);

        /// <summary>
        /// 根据学员编号删除学员信息
        /// </summary>
        /// <param name="userCode">学员编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteById(int id);
    }
}
