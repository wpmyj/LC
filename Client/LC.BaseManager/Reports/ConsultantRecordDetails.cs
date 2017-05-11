using Aisino.MES.Client.Common;
using LC.Contracts.BaseManager;
using LC.Contracts.TeacherManager;
using LC.Model.Business.BaseModel;
using LC.Model.Business.TeacherModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfClientProxyGenerator.Async;

namespace LC.BaseManager.Reports
{
    [DataObject]
    public class ConsultantRecordDetails
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public async Task<List<ConsultantRecordDetailModel>> ServiceGetConsultantRecordDetails(DateTime startDate, DateTime endDate, int consultantId)
        {
            IAsyncProxy<IConsultantService> _consultantAyncProxy = await Task.Run(() => ServiceHelper.GetConsultantService());
            IList<ConsultantRecordDetailModel> tcrdml = await _consultantAyncProxy.CallAsync(c => c.FindConsultantRecordDetailByDate(new DateTime(2016, 11, 1), new DateTime(2016, 11, 30), 1006));

            return tcrdml.ToList();
        }

        public List<ConsultantRecordDetailModel> get()
        {
            Task<List<ConsultantRecordDetailModel>> results = this.ServiceGetConsultantRecordDetails(new DateTime(2016, 11, 1), new DateTime(2016, 11, 30), 1003);
            return results.Result;
        }

    }
}
