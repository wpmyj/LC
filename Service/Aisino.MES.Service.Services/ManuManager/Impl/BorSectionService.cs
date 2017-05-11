using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class BorSectionService : IBorSectionService
    {
        private Repository<BorLineSection> _borLineSectionDal;
        private Repository<BorSectionEquipment> _borSectionEquipmentDal;
        private Repository<BorSection> _borSectionDal;
        private UnitOfWork _unitOfWork;

        public BorSectionService(Repository<BorLineSection> borLineSectionDal, Repository<BorSectionEquipment> borSectionEquipmentDal, Repository<BorSection> borSectionDal, UnitOfWork unitOfWork)
        {
            _borLineSectionDal = borLineSectionDal;
            _borSectionEquipmentDal = borSectionEquipmentDal;
            _borSectionDal = borSectionDal;
            _unitOfWork = unitOfWork;
        }

        public IList<BorSection> SelectAllBorSection()
        {
            return _borSectionDal.GetAll().Entities.ToList();
        }

        public IList<BorSection> SelectUndelBorSection()
        {
            return _borSectionDal.GetAll().Entities.Where(d => d.bor_section_deleted == false).ToList();
        }

        public BorSection GetBorSection(int id)
        {
            var borSection = _borSectionDal.Single(d => d.id == id);
            if (borSection.HasValue)
            {
                return borSection.Entity;
            }
            else
            {
                return null;
            }
        }

        public BorSection AddBorSection(BorSection newBorSection)
        {
            BorSection borSection = null;
            try
            {
                _borSectionDal.Add(newBorSection);
                borSection = newBorSection;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加工段信息失败！", ex);
            }
            return borSection;
        }

        public BorSection UpdateBorSection(BorSection updBorSection)
        {
            BorSection borSection = null;
            try
            {
                _borSectionDal.Update(updBorSection);
                borSection = updBorSection;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("修改工段信息失败！", ex);
            }
            return borSection;
        }

        public BorSection DeleteBorSection(BorSection delBorSection)
        {
            BorSection borSection = null;
            try
            {
                delBorSection.bor_section_deleted = true;
                _unitOfWork.AddAction(delBorSection, DataActions.Update);
                borSection = delBorSection;
                //删除路线工段表中对应的工段记录
                List<BorLineSection> borLineSectionLst = _borLineSectionDal.Find(d => d.bor_section_id == delBorSection.id).Entities.ToList();
                foreach (BorLineSection borLineSection in borLineSectionLst)
                {
                    _unitOfWork.AddAction(borLineSection, DataActions.Delete);
                }

                //删除工段设备表中对应的工段记录
                List<BorSectionEquipment> borSectionEquipmentLst = _borSectionEquipmentDal.Find(d => d.bor_section_id == delBorSection.id).Entities.ToList();
                foreach(BorSectionEquipment borSectionEquipment in borSectionEquipmentLst)
                {
                    _unitOfWork.AddAction(borSectionEquipment, DataActions.Delete);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除工段信息失败！", ex);
            }
            return borSection;
        }

        public void DeleteBorSectionList(List<BorSection> lstDelBorSection)
        {
            try
            {
                foreach (BorSection delBorSection in lstDelBorSection)
                {
                    if (delBorSection.bor_section_deleted == true)
                    {
                        continue;
                    }
                    delBorSection.bor_section_deleted = true;
                    _unitOfWork.AddAction(delBorSection, DataActions.Update);
                    //删除路线工段表中对应的工段记录
                    List<BorLineSection> borLineSectionLst = _borLineSectionDal.Find(d => d.bor_section_id == delBorSection.id).Entities.ToList();
                    foreach (BorLineSection borLineSection in borLineSectionLst)
                    {
                        _unitOfWork.AddAction(borLineSection, DataActions.Delete);
                    }

                    //删除工段设备表中对应的工段记录
                    List<BorSectionEquipment> borSectionEquipmentLst = _borSectionEquipmentDal.Find(d => d.bor_section_id == delBorSection.id).Entities.ToList();
                    foreach (BorSectionEquipment borSectionEquipment in borSectionEquipmentLst)
                    {
                        _unitOfWork.AddAction(borSectionEquipment, DataActions.Delete);
                    }
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除工段信息失败！", ex);
            }
        }

        public bool CheckBorSectionCodeExist(string borSectionCode)
        {
            var borSection = _borSectionDal.Single(d => d.bor_section_code == borSectionCode);
            if (borSection.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckBorSectionNameExist(string borSectionName)
        {
            var borSection = _borSectionDal.Single(d => d.bor_section_name == borSectionName);
            if (borSection.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
