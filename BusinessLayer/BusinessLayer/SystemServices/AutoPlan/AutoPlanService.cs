using System;
using BL.CrossCutting.Interfaces;
using System.Threading;
using BL.Database.Documents.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemServices.QueueWorker;
using BL.Model.Enums;
using System.Linq;
using BL.CrossCutting.Context;

namespace BL.Logic.SystemServices.AutoPlan
{
    public class AutoPlanService : IAutoPlanService
    {
        protected readonly ILogger Logger;
        private readonly ISystemDbProcess _sysDb;
        private readonly IDocumentsDbProcess _docDb;
        private readonly IQueueWorkerService _workerSrv;
        private readonly ICommandService _cmdService;

        public AutoPlanService(ILogger logger, ICommandService cmdService, IQueueWorkerService workerService, ISystemDbProcess sysDb, IDocumentsDbProcess docDb)
        {
            _sysDb = sysDb;
            _docDb = docDb;
            _cmdService = cmdService;
            _workerSrv = workerService;
            Logger = logger;
        }

        public bool AutoPlanTask(IContext admCtx, int? sendListId = null, int? documentId = null)
        {
            var lst = _sysDb.GetSendListIdsForAutoPlan(admCtx, sendListId, documentId);
            var isRepeat = lst.Any() && documentId != null && sendListId == null;

            foreach (int id in lst)
            {
                try
                {
                    var cmd = DocumentCommandFactory.GetDocumentCommand(EnumActions.LaunchDocumentSendListItem, admCtx, null, id);
                    _cmdService.ExecuteCommand(cmd);
                }
                catch (Exception ex)
                {
                    isRepeat = false;
                    Logger.Error(admCtx, $"AutoPlanService cannot process SendList Id={id} ", ex);
                    try
                    {
                        var docId = _docDb.GetDocumentIdBySendListId(admCtx, id);
                        if (docId > 0)
                        {
                            var cmdStop = DocumentCommandFactory.GetDocumentCommand(EnumActions.StopPlan, admCtx, null, docId);
                            _cmdService.ExecuteCommand(cmdStop);
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger.Error(admCtx, $"AutoPlanService cannot Stop plan for SendList Id={id} ", ex2);
                    }
                }
            }
            return isRepeat;
        }

        public bool ManualRunAutoPlan(IContext userContext, int? sendListId = null, int? documentId = null)
        {
            var admCtx = new AdminContext(userContext);
            var wrkUnit = new QueueTask(() =>
            {
                bool isRepeat = true;
                try
                {
                    while (isRepeat)
                    {
                        isRepeat = AutoPlanTask(admCtx, sendListId, documentId);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(admCtx, "Could not process autoplan", ex);
                }
            });
            _workerSrv.AddNewTask(admCtx, wrkUnit);

            // here we can do it through manual or automate reset events or analize wrkr.WorkCompleted event, but do it just stupped and simple
            while (wrkUnit.Status != EnumWorkStatus.Success && wrkUnit.Status != EnumWorkStatus.Error)
            {
                Thread.Sleep(50);
            }
            return wrkUnit.Status == EnumWorkStatus.Success;
        }
    }
}