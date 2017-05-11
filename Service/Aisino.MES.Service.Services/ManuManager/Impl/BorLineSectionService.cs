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
    public class BorLineSectionService : IBorLineSectionService
    {
        private Repository<BorLineSection> _borLineSectionDal;
        private UnitOfWork _unitOfWork;

        public BorLineSectionService(Repository<BorLineSection> borLineSectionDal, UnitOfWork unitOfWork)
        {
            _borLineSectionDal = borLineSectionDal;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BorLineSection> SelectAllBorLineSection()
        {
            return _borLineSectionDal.GetAll().Entities;
        }

        public IEnumerable<BorLineSection> GetBorLineSectionIEnLine(int borLine_ID)
        {
            var borLineSection = _borLineSectionDal.Find(d => d.bor_line_id == borLine_ID);
            return borLineSection.Entities;
        }

        public IEnumerable<BorLineSection> GetBorLineSectionIEnSection(int borSection_ID)
        {
            var borLineSection = _borLineSectionDal.Find(d => d.bor_section_id == borSection_ID);
            return borLineSection.Entities;
        }

        public BorLineSection GetBorLineSection(int borLine_ID, int borSection_ID)
        {
            var borLineSection = _borLineSectionDal.Find(d => d.bor_line_id == borLine_ID && d.bor_section_id == borSection_ID);
            if (borLineSection.Entities.Count() > 0)
            {
                return borLineSection.Entities.ToList()[0];
            }
            else
            {
                return null;
            }
        }

        public void UpdateBorLineSection(int borLineId, List<BorLineSection> lstBorLineSection)
        {
            try
            {
                List<BorLineSection> oldBorLineSection = _borLineSectionDal.Find(rm => rm.bor_line_id == borLineId).Entities.ToList();

                if (oldBorLineSection != null && oldBorLineSection.Count > 0)
                {
                    foreach (BorLineSection borLineSection in lstBorLineSection)
                    {
                        //查找选择的工段是否已存在
                        if (!oldBorLineSection.Any(d => d.bor_line_id == borLineSection.bor_line_id && d.bor_section_id == borLineSection.bor_section_id))
                        {
                            _unitOfWork.AddAction(borLineSection, DataActions.Add);
                        }
                        //查找选择的工段是否已存在,如果存在，但是序号不同
                        if (oldBorLineSection.Any(d => d.bor_section_id == borLineSection.bor_section_id && d.bor_line_id == borLineSection.bor_line_id) && !oldBorLineSection.Any(d => d.seq == borLineSection.seq))
                        {
                            _unitOfWork.AddAction(borLineSection, DataActions.Update);
                        }
                    }
                }
                else
                {
                    foreach (BorLineSection borLineSection in lstBorLineSection)
                    {
                        _unitOfWork.AddAction(borLineSection, DataActions.Add);
                    }
                }

                //查找之前选择的工段是否已删除
                if (oldBorLineSection != null)
                {
                    //原有路线所含工段不为空，则需要判断是否有删除项
                    foreach (BorLineSection borLineSectionRemove in oldBorLineSection.Where(x => !lstBorLineSection.Any(u => u.bor_line_id == x.bor_line_id && u.bor_section_id == x.bor_section_id)).ToList())
                    {
                        _unitOfWork.AddAction(borLineSectionRemove, DataActions.Delete);
                    }
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("更改工艺路线所含工段失败！", ex);
            }
        }
    }
}
