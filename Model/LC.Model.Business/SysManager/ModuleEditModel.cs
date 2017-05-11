
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 模块编辑模型
    /// </summary>
    public class ModuleEditModel : ModelBase
    {

        /// <summary>
        /// 系统模块
        /// </summary>
        private SysModule Module;

        public void InitEditModel(SysModule sysModule)
        {
            this.Module = sysModule;

            this.ModuleCode = sysModule.ModuleCode;
            this.Name = sysModule.Name;
            this.Remark = sysModule.Remark;
            this.Stopped = sysModule.Stopped;

            if(sysModule.SysFunctions != null)
            {
                ICollection<SysFunction> sysFunctions = sysModule.SysFunctions;
                this.SysFunctions = new List<FunctionEditModel>();
                foreach(SysFunction sysFunction in sysFunctions)
                {
                    FunctionEditModel function = new FunctionEditModel();
                    function.InitEditModel(sysFunction);
                    this.SysFunctions.Add(function);
                }
            }
            else
            {
                this.SysFunctions = null;
            }
            
        }

        public string ModuleCode { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public bool Stopped { get; set; }

        public List<FunctionEditModel> SysFunctions { get; set; }


        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// 是否含有相关方法
        /// </summary>
        public bool HasFunction()
        {
            if (Module.SysFunctions != null && Module.SysFunctions.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }//end ModuleEditModel

}//end namespace SysManager