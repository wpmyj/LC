using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Repository.Helpers;

namespace Aisino.MES.Service.QualityManager
{
    public interface IAssayService
    {

        #region Assay 服务
        /// <summary>
        /// 新增化验单
        /// </summary>
        /// <param name="newAssay">化验单</param>
        /// <returns>新增后的化验单</returns>
        Assay AddAssay(Assay newAssay);
        /// <summary>
        /// 事务中新增化验单
        /// </summary>
        /// <param name="newSample">事务中的扦样</param>
        /// <param name="newSampleDetail">事务中的扦样明细</param>
        /// <param name="user_id">操作人员</param>
        /// <param name="cont">样品份数</param>
        /// <param name="GrainQualityIndexList">对应粮食质量指标</param>
        /// <param name="resultList">检测结果值</param>
        /// <param name="status">化验单状态</param>
        /// <param name="AssayBillIndex">化验单明细尾号</param>
        /// <returns>新增后的化验单</returns>
        Assay AddAssayWithUnitOfWork(Sample newSample, SampleDetail newSampleDetail, int user_id, Contract cont,
                                    IList<GrainQualityIndex> GrainQualityIndexList, List<string> resultList, AssayStatus status, string AssayBillIndex, OrganizationDepartment org, int gradeID);
        /// <summary>
        /// 更新化验单
        /// </summary>
        /// <param name="newAssay">需要更新的化验单</param>
        /// <returns>更新后的化验单</returns>
        Assay UpdAssay(Assay newAssay);
        /// <summary>
        /// 在事务中更新化验单
        /// </summary>
        /// <param name="oldAssay">事务中的化验单</param>
        /// <param name="org">组织机构</param>
        /// <returns>更新后的化验单</returns>
        Assay UpdAssayWithUnitOfWork(Assay oldAssay, OrganizationDepartment org);
        /// <summary>
        /// 事务中更新化验单
        /// </summary>
        /// <param name="assay">事务中的化验单</param>
        /// <param name="assayBill">事务中的化验单明细</param>
        /// <param name="assayResults">事务中的化验结果</param>
        /// <param name="grainQualityIndexList">事务中的粮食质量指标</param>
        /// <param name="resultList">化验结果的具体值</param>
        /// <param name="status">化验单状态</param>
        /// <returns>更新后的化验单</returns>
        Assay UpdAssayWithUnitOfWork(Assay assay, AssayBill assayBill, IList<AssayResult> assayResults,
                                    IList<GrainQualityIndex> grainQualityIndexList, List<string> resultList, AssayStatus status);
        /// <summary>
        /// 删除化验单
        /// </summary>
        /// <param name="delAssay">需要删除的化验单</param>
        void DelAssay(Assay delAssay);
        /// <summary>
        /// 事务中删除化验单
        /// </summary>
        /// <param name="assay">需要删除的化验单</param>
        void DelAssayWithUnitOfWork(Assay assay);
        /// <summary>
        /// 获取所有化验单
        /// </summary>
        /// <returns>所有化验单</returns>
        IEnumerable<Assay> GetAllAssay();
        /// <summary>
        /// 根据SQL获取所有化验单
        /// </summary>
        /// <param name="sqlStr">SQL命令字符串</param>
        /// <returns>所有对应的化验单</returns>
        IEnumerable<Assay> GetAssaysBySQLStr(string sqlStr);
        /// <summary>
        /// 根据条件参数获取化验单
        /// </summary>
        /// <param name="para">条件参数：粮食品种、确认时间、化验单状态</param>
        /// <param name="pageC">页面控件</param>
        /// <returns>对应化验单</returns>
        RepositoryResultList<Assay> GetAssaysByParameter(string[] para, PagingCriteria pageC);
        /// <summary>
        /// 获取化验单编号
        /// </summary>
        /// <returns>化验单编号</returns>
        string GetAssayTempNumber();
        /// <summary>
        /// 根据化验单编号获取化验单
        /// </summary>
        /// <param name="assayNumber">化验单编号</param>
        /// <returns>对应化验单</returns>
        Assay GetAssayByAssayNumber(string assayNumber);
        /// <summary>
        /// 查看化验单编号是否已存在
        /// </summary>
        /// <param name="assayNumber">化验单编号</param>
        /// <returns>True表示已存在，False表示不存在</returns>
        bool CheckAssayNumberExist(string assayNumber);
        /// <summary>
        /// 刷新化验单数据
        /// </summary>
        void AssayRefresh();
        #endregion

