using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;
using Aisino.MES.Service.SysManager.Impl;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class BorLineService : IBorLineService
    {
        private Repository<BorType> _borTypeDal;
        private Repository<BorLine> _borLineDal;
        private Repository<BorLineSection> _borLineSectionDal;
        private UnitOfWork _unitOfWork;

        public BorLineService(Repository<BorType> borTypeDal, Repository<BorLine> borLineDal, Repository<BorLineSection> borLineSectionDal, UnitOfWork unitOfWork)
        {
            _borTypeDal = borTypeDal;
            _borLineDal = borLineDal;
            _borLineSectionDal = borLineSectionDal;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BorLine> SelectAllBorLine()
        {
            return _borLineDal.GetAll().Entities;
        }

        public BorLine GetBorLine(int id)
        {
            var borLine = _borLineDal.Single(d => d.id == id);
            if (borLine.HasValue)
            {
                return borLine.Entity;
            }
            else
            {
                return null;
            }
        }

        public BorLine AddBorLine(BorLine newBorLine)
        {
            BorLine borLine = null;
            try
            {
                _borLineDal.Add(newBorLine);
                borLine = newBorLine;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加工艺路线信息失败！", ex);
            }
            return borLine;
        }

        public BorLine UpdateBorLine(BorLine updBorLine)
        {
            BorLine borLine = null;
            try
            {
                _borLineDal.Update(updBorLine);
                borLine = updBorLine;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("修改工艺路线信息失败！", ex);
            }
            return borLine;
        }

        public BorLine DeleteBorLine(BorLine delBorLine)
        {
            BorLine borLine = null;
            try
            {
                delBorLine.bor_line_deleted = true;
                _unitOfWork.AddAction(delBorLine, DataActions.Update);
                borLine = delBorLine;
                //删除路线工段表中对应的路线记录
                List<BorLineSection> borLineSectionLst = _borLineSectionDal.Find(d => d.bor_line_id == delBorLine.id).Entities.ToList();
                foreach (BorLineSection borLineSection in borLineSectionLst)
                {
                    _unitOfWork.AddAction(borLineSection, DataActions.Delete);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除工艺路线信息失败！", ex);
            }
            return borLine;
        }

        public void DeleteBorLineList(List<BorLine> lstDelBorLine)
        {
            try
            {
                foreach (BorLine delBorLine in lstDelBorLine)
                {
                    if (delBorLine.bor_line_deleted == true)
                    {
                        continue;
                    }

                    delBorLine.bor_line_deleted = true;
                    _unitOfWork.AddAction(delBorLine, DataActions.Update);
                    //删除路线工段表中对应的路线记录
                    List<BorLineSection> borLineSectionLst = _borLineSectionDal.Find(d => d.bor_line_id == delBorLine.id).Entities.ToList();
                    foreach (BorLineSection borLineSection in borLineSectionLst)
                    {
                        _unitOfWork.AddAction(borLineSection, DataActions.Delete);
                    }
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除工艺路线信息失败！", ex);
            }
        }

        public bool CheckBorLineCodeExist(string borLineCode)
        {
            var borLine = _borLineDal.Single(d => d.bor_line_code == borLineCode);
            if (borLine.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckBorLineNameExist(string borLineName)
        {
            var borLine = _borLineDal.Single(d => d.bor_line_name == borLineName);
            if (borLine.HasValue)
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
