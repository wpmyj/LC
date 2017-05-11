using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IBorTypeService
    {
        IEnumerable<BorType> SelectAllBorType();
        BorType GetBorType(int id);
        BorType AddBorType(BorType newBorType);
        BorType UpdateBorType(BorType updBorType);
        BorType DeleteBorType(BorType delBorType);
        void DeleteBorTypeList(List<BorType> lstDelBorType);
        bool CheckBorTypeCodeExist(string borTypeCode);
        bool CheckBorTypeNameExist(string borTypeName);
    }
}
