using LC.Model;
using LC.Model.Business.BaseModel;
using LC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LC.Contracts.BaseManager
{
    [ServiceContract(Namespace = "LC.Service.BaseManager")]
    public interface IClassroomService
    {
        /// <summary>
        /// 获得所有教室信息
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ClassroomModel> GetAllClassroom();

        /// <summary>
        /// 根据中心id查找所有所属教室
        /// </summary>
        /// <param name="centerId"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ClassroomModel> FindClassroomByCenter(int centerId);

        /// <summary>
        /// 创建教室信息
        /// </summary>
        /// <param name="newUserModel">需要创建的教室信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ClassroomModel Add(ClassroomModel newClassroomModel);

        /// <summary>
        /// 更新教室信息
        /// </summary>
        /// <param name="newUserModel">需要更新的教室信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ClassroomModel Update(ClassroomModel newClassroomModel);

        /// <summary>
        /// 删除教室信息
        /// </summary>
        /// <param name="deleteUserModel">删除教室信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Delete(ClassroomModel deleteClassroomModel);

        /// <summary>
        /// 根据教室编号删除教室中心信息
        /// </summary>
        /// <param name="userCode">教学中心编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteById(int id);
    }
}
