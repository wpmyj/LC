using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.EquipmentManager.Impl
{
    public class EquipmentReasonService : IEquipmentReasonService
    {

        private Repository<EquipmentMaintReasonType> _maintReasonTypeDal;
        private Repository<EquipmentMaintReason> _maintReasonDal;
        private UnitOfWork _unitOfWork;

        public EquipmentReasonService(Repository<EquipmentMaintReasonType> maintReasonTypeDal, Repository<EquipmentMaintReason> maintReasonDal, UnitOfWork unitOfWork)
        {
            _maintReasonTypeDal = maintReasonTypeDal;
            _maintReasonDal = maintReasonDal;
            _unitOfWork = unitOfWork;
        }


        public IList<EquipmentMaintReasonType> GetAllMaintReasonType()
        {
               return _maintReasonTypeDal.GetAll().Entities.ToList();
        }


        /// 
        /// <param name="typeCode"></param>
        public bool CheckTypeCodeExist(string typeCode)
        {
            var maintReasonType = _maintReasonTypeDal.Single(s => s.maint_reason_type_code == typeCode);
            if (maintReasonType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

 
        /// 
        /// <param name="maintReasonCode"></param>
        public bool CheckReasonCodeExist(string maintReasonCode)
        {
            var maintReasonType = _maintReasonDal.Single(s => s.maint_reason_code == maintReasonCode);
            if (maintReasonType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        /// 
        /// <param name="maintReasonName"></param>
        public bool CheckReasonNameExist(string maintReasonName)
        {
            var maintReasonType = _maintReasonDal.Single(s => s.maint_reason_name == maintReasonName);
            if (maintReasonType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        /// 
        /// <param name="newEquipmentType"></param>
        public EquipmentMaintReasonType CreateMaintReasonType(EquipmentMaintReasonType newMaintReasonType)
        {
            EquipmentMaintReasonType returnMaintReasonType = null;
            try
            {
                _maintReasonTypeDal.Add(newMaintReasonType);
                returnMaintReasonType = newMaintReasonType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnMaintReasonType;
        }





        /// 
        /// <param name="newEquipmentType"></param>
        public EquipmentMaintReasonType UpdateMaintReasonType(EquipmentMaintReasonType updateMaintReasonType)
        {
            EquipmentMaintReasonType returnMaintReasonType = null;
            try
            {
                _maintReasonTypeDal.Update(updateMaintReasonType);
                returnMaintReasonType = updateMaintReasonType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnMaintReasonType;

        }

        public void DeleteMaintReasonTypeList(List<EquipmentMaintReasonType> lstDelReasonType)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                foreach (EquipmentMaintReasonType reasonType in lstDelReasonType)
                {
                    _unitOfWork.AddAction(reasonType, DataActions.Delete); 
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除维护原因类别信息失败！", ex);
            }
        }

        public IList<EquipmentMaintReason> GetAllMaintReason()
        {
            return _maintReasonDal.GetAll().Entities.ToList();
        }



        /// 
        /// <param name="newEquipment"></param>
        public EquipmentMaintReason CreateMaintReason(EquipmentMaintReason newMaintReason)
        {
 
            EquipmentMaintReason returnMaintReason = null;
            try
            {
 
                //创建对应操作
                //_unitOfWork.AddAction(newMaintReason.EquipmentMaintEffect, DataActions.Add);

                //创建原因
                //newMaintReason.effect_id = newMaintReason.EquipmentMaintEffect.id;

                
                _unitOfWork.AddAction(newMaintReason, DataActions.Add);

                //保存
                _unitOfWork.Save();

                returnMaintReason = newMaintReason;

            }
            catch (RepositoryException ex)
            {
                throw ex;
            }

            return returnMaintReason;
        }


        /// 
        /// <param name="updateMaintReason"></param>
        public EquipmentMaintReason UpdateMaintReason(EquipmentMaintReason updateMaintReason)
        {
            EquipmentMaintReason returnEquipmentMaintReason = null;
            try
            {
                _maintReasonDal.Update(updateMaintReason);
                returnEquipmentMaintReason = updateMaintReason;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnEquipmentMaintReason;

        }

        public void DeleteMaintReasonList(List<EquipmentMaintReason> lstDelMaintReason)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                foreach (EquipmentMaintReason delReason in lstDelMaintReason)
                {
                    _unitOfWork.AddAction(delReason, DataActions.Delete);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除维护原因信息失败！", ex);
            }
        }
    }
}
