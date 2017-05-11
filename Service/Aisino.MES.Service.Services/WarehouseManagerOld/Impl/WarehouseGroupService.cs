using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.WarehouseManager.Impl
{
    public class WarehouseGroupService : IWarehouseGroupService
    {
        private Repository<WarehouseGroup> _warehGroupDal;
        private UnitOfWork _unitOfWork;
        public WarehouseGroupService(Repository<WarehouseGroup> warehGroupDal, UnitOfWork unitOfWork)
        {
            _warehGroupDal = warehGroupDal;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 查询所有仓库组
        /// </summary>
        /// <returns>仓库组列表</returns>
        public IList<WarehouseGroup> SelectAllWarehGroup()
        {
            return _warehGroupDal.GetAll().Entities.ToList();
        }

        /// <summary>
        /// 获取根仓库组
        /// </summary>
        /// <returns>根仓库组</returns>
        public IList<WarehouseGroup> GetRootWarehGroup()
        {
            return _warehGroupDal.Find(s => s.parent_warehouse_group == null).Entities.ToList();
        }

        /// <summary>
        /// 新增一个仓库组
        /// </summary>
        /// <param name="newWarehGroup">新增的仓库组</param>
        /// <returns>新增后的仓库组</returns>
        public WarehouseGroup AddWarehGroup(WarehouseGroup newWarehGroup)
        {
            try
            {
                _warehGroupDal.Add(newWarehGroup);
                return newWarehGroup;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改一个仓库组
        /// </summary>
        /// <param name="updWarehGroup">要修改的仓库组</param>
        /// <returns>修改后的仓库组</returns>
        public WarehouseGroup UpdateWarehGroup(WarehouseGroup updWarehGroup)
        {
            try
            {
                _warehGroupDal.Update(updWarehGroup);
                return updWarehGroup;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除一个仓库组
        /// </summary>
        /// <param name="delWarehGroup">要删除的仓库组</param>
        public void DeleteWarehGroup(WarehouseGroup delWarehGroup)
        {
            try
            {
                DeleteSubWarehGroup(delWarehGroup);
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex; 
            }
        }
                
        private void DeleteSubWarehGroup(WarehouseGroup delWarehGroup)
        {
            if (delWarehGroup.SubWarehouseGroup != null)
            {
                foreach (WarehouseGroup subGroup in delWarehGroup.SubWarehouseGroup)
                {
                    DeleteSubWarehGroup(subGroup); 
                }
            }

            delWarehGroup.warehouse_group_deleted = true;
            _unitOfWork.AddAction(delWarehGroup, DataActions.Update);
        }

        public void DeleteWarehGroupList(List<WarehouseGroup> lstDelWarehGroup)
        {
            try
            {
                foreach (WarehouseGroup delWarehGroup in lstDelWarehGroup)
                {
                    if (delWarehGroup.warehouse_group_deleted == true)
                    {
                        continue;
                    }
                    DeleteSubWarehGroup(delWarehGroup);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除仓库组信息失败！", ex);
            }
        }

        public void DeleteFullyWarehGroup(WarehouseGroup delWarehGroup)
        {
            try
            {
                _warehGroupDal.Delete(delWarehGroup);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检测仓库组编号是否已存在
        /// </summary>
        /// <param name="menuCode">仓库组编号</param>
        /// <returns>若存在，则返回true；若不存在，则返回false</returns>
        public bool CheckWarehGroupCodeExist(string WarehGroupCode)
        {
            var WarehouseGroup = _warehGroupDal.Single(s => s.warehouse_group_id.ToString() == WarehGroupCode);
            if (WarehouseGroup.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测仓库组名称是否已存在
        /// </summary>
        /// <param name="menuCode">仓库组名称</param>
        /// <returns>若存在，则返回true；若不存在，则返回false</returns>
        public bool CheckWarehGroupNameExist(string WarehGroupName)
        {
            var WarehouseGroup = _warehGroupDal.Single(s => s.warehouse_group_name == WarehGroupName);
            if (WarehouseGroup.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public void UpdateAsHQ(IEnumerable<DataCenter.Models.BusinessModel.WarehouseGroup> warehGroupHQ)
        {
            //同步总部数据
            foreach (DataCenter.Models.BusinessModel.WarehouseGroup warehGroup in warehGroupHQ)
            {
                var tempWarehGroup =  _warehGroupDal.Single(w => w.warehouse_group_id == warehGroup.warehouse_group_id);
                if (tempWarehGroup.HasValue)
                {
                    //if exist,just update
                    //如果已经存在了，就更新（Update）
                    WarehouseGroup warehGroupTemp = tempWarehGroup.Entity;
                    warehGroupTemp.warehouse_group_desc = warehGroup.warehouse_group_desc;
                    warehGroupTemp.warehouse_group_name = warehGroup.warehouse_group_name;
                    warehGroupTemp.parent_warehouse_group = warehGroup.parent_warehouse_group;
                    warehGroupTemp.warehouse_group_deleted = false;

                    _unitOfWork.AddAction(warehGroupTemp, DataActions.Update);
                }
                else
                {
                    WarehouseGroup warehGroupTemp = new WarehouseGroup();
                    warehGroupTemp.warehouse_group_deleted = false;
                    warehGroupTemp.warehouse_group_id = warehGroup.warehouse_group_id;
                    warehGroupTemp.warehouse_group_desc = warehGroup.warehouse_group_desc;
                    warehGroupTemp.warehouse_group_name = warehGroup.warehouse_group_name;
                    warehGroupTemp.parent_warehouse_group = warehGroup.parent_warehouse_group;

                    _unitOfWork.AddAction(warehGroupTemp, DataActions.Add);
                }
            }
            _unitOfWork.Save();
            
            //处理parent_id 
            IEnumerable<WarehouseGroup> warehGroupList = _warehGroupDal.Find(w => w.warehouse_group_id != 0).Entities;
            foreach (WarehouseGroup warehGroup in warehGroupList)
            {
                if (warehGroup.parent_warehouse_group != null)
                {
                    warehGroup.parent_warehouse_group = warehGroupList.Single(w => w.warehouse_group_id == warehGroup.parent_warehouse_group.Value).id;
                    _unitOfWork.AddAction(warehGroup, DataActions.Update);
                }
            }
            _unitOfWork.Save();

        }
    }
}
