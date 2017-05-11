using Aisino.MES.Model;
using Aisino.MES.Model.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Aisino.MES.Service.Contracts.Common
{
    [ServiceContract(Namespace = "Aisino.MES.Service.Common")]
    public interface ISysCodeService
    {
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(AisinoMesFault))]
        SysCodeEditModel Add(SysCodeEditModel newSysCodeModel);
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(AisinoMesFault))]
        SysCodeEditModel Update(SysCodeEditModel newSysCodeModel);
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(AisinoMesFault))]
        bool DeleteById(int id);
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(AisinoMesFault))]
        bool Delete(SysCodeEditModel delSysCodeModel);
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(AisinoMesFault))]
        IList<SysCodeDisplayModel> GetAllSysCodes();
        [OperationContract, ApplyDataContractResolver]
        [FaultContractAttribute(typeof(AisinoMesFault))]
        SysCodeEditModel GetSysCodeById(string id);
    }
}
