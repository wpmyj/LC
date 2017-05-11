using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.SysManager
{
    public interface ISysModuleService
    {
        /// <summary>
        /// 查询所有系统模块
        /// </summary>
        /// <returns>系统模块列表</returns>
        IList<SysModule> SelectAllSysModule();

        /// <summary>
        /// 根据主键获取系统模块
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns>查找所得系统模块</returns>
        SysModule GetSysModule(int id);

        /// <summary>
        /// 添加系统模块
        /// </summary>
        /// <param name="module">需要添加的模块</param>
        /// <returns>添加完成的模块信息</returns>
        SysModule AddSysModule(SysModule module);

        /// <summary>
        /// 更新系统模块
        /// </summary>
        /// <param name="module">需要更新的模块信息</param>
        /// <returns>更新完成的模块信息</returns>
        SysModule UpdateSysModule(SysModule module);

        /// <summary>
        /// 删除系统模块
        /// </summary>
        /// <param name="module">需要更新的模块信息</param>
        /// <returns>删除后的系统模块</returns>
        SysModule DeleteSysModule(SysModule module);

        /// <summary>
        /// 删除系统模块列表
        /// </summary>
        /// <param name="lstDelSysModule">需要删除的系统模块列表</param>
        void DeleteSysModuleList(List<SysModule> lstDelSysModule);

        /// <summary>
        /// 判断模块编号是否存在
        /// </summary>
        /// <param name="moduleCode">模块编号</param>
        /// <returns>是否存在</returns>
        bool CheckModuleCodeExist(string moduleCode);

        /// <summary>
        /// 判断模块名称是否存在
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>是否存在</returns>
        bool CheckModuleNameExist(string moduleName);
    }
}
