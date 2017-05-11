using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;

namespace Aisino.MES.Service.SysManager
{
    public interface ISysRightService
    {
        //增、删、改、查、验证是否存在重复编号及名称
        IList<SysRight> SelectAllSysRight();
        IList<SysRight> SelectUndelSysRight();
        SysRight GetSysRight(int id);

        SysRight AddSysRight(SysRight newSysRight);
        SysRight UpdateSysRight(SysRight updSysRight);
        SysRight DeleteSysRight(SysRight delSysRight);
        void DeleteSysRightList(List<SysRight> lstDelSysRight);

        bool CheckRightCodeExist(string rightCode);
        bool CheckRightNameExist(string rightName);
    }
}
