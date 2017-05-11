using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;

namespace Aisino.MES.Service.WarehouseManager.Impl
{
    public class WarehouseUnitAndUnitTypeService : IWarehouseUnitAndUnitTypeService
    {
        private Repository<WarehouseUnit> _warehUnitDal;
        private Repository<WarehouseUnitType> _warehUnitTypeDal;
        private UnitOfWork _unitOfWork;

        public WarehouseUnitAndUnitTypeService(Repository<WarehouseUnit> warehUnitDal, 
                                               Repository<WarehouseUnitType> warehUnitTypeDal,
                                               UnitOfWork unitOfWork)
        {
            _warehUnitDal = warehUnitDal;
            _warehUnitTypeDal = warehUnitTypeDal;
            _unitOfWork = unitOfWork;
        }

        #region “计量单位”部分
        public IEnumerable<WarehouseUnit> SelectAllWarehUnit()
        {
           return _warehUnitDal.GetAll().Entities;
        }

        public IEnumerable<WarehouseUnit> SelectWarehUnit(int unitTypeID)
        {
            return _warehUnitDal.Find(w => w.unit_type_id == unitTypeID).Entities;
        }

        public WarehouseUnit AddWarehUnit(WarehouseUnit addWarehUnit)
        {
            try
            {
                _warehUnitDal.Add(addWarehUnit);
                return addWarehUnit;
            }
            catch(RepositoryException ex)
            {
                throw ex;
            }
        }

        public WarehouseUnit UpdWarehUnit(WarehouseUnit updWarehUnit)
        {
            try
            {
                _warehUnitDal.Update(updWarehUnit);
                return updWarehUnit;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DelWarehUnit(WarehouseUnit delWarehUnit)
        {
            try
            {
                _warehUnitDal.Update(delWarehUnit);
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除计量单位信息失败！", ex);
            }
            
        }

        public void DeleteWarehUnitList(List<WarehouseUnit> lstDelWarehouseUnit)
        {
            try
            {
                foreach (WarehouseUnit unit in lstDelWarehouseUnit)
                {
                    //if (unit.unit_deleted == true)
                    //{
                    //    continue;
                    //}
                    //unit.unit_deleted = true;
                    _warehUnitDal.Update(unit);
                }
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除计量单位信息失败！", ex);
            }
        }

        public void FullyDelWarehUnit(WarehouseUnit delWarehUnit)
        {
            //如果存在就返回
            if (CheckWarehUnitUsed(delWarehUnit))
            {
                return;
            } 
            try
            {
                _warehUnitDal.Delete(delWarehUnit);
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除计量单位信息失败！", ex);
            }
           
        }

        public bool CheckWarehUnitUsed(WarehouseUnit warehUnit)
        {
            bool IsHave = false;
            //Warehouse表中是否引用此单位

            //WarehouseInvmas表中是否引用此单位
            return IsHave;
        }

        public bool CheckWarehUnitCodeExist(string warehUnitCode)
        {
            var warehUnit = _warehUnitDal.Single(w => w.unit_code == warehUnitCode);
            if (warehUnit.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckWarehUnitCNameExist(string warehUnitCName)
        {
            var warehUnit = _warehUnitDal.Single(w => w.unit_cname == warehUnitCName);
            if (warehUnit.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckWarehUnitBaseExist(int unitTypeID)
        {
            //var warehUnit = _warehUnitDal.Single(w => w.unit_type_id == unitTypeID && w.unit_rate.Value.ToString("D") == "1");
            var warehUnit = _warehUnitDal.Single(w => w.unit_type_id == unitTypeID);
            if (warehUnit.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region "计量单位类别"部分

        public IEnumerable<WarehouseUnitType> SelectAllWarehUnitType()
        {
            return _warehUnitTypeDal.GetAll().Entities;
        }

        public WarehouseUnitType AddWarehUnitType(WarehouseUnitType addWarehUnitType)
        {
            try
            {
                _warehUnitTypeDal.Add(addWarehUnitType);
                return addWarehUnitType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public WarehouseUnitType UpdWarehUnitType(WarehouseUnitType updWarehUnitType)
        {
            try
            {
                _warehUnitTypeDal.Update(updWarehUnitType);
                return updWarehUnitType;
            }
            catch(RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DelWarehUnitType(WarehouseUnitType delWarehUnitType)
        {
            try
            {
                //delWarehUnitType.unit_type_deleted = true;
                _warehUnitTypeDal.Update(delWarehUnitType);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DeleteWarehUnitTypeList(List<WarehouseUnitType> lstDelWarehUnitType)
        {
            try
            {
                foreach (WarehouseUnitType delWarehUnitType in lstDelWarehUnitType)
                {
                    //if (delWarehUnitType.unit_type_deleted == true)
                    //{
                    //    continue;
                    //}
                    //delWarehUnitType.unit_type_deleted = true;
                    _warehUnitTypeDal.Update(delWarehUnitType);
                }
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除计量单位类别信息失败！", ex);
            }
        }

        public void FullyDelUnitType(WarehouseUnitType delWarehUnitType)
        {
            try
            {
                _warehUnitTypeDal.Delete(delWarehUnitType);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public bool CheckWarehUnitTypeCodeExist(string warehUnitTypeCode)
        {
            var warehUnitType = _warehUnitTypeDal.Single(w => w.unit_type_code == warehUnitTypeCode);
            if (warehUnitType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public bool CheckWarehUnitTypeNameExist(string warehUnitTypeName)
        {
            var warehUnitType = _warehUnitTypeDal.Single(w => w.unit_type_name == warehUnitTypeName);
            if (warehUnitType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


     
    }
}
