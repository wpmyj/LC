
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 方法编辑对象
    /// </summary>
    public class FunctionEditModel : ModelBase
    {

        /// <summary>
        /// 方法实体对象
        /// </summary>
        private SysFunction Function;

        public void InitEditModel(SysFunction sysFunction)
        {
            this.Function = sysFunction;

            this.Assembly = sysFunction.Assembly;
            this.ClassName = sysFunction.ClassName;
            this.FunctionCode = sysFunction.FunctionCode;
            this.ModuleCode = sysFunction.ModuleCode;
            this.Name = sysFunction.Name;
            this.OperationCode = sysFunction.OperationCode;
            this.OperationName = sysFunction.OperationName;
            this.Params = sysFunction.Params;
            this.Remark = sysFunction.Remark;
            this.Type = sysFunction.Type;
        }

        public string FunctionCode { get; set; }
        public string Name { get; set; }
        public FunctionType Type { get; set; }
        public string Assembly { get; set; }
        public string ClassName { get; set; }
        public string OperationCode { get; set; }
        public string OperationName { get; set; }
        public string Params { get; set; }
        public string Remark { get; set; }
        public string ModuleCode { get; set; }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

    }//end FunctionEditModel

}//end namespace SysManager