using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IBorSectionEquipmentService
    {
        IEnumerable<BorSectionEquipment> SelectAllBorSectionEquip();
        IEnumerable<BorSectionEquipment> GetBorSectionEquipIEnBorSec(int borSection_ID);
        IEnumerable<BorSectionEquipment> GetBorSectionEquipIEnEquip(string equipment_Code);
        BorSectionEquipment GetBorSectionEquip(int borSection_ID, string equipment_Code);
        void UpdateBorSectionEquip(int borSectionId, List<BorSectionEquipment> lstBorSectionEquip);
    }
}
