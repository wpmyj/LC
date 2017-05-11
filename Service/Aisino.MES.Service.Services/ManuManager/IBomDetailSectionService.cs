using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IBomDetailSectionService
    {
        IEnumerable<BomDetailSection> SelectAllBomDetailSection();
        IEnumerable<BomDetailSection> GetBomDetailSectionIEnSub(int bomDetail_ID);
        IEnumerable<BomDetailSection> GetBomDetailSectionIEnSection(int borSection_ID);
        BomDetailSection GetBomDetailSection(int bomDetail_ID, int borSection_ID);
        void UpdateBomDetailSection(int bomDetailId, List<BomDetailSection> lstBomDetailSection);
    }
}
