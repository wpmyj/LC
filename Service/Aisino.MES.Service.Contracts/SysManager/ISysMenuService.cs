using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.SysManager
{
    public interface ISysMenuService
    {  
        //查询、添加、修改、删除
        SysMenu GetRootSysMenu();
        SysMenu GetSysMenu(int id);
        IEnumerable<SysMenu> GetSysMenuListByUserId(int userid);
        /// <summary>
        /// 根据sql获得所需要的菜单
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        IEnumerable<SysMenu> QueryByESql(string strSql);
        IEnumerable<SysMenu> GetOpSysMenuListByUserIdAndParentMenuId(int userId, int parentId);
        int GetMenuCount();

        SysMenu AddSysMenu(SysMenu menu);
        SysMenu UpdateSysMenu(SysMenu menu);
        void DeleteSysMenu(SysMenu menu);
        void DeleteSysMenuList(List<SysMenu> lstDelSysMenu);

        bool CheckMenuCodeExist(string menuCode);
        bool CheckMenuNameExist(string menuName);
    }
}
