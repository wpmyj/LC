using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class BorGroupMasterService : IBorGroupMasterService
    {
        private Repository<BorGroupMaster> _borGroupMasterDal;
        private Repository<BorLine> _borLineDal;
        private UnitOfWork _unitOfWork;

        public BorGroupMasterService(Repository<BorGroupMaster> borGroupMasterDal, Repository<BorLine> borLineDal, UnitOfWork unitOfWork)
        {
            _borGroupMasterDal = borGroupMasterDal;
            _borLineDal = borLineDal;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BorGroupMaster> SelectAllBorGroupMaster()
        {
            return _borGroupMasterDal.GetAll().Entities;
        }

        public BorGroupMaster GetBorGroupMaster(int id)
        {
            var borGroupMaster = _borGroupMasterDal.Single(d => d.id == id);
            if (borGroupMaster.HasValue)
            {
                return borGroupMaster.Entity;
            }
            else
            {
                return null;
            }
        }

        public BorGroupMaster AddBorGroupMaster(BorGroupMaster newBorGroupMaster)
        {
            BorGroupMaster borGroupMaster = null;
            try
            {
                _borGroupMasterDal.Add(newBorGroupMaster);
                borGroupMaster = newBorGroupMaster;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加品号组信息失败！", ex);
            }
            return borGroupMaster;
        }

        public BorGroupMaster UpdateBorGroupMaster(BorGroupMaster updBorGroupMaster)
        {
            BorGroupMaster borGroupMaster = null;
            try
            {
                _borGroupMasterDal.Update(updBorGroupMaster);
                borGroupMaster = updBorGroupMaster;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("修改品号组信息失败！", ex);
            }
            return borGroupMaster;
        }

        public BorGroupMaster DeleteBorGroupMaster(BorGroupMaster delBorGroupMaster)
        {
            BorGroupMaster borGroupMaster = null;
            try
            {
                delBorGroupMaster.bor_group_deleted = true;
                _unitOfWork.AddAction(delBorGroupMaster, DataActions.Update);
                borGroupMaster = delBorGroupMaster;
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除品号组信息失败！", ex);
            }
            return borGroupMaster;
        }

        public void DeleteBorGroupMasterList(List<BorGroupMaster> lstDelBorGroupMaster)
        {
            try
            {
                foreach (BorGroupMaster borGroupMaster in lstDelBorGroupMaster)
                {
                    if (borGroupMaster.bor_group_deleted == true)
                    {
                        continue;
                    }
                    borGroupMaster.bor_group_deleted = true;
                    _unitOfWork.AddAction(borGroupMaster, DataActions.Update);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除物料清单主档信息失败！", ex);
            }
        }

        public bool CheckBorGroupMasterCodeExist(string borGroupMasterCode)
        {
            var borGroupMaster = _borGroupMasterDal.Single(d => d.bor_group_code == borGroupMasterCode);
            if (borGroupMaster.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckBorGroupMasterNameExist(string borGroupMasterName)
        {
            var borGroupMaster = _borGroupMasterDal.Single(d => d.bor_group_name == borGroupMasterName);
            if (borGroupMaster.HasValue)
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
