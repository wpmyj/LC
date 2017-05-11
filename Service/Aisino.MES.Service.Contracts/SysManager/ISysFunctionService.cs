using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.SysManager
{
    public interface ISysFunctionService
    {
        /// <summary>
        /// 获取所有方法
        /// </summary>
        /// <returns>方法列表</returns>
        IList<SysFunction> SelectAllSysFunction();

        /// <summary>
        /// 根据主键获取方法
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>获取的系统方法</returns>
        SysFunction GetSysFunction(int id);

        /// <summary>
        /// 添加系统方法
        /// </summary>
        /// <param name="function">需要添加的系统方法</param>
        /// <returns>添加完成的系统方法</returns>
        SysFunction AddSysFunction(SysFunction function);

        /// <summary>
        /// 更新系统方法
        /// </summary>
        /// <param name="function">需要更新的系统方法</param>
        /// <returns>更新完成的系统方法</returns>
        SysFunction UpdateSysFunction(SysFunction function);

        /// <summary>
        /// 删除系统方法
        /// </summary>
        /// <param name="function">需要删除的系统方法</param>
        /// <returns>删除后的系统方法</returns>
        SysFunction DeleteSysFunction(SysFunction function);

        /// <summary>
        /// 删除系统方法列表
        /// </summary>
        /// <param name="lstDelSysFunction">需要删除的系统方法列表</param>
        void DeleteSysFunctionList(List<SysFunction> lstDelSysFunction);

        /// <summary>
        /// 判断系统方法编号是否存在
        /// </summary>
        /// <param name="functionCode">系统方法编号</param>
        /// <returns>是否存在</returns>
        bool CheckFunctionCodeExist(string functionCode);

        /// <summary>
        /// 判断系统方法名称是否存在
        /// </summary>
        /// <param name="functionName">系统方法名称</param>
        /// <returns>是否存在</returns>
        bool CheckFunctionNameExist(string functionName);
    }
}
