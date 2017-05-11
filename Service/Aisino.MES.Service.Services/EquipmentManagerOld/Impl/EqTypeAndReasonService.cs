using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Helpers;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;

namespace Aisino.MES.Service.EquipmentManager.Impl
{
    public class EqTypeAndReasonService : IEqTypeAndReasonService
    {
        private Repository<EquipmentMaintReasonType> _eqTypeMtReasonDal;
        private UnitOfWork _unitOfWork;

        public EqTypeAndReasonService(Repository<EquipmentMaintReasonType> eqTypeMtReasonDal, UnitOfWork unitOfWork)
        {
            _eqTypeMtReasonDal = eqTypeMtReasonDal;
            _unitOfWork = unitOfWork;
        }

        public EquipmentMaintReasonType AddEqTypeMtReasons(EquipmentMaintReasonType newEqTypeMaintReason)
        {
            EquipmentMaintReasonType rEquipmentType = null;
            try
            {
                _eqTypeMtReasonDal.Add(newEqTypeMaintReason);
                rEquipmentType = newEqTypeMaintReason;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rEquipmentType;
        }

        public void UpdateExistMtReason(IEnumerable<EquipmentMaintReason> eqTypeMtReasons, string equipmentCode)
        {
            try
            {
                //IEnumerable<EquipmentMaintReasonType> oldEqTypeMtReason =
                //    _eqTypeMtReasonDal.Find(e => e.equipment_type_id == equipmentType_id).Entities;
                ////原先没有的，添加进去
                //if (oldEqTypeMtReason != null || oldEqTypeMtReason.ToList().Count > 0)
                //{
                //    foreach (Model.EquipmentManager.EquipmentTypeMaintReason eqTypeMtReason in eqTypeMtReasons)
                //    {
                //        if (!oldEqTypeMtReason.Any(old => old.equipment_type_id == eqTypeMtReason.equipment_type_id
                //                                 && old.equipment_maint_reason_id == eqTypeMtReason.equipment_maint_reason_id))
                //        {
                //            _unitOfWork.AddAction(eqTypeMtReason, DAL.Enums.DataActions.Add);
                //        }
                //    }
                //}
                //else
                //{
                //    foreach (EquipmentMaintReasonType eqTypeMtReason in eqTypeMtReasons)
                //    {
                //        _unitOfWork.AddAction(eqTypeMtReason, DAL.Enums.DataActions.Add);
                //    }
                //}
                ////原先有的，现在没有了，删除
                //if (oldEqTypeMtReason != null)
                //{
                //    foreach (EquipmentMaintReasonType eqTypeMtReason
                //             in oldEqTypeMtReason.Where(old => !eqTypeMtReasons.Any(es =>
                //                 es.equipment_type_id == old.equipment_type_id
                //              && es.equipment_maint_reason_id == old.equipment_maint_reason_id)))
                //    {
                //        _unitOfWork.AddAction(eqTypeMtReason, DAL.Enums.DataActions.Delete);
                //    }
                //}
                ////执行
                //_unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DelEqTypeMtReasons(EquipmentMaintReasonType eqTypeMtReason)
        {
            try
            {
                _eqTypeMtReasonDal.Delete(eqTypeMtReason);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }
    }
}
