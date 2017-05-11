using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model
{
    #region 基础信息管理
    /// <summary>
    /// 标签类型
    /// </summary>
    public enum TagType
    {
        _1356 = 1,
        _900 = 2,
        _433 = 3,
        人工录入无标签实体 = 99
    }

    /// <summary>
    /// 标签分类
    /// </summary>
    public enum TagFlag
    {
        员工卡 = 1,
        业务卡 = 2
    }

    public enum State
    {
        启用 = 1,
        停用 = 2
    }

    /// <summary>
    /// 交易类型
    /// PS:对于交易类单据而言，始终会产生数据增减
    /// </summary>
    public enum TransType
    {
        库存增量 = 1,
        库存扣量 = 2
    }

    /// <summary>
    /// 编号类型
    /// </summary>
    public enum CodeType
    {
        包装类型 = 1,
        运输工具 = 2,
        付款方式 = 3,
        信用等级 = 4,
        付款周期 = 5,
        客户类型 = 6
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        消息 = 1,
        错误 = 2
    }
    #endregion

    #region 系统管理
    /// <summary>
    /// 部门属性中的类型
    /// </summary>    
    public enum DeparrmentType
    {
        组织机构 = 0,
        下属部门 = 1
    }
    /// <summary>
    /// 操作方法类型
    /// </summary>
    public enum FunctionType
    {
        模块构造方法 = 1,
        操作方法 = 2
    }

    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum MenuType
    {
        模块菜单 = 1,
        操作菜单 = 2,
        显示菜单 = 3
    }

    /// <summary>
    /// 人员信息性别
    /// </summary>
    public enum UserSex
    {
        男 = 1,
        女 = 2
    }

    public enum SysCodeType
    {
        包装类型 = 1,
        运输工具 = 2
    }

    public enum SysBillState
    {
        启用 = 1,
        停用 = 2
    }

    public enum MetroTypes
    {
        Single = 1,
        Double = 2,
        Quadruple = 3
    }
    #endregion

    #region 仓储管理
    /// <summary>
    /// 交易类型
    /// 1：入库
    /// 2：出库
    /// </summary>
    public enum InOutType
    {
        入库 = 1,
        出库 = 2,
        调拨 = 3,
        未设置 = 4
    }
    /// <summary>
    /// 仓房状态
    /// </summary>
    public enum WarehouseStatus
    {
        空仓 = 1,
        封仓 = 2,
        作业 = 3
    }
    /// <summary>
    /// 批次管理原则
    /// </summary>
    public enum WarehouseBatchPrinciple
    {
        先入先出 = 1,
        后入先出 = 2
    }

    /// <summary>
    /// 单据状态
    /// </summary>
    public enum WarehouseTransactionMasterStatus
    {
        新建 = 1,
        提交 = 2,
        确认 = 3,
        作废 = 99
    }

    /// <summary>
    /// 执行操作
    /// </summary>
    public enum Operate
    {
        确认 = 1,
        回滚 = 2
    }

    //public enum WarehouseBatchPrinciple
    //{
    //    库存增量 = 1,
    //    库存扣量 = 2
    //}
    #endregion

    #region 设备管理
    /// <summary>
    /// 设备状态
    /// </summary>
    public enum EquipmentStatus
    {
        正常 = 1,
        保养 = 2,
        检修 = 3,
        大修 = 4,
        报废 = 99
    }
    #endregion

    #region 质量管理
    /// <summary>
    /// 化验单状态
    /// </summary>
    public enum AssayMasterStatus
    {
        等待化验 = 1,
        化验中 = 2,
        化验完成 = 3
    }

    /// <summary>
    /// 
    /// </summary>
    public enum DeductType
    {
        百分比扣量 = 1,
        直接扣量 = 2
    }

    /// <summary>
    /// 增量类型
    /// </summary>
    public enum IncreaseType
    {
        百分比扣量 = 1,
        直接扣量 = 2
    }

    /// <summary>
    /// 比较方式
    /// </summary>
    public enum CompareType
    {
        大于 = 1,
        大于等于 = 2,
        等于 = 3,
        小于 = 4,
        小于等于 = 5,
        不等于 = 6,
        区间 = 7
    }

    /// <summary>
    /// 下一指标项组合方式
    /// </summary>
    public enum NextCompareFlag
    {
        or = 1,
        and = 2
    }
    #endregion

    #region 生产管理
    /// <summary>
    /// 生产计划状态
    /// </summary>
    public enum ManuPlanTaskStatus
    {

        草拟 = 1,
        提交 = 2,
        废除 = 3,
        确认 = 4,
        下达 = 5,
        执行 = 6,
        暂停 = 7,
        终止 = 8,
        完成 = 9
    }

    /// <summary>
    /// 计划批次状态
    /// </summary>
    public enum ManuPlanTaskBatchStatus
    {

        下达 = 1,
        执行 = 2,
        暂停 = 3,
        终止 = 4,
        完成 = 5

    }

    /// <summary>
    /// 计划类别类型
    /// </summary>
    public enum PlanTaskMode
    {

        出入库计划 = 1,
        生产计划 = 2

    }

    /// <summary>
    /// 称重计量形式
    /// </summary>
    public enum WeightMode
    {

        先毛后皮 = 1,
        先皮后毛 = 2,
        第一次皮重基准 = 3,
        最后一次皮重基准 = 4

    }
    #endregion

}
