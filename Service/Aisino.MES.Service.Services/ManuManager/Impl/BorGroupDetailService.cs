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
    public class BorGroupDetailService : IBorGroupDetailService
    {
        private Repository<BorGroupDetail> _borGroupDetailDal;
        private UnitOfWork _unitOfWork;

        public BorGroupDetailService(Repository<BorGroupDetail> borGroupDetailDal, UnitOfWork unitOfWork)
        {
            _borGroupDetailDal = borGroupDetailDal;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BorGroupDetail> SelectAllBorGroupDetail()
        {
            return _borGroupDetailDal.GetAll().Entities;
        }

        public IEnumerable<BorGroupDetail> GetBorGroupDetailIEnGroup(int borGroup_ID)
        {
            var borGroupDetail = _borGroupDetailDal.Find(d => d.bor_group_id == borGroup_ID);
            return borGroupDetail.Entities;
        }

        public IEnumerable<BorGroupDetail> GetBorGroupDetailIEnInvmas(int invmas_ID)
        {
            var borLineSection = _borGroupDetailDal.Find(d => d.goods_kind == invmas_ID);
            return borLineSection.Entities;
        }

        public BorGroupDetail GetBorGroupDetail(int borGroup_ID, int invmas_ID)
        {
            var borGroupDetail = _borGroupDetailDal.Find(d => d.bor_group_id == borGroup_ID && d.goods_kind == invmas_ID);
            if (borGroupDetail.Entities.Count() > 0)
            {
                return borGroupDetail.Entities.ToList()[0];
            }
            else
            {
                return null;
            }
        }

        public void UpdateBorGroupDetail(int borGroupId, List<BorGroupDetail> lstBorGroupDetail)
        {
            try
            {
                List<BorGroupDetail> oldBorGroupDetail = _borGroupDetailDal.Find(rm => rm.bor_group_id == borGroupId).Entities.ToList();

                if (oldBorGroupDetail != null && oldBorGroupDetail.Count > 0)
                {
                    foreach (BorGroupDetail borGroupDetail in lstBorGroupDetail)
                    {
                        //查找选择的货品是否已存在
                        if (!oldBorGroupDetail.Any(d => d.bor_group_id == borGroupDetail.bor_group_id && d.goods_kind == borGroupDetail.goods_kind))
                        {
                            _unitOfWork.AddAction(borGroupDetail, DataActions.Add);
                        }
                    }
                }
                else
                {
                    foreach (BorGroupDetail borGroupDetail in lstBorGroupDetail)
                    {
                        _unitOfWork.AddAction(borGroupDetail, DataActions.Add);
                    }
                }

                //查找之前选择的货品是否已删除
                if (oldBorGroupDetail != null)
                {
                    //原有品号组所含货品不为空，则需要判断是否有删除项
                    foreach (BorGroupDetail borGroupDetailRemove in oldBorGroupDetail.Where(x => !lstBorGroupDetail.Any(u => u.bor_group_id == x.bor_group_id && u.goods_kind == x.goods_kind)).ToList())
                    {
                        _unitOfWork.AddAction(borGroupDetailRemove, DataActions.Delete);
                    }
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("更改品号组所含货品失败！", ex);
            }
        }
    }
}
