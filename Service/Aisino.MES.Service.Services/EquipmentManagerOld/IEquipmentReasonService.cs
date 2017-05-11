using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.EquipmentManager
{
    public interface IEquipmentReasonService
    {


        /// 
        /// <param name="reasonCode"></param>
        bool CheckReasonCodeExist(string reasonCode);

        /// 
        /// <param name="reasonName"></param>
        bool CheckReasonNameExist(string reasonName);

        /// 
        /// <param name="typeCode"></param>
        bool CheckTypeCodeExist(string typeCode);

  
        /// 
        /// <param name="newEquipmentReason"></param>
        EquipmentMaintReason CreateMaintReason(EquipmentMaintReason newEquipmentReason);

        /// 
        /// <param name="newEquipmentReasonType"></param>
        EquipmentMaintReasonType CreateMaintReasonType(EquipmentMaintReasonType newEquipmentReasonType);

        IList<EquipmentMaintReason> GetAllMaintReason();

        IList<EquipmentMaintReasonType> GetAllMaintReasonType();

        /// 
        /// <param name="newEquipmentReason"></param>
        EquipmentMaintReason UpdateMaintReason(EquipmentMaintReason newEquipmentReason);

        /// 
        /// <param name="newEquipmentReasonType"></param>
        EquipmentMaintReasonType UpdateMaintReasonType(EquipmentMaintReasonType newEquipmentReasonType);

        /// 
        /// <param name="lstDelReasonType"></param>
        void DeleteMaintReasonTypeList(List<EquipmentMaintReasonType> lstDelReasonType);

        /// 
        /// <param name="lstDelMaintReason"></param>
        void DeleteMaintReasonList(List<EquipmentMaintReason> lstDelMaintReason);
    }
}
