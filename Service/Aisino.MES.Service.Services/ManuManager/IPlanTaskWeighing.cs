using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aisino.MES.Service.ManuManager
{
    public interface IPlanTaskWeighing
    {

        bool UpdatePlanTaskBatchDetail(double dWeight ,string strRfidCardNum);
    }
}
