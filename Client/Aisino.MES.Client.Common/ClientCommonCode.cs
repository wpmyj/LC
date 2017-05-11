using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aisino.MES.Client.Common
{
    public class ClientCommonCode
    {
    }

    [FlagsAttribute]
    public enum OperationMode
    {
        /// <summary>
        /// 无模式
        /// </summary>
        None = 0,
        /// <summary>
        /// 新增模式
        /// </summary>
        AddMode = 1,
        /// <summary>
        /// 修改模式
        /// </summary>
        EditMode = 2,
        /// <summary>
        /// 复制模式
        /// </summary>
        CopyMode=3
    }
}