        #region AssayBasicData 服务
        /// <summary>
        /// 新增化验相关数据
        /// </summary>
        /// <param name="newAssayBasicData">化验相关数据</param>
        /// <returns>新增后的化验相关数据</returns>
        AssayBasicData AddAssayBasicData(AssayBasicData newAssayBasicData);
        /// <summary>
        /// 更新化验相关数据
        /// </summary>
        /// <param name="newAssayBasicData">需要更新的化验相关数据</param>
        /// <returns>更新后的化验相关数据</returns>
        AssayBasicData UpdAssayBasicData(AssayBasicData newAssayBasicData);
        /// <summary>
        /// 删除化验相关数据
        /// </summary>
        /// <param name="delAssayBasicData">需要删除的化验相关数据</param>
        void DelAssayBasicData(AssayBasicData delAssayBasicData);
        /// <summary>
        /// 获取所有粮食等级
        /// </summary>
        /// <returns>所有粮食等级</returns>
        IEnumerable<AssayBasicData> GetGrainGrade();
        /// <summary>
        /// 判断粮食等级
        /// </summary>
        /// <param name="arList">化验结果</param>
        /// <param name="org">组织机构</param>
        /// <param name="cont">合同</param>
        /// <returns>正常等级1、2、3、4、5、6，组织机构没有定粮食等级标准值返回0，未获取等级数据返回-1， 异常返回-2</returns>
        int JudgeGrainGrade(IList<AssayResult> arList, OrganizationDepartment org, int goodsKindID, out decimal unitPrice);

        decimal GetUnitPrice(int grain_kind, int grainGrade);
        #endregion

        #region AssayBill 服务
        /// <summary>
        /// 新增化验单明细
        /// </summary>
        /// <param name="newAssayBill">新增化验单明细</param>
        /// <returns>新增后的化验单明细</returns>
        AssayBill AddAssayBill(AssayBill newAssayBill);
        /// <summary>
        /// 事务中新增化验单明细（此方法中无Save，仅有事务）
        /// </summary>
        /// <param name="newAssay">化验单</param>
        /// <param name="user_id">操作员</param>
        /// <param name="newSampleDetail">扦样明细</param>
        /// <param name="cont">合同</param>
        /// <param name="GrainQualityIndexList">粮食质量指标</param>
        /// <param name="resultList">结果值</param>
        /// <param name="AssayBillIndex">化验单明细尾号</param>
        void AddAssayBillWithUnitOfWork(Assay newAssay, int user_id, SampleDetail newSampleDetail, Contract cont, IList<GrainQualityIndex> GrainQualityIndexList, List<string> resultList, string AssayBillIndex);
        /// <summary>
        /// 事务中新增化验单明细（此方法中有Save）
        /// </summary>
        /// <param name="oldAssay">已存在的化验单</param>
        /// <param name="user_id">操作员</param>
        /// <param name="billIndex">化验单明细尾号</param>
        /// <param name="cont">合同</param>
        /// <param name="GrainQualityIndexList">粮食质量指标</param>
        /// <param name="resultList">结果值</param>
        /// <returns>新增后的化验单明细</returns>
        AssayBill AddAssayBillWithSave(Assay oldAssay, int user_id, string billIndex, Contract cont, IList<GrainQualityIndex> GrainQualityIndexList, List<string> resultList);
        /// <summary>
        /// 更新化验单明细
        /// </summary>
        /// <param name="newAssayBill">需要更新的化验单明细</param>
        /// <returns>更新后的化验单明细</returns>
        AssayBill UpdAssayBill(AssayBill newAssayBill);
        /// <summary>
        /// 删除化验单明细
        /// </summary>
        /// <param name="delAssayBill">需要删除的化验单明细</param>
        void DelAssayBill(AssayBill delAssayBill);
        /// <summary>
        /// 事务中删除化验单明细
        /// </summary>
        /// <param name="delAssay">需要删除的化验单</param>
        void DelAssayBillWithUnitOfWork(Assay delAssay);
        /// <summary>
        /// 查看化验单明细编号是否已经存在
        /// </summary>
        /// <param name="AssayBillNumber">化验单明细编号</param>
        /// <returns>True表示已存在，False表示不存在</returns>
        bool CheckAssayBillNumberExist(string AssayBillNumber);
        #endregion

