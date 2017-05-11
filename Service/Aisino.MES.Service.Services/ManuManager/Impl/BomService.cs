using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.UnitOfWork;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Enums;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class BomService : IBomService
    {
        private Repository<BomMaster> _bomMasterDal;
        private Repository<BomDetail> _bomDetailDal;
        private Repository<BomSubsidiary> _bomSubsidiaryDal;
        private Repository<BomDetailSection> _bomDetailSectionDal;
        private Repository<BomSubsidiarySection> _bomSubsidiarySectionDal;
        private UnitOfWork _unitOfWork;

        public BomService(Repository<BomMaster> bomMasterDal, Repository<BomDetail> bomDetailDal, Repository<BomSubsidiary> bomSubsidiaryDal, Repository<BomDetailSection> bomDetailSectionDal, Repository<BomSubsidiarySection> bomSubsidiarySectionDal, UnitOfWork unitOfWork)
        {
            _bomMasterDal = bomMasterDal;
            _bomDetailDal = bomDetailDal;
            _bomSubsidiaryDal = bomSubsidiaryDal;
            _bomDetailSectionDal = bomDetailSectionDal;
            _bomSubsidiarySectionDal = bomSubsidiarySectionDal;
            _unitOfWork = unitOfWork;
        }

        #region BomMasterService
        public IEnumerable<BomMaster> SelectAllBomMaster()
        {
            return _bomMasterDal.GetAll().Entities;
        }

        public BomMaster GetBomMaster(int bomMaster_id)
        {
            var bomMaster = _bomMasterDal.Single(d => d.id == bomMaster_id);
            if (bomMaster.HasValue)
            {
                return bomMaster.Entity;
            }
            else
            {
                return null;
            }
        }

        public BomMaster AddBomMaster(BomMaster newBomMaster)
        {
            BomMaster bomMaster = null;
            try
            {
                _bomMasterDal.Add(newBomMaster);
                bomMaster = newBomMaster;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加物料清单主档信息失败！", ex);
            }
            return bomMaster;
        }

        public BomMaster UpdateBomMaster(BomMaster updBomMaster)
        {
            BomMaster bomMaster = null;
            try
            {
                _bomMasterDal.Update(updBomMaster);
                bomMaster = updBomMaster;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("修改物料清单主档信息失败！", ex);
            }
            return bomMaster;
        }

        public BomMaster DeleteBomMaster(BomMaster delBomMaster)
        {
            BomMaster bomMaster = null;
            try
            {
                delBomMaster.bom_master_deleted = true;
                _unitOfWork.AddAction(delBomMaster, DataActions.Update);
                bomMaster = delBomMaster;
                //删除BorGroupDetails表中对应的品号组记录
                //List<BorLine> borLineList = _borLineDal.Find(d => d.bor_group_master_id == delBorGroupMaster.id).Entities.ToList();
                //foreach (BorLine borLine in borLineList)
                //{
                //    _unitOfWork.AddAction(borLine, DataActions.Delete);
                //}

                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除物料清单主档信息失败！", ex);
            }
            return bomMaster;
        }

        public void DeleteBomMasterList(List<BomMaster> lstDelBomMaster)
        {
            try
            {
                foreach (BomMaster bomMaster in lstDelBomMaster)
                {
                    if (bomMaster.bom_master_deleted == true)
                    {
                        continue;
                    }
                    bomMaster.bom_master_deleted = true;
                    _bomMasterDal.Update(bomMaster);
                }
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除物料清单主档信息失败！", ex);
            }
        }

        public IEnumerable<BomMaster> SelectSearchBomMaster(string strWhere)
        {
            string esql = "select BomMaster.id,BomMaster.invmas_id,WarehouseInvmas.invmas_name as invmasName,bom_master_qty,bom_master_deleted from BomMaster,WarehouseInvmas where BomMaster.invmas_id=WarehouseInvmas.id and bom_master_deleted='false'";
            esql += strWhere;
            return _bomMasterDal.QueryByESql(esql).Entities;
        }
        #endregion

        #region BomDetailService
        public IEnumerable<BomDetail> SelectAllBomDetail()
        {
            return _bomDetailDal.GetAll().Entities;
        }

        public IEnumerable<BomDetail> GetBomDetailList(int bomMaster_id)
        {
            return _bomDetailDal.Find(d => d.bom_master_id == bomMaster_id).Entities;
        }

        public IEnumerable<BomDetail> GetBomDetailListByInvmasId(int invmas_id)
        {
            string esql = "select bomdetail.* "
                        + " from bomdetail, bommaster"
                        + " where bommaster.id = bomdetail.bom_master_id"
                        + " and bommaster.invmas_id = " + invmas_id.ToString();


            return _bomDetailDal.QueryByESql(esql).Entities;
        }

        public BomDetail GetBomDetail(int id)
        {
            var bomDetail = _bomDetailDal.Single(d => d.id == id);
            if (bomDetail.HasValue)
            {
                return bomDetail.Entity;
            }
            else
            {
                return null;
            }
        }

        public BomDetail AddBomDetail(BomDetail newBomDetail)
        {
            BomDetail bomDetail = null;
            try
            {
                _bomDetailDal.Add(newBomDetail);
                bomDetail = newBomDetail;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加物料清单明细信息失败！", ex);
            }
            return bomDetail;
        }

        public BomDetail UpdateBomDetail(BomDetail updBomDetail)
        {
            BomDetail bomDetail = null;
            try
            {
                _bomDetailDal.Update(updBomDetail);
                bomDetail = updBomDetail;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("修改物料清单明细信息失败！", ex);
            }
            return bomDetail;
        }

        public void DeleteBomDetail(BomDetail delBomDetail)
        {
            try
            {
                _unitOfWork.AddAction(delBomDetail, DataActions.Delete);
                //删除路线工段表中对应的路线记录
                List<BomDetailSection> bomDetailSectionLst = _bomDetailSectionDal.Find(d => d.bom_detail_id == delBomDetail.id).Entities.ToList();
                foreach (BomDetailSection bomDetailSection in bomDetailSectionLst)
                {
                    _unitOfWork.AddAction(bomDetailSection, DataActions.Delete);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除物料清单明细信息失败！", ex);
            }
        }

        public void DeleteBomDetailList(List<BomDetail> lstDelBomDetail)
        {
            try
            {
                foreach (BomDetail bomDetail in lstDelBomDetail)
                {
                    _unitOfWork.AddAction(bomDetail, DataActions.Delete);
                    //删除路线工段表中对应的路线记录
                    List<BomDetailSection> bomDetailSectionLst = _bomDetailSectionDal.Find(d => d.bom_detail_id == bomDetail.id).Entities.ToList();
                    foreach (BomDetailSection bomDetailSection in bomDetailSectionLst)
                    {
                        _unitOfWork.AddAction(bomDetailSection, DataActions.Delete);
                    }
                    
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除物料清单明细信息失败！", ex);
            }
        }

        public bool CheckAddBomDetail(BomDetail addBomDetail, BomMaster selectBomMaster, bool isBomMaster)
        {
            if (!isBomMaster)
            {
                return false;
            }
            foreach (BomMaster bomMaster in addBomDetail.GoodsKind.BomMasters)
            {
                if (bomMaster.BomDetails.Any(d => d.GoodsKind.goods_kind_id == selectBomMaster.GoodsKind.goods_kind_id))
                {
                    isBomMaster = false;
                    break;
                }
                else
                {
                    foreach (BomDetail bomDetail in bomMaster.BomDetails)
                    {
                        isBomMaster = CheckAddBomDetail(bomDetail, selectBomMaster, isBomMaster);
                    }
                }
            }
            return isBomMaster;
        }

        public IEnumerable<BomDetail> SelectSearchBomDetail(string strWhere)
        {
            string esql = "select BomDetail.id,bom_master_id,BomDetail.invmas_id,WarehouseInvmas.invmas_name as invmasName,bom_detail_index,bom_detail_stdqty,bom_detail_stdpar,bom_detail_badrat,bom_detail_badrat_exp,bom_detail_startdate,bom_detail_enddate from BomDetail,WarehouseInvmas where BomDetail.invmas_id=WarehouseInvmas.id";
            esql += strWhere;
            return _bomDetailDal.QueryByESql(esql).Entities;
        }
        #endregion

        #region BomSubsidiaryService
        public IEnumerable<BomSubsidiary> SelectAllBomSubsidiary()
        {
            return _bomSubsidiaryDal.GetAll().Entities;
        }

        public IEnumerable<BomSubsidiary> GetBomSubsidiaryList(int bomMaster_id)
        {
            return _bomSubsidiaryDal.Find(d => d.bom_master_id == bomMaster_id).Entities;
        }

        public IEnumerable<BomSubsidiary> GetBomSubsidiaryListByInvmasId(int invmas_id)
        {
            string esql = "select BomSubsidiary.* "
                        + " from BomSubsidiary, bommaster"
                        + " where bommaster.id = BomSubsidiary.bom_master_id"
                        + " and bommaster.invmas_id = " + invmas_id.ToString();


            return _bomSubsidiaryDal.QueryByESql(esql).Entities;
        }

        public BomSubsidiary GetBomSubsidiary(int id)
        {
            var bomSubsidiary = _bomSubsidiaryDal.Single(d => d.id == id);
            if (bomSubsidiary.HasValue)
            {
                return bomSubsidiary.Entity;
            }
            else
            {
                return null;
            }
        }

        public BomSubsidiary AddBomSubsidiary(BomSubsidiary newBomSubsidiary)
        {
            BomSubsidiary bomSubsidiary = null;
            try
            {
                _bomSubsidiaryDal.Add(newBomSubsidiary);
                bomSubsidiary = newBomSubsidiary;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("添加副产品信息失败！", ex);
            }
            return bomSubsidiary;
        }

        public BomSubsidiary UpdateBomSubsidiary(BomSubsidiary updBomSubsidiary)
        {
            BomSubsidiary bomSubsidiary = null;
            try
            {
                _bomSubsidiaryDal.Update(updBomSubsidiary);
                bomSubsidiary = updBomSubsidiary;
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("修改副产品信息失败！", ex);
            }
            return bomSubsidiary;
        }

        public void DeleteBomSubsidiary(BomSubsidiary delBomSubsidiary)
        {
            try
            {
                _unitOfWork.AddAction(delBomSubsidiary, DataActions.Delete);
                //删除路线工段表中对应的路线记录
                List<BomSubsidiarySection> bomSubsidiarySectionLst = _bomSubsidiarySectionDal.Find(d => d.bom_subsidiary_id == delBomSubsidiary.id).Entities.ToList();
                foreach (BomSubsidiarySection bomSubsidiarySection in bomSubsidiarySectionLst)
                {
                    _unitOfWork.AddAction(bomSubsidiarySection, DataActions.Delete);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("删除副产品信息失败！", ex);
            }
        }

        public void DeleteBomSubsidiarylList(List<BomSubsidiary> lstDelBomSubsidiary)
        {
            try
            {
                foreach (BomSubsidiary bomSubsidiary in lstDelBomSubsidiary)
                {
                    _unitOfWork.AddAction(bomSubsidiary, DataActions.Delete);
                    //删除路线工段表中对应的路线记录
                    List<BomSubsidiarySection> bomSubsidiarySectionLst = _bomSubsidiarySectionDal.Find(d => d.bom_subsidiary_id == bomSubsidiary.id).Entities.ToList();
                    foreach (BomSubsidiarySection bomSubsidiarySection in bomSubsidiarySectionLst)
                    {
                        _unitOfWork.AddAction(bomSubsidiarySection, DataActions.Delete);
                    }
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除副产品信息失败！", ex);
            }
        }

        public IEnumerable<BomSubsidiary> SelectSearchBomSubsidiary(string strWhere)
        {
            string esql = "select BomSubsidiary.id,bom_master_id,BomSubsidiary.id,WarehouseInvmas.invmas_name as invmasName,bom_subsidiary_stdqty from BomSubsidiary,WarehouseInvmas where BomSubsidiary.id=WarehouseInvmas.id";
            esql += strWhere;
            var aa = _bomSubsidiaryDal.QueryByESql(esql).Entities;
            return _bomSubsidiaryDal.QueryByESql(esql).Entities;
        }
        #endregion
    }
}
