using LC.Model;
using LC.Model.Business;
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
    public interface IConsultantService
    {
        /// <summary>
        /// 获得所有会籍顾问信息
        /// </summary>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ConsultantModel> GetAllConsultant();

        /// <summary>
        /// 创建会籍顾问信息
        /// </summary>
        /// <param name="newUserModel">需要创建的教学中心信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ConsultantModel Add(ConsultantModel newConsultantModel);

        /// <summary>
        /// 更新会籍顾问信息
        /// </summary>
        /// <param name="newUserModel">需要更新的教学中心信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        ConsultantModel Update(ConsultantModel newConsultantModel);

        /// <summary>
        /// 删除会籍顾问信息
        /// </summary>
        /// <param name="deleteUserModel">删除教学中心信息</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool Delete(ConsultantModel deleteConsultantModel);

        /// <summary>
        /// 根据会籍顾问编号删除会籍顾问信息
        /// </summary>
        /// <param name="userCode">教学中心编号</param>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool DeleteById(int id);

        /// <summary>
        /// 根据会籍顾问id获取该会籍顾问尚未结账清单
        /// </summary>
        /// <param name="id">会籍顾问id</param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ConsultantCheckModel> GetCheckMonthMoney(int id);

        /// <summary>
        /// 根据月份结算会籍顾问工资
        /// </summary>
        /// <param name="id"></param>
        /// <param name="month"></param>
        /// <param name="money"></param>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        bool CheckMonthMoney(int id, string month, decimal money, LoginModel loginModel);

        /// <summary>
        /// 根据会籍顾问编号以及日期段，查找该会籍顾问在该段时间内的所有上课记录明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(LCFault))]
        IList<ConsultantRecordDetailModel> FindConsultantRecordDetailByDate(DateTime startDate, DateTime endDate, int consultantId);
    }
}
