using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aisino.MES.Model.Models;
//using Aisino.MES.DataCenter.Models.BusinessModel;

namespace Aisino.MES.Service.QualityManager
{
    public interface IQualityTestItemService
    {
        #region 检测指标 部分
        //增删改查
        IEnumerable<QualityIndex> GetRootQTestItem();
        QualityIndex GetQTestItem(int qTestItem_id);
        IEnumerable<QualityIndex> GetAllQTestItem();

        QualityIndex AddQTestItem(QualityIndex newQTestItem);
        QualityIndex UpdQTestItem(QualityIndex updQtestItem);
        void DelQTestItem(QualityIndex delQTestItem);
        void DeleteQTestItemList(List<QualityIndex> lstDelQTestItem);
        void UpdateAsHQ(IEnumerable<QualityIndex> qualityHQ, IEnumerable<QualityIndex> qualityItem);

        bool CheckQTestItemNameExist(string QTestItemName);

        
        #endregion

        #region 相关文本 部分
        //查询、添加、修改、删除
        //IEnumerable<QualityTestItemText> GetTestItemText(int qTestItem_id);
        //QualityTestItemText AddItemText(QualityTestItemText newItemText);
        //QualityTestItemText UpdItemText(QualityTestItemText updItemText);
        //void DelItemText(QualityTestItemText delItemText);
        //void DelItemTextList(List<QualityTestItemText> lstDelItemText);
        //
        #endregion
    }
}
