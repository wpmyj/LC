using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IBomService
    {
        #region IBomMasterService
        IEnumerable<BomMaster> SelectAllBomMaster();
        BomMaster GetBomMaster(int id);
        BomMaster AddBomMaster(BomMaster newBomMaster);
        BomMaster UpdateBomMaster(BomMaster updBomMaster);
        BomMaster DeleteBomMaster(BomMaster delBomMaster);
        void DeleteBomMasterList(List<BomMaster> lstDelBomMaster);
        IEnumerable<BomMaster> SelectSearchBomMaster(string strWhere);
        #endregion

        #region IBomDetailService
        IEnumerable<BomDetail> SelectAllBomDetail();
        IEnumerable<BomDetail> GetBomDetailList(int bomMaster_id);
        IEnumerable<BomDetail> GetBomDetailListByInvmasId(int invmas_id);
        BomDetail GetBomDetail(int id);
        BomDetail AddBomDetail(BomDetail newBomDetail);
        BomDetail UpdateBomDetail(BomDetail updBomDetail);
        void DeleteBomDetail(BomDetail delBomDetail);
        void DeleteBomDetailList(List<BomDetail> lstDelBomDetail);
        bool CheckAddBomDetail(BomDetail addBomDetail, BomMaster selectBomMaster, bool isBomMaster);
        IEnumerable<BomDetail> SelectSearchBomDetail(string strWhere);
        #endregion

        #region IBomSubsidiaryService
        IEnumerable<BomSubsidiary> SelectAllBomSubsidiary();
        IEnumerable<BomSubsidiary> GetBomSubsidiaryList(int bomMaster_id);
        IEnumerable<BomSubsidiary> GetBomSubsidiaryListByInvmasId(int invmas_id);
        BomSubsidiary GetBomSubsidiary(int id);
        BomSubsidiary AddBomSubsidiary(BomSubsidiary newBomSubsidiary);
        BomSubsidiary UpdateBomSubsidiary(BomSubsidiary updBomSubsidiary);
        void DeleteBomSubsidiary(BomSubsidiary delBomSubsidiary);
        void DeleteBomSubsidiarylList(List<BomSubsidiary> lstDelBomSubsidiary);
        IEnumerable<BomSubsidiary> SelectSearchBomSubsidiary(string strWhere);
        #endregion
    }
}
