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
    public interface IClassTypeService
    {
        /// <summary>
        /// 获得所有班级类型信息
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ClassTypeModel> GetAllClassType();

        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ClassTypeModel GetClassTypeById(int id);

        /// <summary>
        /// 创建班级类型信息
        /// </summary>
        /// <param name="newUserModel">需要创建的班级类型信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ClassTypeModel Add(ClassTypeModel newClassTypeModel);

        /// <summary>
        /// 更新班级类型信息
        /// </summary>
        /// <param name="newUserModel">需要更新的班级类型信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ClassTypeModel Update(ClassTypeModel newClassTypeModel);

        /// <summary>
        /// 删除班级类型信息
        /// </summary>
        /// <param name="deleteUserModel">删除班级类型信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Delete(ClassTypeModel deleteClassTypeModel);

        /// <summary>
        /// 根据班级类型编号删除班级类型信息
        /// </summary>
        /// <param name="userCode">班级类型编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteById(int id);

        /// <summary>
        /// 获取所有用于选择对象的班级类型树
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ClassTypeModel> GetAllClassTypeWithClasses();

        #region schemas
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<SchemasEditModel> FindSchemasByClassType(int typeId);

        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        SchemasEditModel AddSchemas(SchemasEditModel newSchemasEditModel);

        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        int GetMaxSeqByClassType(int typeId);
        #endregion
    }
}
