using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.EquipmentManager
{
    public interface IEquipmentService
    {
        /// 
        /// <param name="equipmentTypeCode"></param>
        EquipmentType GetEquipmentType(string equipmentTypeCode);


        /// 
        /// <param name="equipmentCode"></param>
        Equipment GetEquipment(string equipmentCode);

       

        /// 
        /// <param name="equipmentCode"></param>
        bool CheckEquipmentCodeExist(string equipmentCode);

        /// 
        /// <param name="equipmentName"></param>
        bool CheckEquipmentNameExist(string equipmentName);

        /// 
        /// <param name="typeCode"></param>
        bool CheckTypeCodeExist(string typeCode);

        /// 
        /// <param name="typeName"></param>
        bool CheckTypeNameExist(string typeName);

        /// 
        /// <param name="newEquipment"></param>
        Equipment CreateEquipment(Equipment newEquipment);

        /// 
        /// <param name="newEquipmentType"></param>
        EquipmentType CreateEquipmentType(EquipmentType newEquipmentType);

        IList<Equipment> GetAllEquipment();


        IList<EquipmentType> GetAllEquipmentType();
        //IList<EquipmentType> GetNotDelEquipmentType();

        /// 
        /// <param name="newEquipment"></param>
        Equipment UpdateEquipment(Equipment newEquipment);

        /// 
        /// <param name="lstDelEquipment"></param>
        void DeleteEquipmentList(List<Equipment> lstDelEquipment);

        /// 
        /// <param name="newEquipmentType"></param>
        EquipmentType UpdateEquipmentType(EquipmentType newEquipmentType);

        /// 
        /// <param name="lstDelEqpType"></param>
        void DeleteEquipmentTypeList(List<EquipmentType> lstDelEqpType);

        IEnumerable<Equipment> GetEquipmentListByLine(int bor_line_id);
    }
}



