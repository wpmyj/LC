using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IPlanTaskBatchVehicleService
    {
        void RefreshData();

        void AddPlanTaskBatchVehicle(PlanTaskBatch plantaskbatch, List<PlanTaskBatchVehicle> planVehicleList);

        void UpdatePlanTaskBatchVehicle(PlanTaskBatch plantaskbatch, List<PlanTaskBatchVehicle> planVehicleList);

        void DelPlanTaskBatchVehicle(List<PlanTaskBatchVehicle> planVehicleList, List<PlanTaskBatchVehicle> newplanVehicle);
    }
}
