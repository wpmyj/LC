using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IBomSubsidiarySectionService
    {
        IEnumerable<BomSubsidiarySection> SelectAllBomSubsidiarySection();
        IEnumerable<BomSubsidiarySection> GetBomSubsidiarySectionIEnSub(int bomSubsidiary_ID);
        IEnumerable<BomSubsidiarySection> GetBomSubsidiarySectionIEnSection(int borSection_ID);
        BomSubsidiarySection GetBomSubsidiarySection(int bomSubsidiary_ID, int borSection_ID);
        void UpdateBomSubsidiarySection(int bomSubsidiaryId, List<BomSubsidiarySection> lstBomSubsidiarySection);
    }
}
