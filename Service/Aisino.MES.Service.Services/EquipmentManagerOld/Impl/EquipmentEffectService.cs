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
    public class EquipmentEffectService : IEquipmentEffectService
    {


        private Repository<EquipmentMaintEffect> _equipmentEffectDal;

        public EquipmentEffectService(Repository<EquipmentMaintEffect> equipmentEffectDal)
        {
            _equipmentEffectDal = equipmentEffectDal;
        }


        /// 
        /// <param name="effectCode"></param>
        public bool CheckEffectCodeExist(string effectCode)
        {
            var equipmentEffect = _equipmentEffectDal.Single(s => s.effect_code == effectCode);
            if (equipmentEffect.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }     
        }

        /// 
        /// <param name="effectName"></param>
        public bool CheckEffectNameExist(string effectName)
        {
            var equipmentEffect = _equipmentEffectDal.Single(s => s.effect_name == effectName);
            if (equipmentEffect.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }     
        }



        /// 
        /// <param name="newEquipmentEffect"></param>
        public EquipmentMaintEffect CreateEquipmentEffect(EquipmentMaintEffect newEquipmentEffect)
        {
            EquipmentMaintEffect returnMaintEffect = null;
            try
            {
                _equipmentEffectDal.Add(newEquipmentEffect);
                returnMaintEffect = newEquipmentEffect;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnMaintEffect;
        }

    

        //取得处理方法
        public IEnumerable<EquipmentMaintEffect> GetAllMaintEffect()
        {
            return _equipmentEffectDal.GetAll().Entities;
        }

        /// 
        /// <param name="newEquipmentEffect"></param>
        public EquipmentMaintEffect UpdateEquipmentEffect(EquipmentMaintEffect newEquipmentEffect)
        {
            EquipmentMaintEffect returnEquipmentEffect = null;
            try
            {
                _equipmentEffectDal.Update(newEquipmentEffect);
                returnEquipmentEffect = newEquipmentEffect;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnEquipmentEffect;

        }


        public void DelEquipmentEffect(EquipmentMaintEffect delEquipmentEffect)
        {
            try
            {
                _equipmentEffectDal.Delete(delEquipmentEffect);
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }
    }
}
