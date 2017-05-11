using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Aisino.MES.Model.Models;
using Aisino.MES.DAL.Repository.Repositories;
using Aisino.MES.DAL.Interfaces;
using Aisino.MES.Service.StoreProcedures;
using Aisino.MES.DAL.UnitOfWork;

namespace Aisino.MES.Service.ManuManager.Impl
{
    public class PlanTaskBatchStepStatuService : IPlanTaskBatchStepStatuService
    {
        private Repository<PlanTaskBatchStepStatus> _planTBStepStateDal;
        private SPGetSysDateTimeService _sPGetSysDateTimeService;
        private UnitOfWork _unitOfWork;


        public PlanTaskBatchStepStatuService(Repository<PlanTaskBatchStepStatus> planTBStepStateDal,
                                    SPGetSysDateTimeService sPGetSysDateTimeService,
                                    UnitOfWork unitOfWork)
        {
            _planTBStepStateDal = planTBStepStateDal;
            _sPGetSysDateTimeService = sPGetSysDateTimeService;
            _unitOfWork = unitOfWork;
        }

        public void AddPlantaskBatchStepState(PlanTaskBatchStepStatus newPTBStepState)
        {
            newPTBStepState.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
            _planTBStepStateDal.Add(newPTBStepState);
        }


        public void AddPlantaskBatchStepStateByUnitWork(PlanTaskBatchStepStatus newPTBStepState)
        {
            newPTBStepState.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
            _unitOfWork.AddAction(newPTBStepState, DAL.Enums.DataActions.Add);
        }


        public bool ChechPlantaskBatchStepStatus(string batchNumber, int seq)
        {
            var theTemp = _planTBStepStateDal.Single(ptbss => ptbss.plantask_batch_number == batchNumber && ptbss.seq == seq);
            if (theTemp.HasValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public PlanTaskBatchStepStatus GetPlantaskBatchStepStatus(string batchNumber)
        {
            return _planTBStepStateDal.Find(ptbss => ptbss.plantask_batch_number == batchNumber).Entities.ToList().LastOrDefault();
        }

        public void AddStepAndPriceWithUnitWork(PlanTaskBatchStepStatus newPTBStepState, PlanTask thePlanTask, PlanTaskBatch thePlanTaskBatch)
        {
            newPTBStepState.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
            _unitOfWork.AddAction(newPTBStepState, DAL.Enums.DataActions.Add);
            _unitOfWork.AddAction(thePlanTask, DAL.Enums.DataActions.Update);
            _unitOfWork.AddAction(thePlanTaskBatch, DAL.Enums.DataActions.Update);
            _unitOfWork.Save();
        }

        public void UpdPriceWithUnitOfWork(PlanTask thePlanTask, PlanTaskBatch thePlanTaskBatch)
        {
            _unitOfWork.AddAction(thePlanTask, DAL.Enums.DataActions.Update);
            _unitOfWork.AddAction(thePlanTaskBatch, DAL.Enums.DataActions.Update);
            _unitOfWork.Save();
        }

        public void Add5StepPriceWithUnitWork(System.Collections.Generic.List<PlanTaskBatchStepStatus> stepList)
        {
            foreach (PlanTaskBatchStepStatus tempStep in stepList)
            {
                tempStep.operate_time = _sPGetSysDateTimeService.GetSysDateTime();
                _unitOfWork.AddAction(tempStep, DAL.Enums.DataActions.Add);
            }
        }
    }

}
