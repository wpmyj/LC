using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Enums;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.UnitOfWork;
//using Aisino.MES.DataCenter.Models.BusinessModel;

namespace Aisino.MES.Service.QualityManager.Impl
{
    public class QualityTestItemService : IQualityTestItemService
    {
        private Repository<QualityIndex> _qualityTestItemDal;
        //private Repository<QualityTestItemText> _qualityTestItemTextDal;
        private UnitOfWork _unitOfWork;

        public QualityTestItemService(Repository<QualityIndex> qualityTestItemDal, 
                                        //Repository<QualityTestItemText> qualityTestItemTextDal,
                                        UnitOfWork unitOfWork )
        {
            _qualityTestItemDal = qualityTestItemDal;
            //_qualityTestItemTextDal = qualityTestItemTextDal;
            _unitOfWork = unitOfWork;
        }

        #region 检测指标 部分
        public IEnumerable<QualityIndex> GetRootQTestItem()
        {
            //return _qualityTestItemDal.Find(q => q.parent_id == null).Entities;
            return null;
        }

        public QualityIndex GetQTestItem(int qTestItem_id)
        {
            //var qTestItem = _qualityTestItemDal.Single(q => q.id == qTestItem_id);
            //if (qTestItem.HasValue)
            //{
            //    return qTestItem.Entity;
            //}
            //else
            //{
            //    return null;
            //}
            return null;
        }

