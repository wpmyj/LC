using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;

namespace Aisino.MES.Service.SysManager
{
    public interface ISysRoleService
    {
        //增、删、改、查、验证是否存在重复编号及名称
        IList<SysRole> SelectAllRoles();

        SysRole AddSysRole(SysRole newSysRole);
        SysRole UpdateSysRole(SysRole upSysRole);
        void DelSysRole(SysRole delSysRole);
        void DeleteSysRoleList(List<SysRole> lstDelSysRole);

        bool CheckRoleCodeExist(string roleCode);
        bool CheckRoleNameExist(string roleName);
    }
}
