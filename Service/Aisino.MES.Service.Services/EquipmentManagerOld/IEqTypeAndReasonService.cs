using System.Collections.Generic;

using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.EquipmentManager
{
    public interface IEqTypeAndReasonService
    {
        //增删改查
        EquipmentMaintReasonType AddEqTypeMtReasons(EquipmentMaintReasonType newEqTypeMaintReason);
        void UpdateExistMtReason(IEnumerable<EquipmentMaintReason> eqTypeMtReasons, string equipmentCode);
        void DelEqTypeMtReasons(EquipmentMaintReasonType eqTypeMtReason);
    }
}
