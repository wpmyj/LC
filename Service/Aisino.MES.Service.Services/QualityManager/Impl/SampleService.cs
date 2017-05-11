using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Service.SysManager;
using Aisino.MES.Service.StoreProcedures;

namespace Aisino.MES.Service.QualityManager.Impl
{
    public class SampleService : ISampleService
    {
        private static int BiLv = 1;
        private Repository<SampleType> _sampleTypeDal;
        private Repository<Sample> _sampleDal;
        private Repository<SampleDetail> _sampleDetailDal;
        private Repository<QualityIndex> _qualityIndexDal;
        private Repository<GrainQualityIndex> _grainQualityDal;
        private ISysBillNoService _sysBillNoService;
        private IAssayService _assayService;
        private SPGetSysDateTimeService _getSysDateTimeService;
        private UnitOfWork _unitOfWork;

        public SampleService(Repository<SampleType> sampleTypeDal,
                            Repository<Sample> sampleDal,
                            Repository<SampleDetail> sampleDetailDal,
                            Repository<QualityIndex> qualityIndexDal,
                            Repository<GrainQualityIndex> grainQualityDal,
                            ISysBillNoService sysBillNoService,
                            IAssayService assayService,
                            SPGetSysDateTimeService sPGetSysDateTimeService,
                                       UnitOfWork unitOfWork)
        {
            _sampleTypeDal = sampleTypeDal;
            _sampleDal = sampleDal;
            _sampleDetailDal = sampleDetailDal;
            _qualityIndexDal = qualityIndexDal;
            _grainQualityDal = grainQualityDal;
            _sysBillNoService = sysBillNoService;
            _assayService = assayService;
            _getSysDateTimeService = sPGetSysDateTimeService;
            _unitOfWork = unitOfWork;
        }
        #region SampleType 服务
        public SampleType AddSampleType(SampleType newSampleType)
        {
            SampleType reSampleType = null;
            try
            {
                _sampleTypeDal.Add(newSampleType);
                reSampleType = newSampleType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reSampleType;
        }

        public SampleType UpdSampleType(SampleType newSampleType)
        {
            SampleType reSampleType = null;
            try
            {
                _sampleTypeDal.Update(newSampleType);
                reSampleType = newSampleType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reSampleType;
        }

        public void DelSampleType(SampleType delSampleType)
        {
            try
            {
                _sampleTypeDal.Delete(delSampleType);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public IEnumerable<SampleType> GetAllSampleTypeList()
        {
            return _sampleTypeDal.GetAll().Entities;
        }

        public IEnumerable<SampleType> GetSampleTypeBySQL(string sqlString)
        {
            return _sampleTypeDal.QueryByESql(sqlString).Entities;
        }
        public bool CheckSampleTypeCodeExist(string sampleTypeCode)
        {
            var sampleTypeTemp = _sampleTypeDal.Single(s => s.sample_type_code == sampleTypeCode);
            if (sampleTypeTemp.HasValue)
                return true;
            else
                return false;
        }

        public bool CheckSampleTypeNameExist(string sampleTypeName)
        {
            var sampleTypeTemp = _sampleTypeDal.Single(s => s.sample_type_name == sampleTypeName);
            if (sampleTypeTemp.HasValue)
                return true;
            else
                return false;
        }
        #endregion

        #region Sample 服务
        public Sample AddSample(Sample newSample)
        {
            Sample reSample = null;
            try
            {
                _sampleDal.Add(newSample);
                reSample = newSample;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reSample;
        }

        public Sample AddSample(PlanTaskBatch planTaskBatch, OrganizationEmployee employee, int sampleCount)
        {
            try
            {
                Sample sample = new Sample();
                sample.bill_owner_org = AisinoMesServiceHelper.GetOriginalDeptId(employee);
                sample.assay_type = 1;
                sample.enrolment = planTaskBatch.PlanTask.enrolment_number;
                sample.goods_kind = planTaskBatch.PlanTask.goods_kind.Value;
                sample.plantask_batch_number = planTaskBatch.plantask_batch_number;
                sample.sample_count = sampleCount;
                sample.sample_saved = planTaskBatch.PlanTask.Enrolment.SampleType.sample_saved;
                sample.sample_type_code = planTaskBatch.PlanTask.Enrolment.sample_type_code;
                sample.sample_user = employee.employee_id;
                sample.sample_time = _getSysDateTimeService.GetSysDateTime();
                int billID = _sysBillNoService.GetBillNoID("QUM", "SAN");
                sample.sample_number = _sysBillNoService.GetBillNo(billID);

                _unitOfWork.AddAction(sample, DataActions.Add);
                _unitOfWork.Save();
                return sample;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Assay AddSampleWithUnitOfWork(PlanTaskBatch planTaskBatch, IList<GrainQualityIndex> grainQualityIndexList,
                                                                    List<string> resultList, int user_id, AssayStatus status, string AssayBillIndex, OrganizationDepartment org, int gradeID)
        {
            Contract cont = planTaskBatch.PlanTask.Enrolment.Contract;
            Sample newSample = new Sample();
            if (planTaskBatch.Samples.Count > 0)
            {
                newSample = planTaskBatch.Samples.First();
            }
            else
            {
                //新建 Sample
                newSample.assay_type = 1;
                //newSample.bill_owner_org = 1;
                newSample.enrolment = planTaskBatch.PlanTask.enrolment_number;
                newSample.goods_kind = planTaskBatch.PlanTask.Enrolment.goods_kind;
                newSample.plantask_batch_number = planTaskBatch.plantask_batch_number;
                newSample.sample_count = planTaskBatch.PlanTask.Enrolment.SampleType.sample_count.Value;
                int billID = _sysBillNoService.GetBillNoID("QUM", "SAN");
                newSample.sample_number = _sysBillNoService.GetBillNo(billID);
                newSample.sample_saved = planTaskBatch.PlanTask.Enrolment.SampleType.sample_saved;
                newSample.sample_time = _getSysDateTimeService.GetSysDateTime();
                newSample.sample_type_code = planTaskBatch.PlanTask.Enrolment.sample_type_code;
                newSample.sample_user = user_id;
                newSample.send_grain_unit = "unit";
                newSample.simple_source = 0;
                //newSample.warehouse_number = "";
                //newSample.warehouse_store = "";
                _unitOfWork.AddAction(newSample, DataActions.Add);
            }
            SampleDetail newSampleDetail = AddSampleDetailWithUnitofWork(newSample);
            //新建Assay            
            Assay newAssay = _assayService.AddAssayWithUnitOfWork(newSample, newSampleDetail, user_id, cont, grainQualityIndexList, resultList, status, AssayBillIndex, org, gradeID);
            _unitOfWork.Save();

            _assayService.RefreshAssayTypeData();
            return newAssay;
        }

        public Sample UpdSample(Sample newSample)
        {
            Sample reSample = null;
            try
            {
                _sampleDal.Update(newSample);
                reSample = newSample;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reSample;
        }

        public void DelSample(Sample delSample)
        {
            try
            {
                _sampleDal.Delete(delSample);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public string GetSempleNumber()
        {
            int billID = _sysBillNoService.GetBillNoID("QUM", "SAN");
            return _sysBillNoService.GetBillNoTemp(billID);
        }

        public IEnumerable<Sample> GetAllSampleList()
        {
            return _sampleDal.GetAll().Entities;
        }

        public void RefreshSampleData()
        {
            _sampleDal.RefreshData();
        }
        #endregion

        #region SampleDetail 服务
        public SampleDetail AddSampleDetail(SampleDetail newSampleDetail)
        {
            SampleDetail reSampleDetail = null;
            try
            {
                _sampleDetailDal.Add(newSampleDetail);
                reSampleDetail = newSampleDetail;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reSampleDetail;
        }

        public SampleDetail AddSampleDetailWithUnitofWork(Sample theSample)
        {
            //新建SampleDetail 
            SampleDetail newSampleDetail = new SampleDetail();
            newSampleDetail.sample_number = theSample.sample_number;
            newSampleDetail.sample_detail_number = theSample.sample_number + "01";
            newSampleDetail.saved = theSample.sample_saved;
            _unitOfWork.AddAction(newSampleDetail, DataActions.Add);
            return newSampleDetail;
        }
        public SampleDetail UpdSampleDetail(SampleDetail newSampleDetail)
        {
            SampleDetail reSampleDetail = null;
            try
            {
                _sampleDetailDal.Update(newSampleDetail);
                reSampleDetail = newSampleDetail;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reSampleDetail;
        }

        public void DelSampleDetail(SampleDetail delSampleDetail)
        {
            try
            {
                _sampleDetailDal.Delete(delSampleDetail);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region QualityIndex 服务
        public QualityIndex AddQualityIndex(QualityIndex newQualityIndex)
        {
            QualityIndex reQualityIndex = null;
            try
            {
                _qualityIndexDal.Add(newQualityIndex);
                reQualityIndex = newQualityIndex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reQualityIndex;
        }

        public QualityIndex UpdQualityIndex(QualityIndex newQualityIndex)
        {
            QualityIndex reQualityIndex = null;
            try
            {
                _qualityIndexDal.Update(newQualityIndex);
                reQualityIndex = newQualityIndex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reQualityIndex;
        }

        public QualityIndex DelQualityIndex(QualityIndex delQualityIndex)
        {
            try
            {
                if (delQualityIndex.GrainQualityIndexes == null)
                {
                    _qualityIndexDal.Delete(delQualityIndex);
                    return delQualityIndex;
                }
                if (delQualityIndex.GrainQualityIndexes.Count != 0)
                {
                    return null;
                }
                else
                {
                    _qualityIndexDal.Delete(delQualityIndex);
                    return delQualityIndex;
                }
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public IEnumerable<QualityIndex> GetAllQualityIndex()
        {
            return _qualityIndexDal.GetAll().Entities;
        }

        //add by yangyang 2014-04-22
        //比较值之间的逻辑关系控制
        public bool CheckQTestCompareValue(string qualityIndexAssess, string qualityIndexValue, bool flag, string qualityIndexAssessAnd, string qualityIndexValueAnd)
        {
            if (flag == false)
            {
                return true;
            }
            else
            {
                if ((qualityIndexAssess.Equals(">")) || (qualityIndexAssess.Equals(">=")))
                {
                    if ((qualityIndexAssessAnd.Equals(">")) || qualityIndexAssessAnd.Equals(">="))
                    {
                        return false;
                    }
                    else
                    {
                        if (decimal.Parse(qualityIndexValue) >= decimal.Parse(qualityIndexValueAnd))
                        {
                            return false;
                        }
                    }
                }
                else if ((qualityIndexAssess.Equals("<")) || (qualityIndexAssess.Equals("<=")))
                {
                    if ((qualityIndexAssessAnd.Equals("<")) || qualityIndexAssessAnd.Equals("<="))
                    {
                        return false;
                    }
                    else
                    {
                        if (decimal.Parse(qualityIndexValue) <= decimal.Parse(qualityIndexValueAnd))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        #endregion

        #region GrainQualityIndex 服务
        public GrainQualityIndex AddGrainQualityIndex(GrainQualityIndex newGrainQualityIndex)
        {
            GrainQualityIndex reGrainQualityIndex = null;
            try
            {
                _grainQualityDal.Add(newGrainQualityIndex);
                reGrainQualityIndex = newGrainQualityIndex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reGrainQualityIndex;
        }

        public GrainQualityIndex UpdGrainQualityIndex(GrainQualityIndex newGrainQualityIndex)
        {
            GrainQualityIndex reGrainQualityIndex = null;
            try
            {
                _grainQualityDal.Update(newGrainQualityIndex);
                reGrainQualityIndex = newGrainQualityIndex;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reGrainQualityIndex;
        }

        public GrainQualityIndex DelGrainQualityIndex(GrainQualityIndex delGrainQualityIndex)
        {
            try
            {
                if ((delGrainQualityIndex.ContractGrainQualityIndexes == null || delGrainQualityIndex.ContractGrainQualityIndexes.Count == 0) && (delGrainQualityIndex.AssayResults == null || delGrainQualityIndex.AssayResults.Count == 0) && (delGrainQualityIndex.SubGrainQualityIndex == null || delGrainQualityIndex.SubGrainQualityIndex.Count == 0) && (delGrainQualityIndex.TargetPrices == null || delGrainQualityIndex.TargetPrices.Count == 0))
                {
                    _grainQualityDal.Delete(delGrainQualityIndex);
                    return delGrainQualityIndex;
                }
                else
                {
                    return null;
                }
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public IEnumerable<GrainQualityIndex> GetQualityIndexWithGoodsKind(int goodsKind_id)
        {
            return _grainQualityDal.Find(gq => gq.grain_kind.HasValue && gq.grain_kind.Value == goodsKind_id).Entities;
        }

        #endregion

        public bool CheckQuyalityIndexNameExist(string QTestItemName)
        {
            var quilityIndex = _qualityIndexDal.Single(qi => qi.quality_index_name == QTestItemName);
            if (quilityIndex.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<QualityIndex> GetRootQTestItem()
        {
            return _qualityIndexDal.Find(qi => qi.parent_quality_index == null).Entities;
        }

        public bool CheckSampleNumberExist(string sampleNumber)
        {
            var sampleTemp = _sampleDal.Single(sm => sm.sample_number == sampleNumber);
            if (sampleTemp.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RefreshData()
        {
            this._grainQualityDal.RefreshData();
            this._qualityIndexDal.RefreshData();
            this._sampleDal.RefreshData();
            this._sampleDetailDal.RefreshData();
            this._sampleTypeDal.RefreshData();
        }

        public List<BusinessDisposeDetail> GetKouLiang(IList<AssayResult> theAssayResultList, int goodsKind_id, BusinessDispose theBusinessDispose)
        {
            //decimal kouLiang = 0;
            //decimal zengLiang = 0;
            decimal tempPercent = 0;
            List<GrainQualityIndex> theCheckGrainQualityIndexList = null;
            List<BusinessDisposeDetail> theBusinessDisposeDetailList = new List<BusinessDisposeDetail>();
            theCheckGrainQualityIndexList = _grainQualityDal.Find(grainQI => grainQI.grain_kind.Value == goodsKind_id && grainQI.more_than_deduct.HasValue).Entities.ToList();

            if (theCheckGrainQualityIndexList.Count > 0)
            {
                foreach (GrainQualityIndex gqi in theCheckGrainQualityIndexList)
                {
                    AssayResult theTempAssayResult = theAssayResultList.SingleOrDefault(ar => ar.GrainQualityIndex == gqi);
                    if (theTempAssayResult != null)
                    {
                        if (gqi.more_than_deduct.HasValue)
                        {
                            //如果是高了扣，低了增
                            if (gqi.more_than_deduct.Value)
                            {
                                if (!gqi.deduct_rate.HasValue || !gqi.deduct_value.HasValue)
                                {
                                    continue;
                                }
                                int buchang;
                                if (decimal.Parse(theTempAssayResult.assay_result_value) >= decimal.Parse(gqi.value))    //高了扣
                                {
                                    buchang = (int)((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.value)) / (gqi.deduct_rate.Value));
                                    tempPercent = buchang * gqi.deduct_value.Value;
                                    //kouLiang += buchang * gqi.deduct_value.Value;
                                    //如果有上限，而且超过上限，则以上限计算
                                    if (gqi.deduct_limit.HasValue)
                                        if ((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.value)) > gqi.deduct_limit.Value)
                                        {
                                            tempPercent = gqi.deduct_limit.Value;
                                            //kouLiang += buchang * gqi.deduct_value.Value;
                                        }
                                }
                                else  //低了增
                                {
                                    if (!gqi.increment_rate.HasValue || !gqi.increment_value.HasValue)
                                    {
                                        continue;
                                    }
                                    buchang = (int)((decimal.Parse(gqi.value) - decimal.Parse(theTempAssayResult.assay_result_value)) / (gqi.increment_rate.Value));
                                    tempPercent = 0 - buchang * gqi.increment_value.Value;
                                    //zengLiang += buchang * gqi.increment_value.Value;
                                    //如果有上限，而且超过上限，则以上限计算
                                    if (gqi.increment_limit.HasValue)
                                        if ((decimal.Parse(gqi.value) - decimal.Parse(theTempAssayResult.assay_result_value) >= gqi.increment_limit.Value))
                                        {
                                            tempPercent = 0 - gqi.increment_limit.Value;
                                            //zengLiang += buchang * gqi.increment_value.Value;
                                        }
                                }
                            }
                            //如果是低了扣，高了增
                            else
                            {
                                if (!gqi.deduct_rate.HasValue || !gqi.deduct_value.HasValue)
                                {
                                    continue;
                                }
                                int buchang;
                                if (decimal.Parse(theTempAssayResult.assay_result_value) <= decimal.Parse(gqi.value)) //低了扣
                                {
                                    buchang = (int)((decimal.Parse(gqi.value) - decimal.Parse(theTempAssayResult.assay_result_value)) / (gqi.deduct_rate.Value));
                                    tempPercent = buchang * gqi.deduct_value.Value;
                                    //kouLiang += buchang * gqi.deduct_value.Value;
                                    //如果有上限，而且超过上限，则以上限计算
                                    if (gqi.deduct_limit.HasValue)
                                        if ((decimal.Parse(gqi.value) - decimal.Parse(theTempAssayResult.assay_result_value) >= gqi.deduct_limit.Value))
                                        {
                                            tempPercent = gqi.deduct_limit.Value;
                                            //kouLiang += buchang * gqi.deduct_value.Value;
                                        }
                                }
                                else  //高了增
                                {
                                    if (!gqi.increment_rate.HasValue || !gqi.increment_value.HasValue)
                                    {
                                        continue;
                                    }
                                    buchang = (int)((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.value)) / (gqi.increment_rate.Value));
                                    tempPercent = 0 - buchang * gqi.increment_value.Value;
                                    //zengLiang += buchang * gqi.increment_value.Value;
                                    //如果有上限，而且超过上限，则以上限计算
                                    if (gqi.increment_limit.HasValue)
                                        if ((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.value) >= gqi.increment_limit.Value))
                                        {
                                            tempPercent = 0 - gqi.increment_limit.Value;
                                            //zengLiang += buchang * gqi.increment_value.Value;
                                        }
                                }
                            }
                            //水分 shuifen_deduct_percent、杂质zazhi_deduct_percent、整精米率zhengshaifei_deduct_percent
                            //谷外糙米qingzafei_deduct_percent、互混率second_shuifen_deduct_percent、黄粒米second_zazhi_deduct_percent
                            //出米率 second_qingzafei_deduct_percent
                            BusinessDisposeDetail theBusinessDD = new BusinessDisposeDetail();
                            theBusinessDD.dispose_count = tempPercent;
                            theBusinessDD.dispose_type = (int)BusinessDisposeDetailType.扣百分比;
                            theBusinessDD.grain_quality_index = gqi.index_id;
                            theBusinessDisposeDetailList.Add(theBusinessDD);
                        }
                    }
                }
            }
            int billID = 1;
            billID = _sysBillNoService.GetBillNoID("BUS", "BDN");
            theBusinessDispose.business_dispose_number = _sysBillNoService.GetBillNo(billID);
            string bdNumber = theBusinessDispose.business_dispose_number;
            theBusinessDispose.start_time = _getSysDateTimeService.GetSysDateTime();
            _unitOfWork.AddAction(theBusinessDispose, DataActions.Add);

            foreach (BusinessDisposeDetail bdd in theBusinessDisposeDetailList)
            {
                bdd.business_dispose_number = bdNumber;
                _unitOfWork.AddAction(bdd, DataActions.Add);
            }
            _unitOfWork.Save();
            return theBusinessDisposeDetailList;
        }
        public List<BusinessDisposeDetail> GetKouLiangWithCont(Contract theCont, IList<AssayResult> theAssayResultList, int goodsKind_id, BusinessDispose theBusinessDispose)
        {
            //decimal kouLiang = 0;
            //decimal zengLiang = 0;
            decimal tempPercent = 0;
            List<ContractGrainQualityIndex> theCheckGrainQualityIndexList = null;
            List<BusinessDisposeDetail> theBusinessDisposeDetailList = new List<BusinessDisposeDetail>();
            if (theCont != null && theCont.ContractGrainQualityIndexes != null && theCont.ContractGrainQualityIndexes.Count > 0)
            {
                theCheckGrainQualityIndexList = theCont.ContractGrainQualityIndexes.ToList();
            }

            try
            {
                if (theCheckGrainQualityIndexList.Count > 0)
                {
                    foreach (ContractGrainQualityIndex gqi in theCheckGrainQualityIndexList)
                    {
                        AssayResult theTempAssayResult = theAssayResultList.SingleOrDefault(ar => ar.GrainQualityIndex.name == gqi.GrainQualityIndex.name);
                        if (theTempAssayResult != null)
                        {
                            if (gqi.more_than_deduct.HasValue)
                            {
                                //如果是高了扣，低了增
                                if (gqi.more_than_deduct.Value)
                                {
                                    if (!gqi.deduct_rate.HasValue || !gqi.deduct_value.HasValue)
                                    {
                                        continue;
                                    }
                                    int buchang;
                                    if (decimal.Parse(theTempAssayResult.assay_result_value) >= decimal.Parse(gqi.contract_quality_index_value))    //高了扣
                                    {
                                        buchang = (int)((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.contract_quality_index_value)) / (gqi.deduct_rate.Value));
                                        tempPercent = buchang * gqi.deduct_value.Value;
                                        //kouLiang += buchang * gqi.deduct_value.Value;
                                        //如果有上限，而且超过上限，则以上限计算
                                        if (gqi.deduct_limit.HasValue)
                                            if ((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.contract_quality_index_value)) > gqi.deduct_limit.Value)
                                            {
                                                tempPercent = gqi.deduct_limit.Value;
                                                //kouLiang += buchang * gqi.deduct_value.Value;
                                            }
                                    }
                                    else  //低了增
                                    {
                                        if (!gqi.increment_rate.HasValue || !gqi.increment_value.HasValue)
                                        {
                                            continue;
                                        }
                                        buchang = (int)((decimal.Parse(gqi.contract_quality_index_value) - decimal.Parse(theTempAssayResult.assay_result_value)) / (gqi.increment_rate.Value));
                                        tempPercent = 0 - buchang * gqi.increment_value.Value;
                                        //zengLiang += buchang * gqi.increment_value.Value;
                                        //如果有上限，而且超过上限，则以上限计算
                                        if (gqi.increment_limit.HasValue)
                                            if ((decimal.Parse(gqi.contract_quality_index_value) - decimal.Parse(theTempAssayResult.assay_result_value) >= gqi.increment_limit.Value))
                                            {
                                                tempPercent = 0 - gqi.increment_limit.Value;
                                                //zengLiang += buchang * gqi.increment_value.Value;
                                            }
                                    }
                                }
                                //如果是低了扣，高了增
                                else
                                {
                                    if (!gqi.deduct_rate.HasValue || !gqi.deduct_value.HasValue)
                                    {
                                        continue;
                                    }
                                    int buchang;
                                    if (decimal.Parse(theTempAssayResult.assay_result_value) <= decimal.Parse(gqi.contract_quality_index_value)) //低了扣
                                    {
                                        buchang = (int)((decimal.Parse(gqi.contract_quality_index_value) - decimal.Parse(theTempAssayResult.assay_result_value)) / (gqi.deduct_rate.Value));
                                        tempPercent = buchang * gqi.deduct_value.Value;
                                        //kouLiang += buchang * gqi.deduct_value.Value;
                                        //如果有上限，而且超过上限，则以上限计算
                                        if (gqi.deduct_limit.HasValue)
                                            if ((decimal.Parse(gqi.contract_quality_index_value) - decimal.Parse(theTempAssayResult.assay_result_value) >= gqi.deduct_limit.Value))
                                            {
                                                tempPercent = gqi.deduct_limit.Value;
                                                //kouLiang += buchang * gqi.deduct_value.Value;
                                            }
                                    }
                                    else  //高了增
                                    {
                                        if (!gqi.increment_rate.HasValue || !gqi.increment_value.HasValue)
                                        {
                                            continue;
                                        }
                                        buchang = (int)((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.contract_quality_index_value)) / (gqi.increment_rate.Value));
                                        tempPercent = 0 - buchang * gqi.increment_value.Value;
                                        //zengLiang += buchang * gqi.increment_value.Value;
                                        //如果有上限，而且超过上限，则以上限计算
                                        if (gqi.increment_limit.HasValue)
                                            if ((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.contract_quality_index_value) >= gqi.increment_limit.Value))
                                            {
                                                tempPercent = 0 - gqi.increment_limit.Value;
                                                //zengLiang += buchang * gqi.increment_value.Value;
                                            }
                                    }
                                }
                                //水分 shuifen_deduct_percent、杂质zazhi_deduct_percent、整精米率zhengshaifei_deduct_percent
                                //谷外糙米qingzafei_deduct_percent、互混率second_shuifen_deduct_percent、黄粒米second_zazhi_deduct_percent
                                //出米率 second_qingzafei_deduct_percent
                                BusinessDisposeDetail theBusinessDD = new BusinessDisposeDetail();
                                theBusinessDD.dispose_count = tempPercent;
                                theBusinessDD.dispose_type = (int)BusinessDisposeDetailType.扣百分比;
                                theBusinessDD.grain_quality_index = gqi.grain_quality_index.Value;
                                theBusinessDisposeDetailList.Add(theBusinessDD);
                            }
                        }
                    }
                }
                int billID = 1;
                billID = _sysBillNoService.GetBillNoID("BUS", "BDN");
                theBusinessDispose.business_dispose_number = _sysBillNoService.GetBillNo(billID);
                string bdNumber = theBusinessDispose.business_dispose_number;
                theBusinessDispose.start_time = _getSysDateTimeService.GetSysDateTime();
                _unitOfWork.AddAction(theBusinessDispose, DataActions.Add);

                foreach (BusinessDisposeDetail bdd in theBusinessDisposeDetailList)
                {
                    bdd.business_dispose_number = bdNumber;
                    _unitOfWork.AddAction(bdd, DataActions.Add);
                }
                _unitOfWork.Save();
            }
            catch
            {

            }
            return theBusinessDisposeDetailList;
        }
        public List<BusinessDisposeDetail> GetKouLiangWithContWithoutSave(Contract theCont, IList<AssayResult> theAssayResultList, int goodsKind_id, BusinessDispose theBusinessDispose)
        {
            decimal tempPercent = 0;
            List<ContractGrainQualityIndex> theCheckGrainQualityIndexList = null;
            List<BusinessDisposeDetail> theBusinessDisposeDetailList = new List<BusinessDisposeDetail>();
            if (theCont != null && theCont.ContractGrainQualityIndexes != null && theCont.ContractGrainQualityIndexes.Count > 0)
            {
                theCheckGrainQualityIndexList = theCont.ContractGrainQualityIndexes.ToList();
            }
            try
            {
                if (theCheckGrainQualityIndexList != null && theCheckGrainQualityIndexList.Count > 0)
                {
                    foreach (ContractGrainQualityIndex gqi in theCheckGrainQualityIndexList)
                    {
                        AssayResult theTempAssayResult = theAssayResultList.SingleOrDefault(ar => ar.GrainQualityIndex.name == gqi.GrainQualityIndex.name);
                        if (theTempAssayResult != null)
                        {
                            if (gqi.more_than_deduct.HasValue)
                            {
                                //如果是高了扣，低了增 
                                if (gqi.more_than_deduct.Value)
                                {
                                    if (!gqi.deduct_rate.HasValue || !gqi.deduct_value.HasValue)
                                    {
                                        continue;
                                    }
                                    int buchang;
                                    if (decimal.Parse(theTempAssayResult.assay_result_value) >= decimal.Parse(gqi.contract_quality_index_value))    //高了扣
                                    {
                                        buchang = (int)((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.contract_quality_index_value)) / (gqi.deduct_rate.Value));
                                        tempPercent = buchang * gqi.deduct_value.Value;
                                        //kouLiang += buchang * gqi.deduct_value.Value;
                                        //如果有上限，而且超过上限，则以上限计算
                                        if (gqi.deduct_limit.HasValue)
                                            if ((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.contract_quality_index_value)) > gqi.deduct_limit.Value)
                                            {
                                                tempPercent = gqi.deduct_limit.Value;
                                                //kouLiang += buchang * gqi.deduct_value.Value;
                                            }
                                    }
                                    else  //低了增
                                    {
                                        if (!gqi.increment_rate.HasValue || !gqi.increment_value.HasValue)
                                        {
                                            continue;
                                        }
                                        buchang = (int)((decimal.Parse(gqi.contract_quality_index_value) - decimal.Parse(theTempAssayResult.assay_result_value)) / (gqi.increment_rate.Value));
                                        tempPercent = 0 - buchang * gqi.increment_value.Value;
                                        //zengLiang += buchang * gqi.increment_value.Value;
                                        //如果有上限，而且超过上限，则以上限计算
                                        if (gqi.increment_limit.HasValue)
                                            if ((decimal.Parse(gqi.contract_quality_index_value) - decimal.Parse(theTempAssayResult.assay_result_value) >= gqi.increment_limit.Value))
                                            {
                                                tempPercent = 0 - gqi.increment_limit.Value;
                                                //zengLiang += buchang * gqi.increment_value.Value;
                                            }
                                    }
                                }
                                //如果是低了扣，高了增
                                else
                                {
                                    if (!gqi.deduct_rate.HasValue || !gqi.deduct_value.HasValue)
                                    {
                                        continue;
                                    }
                                    int buchang;
                                    if (decimal.Parse(theTempAssayResult.assay_result_value) <= decimal.Parse(gqi.contract_quality_index_value)) //低了扣
                                    {
                                        buchang = (int)((decimal.Parse(gqi.contract_quality_index_value) - decimal.Parse(theTempAssayResult.assay_result_value)) / (gqi.deduct_rate.Value));
                                        tempPercent = buchang * gqi.deduct_value.Value;
                                        //kouLiang += buchang * gqi.deduct_value.Value;
                                        //如果有上限，而且超过上限，则以上限计算
                                        if (gqi.deduct_limit.HasValue)
                                            if ((decimal.Parse(gqi.contract_quality_index_value) - decimal.Parse(theTempAssayResult.assay_result_value) >= gqi.deduct_limit.Value))
                                            {
                                                tempPercent = gqi.deduct_limit.Value;
                                                //kouLiang += buchang * gqi.deduct_value.Value;
                                            }
                                    }
                                    else  //高了增
                                    {
                                        if (!gqi.increment_rate.HasValue || !gqi.increment_value.HasValue)
                                        {
                                            continue;
                                        }
                                        buchang = (int)((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.contract_quality_index_value)) / (gqi.increment_rate.Value));
                                        tempPercent = 0 - buchang * gqi.increment_value.Value;
                                        //zengLiang += buchang * gqi.increment_value.Value;
                                        //如果有上限，而且超过上限，则以上限计算
                                        if (gqi.increment_limit.HasValue)
                                            if ((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.contract_quality_index_value) >= gqi.increment_limit.Value))
                                            {
                                                tempPercent = 0 - gqi.increment_limit.Value;
                                                //zengLiang += buchang * gqi.increment_value.Value;
                                            }
                                    }
                                }
                                //水分 shuifen_deduct_percent、杂质zazhi_deduct_percent、整精米率zhengshaifei_deduct_percent
                                //谷外糙米qingzafei_deduct_percent、互混率second_shuifen_deduct_percent、黄粒米second_zazhi_deduct_percent
                                //出米率 second_qingzafei_deduct_percent
                                BusinessDisposeDetail theBusinessDD = new BusinessDisposeDetail();
                                theBusinessDD.dispose_count = tempPercent;
                                theBusinessDD.dispose_type = (int)BusinessDisposeDetailType.扣百分比;
                                theBusinessDD.grain_quality_index = gqi.grain_quality_index.Value;
                                theBusinessDisposeDetailList.Add(theBusinessDD);
                            }
                        }
                    }
                }
                else
                {
                    theBusinessDisposeDetailList = GetKouLiangWithoutSave(theAssayResultList, goodsKind_id, theBusinessDispose);
                }
            }
            catch
            {

            }
            return theBusinessDisposeDetailList;
        }
        public List<BusinessDisposeDetail> GetKouLiangWithoutSave(IList<AssayResult> theAssayResultList, int goodsKind_id, BusinessDispose theBusinessDispose)
        {
            //decimal kouLiang = 0;
            //decimal zengLiang = 0;
            decimal tempPercent = 0;
            List<GrainQualityIndex> theCheckGrainQualityIndexList = null;
            List<BusinessDisposeDetail> theBusinessDisposeDetailList = new List<BusinessDisposeDetail>();
            theCheckGrainQualityIndexList = _grainQualityDal.Find(grainQI => grainQI.grain_kind.Value == goodsKind_id && grainQI.more_than_deduct.HasValue).Entities.ToList();

            if (theCheckGrainQualityIndexList.Count > 0)
            {
                foreach (GrainQualityIndex gqi in theCheckGrainQualityIndexList)
                {
                    AssayResult theTempAssayResult = theAssayResultList.SingleOrDefault(ar => ar.grain_quality_index == gqi.index_id);
                    if (theTempAssayResult != null)
                    {
                        if (gqi.more_than_deduct.HasValue)
                        {
                            //如果是高了扣，低了增
                            if (gqi.more_than_deduct.Value)
                            {
                                if (!gqi.deduct_rate.HasValue || !gqi.deduct_value.HasValue)
                                {
                                    continue;
                                }
                                int buchang;
                                if (decimal.Parse(theTempAssayResult.assay_result_value) >= decimal.Parse(gqi.value))    //高了扣
                                {
                                    buchang = (int)((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.value)) / (gqi.deduct_rate.Value));
                                    tempPercent = buchang * gqi.deduct_value.Value;
                                    //kouLiang += buchang * gqi.deduct_value.Value;
                                    //如果有上限，而且超过上限，则以上限计算
                                    if (gqi.deduct_limit.HasValue)
                                        if ((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.value)) > gqi.deduct_limit.Value)
                                        {
                                            tempPercent = gqi.deduct_limit.Value;
                                            //kouLiang += buchang * gqi.deduct_value.Value;
                                        }
                                }
                                else  //低了增
                                {
                                    if (!gqi.increment_rate.HasValue || !gqi.increment_value.HasValue)
                                    {
                                        continue;
                                    }
                                    buchang = (int)((decimal.Parse(gqi.value) - decimal.Parse(theTempAssayResult.assay_result_value)) / (gqi.increment_rate.Value));
                                    tempPercent = 0 - buchang * gqi.increment_value.Value;
                                    //zengLiang += buchang * gqi.increment_value.Value;
                                    //如果有上限，而且超过上限，则以上限计算
                                    if (gqi.increment_limit.HasValue)
                                        if ((decimal.Parse(gqi.value) - decimal.Parse(theTempAssayResult.assay_result_value) >= gqi.increment_limit.Value))
                                        {
                                            tempPercent = 0 - gqi.increment_limit.Value;
                                            //zengLiang += buchang * gqi.increment_value.Value;
                                        }
                                }
                            }
                            //如果是低了扣，高了增
                            else
                            {
                                if (!gqi.deduct_rate.HasValue || !gqi.deduct_value.HasValue)
                                {
                                    continue;
                                }
                                int buchang;
                                if (decimal.Parse(theTempAssayResult.assay_result_value) <= decimal.Parse(gqi.value)) //低了扣
                                {
                                    buchang = (int)((decimal.Parse(gqi.value) - decimal.Parse(theTempAssayResult.assay_result_value)) / (gqi.deduct_rate.Value));
                                    tempPercent = buchang * gqi.deduct_value.Value;
                                    //kouLiang += buchang * gqi.deduct_value.Value;
                                    //如果有上限，而且超过上限，则以上限计算
                                    if (gqi.deduct_limit.HasValue)
                                        if ((decimal.Parse(gqi.value) - decimal.Parse(theTempAssayResult.assay_result_value) >= gqi.deduct_limit.Value))
                                        {
                                            tempPercent = gqi.deduct_limit.Value;
                                            //kouLiang += buchang * gqi.deduct_value.Value;
                                        }
                                }
                                else  //高了增
                                {
                                    if (!gqi.increment_rate.HasValue || !gqi.increment_value.HasValue)
                                    {
                                        continue;
                                    }
                                    buchang = (int)((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.value)) / (gqi.increment_rate.Value));
                                    tempPercent = 0 - buchang * gqi.increment_value.Value;
                                    //zengLiang += buchang * gqi.increment_value.Value;
                                    //如果有上限，而且超过上限，则以上限计算
                                    if (gqi.increment_limit.HasValue)
                                        if ((decimal.Parse(theTempAssayResult.assay_result_value) - decimal.Parse(gqi.value) >= gqi.increment_limit.Value))
                                        {
                                            tempPercent = 0 - gqi.increment_limit.Value;
                                            //zengLiang += buchang * gqi.increment_value.Value;
                                        }
                                }
                            }
                            //水分 shuifen_deduct_percent、杂质zazhi_deduct_percent、整精米率zhengshaifei_deduct_percent
                            //谷外糙米qingzafei_deduct_percent、互混率second_shuifen_deduct_percent、黄粒米second_zazhi_deduct_percent
                            //出米率 second_qingzafei_deduct_percent
                            BusinessDisposeDetail theBusinessDD = new BusinessDisposeDetail();
                            theBusinessDD.dispose_count = tempPercent;
                            theBusinessDD.dispose_type = (int)BusinessDisposeDetailType.扣百分比;
                            theBusinessDD.grain_quality_index = gqi.index_id;
                            theBusinessDisposeDetailList.Add(theBusinessDD);
                        }
                    }
                }
            }
            return theBusinessDisposeDetailList;
        }
        public bool AddKouLiangWithUnitOfWorkWithoutSave(BusinessDispose theBusinessDispose, List<BusinessDisposeDetail> theBusinessDisposeDetailList, ref string ErrorMessage)
        {
            try
            {
                int billID = 1;
                billID = _sysBillNoService.GetBillNoID("BUS", "BDN");
                theBusinessDispose.business_dispose_number = _sysBillNoService.GetBillNo(billID);
                string bdNumber = theBusinessDispose.business_dispose_number;
                theBusinessDispose.start_time = _getSysDateTimeService.GetSysDateTime();
                _unitOfWork.AddAction(theBusinessDispose, DataActions.Add);

                foreach (BusinessDisposeDetail bdd in theBusinessDisposeDetailList)
                {
                    bdd.business_dispose_number = bdNumber;
                    _unitOfWork.AddAction(bdd, DataActions.Add);
                }
                return true;
            }
            catch
            {
                ErrorMessage = "保存扣量信息出错";
                return false;
            }
        }
        public Assay AddSampleAndStepWithUnitOfWork(PlanTaskBatchStepStatus oneStep, PlanTaskBatch planTaskBatch, IList<GrainQualityIndex> grainQualityIndexList, List<string> resultList, int user_id, AssayStatus status, string AssayBillIndex, OrganizationDepartment org, int gradeID)
        {
            Contract cont = planTaskBatch.PlanTask.Enrolment.Contract;
            DateTime dt = _getSysDateTimeService.GetSysDateTime();
            Sample newSample = new Sample();
            if (planTaskBatch.Samples.Count > 0)
            {
                newSample = planTaskBatch.Samples.First();
            }
            else
            {
                //新建 Sample
                //newSample.assay_type = 1;
                //newSample.bill_owner_org = 1;
                newSample.enrolment = planTaskBatch.PlanTask.enrolment_number;
                newSample.goods_kind = planTaskBatch.PlanTask.Enrolment.goods_kind;
                newSample.plantask_batch_number = planTaskBatch.plantask_batch_number;
                newSample.sample_count = planTaskBatch.PlanTask.Enrolment.SampleType.sample_count.Value;
                int billID = _sysBillNoService.GetBillNoID("QUM", "SAN");
                newSample.sample_number = _sysBillNoService.GetBillNo(billID);
                newSample.sample_saved = planTaskBatch.PlanTask.Enrolment.SampleType.sample_saved;
                newSample.sample_time = dt;
                newSample.sample_type_code = planTaskBatch.PlanTask.Enrolment.sample_type_code;
                newSample.sample_user = user_id;
                //newSample.send_grain_unit = "unit";
                newSample.simple_source = 0;
                //newSample.warehouse_number = "";
                //newSample.warehouse_store = "";
                _unitOfWork.AddAction(newSample, DataActions.Add);
            }
            SampleDetail newSampleDetail = AddSampleDetailWithUnitofWork(newSample);
            //新建Assay            
            Assay newAssay = _assayService.AddAssayWithUnitOfWork(newSample, newSampleDetail, user_id, cont, grainQualityIndexList, resultList, status, AssayBillIndex, org, gradeID);
            //Step
            oneStep.operate_time = dt;
            _unitOfWork.AddAction(oneStep, DataActions.Add);
            _unitOfWork.Save();
            return newAssay;
        }
        public Assay AddSampleAndStepWithUnitOfWorkWithoutSave(List<PlanTaskBatchStepStatus> thePTBatchStepList, PlanTaskBatch planTaskBatch, IList<GrainQualityIndex> grainQualityIndexList, List<string> resultList, int user_id, AssayStatus status, string AssayBillIndex, OrganizationDepartment org, int gradeID)
        {
            Contract cont = planTaskBatch.PlanTask.Enrolment.Contract;
            DateTime dt = _getSysDateTimeService.GetSysDateTime();
            Sample newSample = new Sample();
            if (planTaskBatch.Samples.Count > 0)
            {
                newSample = planTaskBatch.Samples.First();
            }
            else
            {
                //新建 Sample
                //newSample.assay_type = 1;
                //newSample.bill_owner_org = 1;
                newSample.enrolment = planTaskBatch.PlanTask.enrolment_number;
                newSample.goods_kind = planTaskBatch.PlanTask.Enrolment.goods_kind;
                newSample.plantask_batch_number = planTaskBatch.plantask_batch_number;
                newSample.sample_count = planTaskBatch.PlanTask.Enrolment.SampleType == null ? 1 : planTaskBatch.PlanTask.Enrolment.SampleType.sample_count.Value;
                int billID = _sysBillNoService.GetBillNoID("QUM", "SAN");
                newSample.sample_number = _sysBillNoService.GetBillNo(billID);
                newSample.sample_saved = planTaskBatch.PlanTask.Enrolment.SampleType == null ? false : planTaskBatch.PlanTask.Enrolment.SampleType.sample_saved;
                newSample.sample_time = dt;
                newSample.sample_type_code = planTaskBatch.PlanTask.Enrolment.sample_type_code;
                newSample.sample_user = user_id;
                //newSample.send_grain_unit = "unit";
                newSample.simple_source = 0;
                //newSample.warehouse_number = "";
                //newSample.warehouse_store = "";
                _unitOfWork.AddAction(newSample, DataActions.Add);
            }
            SampleDetail newSampleDetail = AddSampleDetailWithUnitofWork(newSample);
            //新建Assay            
            Assay newAssay = _assayService.AddAssayWithUnitOfWork(newSample, newSampleDetail, user_id, cont, grainQualityIndexList, resultList, status, AssayBillIndex, org, gradeID);
            //Step
            foreach (PlanTaskBatchStepStatus tempPTBatchStep in thePTBatchStepList)
            {
                tempPTBatchStep.operate_time = dt;
                _unitOfWork.AddAction(tempPTBatchStep, DataActions.Add);
            }
            return newAssay;
        }
    }
}
