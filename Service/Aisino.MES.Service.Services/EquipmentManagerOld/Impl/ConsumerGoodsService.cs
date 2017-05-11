using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.EquipmentManager.Impl
{
    public class ConsumerGoodsService : IConsumerGoodsService
    {
        private Repository<EquipmentConsumerType> _eqConsumerTypeDal;
        private Repository<EquipmentConsumerGood> _eqConsumerGoodDal;
        private UnitOfWork _unitOfWork;

        public ConsumerGoodsService(Repository<EquipmentConsumerType> eqConsumerTypeDal,
                                    Repository<EquipmentConsumerGood> eqConsumerGoodDal,
                                    UnitOfWork unitOfWork)
        {
            _eqConsumerTypeDal = eqConsumerTypeDal;
            _eqConsumerGoodDal = eqConsumerGoodDal;
            _unitOfWork = unitOfWork;
        }

        #region 耗材类别

        public EquipmentConsumerType AddConsumerGoodType(EquipmentConsumerType newConsumerType)
        {
            EquipmentConsumerType rtEquipmentConsumerType = null;
            try
            {
                _eqConsumerTypeDal.Add(newConsumerType);
                rtEquipmentConsumerType = newConsumerType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rtEquipmentConsumerType;
        }

        public EquipmentConsumerType UpdConsumerGoodType(EquipmentConsumerType updConsumerType)
        {
            EquipmentConsumerType rtEquipmentConsumerType = null;
            try
            {
                _eqConsumerTypeDal.Update(updConsumerType);
                rtEquipmentConsumerType = updConsumerType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rtEquipmentConsumerType;
        }

        public EquipmentConsumerType DelConsumerGoodType(EquipmentConsumerType delConsumerType)
        {
            EquipmentConsumerType rtEquipmentConsumerType = null;
            try
            {
                if (delConsumerType.EquipmentConsumerGoods.Count != 0)
                {
                    throw new Exception("该耗材类别有下属耗材，无法删除");
                }
                _eqConsumerTypeDal.Delete(delConsumerType);
                rtEquipmentConsumerType = delConsumerType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rtEquipmentConsumerType;
        }

        public void DelConsumerGoodTypeList(List<EquipmentConsumerType> delConsumerTypeList)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                foreach (EquipmentConsumerType reasonType in delConsumerTypeList)
                {
                    _unitOfWork.AddAction(reasonType, DataActions.Delete);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除耗材类别信息失败！", ex);
            }
        }

        public IEnumerable<EquipmentConsumerType> GetAllConsumerType()
        {
            return _eqConsumerTypeDal.GetAll().Entities;
        }
        #endregion



        #region 耗材

        public EquipmentConsumerGood AddConsumerGood(EquipmentConsumerGood newConsumerType)
        {
            EquipmentConsumerGood rtEqConsumnerGood = null;
            try
            {
                _eqConsumerGoodDal.Add(newConsumerType);
                rtEqConsumnerGood = newConsumerType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rtEqConsumnerGood;
        }

        public EquipmentConsumerGood UpdConsumerGood(EquipmentConsumerGood updConsumerType)
        {
            EquipmentConsumerGood rtEqConsumnerGood = null;
            try
            {
                _eqConsumerGoodDal.Update(updConsumerType);
                rtEqConsumnerGood = updConsumerType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rtEqConsumnerGood;
        }

        public EquipmentConsumerGood DelConsumerGood(EquipmentConsumerGood delConsumerGood)
        {
            EquipmentConsumerGood rtEqConsumnerGood = null;
            try
            {
                if (delConsumerGood.EquipmentMaintDetailConsumers.Count != 0)
                //throw new RepositoryException("耗材 " + delConsumerGood.consumer_goods_name + " 已被使用，无法删除。", new Exception()); 
                {

                }
                _eqConsumerGoodDal.Delete(delConsumerGood);
                rtEqConsumnerGood = delConsumerGood;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return rtEqConsumnerGood;
        }

        public void DelConsumerGoodList(List<EquipmentConsumerGood> delConsumerGoodList)
        {
            try
            {
                _unitOfWork.Actions.Clear();
                foreach (EquipmentConsumerGood reasonGood in delConsumerGoodList)
                {
                    if (reasonGood.EquipmentMaintDetailConsumers.Count != 0)
                    //throw new RepositoryException("耗材 " + reasonGood.consumer_goods_name + " 已被使用，无法删除。", new Exception());
                    { }
                    _unitOfWork.AddAction(reasonGood, DataActions.Delete);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除耗材信息失败！", ex);
            }
        }

        public IEnumerable<EquipmentConsumerGood> GetAllConsumerGood()
        {
            return _eqConsumerGoodDal.GetAll().Entities;
        }

        public EquipmentConsumerGood SelectOneConsumerGood(int id)
        {
            return _eqConsumerGoodDal.Single(eqc => eqc.id == id).Entity;
        }

        #endregion



        public bool CheckExistConsumerTypeCode(string TypeCode)
        {
            var eqConsumerType = _eqConsumerTypeDal.Single(s => s.consumer_type_code == TypeCode);
            if (eqConsumerType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckExistConsumerTypeName(string TypeName)
        {
            var eqConsumerType = _eqConsumerTypeDal.Single(s => s.consumer_type_name == TypeName);
            if (eqConsumerType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckExistConsumerGoodCode(string GoodCode)
        {
            var eqConsumerGood = _eqConsumerGoodDal.Single(s => s.consumer_goods_code == GoodCode);
            if (eqConsumerGood.HasValue)
                return true;
            else
                return false;
        }

        public bool CheckExistConsumerGoodName(string GoodName)
        {
            var eqConsumerGood = _eqConsumerGoodDal.Single(s => s.consumer_goods_name == GoodName);
            if (eqConsumerGood.HasValue)
                return true;
            else
                return false;
        }
    }
}
