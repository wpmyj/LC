using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.StoreProcedure.Procedures;
using Aisino.MES.Model.StoreProcedure;

namespace Aisino.MES.Service.StoreProcedures
{
    public class SPGetSysDateTimeService
    {
        private Repository<GetSysDateTime> _getSysDateTimeDal;

        public SPGetSysDateTimeService(Repository<GetSysDateTime> getSysDateTimeDal)
        {
            _getSysDateTimeDal = getSysDateTimeDal;
        }

        public DateTime GetSysDateTime()
        {
            StoredProc<GetSysDateTime> getSysDateTime = new StoredProc<GetSysDateTime>(typeof(GetSysDateTimeResultSet));
            GetSysDateTime pams1 = new GetSysDateTime();
            ResultsList results = _getSysDateTimeDal.CallStoredProc(getSysDateTime, pams1);
            var r1 = results.ToList<GetSysDateTimeResultSet>();
            return r1.FirstOrDefault().currentDate;
        }
    }
}
