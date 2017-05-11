using Aisino.MES.Client.Common;
using LC.Contracts.TeacherManager;
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
    public class TeacherClassRecordDetails
    {
        //public List<TeacherClassRecordDetailModel> TeacherClassRecordDetails { get; set; }

        
        [DataObjectMethod(DataObjectMethodType.Select)]
        public async Task<List<TeacherClassRecordDetailModel>> ServiceGetTeacherClassRecordDetails(DateTime startDate, DateTime endDate, int teacherId)
        {
            IAsyncProxy<ITeacherService> _teacherAyncProxy = await Task.Run(() => ServiceHelper.GetTeacherService());
            IList<TeacherClassRecordDetailModel> tcrdml = await _teacherAyncProxy.CallAsync(c => c.FindTeacherClassRecordDetailByDate(new DateTime(2016, 11, 1), new DateTime(2016, 11, 30), 1006));

            return tcrdml.ToList();
        }

        public List<TeacherClassRecordDetailModel> get()
        {
            Task<List<TeacherClassRecordDetailModel>> results = this.ServiceGetTeacherClassRecordDetails(new DateTime(2016, 11, 1), new DateTime(2016, 11, 30), 1006);
            return results.Result;
        }

    }
}
