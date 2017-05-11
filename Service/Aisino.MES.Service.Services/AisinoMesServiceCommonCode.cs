using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aisino.MES.Service
{
    /// <summary>
    /// 作业系统自系统
    /// </summary>
    public class MESSystem
    {
        //设备管理系统
        public const string eqm = "EQM";
        //生产管理系统
        public const string man = "MAN";
        //仓储管理系统
        public const string whm = "WHM";
        //报港管理系统
        public const string enm = "ENM";
        //检化验系统
        public const string qum = "QUM";
        //商务处理单
        public const string bus = "BUS";
    }

    /// <summary>
    /// 系统默认单据前缀
    /// </summary>
    public class BillPrefix
    {
        /// <summary>
        /// 仓储保管帐
        /// </summary>
        public const string wsi = "WSI";

        /// <summary>
        /// 出入库记录
        /// </summary>
        public const string ioh = "IOH";
    }

    public class PlanTaskStepCode
    {
        public const string BAO_GANG_TJ = "报港单提交";
        public const string BAO_GANG_QR = "经营确认";
        public const string JI_HUA_BZ = "作业计划编制";
        public const string JI_HUA_ZX = "作业计划执行";
        public const string JIE_SUAN = "结算";
    }

    public class PlanTaskBatchStepCode
    {
        public const string QIAN_YANG = "扦样完成";
        public const string HUA_YAN_TJ = "化验结果提交";
        public const string HUA_YAN_SH = "化验单审核";
        public const string QUE_DING_DJ = "确定单价";
        public const string SHANG_WU = "商务处理";
        public const string MAO_ZHONG = "毛重";
        public const string PI_ZHONG = "皮重";
        public const string HUAN_CHE = "换车";
        public const string HUAN_CANG = "换仓";
        public const string HUAN_ZUOYE = "换作业点";
        public const string PI_CI_ZX = "批次执行";
        public const string PI_CI_WC = "批次完成";
        public const string PI_CI_JS = "结算";
        public const string ZC_XL = "卸粮";
        public const string ZC_ZL = "装粮";
    }
}
