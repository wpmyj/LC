using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IBorLineSectionService
    {
        IEnumerable<BorLineSection> SelectAllBorLineSection();
        IEnumerable<BorLineSection> GetBorLineSectionIEnLine(int borLine_ID);
        IEnumerable<BorLineSection> GetBorLineSectionIEnSection(int borSection_ID);
        BorLineSection GetBorLineSection(int borLine_ID, int borSection_ID);
        void UpdateBorLineSection(int borLineId, List<BorLineSection> lstBorLineSection);
    }
}
