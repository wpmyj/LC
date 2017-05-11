using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.EquipmentManager
{
    public interface IEquipmentEffectService
    {


        /// 
        /// <param name="effectCode"></param>
        bool CheckEffectCodeExist(string effectCode);

        /// 
        /// <param name="effectName"></param>
        bool CheckEffectNameExist(string effectName);

   

        /// 
        /// <param name="newEquipmentEffect"></param>
        EquipmentMaintEffect CreateEquipmentEffect(EquipmentMaintEffect newEquipmentEffect);


 
        //取得处理方法
        IEnumerable<EquipmentMaintEffect> GetAllMaintEffect();

        /// 
        /// <param name="newEquipmentEffect"></param>
        EquipmentMaintEffect UpdateEquipmentEffect(EquipmentMaintEffect newEquipmentEffect);

        void DelEquipmentEffect(EquipmentMaintEffect delEquipmentEffect);



    }
}
