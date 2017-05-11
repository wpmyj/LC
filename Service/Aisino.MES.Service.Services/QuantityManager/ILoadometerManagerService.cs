using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using System.Collections;
using Aisino.MES.DAL.Repository.Repositories;

namespace Aisino.MES.Service.QuantityManager
{
    public interface ILoadometerManagerService
    {
        /// <summary>
        /// 根据标签号判断是否可以上磅
        /// </summary>
        /// <param name="VehicleRFIDTag">rfid标签号</param>
        /// <param name="TagType">标签类型，值为mainid代表13.56的，值为subid代表900M</param>
        /// <param name="ScaleID">磅秤编号</param>
        /// <returns></returns>
        CheckResult CheckVehicleWeight(string VehicleRFIDTag, string TagType, string ScaleID);

        /// <summary>
        /// 更新称重记录
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="plantaskBatchNumber">对应计划批次编号</param>
        /// <param name="plantaskBatchId">计划批次genetorId</param>
        /// <param name="warehouseId">仓房编号</param>
        /// <param name="plateNumber">车船号</param>
        /// <param name="weightMode">称重模式：1先毛后皮，2先皮后毛</param>
        /// <param name="scaleId">磅点id</param>
        /// <param name="weight">重量</param>
        /// <param name="weightType">称重类型：0皮重，1毛重</param>
        /// <param name="operatorUser">司磅员</param>
        /// <returns>创建或保存成功的称重记录单头</returns>
        QuantityRecordHead UpdateQuantity(string tagId, string plantaskBatchNumber, int plantaskBatchId, int warehouseId, string plateNumber, int weightMode, string scaleId, int weight, int weightType, int operatorUser, DateTime weightTime, ref string strTime);

        /// <summary>
        /// 撤销磅单
        /// </summary>
        /// <param name="ScaleBillNumber"></param>
        /// <returns></returns>
        UploadDataResult CancelVehicleScaleBill(int ScaleBillNumber);

        /// <summary>
        /// 根据称重日期和磅点编号查找对应的称重记录
        /// </summary>
        /// <param name="weightDate">称重日期</param>
        /// <param name="scaleId">磅点编号</param>
        /// <param name="withCancel">是否包含已废除的</param>
        /// <returns>称重记录</returns>
        IEnumerable<QuantityRecordHead> FindQuantityHeadByDateAndScale(DateTime weightDate,string scaleId,bool withCancel);

        /// <summary>
        /// 根据主键获得对应的磅单
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>相应磅单</returns>
        QuantityRecordHead GetQuantityHeadByID(int id);

        /// <summary>
        /// 根据作业批次统计称重信息
        /// </summary>
        /// <param name="PlanTaskBatchNums"></param>
        /// <returns></returns>
        RepositoryResultList<QuantityRecordHead> GetQuantityHeadByBatchNums(String[] PlanTaskBatchNums);

        /// <summary>
        /// 根据条件统计称重信息
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="warehouseid"></param>
        /// <returns></returns>
        RepositoryResultList<QuantityRecordHead> GetQuantityHeadByConditions(DateTime starttime, DateTime endtime, Warehouse selectwarehouse);
    

    }
}
