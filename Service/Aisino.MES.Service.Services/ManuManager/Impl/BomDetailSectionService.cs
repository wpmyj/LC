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
    public class BomDetailSectionService : IBomDetailSectionService
    {
        private Repository<BomDetailSection> _bomDetailSectionDal;
        private UnitOfWork _unitOfWork;

        public BomDetailSectionService(Repository<BomDetailSection> bomDetailSectionDal, UnitOfWork unitOfWork)
        {
            _bomDetailSectionDal = bomDetailSectionDal;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BomDetailSection> SelectAllBomDetailSection()
        {
            return _bomDetailSectionDal.GetAll().Entities;
        }

        public IEnumerable<BomDetailSection> GetBomDetailSectionIEnSub(int bomDetail_ID)
        {
            var bomDetailSection = _bomDetailSectionDal.Find(d => d.bom_detail_id == bomDetail_ID);
            return bomDetailSection.Entities;
        }

        public IEnumerable<BomDetailSection> GetBomDetailSectionIEnSection(int borSection_ID)
        {
            var bomDetailSection = _bomDetailSectionDal.Find(d => d.bor_section_id == borSection_ID);
            return bomDetailSection.Entities;
        }

        public BomDetailSection GetBomDetailSection(int bomDetail_ID, int borSection_ID)
        {
            var bomDetailSection = _bomDetailSectionDal.Find(d => d.bom_detail_id == bomDetail_ID && d.bor_section_id == borSection_ID);
            if (bomDetailSection.Entities.Count() > 0)
            {
                return bomDetailSection.Entities.ToList()[0];
            }
            else
            {
                return null;
            }
        }

        public void UpdateBomDetailSection(int bomDetailId, List<BomDetailSection> lstBomDetailSection)
        {
            try
            {
                List<BomDetailSection> oldBomDetailSection = _bomDetailSectionDal.Find(rm => rm.bom_detail_id == bomDetailId).Entities.ToList();

                if (oldBomDetailSection != null && oldBomDetailSection.Count > 0)
                {
                    foreach (BomDetailSection bomDetailSection in lstBomDetailSection)
                    {
                        //查找选择的工段是否已存在
                        if (!oldBomDetailSection.Any(d => d.bom_detail_id == bomDetailSection.bom_detail_id && d.bor_section_id == bomDetailSection.bor_section_id))
                        {
                            _unitOfWork.AddAction(bomDetailSection, DataActions.Add);
                        }
                    }
                }
                else
                {
                    foreach (BomDetailSection bomDetailSection in lstBomDetailSection)
                    {
                        _unitOfWork.AddAction(bomDetailSection, DataActions.Add);
                    }
                }

                //查找之前选择的工段是否已删除
                if (oldBomDetailSection != null)
                {
                    //原有路线所含工段不为空，则需要判断是否有删除项
                    foreach (BomDetailSection bomDetailSectionRemove in oldBomDetailSection.Where(x => !lstBomDetailSection.Any(u => u.bom_detail_id == x.bom_detail_id && u.bor_section_id == x.bor_section_id)).ToList())
                    {
                        _unitOfWork.AddAction(bomDetailSectionRemove, DataActions.Delete);
                    }
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("更改原料投料工段配置失败！", ex);
            }
        }
    }
}
