using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.QualityManager
{
    public interface ISampleService
    {
        #region SampleType 服务
        /// <summary>
        /// 新增一个扦样类别
        /// </summary>
        /// <param name="newSampleType">新增的扦样类别</param>
        /// <returns>新增成功的扦样类别</returns>
        SampleType AddSampleType(SampleType newSampleType);
        /// <summary>
        /// 更新扦样类别
        /// </summary>
        /// <param name="newSampleType">需要更新的扦样类别实例</param>
        /// <returns>更新后的扦样类别</returns>
        SampleType UpdSampleType(SampleType newSampleType);
        /// <summary>
        /// 删除扦样类别
        /// </summary>
        /// <param name="delSampleType">需要删除的扦样类别实例</param>
        void DelSampleType(SampleType delSampleType);
        /// <summary>
        /// 获取所有扦样类别
        /// </summary>
        /// <returns>所有扦样类别</returns>
        IEnumerable<SampleType> GetAllSampleTypeList();
        /// <summary>
        /// 根据SQL字符串获取对应扦样类别
        /// </summary>
        /// <param name="sqlString">SQL命令字符串</param>
        /// <returns>对应扦样类别</returns>
        IEnumerable<SampleType> GetSampleTypeBySQL(string sqlString);
        /// <summary>
        /// 查看此扦样类别编号是否已存在
        /// </summary>
        /// <param name="sampleTypeCode">扦样类别编号</param>
        /// <returns>True表示已存在，False表示不存在</returns>
        bool CheckSampleTypeCodeExist(string sampleTypeCode);
        /// <summary>
        /// 查看此扦样类别名称是否已存在
        /// </summary>
        /// <param name="sampleTypeName">扦样类别名称</param>
        /// <returns>True表示已存在，False表示不存在</returns>
        bool CheckSampleTypeNameExist(string sampleTypeName);
        #endregion

        #region Sample 服务
        /// <summary>
        /// 新增一个扦样Sample，不包含其他
        /// </summary>
        /// <param name="newSample"></param>
        /// <returns>新增的扦样</returns>
        Sample AddSample(Sample newSample);
        /// <summary>
        /// 根据作业批次和当前人员创建扦样记录
        /// </summary>
        /// <param name="planTaskBatch">计划批次</param>
        /// <param name="employee">人员</param>
        /// <returns></returns>
        Sample AddSample(PlanTaskBatch planTaskBatch, OrganizationEmployee employee, int sampleCount);
        /// <summary>
        /// 新建一个完整的扦样(Sample)事务,其中包含SampleDetail，Assay，AssayBill，AssayResult
        /// </summary>
        /// <param name="planTaskBatch">计划批次</param>
        /// <param name="grainQualityIndexList">粮食质量指标项</param>
        /// <param name="resultList">结果值</param>
        /// <param name="user_id">操作员工id</param>
        /// <param name="status">化验单状态</param>
        /// <param name="AssayBillIndex">化验单尾号</param>
        /// <returns>新增的扦样</returns>
        Assay AddSampleWithUnitOfWork(PlanTaskBatch planTaskBatch, IList<GrainQualityIndex> grainQualityIndexList, List<string> resultList, int user_id, AssayStatus status, string AssayBillIndex, OrganizationDepartment org, int gradeID);
        /// <summary>
        /// 更新一个扦样Sample实例，不包含其他
        /// </summary>
        /// <param name="newSample">要更新的扦样实例</param>
        /// <returns>更新后的扦样实例</returns>
        Sample UpdSample(Sample newSample);
        /// <summary>
        /// 删除扦样
        /// </summary>
        /// <param name="delSample">要删除的扦样实例</param>
        void DelSample(Sample delSample);
        /// <summary>
        /// 获取扦样编号
        /// </summary>
        /// <returns>扦样编号</returns>
        string GetSempleNumber();
        /// <summary>
        /// 获取所有扦样
        /// </summary>
        /// <returns>所有扦样</returns>
        IEnumerable<Sample> GetAllSampleList();
        /// <summary>
        /// 查看此样品是否已存在
        /// </summary>
        /// <param name="sampleNumber">样品的编号</param>
        /// <returns>True表示已存在，False表示不存在</returns>
        bool CheckSampleNumberExist(string sampleNumber);
        /// <summary>
        /// 刷新扦样信息
        /// </summary>
        void RefreshSampleData();
        #endregion

        #region SampleDetail 服务
        /// <summary>
        /// 新增一个扦样明细
        /// </summary>
        /// <param name="newSampleDetail">扦样明细实例</param>
        /// <returns>新增后的扦样明细实例</returns>
        SampleDetail AddSampleDetail(SampleDetail newSampleDetail);
        /// <summary>
        /// 在事务中新增一个扦样明细
        /// </summary>
        /// <param name="theSample">事务中的扦样明细实例</param>
        /// <returns>事务后的扦样明细实例</returns>
        SampleDetail AddSampleDetailWithUnitofWork(Sample theSample);
        /// <summary>
        /// 更新扦样明细
        /// </summary>
        /// <param name="newSampleDetail">需要更新的扦样明细实例</param>
        /// <returns>更新后的扦样明细实例</returns>
        SampleDetail UpdSampleDetail(SampleDetail newSampleDetail);
        /// <summary>
        /// 删除扦样明细
        /// </summary>
        /// <param name="delSampleDetail">需要删除的扦样明细实例</param>
        void DelSampleDetail(SampleDetail delSampleDetail);
        #endregion

        #region QualityIndex 服务
        /// <summary>
        /// 新增质量指标
        /// </summary>
        /// <param name="newQualityIndex">质量指标实例</param>
        /// <returns>新增后的质量指标</returns>
        QualityIndex AddQualityIndex(QualityIndex newQualityIndex);
        /// <summary>
        /// 更新质量指标
        /// </summary>
        /// <param name="newQualityIndex">需要更新的质量指标实例</param>
        /// <returns>更新后的质量指标</returns>
        QualityIndex UpdQualityIndex(QualityIndex newQualityIndex);
        /// <summary>
        /// 删除质量指标
        /// </summary>
        /// <param name="delQualityIndex">需要删除的质量指标</param>
        QualityIndex DelQualityIndex(QualityIndex delQualityIndex);
        /// <summary>
        /// 获取所有质量指标
        /// </summary>
        /// <returns>所有的质量指标</returns>
        IEnumerable<QualityIndex> GetAllQualityIndex();
        /// <summary>
        /// 查看此质量指标名称是否已存在
        /// </summary>
        /// <param name="QTestItemName">质量指标名称</param>
        /// <returns>True表示已存在，False表示不存在</returns>
        bool CheckQuyalityIndexNameExist(string QTestItemName);
        /// <summary>
        /// 获取所有根节点质量指标
        /// </summary>
        /// <returns>所有根节点质量指标</returns>
        IEnumerable<QualityIndex> GetRootQTestItem();

        //add by yangyang 2014-04-22
        //比较值之间的逻辑关系控制
        bool CheckQTestCompareValue(string qualityIndexAssess, string qualityIndexValue, bool flag, string qualityIndexAssessAnd, string qualityIndexValueAnd);


        #endregion

        #region GrainQualityIndex 服务
        /// <summary>
        /// 新增粮食质量指标
        /// </summary>
        /// <param name="newGrainQualityIndex">粮食质量指标</param>
        /// <returns>新增后的粮食质量指标</returns>
        GrainQualityIndex AddGrainQualityIndex(GrainQualityIndex newGrainQualityIndex);
        /// <summary>
        /// 更新粮食质量指标
        /// </summary>
        /// <param name="newGrainQualityIndex">需要更新的粮食质量指标</param>
        /// <returns>更新后的粮食质量指标</returns>
        GrainQualityIndex UpdGrainQualityIndex(GrainQualityIndex newGrainQualityIndex);
        /// <summary>
        /// 删除粮食质量指标
        /// </summary>
        /// <param name="delGrainQualityIndex">需要删除的粮食质量指标</param>
        GrainQualityIndex DelGrainQualityIndex(GrainQualityIndex delGrainQualityIndex);
        /// <summary>
        /// 根据粮食品种获取所有的粮食质量指标
        /// </summary>
        /// <param name="goodsKind_id">粮食品种编号</param>
        /// <returns>对应粮食的所有粮食质量指标</returns>
        IEnumerable<GrainQualityIndex> GetQualityIndexWithGoodsKind(int goodsKind_id);

        #endregion

        /// <summary>
        /// 刷新数据
        /// </summary>
        void RefreshData();

        List<BusinessDisposeDetail> GetKouLiang(IList<AssayResult> theAssayResultList, int goodsKind_id, BusinessDispose theBusinessDispose);

        List<BusinessDisposeDetail> GetKouLiangWithCont(Contract theCont, IList<AssayResult> theAssayResultList, int goodsKind_id, BusinessDispose theBusinessDispose);

        List<BusinessDisposeDetail> GetKouLiangWithContWithoutSave(Contract theCont, IList<AssayResult> theAssayResultList, int goodsKind_id, BusinessDispose theBusinessDispose);

        List<BusinessDisposeDetail> GetKouLiangWithoutSave(IList<AssayResult> theAssayResultList, int goodsKind_id, BusinessDispose theBusinessDispose);

        /// <summary>
        /// 一站式收购保存扣量信息
        /// </summary>
        /// <param name="theBusinessDispose">扣量表头</param>
        /// <param name="theBusinessDisposeDetailList">扣量明细</param>
        /// <param name="ErrorMessage">错误信息</param>
        /// <returns>是否出现异常</returns>
        bool AddKouLiangWithUnitOfWorkWithoutSave(BusinessDispose theBusinessDispose, List<BusinessDisposeDetail> theBusinessDisposeDetailList, ref string ErrorMessage);

        Assay AddSampleAndStepWithUnitOfWork(PlanTaskBatchStepStatus oneStep, PlanTaskBatch planTaskBatch, IList<GrainQualityIndex> grainQualityIndexList, List<string> resultList, int user_id, AssayStatus status, string AssayBillIndex, OrganizationDepartment org, int gradeID);
        Assay AddSampleAndStepWithUnitOfWorkWithoutSave(List<PlanTaskBatchStepStatus> thePTBatchStepList, PlanTaskBatch planTaskBatch, IList<GrainQualityIndex> grainQualityIndexList, List<string> resultList, int user_id, AssayStatus status, string AssayBillIndex, OrganizationDepartment org, int gradeID);
    }
}
