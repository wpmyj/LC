using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    public class MainMenuModel : ModelBase
    {
        public MainMenuModel()
        {
            this.Assembly = "";
            this.ClassName = "";
        }
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        public string ParentCode { get; set; }

        public string DisplayName { get; set; }

        public int Layer { get; set; }

        public string Assembly { get; set; }

        public string ClassName { get; set; }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
