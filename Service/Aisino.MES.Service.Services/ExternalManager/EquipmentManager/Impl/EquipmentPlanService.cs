using System.Collections.Generic;
using System.Linq;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Helpers;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Service.SysManager;

namespace Aisino.MES.Service.EquipmentManager.Impl
{
    public class EquipmentPlanService : IEquipmentPlanService
    {

        private Repository<EquipmentPlan> _equipmentPlanDal;
        private Repository<EquipmentMaintDetail> _equipmentMaintDetailDal;
        private Repository<EquipmentMaintDetailConsumer> _equipmentMaintDetailConsumerDal;
        private UnitOfWork _unitOfWork;
        private ISysBillNoService _sysBillNoService;

        public EquipmentPlanService(Repository<EquipmentPlan> equipmentPlanDal, 
                                    Repository<EquipmentMaintDetail> equipmentMaintDetailDal,
                                    Repository<EquipmentMaintDetailConsumer> equipmentMaintDetailConsumerDal,
                                    UnitOfWork unitOfWork,
                                    ISysBillNoService sysBillNoService)
        {
            _equipmentPlanDal = equipmentPlanDal;
            _equipmentMaintDetailDal = equipmentMaintDetailDal;
            _equipmentMaintDetailConsumerDal = equipmentMaintDetailConsumerDal;

            _unitOfWork = unitOfWork;
            _sysBillNoService = sysBillNoService;
        }

 

        /// 
        /// <param name="equipmentPlanId"></param>
        public EquipmentPlan GetEquipmentPlan(string equipmentPlanNumber)
        {
            return null;
            var equipmentPlan = _equipmentPlanDal.Single(s => s.equipment_plan_number == equipmentPlanNumber);
            if (equipmentPlan.HasValue)
            {
                return equipmentPlan.Entity;
            }
            else
            {
                return null;
            }
        }


        /// 
        /// <param name="maintDetailId"></param>
        public EquipmentMaintDetail GetEquipmentMaintDetail(string maintDetailNumber)
        {
            return null;
            var equipmentMaintDetail = _equipmentMaintDetailDal.Single(s => s.equipment_maint_detail_number == maintDetailNumber);
            if (equipmentMaintDetail.HasValue)
            {
                return equipmentMaintDetail.Entity;
            }
            else
            {
                return null;
            }
        }
           

