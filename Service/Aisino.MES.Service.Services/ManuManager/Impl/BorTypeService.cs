using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class BorTypeService : IBorTypeService
    {
        private Repository<BorType> _borTypeDal;
        private Repository<BorLine> _borLineDal;
        private UnitOfWork _unitOfWork;

        public BorTypeService(Repository<BorType> borTypeDal, Repository<BorLine> borLineDal, UnitOfWork unitOfWork)
        {
            _borTypeDal = borTypeDal;
            _borLineDal = borLineDal;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BorType> SelectAllBorType()
        {
            return _borTypeDal.GetAll().Entities;
        }

        public BorType GetBorType(int id)
        {
            var borType = _borTypeDal.Single(d => d.id == id);
            if (borType.HasValue)
            {
                return borType.Entity;
            }
            else
            {
                return null;
            }
        }

        public BorType AddBorType(BorType newBorType)
        {
            BorType borType = null;
            try
            {
                _borTypeDal.Add(newBorType);
                borType = newBorType;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加工艺类型信息失败！", ex);
            }
            return borType;
        }

        public BorType UpdateBorType(BorType updBorType)
        {
            BorType borType = null;
            try
            {
                _borTypeDal.Update(updBorType);
                borType = updBorType;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("修改工艺类型信息失败！", ex);
            }
            return borType;
        }

        public BorType DeleteBorType(BorType delBorType)
        {
            BorType borType = null;
            try
            {
                delBorType.bor_type_deleted = true;
                _borTypeDal.Update(delBorType);
                borType = delBorType;
                return borType;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除工艺类型信息失败！", ex);
            }
        }

        public void DeleteBorTypeList(List<BorType> lstDelBorType)
        {
            try
            {
                foreach (BorType borType in lstDelBorType)
                {
                    if (borType.bor_type_deleted == true)
                    {
                        continue;
                    }
                    borType.bor_type_deleted = true;
                    _borTypeDal.Update(borType);
                }
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除工艺类型信息失败！", ex);
            }
        }

        public bool CheckBorTypeCodeExist(string borTypeCode)
        {
            var borType = _borTypeDal.Single(d => d.bor_type_code == borTypeCode);
            if (borType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckBorTypeNameExist(string borTypeName)
        {
            var borType = _borTypeDal.Single(d => d.bor_type_name == borTypeName);
            if (borType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
