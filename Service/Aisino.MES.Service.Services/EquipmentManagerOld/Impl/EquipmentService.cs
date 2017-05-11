using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;

namespace Aisino.MES.Service.EquipmentManager.Impl
{
    public class EquipmentService : IEquipmentService
    {

        private Repository<EquipmentType> _equipmentTypeDal;
        private Repository<Equipment> _equipmentDal;

        public EquipmentService(Repository<EquipmentType> equipmentTypeDal, Repository<Equipment> equipmentDal)
        {
            _equipmentTypeDal = equipmentTypeDal;
            _equipmentDal = equipmentDal;
        }

        /// 
        /// <param name="equipmentTypeId"></param>
        public EquipmentType GetEquipmentType(string equipmentTypeCode)
        {
            var equipmentType = _equipmentTypeDal.Single(s => s.type_code == equipmentTypeCode);
            if (equipmentType.HasValue)
            {
                return equipmentType.Entity;
            }
            else
            {
                return null;
            }
        }


        /// 
        /// <param name="equipmentCode"></param>
        public Equipment GetEquipment(string equipmentCode)
        {
            var equipment = _equipmentDal.Single(s => s.equipment_code == equipmentCode);
            if (equipment.HasValue)
            {
                return equipment.Entity;
            }
            else
            {
                return null;
            }
        }
        
        public IList<EquipmentType> GetAllEquipmentType()
        {
               return _equipmentTypeDal.GetAll().Entities.ToList();
        }

        /// 
        /// <param name="typeCode"></param>
        public bool CheckTypeCodeExist(string typeCode)
        {
            var equipmentType = _equipmentTypeDal.Single(s => s.type_code == typeCode);
            if (equipmentType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        /// 
        /// <param name="typeName"></param>
        public bool CheckTypeNameExist(string typeName)
        {
            var equipmentType = _equipmentTypeDal.Single(s => s.type_name == typeName);
            if (equipmentType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }


        /// 
        /// <param name="equipmentCode"></param>
        public bool CheckEquipmentCodeExist(string equipmentCode)
        {
            var equipmentType = _equipmentDal.Single(s => s.equipment_code == equipmentCode);
            if (equipmentType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        public bool CheckEquipmentNameExist(string equipmentName)
        {
            var equipmentType = _equipmentDal.Single(s => s.equipment_name == equipmentName);
            if (equipmentType.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        /// 
        /// <param name="newEquipmentType"></param>
        public EquipmentType CreateEquipmentType(EquipmentType newEquipmentType)
        {
            EquipmentType returnEquipmentType = null;
            try
            {
                _equipmentTypeDal.Add(newEquipmentType);
                returnEquipmentType = newEquipmentType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnEquipmentType;
        }

        /// 
        /// <param name="newEquipmentType"></param>
        public EquipmentType UpdateEquipmentType(EquipmentType updateEquipmentType)
        {
            EquipmentType returnEquipmentType = null;
            try
            {
                _equipmentTypeDal.Update(updateEquipmentType);
                returnEquipmentType = updateEquipmentType;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnEquipmentType;

        }

        public void DeleteEquipmentTypeList(List<EquipmentType> lstDelEqpType)
        {
            try
            {
                //foreach (EquipmentType eqpType in lstDelEqpType)
                //{
                //    if (eqpType.type_deleted == true)
                //    {
                //        continue;
                //    }
                //    eqpType.type_deleted = true;
                //    _equipmentTypeDal.Update(eqpType);
                //}
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除设备类别信息失败！", ex);
            }
        }

        public IList<Equipment> GetAllEquipment()
        {
            return _equipmentDal.GetAll().Entities.ToList();
        }

        /// 
        /// <param name="newEquipment"></param>
        public Equipment CreateEquipment(Equipment newEquipment)
        {
            Equipment returnEquipment = null;
            try
            {
                _equipmentDal.Add(newEquipment);
                returnEquipment = newEquipment;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnEquipment;

        }



        /// 
        /// <param name="newEquipment"></param>
        public Equipment UpdateEquipment(Equipment updateEquipment)
        {
            Equipment returnEquipment = null;
            try
            {
                _equipmentDal.Update(updateEquipment);
                returnEquipment = updateEquipment;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return returnEquipment;

        }

        /// <summary>
        /// 批量删除设备
        /// </summary>
        /// <param name="lstDelEquipment"></param>
        public void DeleteEquipmentList(List<Equipment> lstDelEquipment)
        {
            try
            {
                foreach (Equipment delEquipment in lstDelEquipment)
                {
                    if (delEquipment.equipment_deleted == true)
                    {
                        continue;
                    }
                    delEquipment.equipment_deleted = true;
                    _equipmentDal.Update(delEquipment);
                }
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除设备信息失败！", ex);
            }
        }

        public IList<Equipment> GetNotDelEquipment()
        {
            return _equipmentDal.Find(e => e.equipment_deleted == false).Entities.ToList();
        }



        public IEnumerable<Equipment> GetEquipmentListByLine(int bor_line_id)
        {
            string esql = "select distinct equ.*"
                        + " from  BorLineSection sec, BorSectionEquipment secequ, Equipment equ"
                        + " where sec.bor_section_id = secequ.bor_section_id"
                        + " and secequ.equipment_id = equ.id"
                        + " and sec.bor_line_id = " + bor_line_id.ToString();


            return _equipmentDal.QueryByESql(esql).Entities;
        }
    }
}
