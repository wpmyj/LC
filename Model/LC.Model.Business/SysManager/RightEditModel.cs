
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 权限编辑对象
    /// </summary>
    public class RightEditModel : ModelBase
    {

        /// <summary>
        /// 权限对象
        /// </summary>
        private SysRight Right;

        public void InitEditModel(SysRight sysRight)
        {
            this.Right = sysRight;

            this.RightCode = sysRight.RightCode;
            this.Name = sysRight.Name;
            this.Remark = sysRight.Remark;
            this.Stopped = sysRight.Stopped;

            if (sysRight.SysMenus != null)
            {
                ICollection<SysMenu> sysMenus = sysRight.SysMenus;
                this.SysMenus = new List<MenuEditModel>();
                foreach (SysMenu sysMenu in sysMenus)
                {
                    MenuEditModel menu = new MenuEditModel();
                    menu.InitEditModel(sysMenu);
                    this.SysMenus.Add(menu);
                }
            }
            else
            {
                this.SysMenus = null;
            }
        }

        public string RightCode { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> Stopped { get; set; }

        public List<MenuEditModel> SysMenus { get; set; }

        public List<RoleEditModel> SysRoles { get; set; }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {
            return "";
        }

        /// <summary>
        /// 是否有角色使用了该权限么
        /// </summary>
        public bool HasRole()
        {
            return false;
        }

    }//end RightEditModel

}//end namespace SysManager