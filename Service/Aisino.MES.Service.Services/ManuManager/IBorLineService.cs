using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IBorLineService
    {
        IEnumerable<BorLine> SelectAllBorLine();
        BorLine GetBorLine(int id);
        BorLine AddBorLine(BorLine newBorLine);
        BorLine UpdateBorLine(BorLine updBorLine);
        BorLine DeleteBorLine(BorLine delBorLine);
        void DeleteBorLineList(List<BorLine> lstDelBorLine);
        bool CheckBorLineCodeExist(string borLineCode);
        bool CheckBorLineNameExist(string borLineName);
    }
}
