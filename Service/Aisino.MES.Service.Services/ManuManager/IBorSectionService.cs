using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IBorSectionService
    {
        IList<BorSection> SelectAllBorSection();
        IList<BorSection> SelectUndelBorSection();
        BorSection GetBorSection(int id);
        BorSection AddBorSection(BorSection newBorSection);
        BorSection UpdateBorSection(BorSection updBorSection);
        BorSection DeleteBorSection(BorSection delBorSection);
        void DeleteBorSectionList(List<BorSection> lstDelBorSection);
        bool CheckBorSectionCodeExist(string borSectionCode);
        bool CheckBorSectionNameExist(string borSectionName);
    }
}
