
using LC.Model.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC.Model.Business.SysManager
{
    /// <summary>
    /// 菜单编辑模块
    /// </summary>
    public class MenuEditModel : ModelBase
    {

        /// <summary>
        /// 系统菜单
        /// </summary>
        private SysMenu Menu { get; set; }

        public void InitEditModel(SysMenu sysMenu)
        {
            this.Menu = sysMenu;

            this.MenuCode = sysMenu.MenuCode;
            this.BigImage = sysMenu.BigImage;
            this.ControlType = sysMenu.ControlType;
            this.DisplayName = sysMenu.DisplayName;
            this.FunctionCode = sysMenu.FunctionCode;
            this.ImagePosition = sysMenu.ImagePosition;
            this.Layer = sysMenu.Layer;
            this.ModuleCode = sysMenu.ModuleCode;
            this.Name = sysMenu.Name;
            this.ParentCode = sysMenu.ParentCode;
            this.Remark = sysMenu.Remark;
            this.ShowImage = sysMenu.ShowImage;
            this.ShowIndex = sysMenu.ShowIndex;
            this.ShowText = sysMenu.ShowText;
            this.SmallImage = sysMenu.SmallImage;
            this.Type = sysMenu.Type;

            if(sysMenu.ParentModule != null)
            {
                ModuleEditModel moduleEditModel = new ModuleEditModel();
                moduleEditModel.InitEditModel(sysMenu.ParentModule);
                this.ParentModule = moduleEditModel;
            }
            else
            {
                this.ParentModule = null;
            }

            if (sysMenu.ParentFunction != null)
            {
                FunctionEditModel functionEditModel = new FunctionEditModel();
                functionEditModel.InitEditModel(sysMenu.ParentFunction);
                this.ParentFunction = functionEditModel;
            }
            else
            {
                this.ParentFunction = null;
            }

            if(sysMenu.SubMenu!=null && sysMenu.SubMenu.Count>0)
            {
                this.SubMenus = new List<MenuEditModel>();
                ICollection<SysMenu> subMenus = sysMenu.SubMenu;
                foreach (SysMenu sysmenu in subMenus)
                {
                    MenuEditModel submenu = new MenuEditModel();
                    submenu.InitEditModel(sysmenu);
                    this.SubMenus.Add(submenu);
                }
            }
            else
            {
                this.SubMenus = null;
            }
        }

        public string MenuCode { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Nullable<int> ShowIndex { get; set; }
        public string Remark { get; set; }
        public MenuType Type { get; set; }
        public Nullable<int> Layer { get; set; }
        public string ModuleCode { get; set; }
        public string FunctionCode { get; set; }
        public byte[] BigImage { get; set; }
        public byte[] SmallImage { get; set; }
        public Nullable<bool> ShowImage { get; set; }
        public Nullable<bool> ShowText { get; set; }
        public string ControlType { get; set; }
        public string ImagePosition { get; set; }
        public string ParentCode { get; set; }

        public ModuleEditModel ParentModule { get; set; }

        public FunctionEditModel ParentFunction { get; set; }

        public List<MenuEditModel> SubMenus { get; set; }

        /// <summary>
        /// 类型转换string方法
        /// </summary>
        public override string ToString()
        {
            return this.DisplayName;
        }

        /// <summary>
        /// 是否含有子菜单
        /// </summary>
        public bool HasSubMenu()
        {
            if (this.Menu.SubMenu != null && this.Menu.SubMenu.Count > 0)
            {
                return true;
            }
            else
                return false;
        }

    }//end MenuEditModel

}//end namespace SysManager