        /// 
        /// <param name="newMaintDetail"></param>
        public EquipmentMaintDetail CreateMaintDetail(EquipmentMaintDetail newMaintDetail)
        {
            EquipmentMaintDetail returnEquipmentMaintDetail = null;
            try
            {
                _equipmentMaintDetailDal.Add(newMaintDetail);

                returnEquipmentMaintDetail = newMaintDetail;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnEquipmentMaintDetail;
        }

        /// 
        /// <param name="newEquipmentPlan"></param>
        public EquipmentPlan CreateEquipmentPlan(EquipmentPlan newEquipmentPlan, string billNoSystem, string billNoPrefix)
        {
            EquipmentPlan returnEquipmentPlan = null;
            try
            {
                //设备维护单号
                int billNoID = _sysBillNoService.GetBillNoID(billNoSystem, billNoPrefix);
                if (billNoID > 0)
                {
                    newEquipmentPlan.equipment_plan_number = _sysBillNoService.GetBillNo(billNoID);
                }
 
                _unitOfWork.AddAction(newEquipmentPlan, DataActions.Add);
                _unitOfWork.Save();

                returnEquipmentPlan = newEquipmentPlan;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnEquipmentPlan;
        }

        public IEnumerable<EquipmentMaintDetail> GetAllMaintDetail()
        {
            return _equipmentMaintDetailDal.GetAll().Entities.ToList();
        }

        public IEnumerable<EquipmentPlan> GetAllEquipmentPlan()
        {
            return _equipmentPlanDal.GetAll().Entities.ToList();
        }

        /// 
        /// <param name="paging"></param>
        public IEnumerable<EquipmentPlan> GetAllPagedEquipmentPlan(PagingCriteria paging)
        {
            return _equipmentPlanDal.GetAll(paging).Entities.ToList();
        }

        public IEnumerable<EquipmentPlan> GetPagedEquipmentPlanBySql(PagingCriteria paging, string strWhere)
        {
            string esql = "select *  from  EquipmentPlan";
            esql +=  strWhere;
            var equipmentPlanList = _equipmentPlanDal.QueryByESql(esql, paging).Entities;
            if (equipmentPlanList == null)
                return null;
            else
                return _equipmentPlanDal.QueryByESql(esql, paging).Entities.ToList();
        }

        /// 
        /// <param name=""></param>
        public int GetEquipmentPlanCount(string strWhere)
        {

            string esql = "select *  from  EquipmentPlan";
            esql += strWhere;

            IEnumerable<EquipmentPlan> planList = _equipmentPlanDal.QueryByESql(esql).Entities;

            return planList.Count();
        }



        /// 
        /// <param name="newMaintDetail"></param>
        public EquipmentMaintDetail UpdateMaintDetail(EquipmentMaintDetail updateMaintDetail)
        {
            EquipmentMaintDetail returnMaintDetail = null;
            try
            {
                _equipmentMaintDetailDal.Update(updateMaintDetail);
                returnMaintDetail = updateMaintDetail;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnMaintDetail;

        }

        /// 
        /// <param name="deleteMaintDetail"></param>
        public EquipmentMaintDetail DeleteMaintDetail(EquipmentMaintDetail deleteMaintDetail)
        {
            EquipmentMaintDetail returnMaintDetail = null;
            try
            {
                _equipmentMaintDetailDal.Delete(deleteMaintDetail);

                returnMaintDetail = deleteMaintDetail;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnMaintDetail;
        }


        /// 
        /// <param name="newEquipmentPlan"></param>
        public EquipmentPlan UpdateEquipmentPlan(EquipmentPlan updateEquipmentPlan)
        {
            EquipmentPlan returnEquipment = null;
            try
            {
                _equipmentPlanDal.Update(updateEquipmentPlan);
                returnEquipment = updateEquipmentPlan;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnEquipment;

        }


        #region 维护消耗材料

        public IEnumerable<EquipmentMaintDetailConsumer> SelectMaintConsumer(int equipment_maint_detail_id)
        {
            //return _equipmentMaintDetailConsumerDal.Find(c => c.equipment_maint_detail_id.Value == equipment_maint_detail_id).Entities;
            return null;
        }

        public void AddMaintConsumer(EquipmentMaintDetailConsumer newMaintConsumer)
        {
            try
            {
                _equipmentMaintDetailConsumerDal.Add(newMaintConsumer);
            }
            catch(RepositoryException ex)
            {
                throw ex;
            }
        }

        public void UpdMaintConsumer(EquipmentMaintDetailConsumer updMaintConsumer)
        {
            try
            {
                _equipmentMaintDetailConsumerDal.Update(updMaintConsumer);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void DelMaintConsumer(EquipmentMaintDetailConsumer delMaintConsumer)
        {
            try
            {
                _equipmentMaintDetailConsumerDal.Delete(delMaintConsumer);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        public void BatchUpdate(string maintDetailNumber, List<EquipmentMaintDetailConsumer> listMaintConsumer)
        {
            List<EquipmentMaintDetailConsumer> oldList = _equipmentMaintDetailConsumerDal.Find(w => w.equipment_maint_detail_number == maintDetailNumber).Entities.ToList();
            if (oldList != null && oldList.Count > 0)
            {
                listMaintConsumer.ForEach(newMC =>
                {
                    //原先有的，修改
                    if (oldList.Any(old => old.equipment_maint_detail_number == newMC.equipment_maint_detail_number))
                    {
                        _equipmentMaintDetailConsumerDal.Update(newMC);
                    }
                    //原先没有的，添加
                    else
                    {
                        _equipmentMaintDetailConsumerDal.Add(newMC);
                    }
                });
                oldList.ForEach(oldMC =>
                {
                    //原先有的，现在没了，要删除
                    if (!listMaintConsumer.Any(w => w.equipment_maint_detail_number == oldMC.equipment_maint_detail_number))
                    {
                        _equipmentMaintDetailConsumerDal.Delete(oldMC);
                    }
                });
            }
            else
            {
                listMaintConsumer.ForEach(newMC =>
                {
                    _equipmentMaintDetailConsumerDal.Add(newMC);
                });
            }
        }

        #endregion




        public IList<double> GetEquipmentPlanTimes(Equipment theEquipment)
        {
            return theEquipment.EquipmentPlans.
                                Where(we => we.exe_start_date.HasValue && we.exe_end_date.HasValue).
                                Select(we => (we.exe_end_date.Value - we.exe_start_date.Value).TotalHours).ToList();
        }
    }
}
