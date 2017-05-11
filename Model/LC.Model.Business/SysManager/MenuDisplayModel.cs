using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 菜单显示对象
    /// </summary>
    public class MenuDisplayModel : ModelBase
    {

        /// <summary>
        /// 菜单编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 所含子菜单
        /// </summary>
        public List<MenuDisplayModel> SubMenus { get; set; }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {
            return this.Name;
        }

    }//end MenuDisplayModel

}//end namespace SysManager