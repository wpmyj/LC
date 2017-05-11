using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.EquipmentManager
{
    public interface IConsumerGoodsService
    {
        #region 耗材类别
        EquipmentConsumerType AddConsumerGoodType(EquipmentConsumerType newConsumerType);
        EquipmentConsumerType UpdConsumerGoodType(EquipmentConsumerType updConsumerType);
        EquipmentConsumerType DelConsumerGoodType(EquipmentConsumerType delConsumerType);
        void DelConsumerGoodTypeList(List<EquipmentConsumerType> delConsumerTypeList);
        IEnumerable<EquipmentConsumerType> GetAllConsumerType();
        bool CheckExistConsumerTypeCode(string TypeCode);
        bool CheckExistConsumerTypeName(string TypeName);
        #endregion


        #region 耗材
        EquipmentConsumerGood AddConsumerGood(EquipmentConsumerGood newConsumerType);
        EquipmentConsumerGood UpdConsumerGood(EquipmentConsumerGood updConsumerType);
        EquipmentConsumerGood DelConsumerGood(EquipmentConsumerGood delConsumerType);
        void DelConsumerGoodList(List<EquipmentConsumerGood> delConsumerGoodList);
        IEnumerable<EquipmentConsumerGood> GetAllConsumerGood();
        EquipmentConsumerGood SelectOneConsumerGood(int id);
        bool CheckExistConsumerGoodCode(string GoodCode);
        bool CheckExistConsumerGoodName(string GoodName);

        #endregion
    }
}
