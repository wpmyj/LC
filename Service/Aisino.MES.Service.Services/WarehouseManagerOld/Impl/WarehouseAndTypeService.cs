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
    public class WarehouseAndTypeService : IWarehouseAndTypeService
    {
        private Repository<WarehouseType> _warehTypeDal;
        private Repository<Warehouse> _warehouseDal;
        private Repository<WarehouseLocation> _warehouseLocationDal;
        private UnitOfWork _unitOfWork;

        public WarehouseAndTypeService(Repository<WarehouseType> warehTypeDal, Repository<Warehouse> warehouseDal,Repository<WarehouseLocation> warehouseLocationDal, UnitOfWork unitOfWork)
        {
            _warehTypeDal = warehTypeDal;
            _warehouseDal = warehouseDal;
            _warehouseLocationDal = warehouseLocationDal;
            _unitOfWork = unitOfWork;
        }

        #region 仓库类别部分
        /// <summary>
        /// 查询所有仓库类别
        /// </summary>
        /// <returns>仓库类别列表</returns>
        public IList<WarehouseType> SelectAllWarehType()
        {
            return _warehTypeDal.GetAll().Entities.ToList();
        }

        /// <summary>
        /// 增加一个仓库类别
        /// </summary>
        /// <param name="newWarehType">要增加的仓库类别</param>
        /// <returns>增加后的仓库类别</returns>
        public WarehouseType AddWarehType(WarehouseType newWarehType)
        {
            try
            {
                _warehTypeDal.Add(newWarehType);
                return newWarehType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改一个仓库类别
        /// </summary>
        /// <param name="updWarehType">要修改的仓库类别</param>
        /// <returns>修改后的仓库类别</returns>
        public WarehouseType UpdateWarehType(WarehouseType updWarehType)
        {
            try
            {
                _warehTypeDal.Update(updWarehType);
                return updWarehType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 彻底删除一个仓库类别
        /// </summary>
        /// <param name="delWarehType">要彻底删除的仓库类别</param>
        public void DelFullyWarehType(WarehouseType delWarehType)
        {
            try
            {
                _warehTypeDal.Delete(delWarehType);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检测仓库类别编号是否已存在
        /// </summary>
        /// <param name="menuCode">仓库类别编号</param>
        /// <returns>若存在，则返回true；若不存在，则返回false</returns>
        public bool CheckWarehTypeCodeExist(int WarehTypeCode)
        {
            var WarehType = _warehTypeDal.Single(s => s.warehouse_type_id == WarehTypeCode);
            if (WarehType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测仓库类别名称是否已存在
        /// </summary>
        /// <param name="menuCode">仓库类别名称</param>
        /// <returns>若存在，则返回true；若不存在，则返回false</returns>
        public bool CheckWarehTypeNameExist(string WarehTypeName)
        {
            var WarehType = _warehTypeDal.Single(s => s.warehouse_type_name == WarehTypeName);
            if (WarehType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 仓库部分

        /// <summary>
        /// 查询所有的仓库
        /// </summary>
        /// <returns>返回所有仓库列表</returns>
        public IEnumerable<Warehouse> SelectAllWarehouse()
        {
            return _warehouseDal.GetAll().Entities;
        }

        /// <summary>
        /// 查询该类型的所有仓库
        /// </summary>
        /// <param name="TypeID">该类型ID</param>
        /// <returns>返回该类型的所有仓库列表</returns>
        public IEnumerable<Warehouse> SelectTypeWarehouse(int TypeID)
        {
            return _warehouseDal.Find(w => w.warehouse_type == TypeID).Entities;
        }


        public Warehouse GetWarehouse(string id)
        {
            return _warehouseDal.Single(s => s.warehouse_id == id).Entity;
        }

        /// <summary>
        /// 新增一个仓库
        /// </summary>
        /// <param name="newWarehouse">要新增的仓库</param>
        /// <returns>新增后的仓库</returns>
        public Warehouse AddWarehouse(Warehouse newWarehouse)
        {
            try
            {
                _warehouseDal.Add(newWarehouse);
                return newWarehouse;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 修改一个仓库
        /// </summary>
        /// <param name="udpWarehouse">要修改的仓库</param>
        /// <returns>返回修改后的仓库</returns>
        public Warehouse UpdateWarehouse(Warehouse udpWarehouse)
        {
            try
            {
                _warehouseDal.Update(udpWarehouse);
                return udpWarehouse;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 验证仓库编号是否已存在
        /// </summary>
        /// <param name="WarehouseCode">仓库编号</param>
        /// <returns>存在则返回true，不存在则返回false</returns>
        public bool CheckWarehouseCodeExist(string WarehouseCode)
        {
            var warehouse = _warehouseDal.Single(w => w.warehouse_number == WarehouseCode);
            if (warehouse.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证仓库名称是否已存在
        /// </summary>
        /// <param name="WarehouseName">仓库名称</param>
        /// <returns>存在则返回true，不存在则返回false</returns>
        public bool CheckWarehouseNameExist(string WarehouseName)
        {
            var warehouse = _warehouseDal.Single(w => w.warehouse_name == WarehouseName);
            if (warehouse.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckWarehouseIdExist(string WarehouseId)
        {
            var warehouse = _warehouseDal.Single(w => w.warehouse_id == WarehouseId);
            if (warehouse.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Warehouse SetWarehouseDuty(Warehouse warehouse, List<OrganizationEmployee> dutyUsers)
        {
            warehouse.WarehouseDuties = dutyUsers;
            _warehouseDal.Update(warehouse);
            return warehouse;
        }

        #endregion


        //public bool CheckWarehouseUnitExist(int unit_id)
        //{
        //    //return _warehouseDal.GetAllEntity().Any(w => w.warehouse_unit_id == unit_id);
        //}

        #region 仓库位置
        public IEnumerable<WarehouseLocation> FindAllWarehouseLocation()
        {
            return _warehouseLocationDal.GetAllEntity();
        }

        public void SaveWarehouseLocations(List<WarehouseLocation> warehouseLocationList)
        {
            try
            {
                List<WarehouseLocation> oldLocation = FindAllWarehouseLocation().ToList();

                if (oldLocation != null && oldLocation.Count > 0)
                {
                    foreach (WarehouseLocation warehouseLocation in warehouseLocationList)
                    {
                        //查找选择的位置信息是否已存在
                        if (!oldLocation.Any(d => d.id == warehouseLocation.id))
                        {
                            _unitOfWork.AddAction(warehouseLocation, DataActions.Add);
                        }
                        else
                        {
                            _unitOfWork.AddAction(warehouseLocation, DataActions.Update);
                        }
                    }
                }
                else
                {
                    foreach (WarehouseLocation warehouseLocation in warehouseLocationList)
                    {
                        _unitOfWork.AddAction(warehouseLocation, DataActions.Add);
                    }
                }

                ////查找之前选择的菜单是否已删除
                //if (oldLocation != null)
                //{
                //    //原有单头所含明细不为空，则需要判断是否有删除项
                //    foreach (WarehouseLocation warehouseLocation in oldLocation.Where(x => !warehouseLocationList.Any(u => u.id == x.id)).ToList())
                //    {
                //        _unitOfWork.AddAction(warehouseLocation, DataActions.Delete);
                //    }

                //}
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("保存仓库位置信息失败！", ex);
            }
        }
        #endregion
    }
}
