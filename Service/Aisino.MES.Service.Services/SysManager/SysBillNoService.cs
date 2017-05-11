using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Service.StoreProcedures;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Model.Models;
using Aisino.MES.Model.StoreProcedure.Procedures;
using Aisino.MES.Model.StoreProcedure;

namespace Aisino.MES.Service.SysManager.Impl
{
    public class SysBillNoService : ISysBillNoService
    {
        private SPGetSysDateTimeService _getSysDateTimeService;
        private Repository<GetBillNo> _sysBillNoDal;
        private Repository<SysBillNo> _sysBillNo;
        private UnitOfWork _unitOfWork;

        public SysBillNoService(SPGetSysDateTimeService sPGetSysDateTimeService,Repository<GetBillNo> sysBillNoDal, Repository<SysBillNo> sysBillNo, UnitOfWork unitOfWork)
        {
            _getSysDateTimeService = sPGetSysDateTimeService;
            _sysBillNoDal = sysBillNoDal;
            _sysBillNo = sysBillNo;
            _unitOfWork = unitOfWork;
        }

        public string GetBillNoTemp(int billNoId)
        {
            string billNo = "";
            var sysBillNOEntity = _sysBillNo.SingleAsNoTracking(b => b.id == billNoId);
            string currentDate = _getSysDateTimeService.GetSysDateTime().ToString("yyyyMMdd");
            
            if (sysBillNOEntity.HasValue)
            {
                //获取最大流水号
                int maxNo = 0;
                SysBillNo sysBillNo = sysBillNOEntity.Entity;

                if (sysBillNo.max_date == null || sysBillNo.max_date != currentDate || sysBillNo.max_no == null)
                {
                    maxNo = 1;
                }
                else
                {
                    maxNo = sysBillNo.max_no.Value + 1;
                }

                //设置单据编号
                billNo = sysBillNo.prefix.Trim() + currentDate + maxNo.ToString().PadLeft(sysBillNo.num_bit, '0');
            }

            return billNo;
        }

        public string GetBillNo(int billNoId)
        {
            StoredProc<GetBillNo> getBillNo = new StoredProc<GetBillNo>(typeof(GetBillNoResultSet));
            GetBillNo pams1 = new GetBillNo();
            pams1.bill_id = billNoId;
            pams1.num_bit = 3;
            pams1.org_code = "";
            ResultsList results = _sysBillNoDal.CallStoredProc(getBillNo, pams1);
            var r1 = results.ToList<GetBillNoResultSet>();
            return r1.FirstOrDefault().billNumber;
        }

        public string GetBillNo(int billNoId, string OriginalDept)
        {
            StoredProc<GetBillNo> getBillNo = new StoredProc<GetBillNo>(typeof(GetBillNoResultSet));
            GetBillNo pams1 = new GetBillNo();
            pams1.bill_id = billNoId;
            pams1.num_bit = 3;
            pams1.org_code = OriginalDept;
            ResultsList results = _sysBillNoDal.CallStoredProc(getBillNo, pams1);
            var r1 = results.ToList<GetBillNoResultSet>();
            return r1.FirstOrDefault().billNumber;
        }

        //根据系统system 和前缀 prefix 取得单据编号ID
        public int GetBillNoID(string billNoSystem, string billNoPrefix)
        {
            int billNoID = 0;
      
            //var sysBillNOEntity = _sysBillNo.SingleAsNoTracking(d => d.system == billNoSystem && d.prefix == billNoPrefix);
            var sysBillNOEntity = _sysBillNo.Single(d => d.system == billNoSystem && d.prefix == billNoPrefix);
            if (sysBillNOEntity.HasValue)
            {
                SysBillNo sysBillNo = sysBillNOEntity.Entity;

                billNoID = sysBillNo.id;
            }

            return billNoID;
        }



        /// <summary>
        /// 根据system获取单据类别列表
        /// </summary>
        /// <param name="system"></param>
        /// <returns>单据类别列表</returns>
        public IEnumerable<SysBillNo> SelectAllBillNo(string system)
        {
            return _sysBillNo.GetAll().Entities.Where(d => d.system == system).ToList();

        }

        public IList<SysBillNo> GetMotifyTypes(string MESSystemString)
        {
            return _sysBillNo.Find(s => s.system == MESSystemString).Entities.ToList();
        }

        public void RefreshData()
        {
            this._sysBillNo.RefreshData();
        }
    }
}
