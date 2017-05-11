using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aisino.MES.Model.Models;

namespace Aisino.MES.Service.ManuManager
{
    public interface IPlanTaskBatchStepStatuService
    {
        void AddPlantaskBatchStepState(PlanTaskBatchStepStatus newPTBStepState);
        void AddPlantaskBatchStepStateByUnitWork(PlanTaskBatchStepStatus newPTBStepState);
        bool ChechPlantaskBatchStepStatus(string batchNumber, int seq);
        PlanTaskBatchStepStatus GetPlantaskBatchStepStatus(string batchNumber);

        void AddStepAndPriceWithUnitWork(PlanTaskBatchStepStatus newPTBStepState, PlanTask thePlanTask, PlanTaskBatch thePlanTaskBatch);
        void UpdPriceWithUnitOfWork(PlanTask thePlanTask, PlanTaskBatch thePlanTaskBatch);
        void Add5StepPriceWithUnitWork(List<PlanTaskBatchStepStatus> stepList);
    }
}
