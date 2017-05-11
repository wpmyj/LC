using LC.Model;
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
    public interface IClassesService
    {
        /// <summary>
        /// 根据班级类型获得所有班级信息
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ClassDisplayModel> FindClassByClassType(int classTypeId);

        /// <summary>
        /// 根据教师查找其对应的上课记录
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ClassRecordDisplayModel> FindClassRecordByTeacher(int teacherId);

        /// <summary>
        /// 获取所有班级信息
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ClassDisplayModel> GetAllClasses();

        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ClassDisplayModel> GetAllActiveClasses();

        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ClassEditModel GetClassById(int id);

        /// <summary>
        /// 创建班级信息
        /// </summary>
        /// <param name="newClassEditModel">需要创建的班级信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ClassEditModel Add(ClassEditModel newClassEditModel);

        /// <summary>
        /// 更新班级信息
        /// </summary>
        /// <param name="newClassEditModel">需要更新的班级信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ClassEditModel Update(ClassEditModel newClassEditModel);

        /// <summary>
        /// 更新班级学员信息
        /// </summary>
        /// <param name="newClassEditModel">需要更新的班级学员信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ClassEditModel UpdateClassStudents(int classId, List<int> studentIds);

        /// <summary>
        /// 删除班级信息
        /// </summary>
        /// <param name="deleteUserModel">删除班级信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Delete(ClassEditModel deleteClassEditModel);

        /// <summary>
        /// 根据班级类型编号删除班级信息
        /// </summary>
        /// <param name="userCode">班级编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteById(int id);
    }
}
