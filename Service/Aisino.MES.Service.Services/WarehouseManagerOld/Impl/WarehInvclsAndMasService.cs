using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.WarehouseManager.Impl
{
    public class WarehInvclsAndMasService : IWarehInvclsAndMasService
    {
        private Repository<WarehouseInvcls> _warehInvclsDal;
        private Repository<WarehouseInvmas> _warehInvmasDal;
        private UnitOfWork _unitOfWork;

        public WarehInvclsAndMasService(Repository<WarehouseInvcls> warehInvclsDal,
                                        Repository<WarehouseInvmas> warehInvmasDal,
                                        UnitOfWork unitOfWork)
        {
            _warehInvclsDal = warehInvclsDal;
            _warehInvmasDal = warehInvmasDal;
            _unitOfWork = unitOfWork;
        }

        #region 货品大类 部分
        /// <summary>
        /// 查询根货品大类
        /// </summary>
        /// <returns>根货品大类</returns>
        public WarehouseInvcls SelectRootWarehInvcls()
        {
            return _warehInvclsDal.Single(w => w.invcls_parent_id == null).Entity;
        }

        /// <summary>
        /// 查询单个货品大类
        /// </summary>
        /// <param name="warehInvclsID">该货品大类ID</param>
        /// <returns>该货品大类</returns>
        public WarehouseInvcls SelectOneWarehInvcls(int warehInvclsID)
        {
            return _warehInvclsDal.Single(w => w.id == warehInvclsID).Entity;
        }

        /// <summary>
        /// 查询存在货品主档的货品大类
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WarehouseInvcls> SelectExistInvmasWarehInvcls()
        {
            return _warehInvclsDal.Find(w => w.WarehouseInvmas.Count != 0).Entities;
        }
        
        public IEnumerable<WarehouseInvcls> SelectConsumerWarehInvcls()
        {
            return _warehInvclsDal.Find(c => c.invcls_type.Value == 5 && c.invcls_parent_id.Value == 1).Entities;
        }

        public IEnumerable<WarehouseInvcls> SelectEnableConsumerWarehInvcls()
        {
            return SelectConsumerWarehInvcls().Where(c => c.invcls_deleted == false);
        }

        /// <summary>
        /// 添加一个货品大类
        /// </summary>
        /// <param name="newWarehInvcls">要添加的货品大类</param>
        /// <returns>添加后的货品大类</returns>
        public WarehouseInvcls AddWarehInvcls(WarehouseInvcls newWarehInvcls)
        {
            try
            {
                _warehInvclsDal.Add(newWarehInvcls);
                return newWarehInvcls;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改一个货品大类
        /// </summary>
        /// <param name="updWarehInvcls">要修改的货品大类</param>
        /// <returns>修改后的货品大类</returns>
        public WarehouseInvcls UpdateWarehInvcls(WarehouseInvcls updWarehInvcls)
        {
            try
            {
                _warehInvclsDal.Update(updWarehInvcls);
                return updWarehInvcls;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除一个货品大类及下属货品大类
        /// </summary>
        /// <param name="delWarehInvcls">要删除的货品大类</param>
        public void DeleteWarehInvcls(WarehouseInvcls delWarehInvcls)
        {
            try
            {
                DelSubWarehInvcls(delWarehInvcls);
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        private void DelSubWarehInvcls(WarehouseInvcls delWarehInvcls)
        {
            if (delWarehInvcls.SubWarehouseInvcls != null)
            {
                foreach(WarehouseInvcls invcls in delWarehInvcls.SubWarehouseInvcls)
                {
                    DelSubWarehInvcls(invcls);
                }
            }
            delWarehInvcls.invcls_deleted = true;
            _unitOfWork.AddAction(delWarehInvcls, DataActions.Update);
             
        }
        
        public void DelFullyWarehInvcls(WarehouseInvcls delWarehInvcls)
        {
            _warehInvclsDal.Delete(delWarehInvcls);
        }        

        public void DeleteWarehInvclsList(List<WarehouseInvcls> lstDelWarehInvcls)
        {
            try
            {
                foreach (WarehouseInvcls delWarehInvcls in lstDelWarehInvcls)
                {
                    if (delWarehInvcls.invcls_deleted == true)
                    {
                        continue;
                    }
                    DelSubWarehInvcls(delWarehInvcls);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除货品大类信息失败！", ex);
            }
        }
        /// <summary>
        /// 检测货品大类编号是否已存在
        /// </summary>
        /// <param name="warehInvclsCode">货品大类编号</param>
        /// <returns>存在则返回true, 不存在则返回false</returns>
        public bool CheckWarehInvclsCodeExist(string warehInvclsCode)
        {
            var warehInvcls = _warehInvclsDal.Single(w => w.invcls_code == warehInvclsCode);
            if (warehInvcls.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测货品大类名称是否已存在
        /// </summary>
        /// <param name="warehInvclsCode">货品大类名称</param>
        /// <returns>存在则返回true, 不存在则返回false</returns>
        public bool CheckWarehInvclsNameExist(string warehInvclsName)
        {
            var warehInvcls = _warehInvclsDal.Single(w => w.invcls_name == warehInvclsName);
            if (warehInvcls.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 货品主档 部分
        /// <summary>
        /// 查询所有货品主档
        /// </summary>
        /// <returns>所有货品主档列表</returns>
        public IEnumerable<WarehouseInvmas> SelectAllWarehInvmas()
        {
            return _warehInvmasDal.GetAll().Entities;
        }

        /// <summary>
        /// 查询单个货品主档
        /// </summary>
        /// <param name="warehInvmasID">货品主档ID</param>
        /// <returns>单个货品主档</returns>
        public WarehouseInvmas SelectOneWarehInvmas(int warehInvmasID)
        {
            return _warehInvmasDal.Single(w => w.id == warehInvmasID).Entity;
        }

        /// <summary>
        /// 添加一个货品主档
        /// </summary>
        /// <param name="newWarehInvmas">要添加的货品主档</param>
        /// <returns>添加后的货品主档</returns>
        public WarehouseInvmas AddWarehInvmas(WarehouseInvmas newWarehInvmas)
        {
            try
            {
                _warehInvmasDal.Add(newWarehInvmas);
                return newWarehInvmas;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改一个货品主档
        /// </summary>
        /// <param name="updWarehInvmas">要修改的货品主档</param>
        /// <returns>修改后的货品主档</returns>
        public WarehouseInvmas UpdateWarehInvmas(WarehouseInvmas updWarehInvmas)
        {
            try
            {
                _warehInvmasDal.Update(updWarehInvmas);
                return updWarehInvmas;
            }
            catch(RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除一个货品主档
        /// </summary>
        /// <param name="delWarehInvmas">要删除的货品主档</param>
        public void DelWarehInvmas(WarehouseInvmas delWarehInvmas)
        {
            try
            {
                delWarehInvmas.invmas_deleted = true;
                _warehInvmasDal.Update(delWarehInvmas);
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除货物主档信息失败！", ex);
            }
        }

        /// <summary>
        /// 批量删除货物主档
        /// </summary>
        /// <param name="lstDelInvmas"></param>
        public void DeleteWarehInvmasList(List<WarehouseInvmas> lstDelInvmas)
        {
            try
            {
                foreach (WarehouseInvmas warehouseInvmas in lstDelInvmas)
                {
                    if (warehouseInvmas.invmas_deleted == true)
                    {
                        continue;
                    }
                    warehouseInvmas.invmas_deleted = true;
                    _warehInvmasDal.Update(warehouseInvmas);
                }
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除货物主档信息失败！", ex);
            }
        }

        /// <summary>
        /// 检测货品主档编号是否已存在
        /// </summary>
        /// <param name="warehInvmasCode">货品主档编号</param>
        /// <returns>存在返回true, 不存在返回false</returns>
        public bool CheckWarehInvmasCodeExist(string warehInvmasCode)
        {
            var warehInvmas = _warehInvmasDal.Single(w => w.invmas_code == warehInvmasCode);
            if (warehInvmas.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测货品主档名称是否已存在
        /// </summary>
        /// <param name="warehInvmasCode">货品主档名称</param>
        /// <returns>存在返回true, 不存在返回false</returns>
        public bool CheckWarehInvmasNameExist(string warehInvmasName)
        {
            var warehInvmas = _warehInvmasDal.Single(w => w.invmas_name == warehInvmasName);
            if (warehInvmas.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 根据仓库ID查询货品主档
        /// </summary>
        /// <param name="warehInvmasID">仓库ID</param>
        /// <returns>货品主档</returns>
        public IEnumerable<WarehouseInvmas> GetInvmasByWareid(int warehouse_id)
        {

            string esql = "select invmas.*"
            + " from  WarehouseInvmas invmas, WarehouseAmount amt, WarehouseBatchHead head"
            + " where head.warehouse_id = amt.warehouse_id"
            + " and amt.invmas_id = invmas.id"
            + " and head.warehouse_id = " + warehouse_id.ToString();

            return _warehInvmasDal.QueryByESql(esql).Entities;
        }

        #endregion



        public bool CheckWarehInvmasUnitUsed(int unit_id)
        {
            return _warehInvmasDal.GetAllEntity().Any(w => w.unit_id.HasValue && w.unit_id.Value == unit_id);
        }
    }
}