        public QualityIndex AddQTestItem(QualityIndex newQTestItem)
        {
            QualityIndex reQTestItem = null;
            try
            {
                _qualityTestItemDal.Add(newQTestItem);
                reQTestItem = newQTestItem;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reQTestItem;
        }

        public QualityIndex UpdQTestItem(QualityIndex updQtestItem)
        {
            QualityIndex reQTestItem = null;
            try
            {
                _qualityTestItemDal.Update(updQtestItem);
                reQTestItem = updQtestItem;
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
            return reQTestItem;
        }

        public void DelQTestItem(QualityIndex delQTestItem)
        {
            try
            {
                DelSubQTestItem(delQTestItem);
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw ex;
            }
        }

        private void DelSubQTestItem(QualityIndex delSubQTestItem)
        {
            //if (delSubQTestItem.SubQualityTestItem != null || delSubQTestItem.SubQualityTestItem.Count != 0)
            //{
            //    foreach (QualityTestItem subQuality in delSubQTestItem.SubQualityTestItem)
            //    {
            //        DelSubQTestItem(subQuality);
            //    }
            //}
            _unitOfWork.AddAction(delSubQTestItem, DataActions.Delete);
        }

        public void DeleteQTestItemList(List<QualityIndex> lstDelQTestItem)
        {
            try
            {
                foreach (QualityIndex delQTestItem in lstDelQTestItem)
                {
                    DelSubQTestItem(delQTestItem);
                }
                _unitOfWork.Save();
            }
            catch (RepositoryException ex)
            {
                throw new AisinoMesServiceException("批量删除指标信息失败！", ex);
            }
        }

        public bool CheckQTestItemNameExist(string QTestItemName)
        {
            //var qTestItem = _qualityTestItemDal.Single(q => q.quality_testitem_name == QTestItemName);
            //if (qTestItem.HasValue)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return false;
        }

        

        #endregion

        #region 相关文本  部分

        //public IEnumerable<QualityTestItemText> GetTestItemText(int qTestItem_id)
        //{
        //    return _qualityTestItemTextDal.Find(q => q.quality_testitem_id == qTestItem_id).Entities;
        //}

        //public QualityTestItemText AddItemText(QualityTestItemText newItemText)
        //{
        //    QualityTestItemText reQTestItemText = null;
        //    try
        //    {
        //        _qualityTestItemTextDal.Add(newItemText);
        //        reQTestItemText = newItemText;
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        throw ex;
        //    }
        //    return reQTestItemText;
        //}

        //public QualityTestItemText UpdItemText(QualityTestItemText updItemText)
        //{
        //    QualityTestItemText reQTestItemText = null;
        //    try
        //    {
        //        _qualityTestItemTextDal.Update(updItemText);
        //        reQTestItemText = updItemText;
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        throw ex;
        //    }
        //    return reQTestItemText;
        //}

        //public void DelItemText(QualityTestItemText delItemText)
        //{
        //    try
        //    {
        //        _qualityTestItemTextDal.Delete(delItemText);
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public void DelItemTextList(List<QualityTestItemText> lstDelItemText)
        //{
        //    try
        //    {
        //        foreach (QualityTestItemText delItemText in lstDelItemText)
        //        {
        //            _qualityTestItemTextDal.Delete(delItemText);
        //        }
        //    }
        //    catch (RepositoryException ex)
        //    {
        //        throw new AisinoMesServiceException("批量删除相关文本信息失败！", ex);
        //    }
        //}
        #endregion

        public IEnumerable<QualityIndex> GetAllQTestItem()
        {
            return _qualityTestItemDal.GetAll().Entities;
        }


        public void UpdateAsHQ(IEnumerable<QualityIndex> qualityHQ, IEnumerable<QualityIndex> qualityItem)
        {
            foreach (QualityIndex qIndex in qualityHQ)
            {                
                var temp = _qualityTestItemDal.Single(q => q.quality_index_id == qIndex.quality_index_id);
                if (temp.HasValue)
                {
                    //if exist,just update
                    //如果已经存在了，就更新（Update）
                    QualityIndex qualityTemp = temp.Entity;
                    //qualityTemp.parent_id = qIndex.parent_quality_index;
                    //qualityTemp.quality_index_id = qIndex.quality_index_id;
                    //qualityTemp.quality_testitem_and_flag = qIndex.quality_index_and_flag;
                    //qualityTemp.quality_testitem_assess = qIndex.quality_index_assess;
                    //qualityTemp.quality_testitem_assess_and = qIndex.quality_index_assess_and;
                    //qualityTemp.quality_testitem_center 
                    //qualityTemp.quality_testitem_des 
                    //qualityTemp.quality_testitem_downlim
                    //qualityTemp.quality_testitem_is_comparsion = qIndex.quality_index_is_comparsion;
                    //qualityTemp.quality_testitem_is_necessary = qIndex.quality_index_is_necessary;
                    //qualityTemp.quality_testitem_name = qIndex.quality_index_name;
                    //qualityTemp.quality_testitem_property_type = qIndex.quality_index_property_type;

                    //qualityTemp.quality_testitem_type 
                    //qualityTemp.quality_testitem_uplim
                    //qualityTemp.quality_testitem_value = qIndex.quality_index_value;
                    //qualityTemp.quality_testitem_value_and = qIndex.quality_index_value_and;
                    //qualityTemp.quality_testitme_unit = qIndex.quality_index_unit;

                    _unitOfWork.AddAction(qualityTemp, DataActions.Update);
                }
                else
                {
                    //if not exist,just add a new QualityTestItem
                    //如果不存在，就添加
                    QualityIndex qualityTemp = new QualityIndex();
                    //qualityTemp.parent_id = qIndex.parent_quality_index;
                    //qualityTemp.quality_index_id = qIndex.quality_index_id;
                    //qualityTemp.quality_testitem_and_flag = qIndex.quality_index_and_flag;
                    //qualityTemp.quality_testitem_assess = qIndex.quality_index_assess;
                    //qualityTemp.quality_testitem_assess_and = qIndex.quality_index_assess_and;
                    //qualityTemp.quality_testitem_center 
                    //qualityTemp.quality_testitem_des 
                    ////qualityTemp.quality_testitem_downlim
                    //qualityTemp.quality_testitem_is_comparsion = qIndex.quality_index_is_comparsion;
                    //qualityTemp.quality_testitem_is_necessary = qIndex.quality_index_is_necessary;
                    //qualityTemp.quality_testitem_name = qIndex.quality_index_name;
                    //qualityTemp.quality_testitem_property_type = qIndex.quality_index_property_type;

                    //qualityTemp.quality_testitem_type 
                    ////qualityTemp.quality_testitem_uplim
                    //qualityTemp.quality_testitem_value = qIndex.quality_index_value;
                    //qualityTemp.quality_testitem_value_and = qIndex.quality_index_value_and;
                    //qualityTemp.quality_testitme_unit = qIndex.quality_index_unit;

                    _unitOfWork.AddAction(qualityTemp, DataActions.Add);
                }
            }
            _unitOfWork.Save();
            
            //处理parent_id
            IEnumerable<QualityIndex> qualityList = _qualityTestItemDal.Find(q => q.quality_index_id != null).Entities;
            foreach (QualityIndex qTestItem in qualityList)
            {
                //if (qTestItem.parent_id != null)
                //{
                //    qTestItem.parent_id = qualityList.Single(q => q.quality_index_id == qTestItem.parent_id).id;
                //    _unitOfWork.AddAction(qTestItem, DataActions.Update);
                //}
            }
            _unitOfWork.Save();
        }
    }
}
