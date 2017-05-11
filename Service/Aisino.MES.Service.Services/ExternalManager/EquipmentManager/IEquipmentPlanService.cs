using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Helpers;

namespace Aisino.MES.Service.EquipmentManager
{
    public interface IEquipmentPlanService
    {

        /// 
        /// <param name="strWhere"></param>
        int GetEquipmentPlanCount(string strWhere);

        /// 
        /// <param name="equipmentPlanNumber"></param>
        EquipmentPlan GetEquipmentPlan(string equipmentPlanNumber);

        /// 
        /// <param name="maintDetailNumber"></param>
        EquipmentMaintDetail GetEquipmentMaintDetail(string maintDetailNumber);

        /// 
        /// <param name="newMaintDetail"></param>
        EquipmentMaintDetail CreateMaintDetail(EquipmentMaintDetail newMaintDetail);

        /// 
        /// <param name="newEquipmentPlan"></param>
        EquipmentPlan CreateEquipmentPlan(EquipmentPlan newEquipmentPlan, string billNoSystem, string billNoPrefix);

        IEnumerable<EquipmentMaintDetail> GetAllMaintDetail();

        IEnumerable<EquipmentPlan> GetAllEquipmentPlan();

        /// 
        /// <param name="paging"></param>
        IEnumerable<EquipmentPlan> GetAllPagedEquipmentPlan(PagingCriteria paging);

        IEnumerable<EquipmentPlan> GetPagedEquipmentPlanBySql(PagingCriteria paging, string strWhere);
     

        /// 
        /// <param name="newMaintDetail"></param>
        EquipmentMaintDetail UpdateMaintDetail(EquipmentMaintDetail updateMaintDetail);


        /// 
        /// <param name="deleteMaintDetail"></param>
        EquipmentMaintDetail DeleteMaintDetail(EquipmentMaintDetail deleteMaintDetail);

        /// 
        /// <param name="newEquipmentPlan"></param>
        EquipmentPlan UpdateEquipmentPlan(EquipmentPlan updateEquipmentPlan);

        #region 维修使用耗材
        IEnumerable<EquipmentMaintDetailConsumer> SelectMaintConsumer(int equipment_maint_detail_id);
        void AddMaintConsumer(EquipmentMaintDetailConsumer newMaintConsumer);
        void UpdMaintConsumer(EquipmentMaintDetailConsumer updMaintConsumer);
        void BatchUpdate(string maintDetailNumber, List<EquipmentMaintDetailConsumer> listMaintConsumer);
        void DelMaintConsumer(EquipmentMaintDetailConsumer delMaintConsumer);

        #endregion

        IList<double> GetEquipmentPlanTimes(Equipment theEquipment);

    }
}
