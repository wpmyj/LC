using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class BorSectionEquipmentService : IBorSectionEquipmentService
    {
        private Repository<BorSectionEquipment> _borSectionEquipmentDal;
        private UnitOfWork _unitOfWork;

        public BorSectionEquipmentService(Repository<BorSectionEquipment> borSectionEquipmentDal, UnitOfWork unitOfWork)
        {
            _borSectionEquipmentDal = borSectionEquipmentDal;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BorSectionEquipment> SelectAllBorSectionEquip()
        {
            return _borSectionEquipmentDal.GetAll().Entities;
        }

        public IEnumerable<BorSectionEquipment> GetBorSectionEquipIEnBorSec(int borSection_ID)
        {
            var borSectionEquip = _borSectionEquipmentDal.Find(d => d.bor_section_id == borSection_ID);
            return borSectionEquip.Entities;
        }

        public IEnumerable<BorSectionEquipment> GetBorSectionEquipIEnEquip(string equipment_Code)
        {
            var borSectionEquip = _borSectionEquipmentDal.Find(d => d.equipment_code == equipment_Code);
            return borSectionEquip.Entities;
        }

        public BorSectionEquipment GetBorSectionEquip(int borSection_ID, string equipment_Code)
        {
            var borSectionEquip = _borSectionEquipmentDal.Find(d => d.bor_section_id == borSection_ID && d.equipment_code == equipment_Code);
            if (borSectionEquip.Entities.Count() > 0)
            {
                return borSectionEquip.Entities.ToList()[0];
            }
            else
            {
                return null;
            }
        }

        public void UpdateBorSectionEquip(int borSectionId, List<BorSectionEquipment> lstBorSectionEquip)
        {
            try
            {
                List<BorSectionEquipment> oldBorSectionEquip = _borSectionEquipmentDal.Find(rm => rm.bor_section_id == borSectionId).Entities.ToList();

                if (oldBorSectionEquip != null && oldBorSectionEquip.Count > 0)
                {
                    foreach (BorSectionEquipment borSectionEquip in lstBorSectionEquip)
                    {
                        //查找选择的设备是否已存在
                        if (!oldBorSectionEquip.Any(d => d.bor_section_id == borSectionEquip.bor_section_id && d.equipment_code == borSectionEquip.equipment_code))
                        {
                            _unitOfWork.AddAction(borSectionEquip, DataActions.Add);
                        }
                        //查找选择的设备是否已存在,如果存在，但是序号不同
                        if (oldBorSectionEquip.Any(d => d.bor_section_id == borSectionEquip.bor_section_id && d.equipment_code == borSectionEquip.equipment_code) && !oldBorSectionEquip.Any(d => d.seq == borSectionEquip.seq))
                        {
                            _unitOfWork.AddAction(borSectionEquip, DataActions.Update);
                        }
                    }
                }
                else
                {
                    foreach (BorSectionEquipment borSectionEquip in lstBorSectionEquip)
                    {
                        _unitOfWork.AddAction(borSectionEquip, DataActions.Add);
                    }
                }

                //查找之前选择的设备是否已删除
                if (oldBorSectionEquip != null)
                {
                    //原有工段所含设备不为空，则需要判断是否有删除项
                    foreach (BorSectionEquipment borSectionEquipRemove in oldBorSectionEquip.Where(x => !lstBorSectionEquip.Any(u => u.bor_section_id == x.bor_section_id && u.equipment_code == x.equipment_code)).ToList())
                    {
                        _unitOfWork.AddAction(borSectionEquipRemove, DataActions.Delete);
                    }
                }
                _unitOfWork.Save();
            }
            catch(RepositoryException ex)
            {
                throw new AisinoMesServiceException("更改工段所含设备失败！", ex);
            }
        }
    }
}
