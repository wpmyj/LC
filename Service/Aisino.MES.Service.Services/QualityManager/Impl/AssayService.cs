using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.Service.ManuManager;
using Aisino.MES.Service.SysManager;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Service.StoreProcedures;
using Aisino.MES.Service.BusinessManager;
using Aisino.MES.DAL.Repository.Helpers;


namespace Aisino.MES.Service.QualityManager.Impl
{
    public partial class AssayService : IAssayService
    {
        private Repository<Assay> _assayDal;
        private Repository<AssayBasicData> _assayBDDal;
        private Repository<AssayBill> _assayBillDal;
        private Repository<AssayResult> _assayResultDal;
        private Repository<AssayType> _assayTypeDal;
        private ISysBillNoService _sysBillNoService;
        SPGetSysDateTimeService _sPGetSysDateTimeService;
        private ITargetPriceService _targetPriceService;
        //private IPlanTaskService _planTaskService;
        //private Repository<QualityTestItemText> _qualityTestItemTextDal;
        private UnitOfWork _unitOfWork;

        public AssayService(Repository<Assay> assayDal,
                            Repository<AssayBasicData> assayBDDal,
                            Repository<AssayBill> assayBillDal,
                            Repository<AssayResult> assayResult,
                            Repository<AssayType> assayType,
                            ISysBillNoService sysBillNoService,
                            SPGetSysDateTimeService sPGetSysDateTimeService,
                            ITargetPriceService targetPriceService,
            //IPlanTaskService planTaskService,
                                       UnitOfWork unitOfWork)
        {
            _assayDal = assayDal;
            _assayBDDal = assayBDDal;
            _assayBillDal = assayBillDal;
            _assayResultDal = assayResult;
            _assayTypeDal = assayType;
            _sysBillNoService = sysBillNoService;
            _sPGetSysDateTimeService = sPGetSysDateTimeService;
            _targetPriceService = targetPriceService;
            //_planTaskService = planTaskService;
            _unitOfWork = unitOfWork;
        }
        #region Assay 服务
        public Assay AddAssay(Assay newAssay)
        {
            Assay reAssay = null;
            try
            {
                _assayDal.Add(newAssay);
                reAssay = newAssay;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssay;
        }

        public Assay AddAssayWithUnitOfWork(Sample newSample, SampleDetail newSampleDetail, int user_id, Contract cont,
                                           IList<GrainQualityIndex> GrainQualityIndexList, List<string> resultList,
                                            AssayStatus status, string AssayBillIndex, OrganizationDepartment org, int gradeID)
        {
            Assay newAssay = new Assay();
            int billID = _sysBillNoService.GetBillNoID("QUM", "ASN");
            newAssay.assay_number = _sysBillNoService.GetBillNo(billID);
            //newAssay.assay_result = null;
            //newAssay.bill_owner_org = 1;
            //newAssay.confirm_time = null;
            newAssay.confirm_user = user_id;
            newAssay.create_time = _sPGetSysDateTimeService.GetSysDateTime();
            newAssay.grain_grade = gradeID;
            newAssay.remark = "";
            newAssay.sample_number = newSample.sample_number;
            newAssay.status = (int)status;
            _unitOfWork.AddAction(newAssay, DataActions.Add);

            AddAssayBillWithUnitOfWork(newAssay, user_id, newSampleDetail, cont, GrainQualityIndexList, resultList, AssayBillIndex);
            //newAssay.grain_grade = JudgeGrainGrade(newAssay.AssayBills.First().AssayResults,
            return newAssay;
        }

        public Assay UpdAssay(Assay newAssay)
        {
            Assay reAssay = null;
            try
            {
                newAssay.confirm_time = _sPGetSysDateTimeService.GetSysDateTime();
                _assayDal.Update(newAssay);
                reAssay = newAssay;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssay;
        }
        public Assay UpdAssayWithUnitOfWork(Assay oldAssay, OrganizationDepartment org)
        {
            Assay reAssay = null;
            try
            {
                decimal unitprice = 0;
                int grainGrade;
                grainGrade = JudgeGrainGrade(oldAssay.AssayBills.ToList()[0].AssayResults.ToList(), org, oldAssay.Sample.goods_kind, out unitprice);
                if (grainGrade > 0)
                {
                    oldAssay.grain_grade = grainGrade;
                }

                oldAssay.confirm_time = _sPGetSysDateTimeService.GetSysDateTime();
                _unitOfWork.AddAction(oldAssay, DataActions.Update);

                if (oldAssay.Sample.Enrolment1.Contract != null && oldAssay.Sample.Enrolment1.Contract.grain_price.HasValue)
                {
                    oldAssay.Sample.PlanTaskBatch.unit_price = oldAssay.Sample.Enrolment1.Contract.grain_price.Value / 1000;
                    oldAssay.Sample.PlanTaskBatch.PlanTask.unit_price = oldAssay.Sample.Enrolment1.Contract.grain_price.Value / 1000;

                    _unitOfWork.AddAction(oldAssay.Sample.PlanTaskBatch, DataActions.Update);

                    _unitOfWork.AddAction(oldAssay.Sample.PlanTaskBatch.PlanTask, DataActions.Update);
                }
                else if (unitprice != 0)
                {
                    oldAssay.Sample.PlanTaskBatch.unit_price = unitprice;
                    oldAssay.Sample.PlanTaskBatch.PlanTask.unit_price = unitprice;
                    _unitOfWork.AddAction(oldAssay.Sample.PlanTaskBatch, DataActions.Update);

                    _unitOfWork.AddAction(oldAssay.Sample.PlanTaskBatch.PlanTask, DataActions.Update);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssay;
        }

        public Assay UpdAssayWithUnitOfWork(Assay assay, AssayBill assayBill, IList<AssayResult> assayResults,
                                            IList<GrainQualityIndex> grainQualityIndexList, List<string> resultList, AssayStatus status)
        {
            assay.status = (int)status;
            _unitOfWork.AddAction(assay, DataActions.Update);
            ////删除
            foreach (AssayResult ar in assayBill.AssayResults)
            {
                _unitOfWork.AddAction(ar, DataActions.Delete);
            }
            //新增
            if (grainQualityIndexList.Count != 0)
            {
                AddAssayResultWithUnitOfWork(assayBill, assay.Sample.Enrolment1.Contract, grainQualityIndexList, resultList);
            }

            _unitOfWork.Save();
            return assay;
        }

        public void DelAssay(Assay delAssay)
        {
            try
            {
                _assayDal.Delete(delAssay);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DelAssayWithUnitOfWork(Assay assay)
        {
            DelAssayBillWithUnitOfWork(assay);

            _unitOfWork.AddAction(assay, DataActions.Delete);
            _unitOfWork.Save();
        }
        public IEnumerable<Assay> GetAllAssay()
        {
            return _assayDal.Find(ay => ay.assay_number.StartsWith("ASN")).Entities;
        }

        public IEnumerable<Assay> GetAssaysBySQLStr(string sqlStr)
        {
            return _assayDal.QueryByESql(sqlStr).Entities.Where(ay => ay.assay_number.StartsWith("ASN"));
        }

        public RepositoryResultList<Assay> GetAssaysByParameter(string[] para, PagingCriteria pageC)
        {
            //            string SQLStr = string.Format(@"select a.* from Assay as a  
            //                                             right join Sample as s on a.sample_number = s.sample_number
            //                                             right join GoodsKind as g on s.goods_kind = g.goods_kind_id
            //                                             right join AssayType ayt on s.assay_type = ayt.id
            //                                             where g.goods_kind_name like '%[0]%' or s.sample_time like '%[1]%' 
            //                                                or ayt.name like '%[2]%' and 1=1", para[0], para[1], para[2]);

            var queryAssay = PredicateBuilder.True<Assay>();
            if (para[0] != "")
                queryAssay = queryAssay.And(a => a.Sample.GoodsKindGK.goods_kind_name.Contains(para[0]));
            if (para[1] != string.Empty)
                queryAssay = queryAssay.And(a => a.create_time.HasValue && a.create_time.Value.ToShortDateString().Contains(para[1]));
            if (para[2] != "")
                queryAssay = queryAssay.And(a => a.status == int.Parse(para[2]));
            queryAssay = queryAssay.And(a => a.assay_number.StartsWith("ASN"));

            return _assayDal.Find(queryAssay, pageC, "AssayBills", "AssayBills.AssayResults");
        }

        public string GetAssayTempNumber()
        {
            int billID = _sysBillNoService.GetBillNoID("QUM", "ASN");
            return _sysBillNoService.GetBillNoTemp(billID);
        }

        public bool CheckAssayNumberExist(string assayNumber)
        {
            var assayTemp = _assayDal.Single(ay => ay.assay_number == assayNumber);
            if (assayTemp.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Assay GetAssayByAssayNumber(string assayNumber)
        {
            var rtAssay = _assayDal.Single(ay => ay.assay_number == assayNumber);
            if (rtAssay.HasValue)
            {
                return rtAssay.Entity;
            }
            else
            {
                return null;
            }
        }

        public void AssayRefresh()
        {
            _assayDal.RefreshData();
        }
        #endregion

        #region AssayBasicData 服务
        public AssayBasicData AddAssayBasicData(AssayBasicData newAssayBasicData)
        {
            AssayBasicData reAssayBD = null;
            try
            {
                _assayBDDal.Add(newAssayBasicData);
                reAssayBD = newAssayBasicData;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssayBD;
        }

        public AssayBasicData UpdAssayBasicData(AssayBasicData newAssayBasicData)
        {
            AssayBasicData reAssayBD = null;
            try
            {
                _assayBDDal.Update(newAssayBasicData);
                reAssayBD = newAssayBasicData;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssayBD;
        }

        public void DelAssayBasicData(AssayBasicData delAssayBasicData)
        {
            try
            {
                _assayBDDal.Delete(delAssayBasicData);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        #endregion

        #region AssayBill 服务
        public AssayBill AddAssayBill(AssayBill newAssayBill)
        {
            AssayBill reAssayBill = null;
            try
            {
                _assayBillDal.Add(newAssayBill);
                reAssayBill = newAssayBill;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssayBill;
        }

        public void AddAssayBillWithUnitOfWork(Assay newAssay, int user_id, SampleDetail newSampleDetail, Contract cont,
                                                IList<GrainQualityIndex> GrainQualityIndexList, List<string> resultList, string AssayBillIndex)
        {
            //AssayBill新增
            AssayBill newAssayBill = new AssayBill();
            if (AssayBillIndex == string.Empty)
            {
                int billNumberIndex = 1;
                if (newAssay != null && newAssay.AssayBills != null && newAssay.AssayBills.Count != 0)
                {
                    billNumberIndex = newAssay.AssayBills.Count + 1;
                }
                AssayBillIndex = billNumberIndex.ToString("D2");
            }
            newAssayBill.assay_bill_number = newAssay.assay_number + AssayBillIndex;
            newAssayBill.assay_number = newAssay.assay_number;
            newAssayBill.assay_time = newAssay.create_time.Value;
            newAssayBill.assay_user = user_id;
            newAssayBill.sample_detail = newSampleDetail.sample_detail_number;
            _unitOfWork.AddAction(newAssayBill, DataActions.Add);

            AddAssayResultWithUnitOfWork(newAssayBill, cont, GrainQualityIndexList, resultList);
        }

        public AssayBill AddAssayBillWithSave(Assay oldAssay, int user_id, string billIndex, Contract cont,
                                                IList<GrainQualityIndex> GrainQualityIndexList, List<string> resultList)
        {
            //新增一个SampleDetail
            SampleDetail newSampleDetail = new SampleDetail();
            newSampleDetail.sample_number = oldAssay.sample_number;
            newSampleDetail.sample_detail_number = oldAssay.sample_number + billIndex;
            newSampleDetail.saved = oldAssay.Sample.sample_saved;
            _unitOfWork.AddAction(newSampleDetail, DataActions.Add);

            //AssayBill新增
            AssayBill newAssayBill = new AssayBill();

            newAssayBill.assay_bill_number = oldAssay.assay_number + billIndex;
            newAssayBill.assay_number = oldAssay.assay_number;
            newAssayBill.assay_time = oldAssay.create_time.Value;
            newAssayBill.assay_user = user_id;
            newAssayBill.sample_detail = newSampleDetail.sample_detail_number;
            _unitOfWork.AddAction(newAssayBill, DataActions.Add);

            AddAssayResultWithUnitOfWork(newAssayBill, cont, GrainQualityIndexList, resultList);

            _unitOfWork.Save();
            return newAssayBill;
        }
        public AssayBill UpdAssayBill(AssayBill newAssayBill)
        {
            AssayBill reAssayBill = null;
            try
            {
                _assayBillDal.Update(newAssayBill);
                reAssayBill = newAssayBill;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssayBill;
        }

        public void DelAssayBill(AssayBill delAssayBill)
        {
            try
            {
                _assayBillDal.Delete(delAssayBill);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DelAssayBillWithUnitOfWork(Assay delAssay)
        {
            foreach (AssayBill ab in delAssay.AssayBills)
            {
                DelAssayResultWithUnitOfWork(ab);
                _unitOfWork.AddAction(ab, DataActions.Delete);
            }
        }

        public bool CheckAssayBillNumberExist(string AssayBillNumber)
        {
            var assayBillTemp = _assayBillDal.Single(abd => abd.assay_bill_number == AssayBillNumber);
            if (assayBillTemp.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region AssayResult  服务
        public AssayResult AddAssayResult(AssayResult newAssayResult)
        {
            AssayResult reAssayResult = null;
            try
            {
                _assayResultDal.Add(newAssayResult);
                reAssayResult = newAssayResult;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssayResult;
        }

        public void AddAssayResultWithUnitOfWork(AssayBill newAssayBill, Contract cont, IList<GrainQualityIndex> GrainQualityIndexList, List<string> resultList)
        {
            //AssayResult新增
            for (int i = 0; i < GrainQualityIndexList.Count; i++)
            {
                AssayResult newAssayResult = new AssayResult();
                newAssayResult.assay_bill_number = newAssayBill.assay_bill_number;
                newAssayResult.assay_result_comparision = IsPassed(GrainQualityIndexList[i], resultList[i]);
                newAssayResult.assay_result_number = newAssayBill.assay_bill_number + (resultList.Count - GrainQualityIndexList.Count + i + 1).ToString("D2");

                if (cont != null && cont.ContractGrainQualityIndexes.Count > 0)
                {
                    if (cont.ContractGrainQualityIndexes.SingleOrDefault(cgq => cgq.GrainQualityIndex == GrainQualityIndexList[i]) != null)
                    {
                        newAssayResult.assay_result_standard = cont.ContractGrainQualityIndexes.SingleOrDefault(cgq => cgq.GrainQualityIndex == GrainQualityIndexList[i]).contract_quality_index_value;
                    }
                    else
                    {
                        newAssayResult.assay_result_standard = GrainQualityIndexList[i].value;
                    }
                }
                else
                {
                    newAssayResult.assay_result_standard = GrainQualityIndexList[i].value;
                }
                newAssayResult.assay_result_value = resultList[i];
                newAssayResult.assay_way_type = 1;
                newAssayResult.grain_quality_index = GrainQualityIndexList[i].index_id;

                _unitOfWork.AddAction(newAssayResult, DataActions.Add);
            }

        }

        public AssayResult UpdAssayResult(AssayResult newAssayResult)
        {
            AssayResult reAssayResult = null;
            try
            {
                _assayResultDal.Update(newAssayResult);
                reAssayResult = newAssayResult;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssayResult;
        }

        public void UpdAssayResultWithUnitOfWork()
        { }

        public void DelAssayResult(AssayResult delAssayResult)
        {
            try
            {
                _assayResultDal.Delete(delAssayResult);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DelAssayResultWithUnitOfWork(AssayBill DelAssayBill)
        {
            foreach (AssayResult ar in DelAssayBill.AssayResults)
            {
                _unitOfWork.AddAction(ar, DataActions.Delete);
            }
        }

        public string IsPassed(GrainQualityIndex gqi, string value)
        {
            if (gqi.is_comparsion.HasValue && gqi.is_comparsion.Value == false)
            {
                return value;
            }
            string boolStr = string.Empty;
            bool passed = false;
            bool passed_and = true;
            //先和第一界限比较
            boolStr = string.Format("{0}{1}{2}", value, gqi.assess, gqi.value);
            bool? boolpass = strToBool(boolStr);
            if (!boolpass.HasValue)
            {
                return "基础指标异常";                //第一指标比较，若不能转化为bool值，则显示异常，提醒用户去维护基础指标
            }
            else
            {
                passed = boolpass.Value;
            }
            if (!gqi.and_flag.HasValue)
                return "基础指标异常";                //获取第二指标标志位失败，提醒用户去维护基础指标
            else if (gqi.and_flag.Value)
            {
                boolStr = string.Format("{0}{1}{2}", value, gqi.assess_and, gqi.value_and);
                boolpass = strToBool(boolStr);
                if (!boolpass.HasValue)
                    return "基础指标异常";            //第二指标比较，若不能转换为bool值，则显示异常，提醒用户去维护基础指标
                else
                    passed_and = boolpass.Value;
            }

            if (passed && passed_and)
                return "合格";
            else
                return "不合格";
        }

        private bool? strToBool(string boolStr)
        {
            bool? IsTrue = null;
            char[] boolChars = boolStr.ToCharArray();
            string number1 = string.Empty;
            string number2 = string.Empty;
            string com = string.Empty;
            int index = 1;
            for (int i = 0; i < boolStr.Length; i++)
            {
                if ('0' <= boolChars[i] && boolChars[i] <= '9' || '.' == boolChars[i])
                    if (index == 1)
                        number1 += boolChars[i].ToString();
                    else
                        number2 += boolChars[i].ToString();
                else
                {
                    index = 2;
                    com += boolChars[i].ToString();
                }
            }
            double num1 = double.Parse(number1);
            double num2 = double.Parse(number2);
            switch (com)
            {
                case ">":
                    IsTrue = (num1 > num2);
                    break;
                case ">=":
                    IsTrue = (num1 >= num2);
                    break;
                case "<":
                    IsTrue = (num1 < num2);
                    break;
                case "<=":
                    IsTrue = (num1 <= num2);
                    break;
                case "==":
                    IsTrue = (num1 == num2);
                    break;
                case "!=":
                    IsTrue = (num1 != num2);
                    break;
                default: break;
            }
            return IsTrue;
        }
        #endregion

        #region AssayType 服务
        public AssayType AddAssayType(AssayType newAssayType)
        {
            AssayType reAssayType = null;
            try
            {
                _assayTypeDal.Add(newAssayType);
                reAssayType = newAssayType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssayType;
        }

        public AssayType UpdAssayType(AssayType newAssayType)
        {
            AssayType reAssayType = null;
            try
            {
                _assayTypeDal.Update(newAssayType);
                reAssayType = newAssayType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssayType;
        }

        public AssayType GetSingleAssayType(int AssayID)
        {
            var assayType = _assayTypeDal.Single(a => a.id == AssayID);
            if (assayType.HasValue)
                return assayType.Entity;
            else
                return null;
        }

        public void DelAssayType(AssayType delAssayType)
        {
            try
            {
                _assayTypeDal.Delete(delAssayType);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void RefreshAssayTypeData()
        {
            _assayTypeDal.RefreshData();
        }
        #endregion

        public IEnumerable<AssayBasicData> GetGrainGrade()
        {
            return _assayBDDal.Find(abd => abd.data_type.HasValue && abd.data_type.Value == 2).Entities.OrderBy(gg => gg.basic_data_id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arList">检测值列表</param>
        /// <param name="org">组织机构</param>
        /// <param name="goodsKindID">粮食品种</param>
        /// <param name="cont">合同</param>
        /// <returns>返回等级，-2表示异常，-1表示无等级基础数据，0表示未获取到组织机构对应的等级判定</returns>
        public int JudgeGrainGrade(IList<AssayResult> arList, OrganizationDepartment org, int goodsKindID, out decimal unitPrice)
        {
            int graidGrade = 0;
            unitPrice = 0;
            IList<AssayBasicData> GrainGradeList = GetGrainGrade().ToList();
            if (GrainGradeList == null || GrainGradeList.Count == 0)
                return -1;
            try
            {
                List<TargetPrice> TargetPriceList;
                if (org.TargetPrices.Count == 0)
                {
                    //未关联组织机构，直接找关联粮食品种的TargetPrice
                    TargetPriceList = _targetPriceService.GetTargetPricesByGoodsKindID(goodsKindID).OrderByDescending(tp => decimal.Parse(tp.min_value)).ToList();
                }
                else
                {
                    TargetPriceList = org.TargetPrices.Where(
                                                    tp => tp.grain_kind == goodsKindID).OrderByDescending(
                                                    otp => decimal.Parse(otp.min_value)).ToList();
                }
                if (TargetPriceList == null || TargetPriceList.Count == 0)
                    return 0;                               //如果没有对应的等级判定，就立即返回0
                //选取默认第一个TargetPrice的grain_quality_index为检索项
                AssayResult newar = arList.SingleOrDefault(ar => ar.grain_quality_index == TargetPriceList[0].grain_quality_index.Value);
                if (newar == null)
                    return 0;
                //判断
                string grain_grade = "";
                #region //////////////////////////////////////////判定等级////////////////////////////////////
                for (int i = 0; i < TargetPriceList.Count; i++)
                {
                    int Result = -1;
                    if (!TargetPriceList[i].min_tag.HasValue)
                    {
                        continue;
                    }
                    switch (TargetPriceList[i].min_tag.Value)
                    {
                        case 0:
                            if (decimal.Parse(newar.assay_result_value) > decimal.Parse(TargetPriceList[i].min_value))
                            {
                                Result = 0;
                                unitPrice = TargetPriceList[i].price.Value;
                            }
                            break;
                        case 1:
                            if (decimal.Parse(newar.assay_result_value) >= decimal.Parse(TargetPriceList[i].min_value))
                            {
                                Result = 0;
                                unitPrice = TargetPriceList[i].price.Value;
                            }
                            break;
                        case 2:
                            if (decimal.Parse(newar.assay_result_value) < decimal.Parse(TargetPriceList[i].min_value))
                            {
                                Result = 0;
                                unitPrice = TargetPriceList[i].price.Value;
                            }
                            break;
                        case 3:
                            if (decimal.Parse(newar.assay_result_value) <= decimal.Parse(TargetPriceList[i].min_value))
                            {
                                Result = 0;
                                unitPrice = TargetPriceList[i].price.Value;
                            }
                            break;
                        default: Result = -1; break;
                    }
                    if (Result != -1)                                                               //满足条件“一等”
                    {
                        switch (i)
                        {
                            case 0:
                                grain_grade = Enum.GetName(typeof(GrainGrade), GrainGrade.一等);
                                break;
                            case 1:
                                grain_grade = Enum.GetName(typeof(GrainGrade), GrainGrade.二等); ;
                                break;
                            case 2:
                                grain_grade = Enum.GetName(typeof(GrainGrade), GrainGrade.三等);
                                break;
                            case 3:
                                grain_grade = Enum.GetName(typeof(GrainGrade), GrainGrade.四等);
                                break;
                            case 4:
                                grain_grade = Enum.GetName(typeof(GrainGrade), GrainGrade.五等);
                                break;
                            case 5:
                                grain_grade = Enum.GetName(typeof(GrainGrade), GrainGrade.等外);
                                break;
                            default: grain_grade = string.Empty; break;
                        }
                        break;      //  赋值以后可退出循环
                    }
                }
                AssayBasicData abd = GrainGradeList.Single(gg => gg.name == grain_grade);
                if (abd != null)
                {
                    graidGrade = abd.basic_data_id;
                }
                #endregion
                return graidGrade;
            }
            catch
            {
                //返回-2是异常
                return -2;
            }
        }

        public decimal GetUnitPrice(int grain_kind, int grainGrade)
        {
            decimal unitprice = 0;
            string unitPirceStr = _targetPriceService.GetUnitPriceByGrainGrade(grain_kind, grainGrade);
            decimal.TryParse(unitPirceStr, out unitprice);
            return unitprice;
        }

        public void RefreshData()
        {
            this._assayBDDal.RefreshData();
            this._assayBillDal.RefreshData();
            this._assayDal.RefreshData();
            this._assayResultDal.RefreshData();
            this._assayTypeDal.RefreshData();
        }

        public IEnumerable<AssayBasicData> GetGrainType()
        {
            var assay = _assayBDDal.Find(a => a.data_type == 2).Entities;
            if (assay != null && assay.Count() > 0)
            {
                return assay.ToList();
            }
            else
            {
                return null;
            }
            //return _assayBDDal.Find(a => a.data_type == 2).Entities.ToList();
        }

        public Assay UpdAssayAndStepWithUnitOfWork(Assay oldAssay, OrganizationDepartment org, PlanTaskBatchStepStatus twoStep)
        {
            Assay reAssay = null;
            DateTime dt = _sPGetSysDateTimeService.GetSysDateTime();
            try
            {
                decimal unitprice = 0;
                int grainGrade;
                grainGrade = JudgeGrainGrade(oldAssay.AssayBills.ToList()[0].AssayResults.ToList(), org, oldAssay.Sample.goods_kind, out unitprice);
                if (grainGrade > 0)
                {
                    oldAssay.grain_grade = grainGrade;
                }

                oldAssay.confirm_time = dt;
                _unitOfWork.AddAction(oldAssay, DataActions.Update);

                if (oldAssay.Sample.Enrolment1.Contract != null && oldAssay.Sample.Enrolment1.Contract.grain_price.HasValue)
                {
                    oldAssay.Sample.PlanTaskBatch.unit_price = oldAssay.Sample.Enrolment1.Contract.grain_price.Value / 1000;
                    oldAssay.Sample.PlanTaskBatch.PlanTask.unit_price = oldAssay.Sample.Enrolment1.Contract.grain_price.Value / 1000;

                    _unitOfWork.AddAction(oldAssay.Sample.PlanTaskBatch, DataActions.Update);

                    _unitOfWork.AddAction(oldAssay.Sample.PlanTaskBatch.PlanTask, DataActions.Update);
                }
                else if (unitprice != 0)
                {
                    oldAssay.Sample.PlanTaskBatch.unit_price = unitprice;
                    oldAssay.Sample.PlanTaskBatch.PlanTask.unit_price = unitprice;
                    _unitOfWork.AddAction(oldAssay.Sample.PlanTaskBatch, DataActions.Update);

                    _unitOfWork.AddAction(oldAssay.Sample.PlanTaskBatch.PlanTask, DataActions.Update);
                }
                //Step
                twoStep.operate_time = dt;
                _unitOfWork.AddAction(twoStep, DataActions.Add);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssay;
        }

        public Assay UpdAssayAndStep3WithUnitOfWork(Assay oldAssay, PlanTaskBatchStepStatus threeStep)
        {
            Assay reAssay = null;
            DateTime dt = _sPGetSysDateTimeService.GetSysDateTime();
            try
            {
                threeStep.operate_time = dt;
                _unitOfWork.AddAction(oldAssay, DataActions.Update);
                _unitOfWork.AddAction(threeStep, DataActions.Add);
                _unitOfWork.Save();
                reAssay = oldAssay;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssay;
        }

        public Assay UpdAssayAndStepWithUnitOfWork(Assay oldAssay, PlanTaskBatchStepStatus twoStep, PlanTaskBatchStepStatus threeStep)
        {
            Assay reAssay = null;
            try
            {
                oldAssay.confirm_time = _sPGetSysDateTimeService.GetSysDateTime();
                twoStep.operate_time = oldAssay.confirm_time.Value;
                threeStep.operate_time = oldAssay.confirm_time.Value;
                _unitOfWork.AddAction(oldAssay, DataActions.Update);
                _unitOfWork.AddAction(twoStep, DataActions.Add);
                _unitOfWork.AddAction(threeStep, DataActions.Add);
                _unitOfWork.Save();
                reAssay = oldAssay;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reAssay;
        }

        public Assay UpdAssayConfirmWithUnitofWork(Assay oldAssay)
        {
            Assay reAssay = null;
            oldAssay.confirm_time = _sPGetSysDateTimeService.GetSysDateTime();

            _unitOfWork.AddAction(oldAssay, DataActions.Update);
            reAssay = oldAssay;
            return reAssay;
        }

        public void UpdClearAssayResultAndAddNew(AssayBill theAssayBill, List<AssayResult> theAssayResultList, IList<GrainQualityIndex> theGrainQualityIndexList)
        {
            foreach (AssayResult tempAR in theAssayBill.AssayResults)
            {
                _unitOfWork.AddAction(tempAR, DataActions.Delete);
            }
            Contract theContract = theAssayBill.Assay.Sample.PlanTaskBatch.PlanTask.Enrolment.Contract;
            //IList<GrainQualityIndex> theGrainQualityIndexList = theAssayResultList.Select(ar => ar.GrainQualityIndex).ToList();
            AddAssayResultWithUnitOfWork(theAssayBill, theContract, theGrainQualityIndexList, theAssayResultList.Select(ar => ar.assay_result_value).ToList());
        }

        public void UpdAssayPlanTaskInUnitOfWork(Assay oldAssay, PlanTask oldPlanTask, PlanTaskBatch oldPlanTaskBatch)
        {
            _unitOfWork.AddAction(oldAssay, DataActions.Update);
            _unitOfWork.AddAction(oldPlanTask, DataActions.Update);
            _unitOfWork.AddAction(oldPlanTaskBatch, DataActions.Update);
        }

        public void UpdBusinessAndDetailsInUnitOfWork(BusinessDispose oldBusinessDispose, List<BusinessDisposeDetail> theBusDisList)
        {
            foreach(BusinessDisposeDetail bdd in oldBusinessDispose.BusinessDisposeDetails)
            {
                _unitOfWork.AddAction(bdd, DataActions.Delete);
            }
            foreach (BusinessDisposeDetail bdd in theBusDisList)
            {
                bdd.business_dispose_number = oldBusinessDispose.business_dispose_number;
                _unitOfWork.AddAction(bdd, DataActions.Add);
            }
            _unitOfWork.Save();
        }
    }
}