        #region AssayResult  服务
        /// <summary>
        /// 新增化验结果
        /// </summary>
        /// <param name="newAssayResult">需要新增的化验结果</param>
        /// <returns>新增后的化验结果</returns>
        AssayResult AddAssayResult(AssayResult newAssayResult);
        /// <summary>
        /// 事务中新增化验结果
        /// </summary>
        /// <param name="newAssayBill">化验单明细</param>
        /// <param name="cont">合同</param>
        /// <param name="GrainQualityIndexList">粮食质量指标</param>
        /// <param name="resultList">结果值</param>
        void AddAssayResultWithUnitOfWork(AssayBill newAssayBill, Contract cont, IList<GrainQualityIndex> GrainQualityIndexList, List<string> resultList);
        /// <summary>
        /// 更新化验结果
        /// </summary>
        /// <param name="newAssayResult">需要更新的化验结果</param>
        /// <returns>更新后的化验结果</returns>
        AssayResult UpdAssayResult(AssayResult newAssayResult);
        /// <summary>
        /// 删除化验结果
        /// </summary>
        /// <param name="delAssayResult">需要删除的化验结果</param>
        void DelAssayResult(AssayResult delAssayResult);
        /// <summary>
        /// 事务中删除化验结果
        /// </summary>
        /// <param name="DelAssayBill">需要删除的化验单明细</param>
        void DelAssayResultWithUnitOfWork(AssayBill DelAssayBill);
        /// <summary>
        /// 判定是否合格
        /// </summary>
        /// <param name="gqi">粮食质量指标</param>
        /// <param name="value">结果值</param>
        /// <returns>True表示合格，False表示不合格</returns>
        string IsPassed(GrainQualityIndex gqi, string value);
        #endregion

        #region AssayType 服务
        /// <summary>
        /// 新增扦样类型
        /// </summary>
        /// <param name="newAssayType">需要新增的扦样类型</param>
        /// <returns>新增后的扦样类型</returns>
        AssayType AddAssayType(AssayType newAssayType);
        /// <summary>
        /// 更新扦样类型
        /// </summary>
        /// <param name="newAssayType">需要更新的扦样类型</param>
        /// <returns>更新后的扦样类型</returns>
        AssayType UpdAssayType(AssayType newAssayType);
        /// <summary>
        /// 删除扦样类型
        /// </summary>
        /// <param name="delAssayType">需要删除的扦样类型</param>
        void DelAssayType(AssayType delAssayType);
        /// <summary>
        /// 根据扦样类型的AssayID获取对应扦样类型
        /// </summary>
        /// <param name="AssayID">扦样类型ID</param>
        /// <returns>对应的扦样类型</returns>
        AssayType GetSingleAssayType(int AssayID);
        /// <summary>
        /// 刷新扦样类别数据
        /// </summary>
        void RefreshAssayTypeData();
        #endregion

        /// <summary>
        /// 刷新数据
        /// </summary>
        void RefreshData();

        IEnumerable<AssayBasicData> GetGrainType();

        Assay UpdAssayConfirmWithUnitofWork(Assay oldAssay);

        Assay UpdAssayAndStepWithUnitOfWork(Assay oldAssay, OrganizationDepartment org, PlanTaskBatchStepStatus twoStep);

        Assay UpdAssayAndStep3WithUnitOfWork(Assay oldAssay, PlanTaskBatchStepStatus threeStep);

        Assay UpdAssayAndStepWithUnitOfWork(Assay oldAssay, PlanTaskBatchStepStatus twoStep, PlanTaskBatchStepStatus threeStep);

        void UpdClearAssayResultAndAddNew(AssayBill theAssayBill, List<AssayResult> theAssayResultList, IList<GrainQualityIndex> theGrainQualityIndexList);

        void UpdAssayPlanTaskInUnitOfWork(Assay oldAssay, PlanTask oldPlanTask, PlanTaskBatch oldPlanTaskBatch);

        void UpdBusinessAndDetailsInUnitOfWork(BusinessDispose oldBusinessDispose, List<BusinessDisposeDetail> theBusDisList);
    }
}
