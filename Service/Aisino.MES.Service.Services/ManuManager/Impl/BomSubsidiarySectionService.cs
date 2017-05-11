using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Enums;
using Aisino.MES.DAL.Interfaces;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class BomSubsidiarySectionService : IBomSubsidiarySectionService
    {
        private Repository<BomSubsidiarySection> _bomSubsidiarySectionDal;
        private UnitOfWork _unitOfWork;

        public BomSubsidiarySectionService(Repository<BomSubsidiarySection> bomSubsidiarySectionDal, UnitOfWork unitOfWork)
        {
            _bomSubsidiarySectionDal = bomSubsidiarySectionDal;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BomSubsidiarySection> SelectAllBomSubsidiarySection()
        {
            return _bomSubsidiarySectionDal.GetAll().Entities;
        }

        public IEnumerable<BomSubsidiarySection> GetBomSubsidiarySectionIEnSub(int bomSubsidiary_ID)
        {
            var bomSubsidiarySection = _bomSubsidiarySectionDal.Find(d => d.bom_subsidiary_id == bomSubsidiary_ID);
            return bomSubsidiarySection.Entities;
        }

        public IEnumerable<BomSubsidiarySection> GetBomSubsidiarySectionIEnSection(int borSection_ID)
        {
            var bomSubsidiarySection = _bomSubsidiarySectionDal.Find(d => d.bor_section_id == borSection_ID);
            return bomSubsidiarySection.Entities;
        }

        public BomSubsidiarySection GetBomSubsidiarySection(int bomSubsidiary_ID, int borSection_ID)
        {
            var bomSubsidiarySection = _bomSubsidiarySectionDal.Find(d => d.bom_subsidiary_id == bomSubsidiary_ID && d.bor_section_id == borSection_ID);
            if (bomSubsidiarySection.Entities.Count() > 0)
            {
                return bomSubsidiarySection.Entities.ToList()[0];
            }
            else
            {
                return null;
            }
        }

        public void UpdateBomSubsidiarySection(int bomSubsidiaryId, List<BomSubsidiarySection> lstBomSubsidiarySection)
        {
            try
            {
                List<BomSubsidiarySection> oldBomSubsidiarySection = _bomSubsidiarySectionDal.Find(rm => rm.bom_subsidiary_id == bomSubsidiaryId).Entities.ToList();

                if (oldBomSubsidiarySection != null && oldBomSubsidiarySection.Count > 0)
                {
                    foreach (BomSubsidiarySection bomSubsidiarySection in lstBomSubsidiarySection)
                    {
                        //查找选择的工段是否已存在
                        if (!oldBomSubsidiarySection.Any(d => d.bom_subsidiary_id == bomSubsidiarySection.bom_subsidiary_id && d.bor_section_id == bomSubsidiarySection.bor_section_id))
                        {
                            _unitOfWork.AddAction(bomSubsidiarySection, DataActions.Add);
                        }
                    }
                }
                else
                {
                    foreach (BomSubsidiarySection bomSubsidiarySection in lstBomSubsidiarySection)
                    {
                        _unitOfWork.AddAction(bomSubsidiarySection, DataActions.Add);
                    }
                }

                //查找之前选择的工段是否已删除
                if (oldBomSubsidiarySection != null)
                {
                    //原有路线所含工段不为空，则需要判断是否有删除项
                    foreach (BomSubsidiarySection bomSubsidiarySectionRemove in oldBomSubsidiarySection.Where(x => !lstBomSubsidiarySection.Any(u => u.bom_subsidiary_id == x.bom_subsidiary_id && u.bor_section_id == x.bor_section_id)).ToList())
                    {
                        _unitOfWork.AddAction(bomSubsidiarySectionRemove, DataActions.Delete);
                    }
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("更改副产品产出工段配置失败！", ex);
            }
        }
    }
}
