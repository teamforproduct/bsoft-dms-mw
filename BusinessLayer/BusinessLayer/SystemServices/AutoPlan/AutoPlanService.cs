using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AutoPlan;
using System.Threading;
using BL.CrossCutting.DependencyInjection;
using BL.Database.Documents.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemServices.QueueWorker;
using BL.Model.Enums;
using BL.Database.Dictionaries;
using System.Linq;

namespace BL.Logic.SystemServices.AutoPlan
{
    public class AutoPlanService : BaseSystemWorkerService, IAutoPlanService
    {
        private readonly Dictionary<AutoPlanSettings, Timer> _timers;
        private ISystemDbProcess _sysDb;
        private IDocumentsDbProcess _docDb;
        private DictionariesDbProcess _dicDb;
        private IQueueWorkerService _workerSrv;
        private ICommandService _cmdService;

        public AutoPlanService(ISettings settings, ILogger logger, ICommandService cmdService, IQueueWorkerService workerService) : base(settings, logger)
        {
            _sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
            _docDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            _dicDb = DmsResolver.Current.Get<DictionariesDbProcess>();
            _cmdService = cmdService;
            _timers = new Dictionary<AutoPlanSettings, Timer>();
            _workerSrv = workerService;
        }

        protected override void InitializeServers()
        {
            foreach (var keyValuePair in ServerContext)
            {
                try
                {
                    var ftsSetting = new AutoPlanSettings
                    {
                        TimeToUpdate = Settings.GetAutoplanTimeoutMinute(keyValuePair.Value),
                        DatabaseKey = keyValuePair.Key,
                    };
                    var tmr = new Timer(OnSinchronize, ftsSetting, ftsSetting.TimeToUpdate * 60000, Timeout.Infinite);
                    _timers.Add(ftsSetting, tmr);
                }
                catch (Exception ex)
                {
                    Logger.Error(keyValuePair.Value, "Could not start AutoPlan for server", ex);
                }
            }
        }

        private Timer GetTimer(AutoPlanSettings key)
        {
            Timer res = null;
            lock (LockObjectTimer)
            {
                if (_timers.ContainsKey(key))
                    res = _timers[key];
            }
            return res;
        }

        public bool ManualRunAutoPlan(IContext userContext, int? sendListId = null, int? documentId = null)
        {
            var srvKey = CommonSystemUtilities.GetServerKey(userContext);

            var ctx = GetAdminContext(srvKey);
            if (ctx == null) return false;

            var wrkUnit = new QueueTask(() =>
            {
                bool isRepeat = true;

                try
                {
                    while (isRepeat)
                    {
                        var lst = _sysDb.GetSendListIdsForAutoPlan(ctx, sendListId, documentId);
                        if (lst.Any() && documentId != null && sendListId == null)
                            isRepeat = true;
                        else
                            isRepeat = false;
                        foreach (int id in lst)
                        {
                            try
                            {
                                var cmd = DocumentCommandFactory.GetDocumentCommand(Model.Enums.EnumDocumentActions.LaunchDocumentSendListItem, ctx, null, id);
                                _cmdService.ExecuteCommand(cmd);
                            }
                            catch (Exception ex)
                            {
                                isRepeat = false;
                                Logger.Error(ctx, $"AutoPlanService cannot process SendList Id={id} ", ex);
                                try
                                {
                                    var docId = _docDb.GetDocumentIdBySendListId(ctx, id);
                                    if (docId > 0)
                                    {
                                        var cmdStop = DocumentCommandFactory.GetDocumentCommand(EnumDocumentActions.StopPlan, ctx, null, docId);
                                        _cmdService.ExecuteCommand(cmdStop);
                                    }
                                }
                                catch (Exception ex2)
                                {
                                    Logger.Error(ctx, $"AutoPlanService cannot Stop plan for SendList Id={id} ", ex2);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ctx, "Could not process autoplan", ex);
                }
            }) {CurrentContext = ctx};
            _workerSrv.AddNewTask(ctx, wrkUnit);

            // here we can do it through manual or automate reset events or analize wrkr.WorkCompleted event, but do it just stupped and simple
            while (wrkUnit.Status != EnumWorkStatus.Success && wrkUnit.Status != EnumWorkStatus.Error)
            {
                Thread.Sleep(50);
            }
            return wrkUnit.Status == EnumWorkStatus.Success;
        }

        private void OnSinchronize(object state)
        {
            var md = state as AutoPlanSettings;

            if (md == null) return;

            var tmr = GetTimer(md);
            var ctx = GetAdminContext(md.DatabaseKey);

            if (ctx == null) return;

            _dicDb.UpdatePositionExecutor(ctx); //TODO временно тут, потом может перенести в отдельный сервис

            var wrkUnit = new QueueTask(() =>
            {
                try
                {
                    var lst = _sysDb.GetSendListIdsForAutoPlan(ctx);
                    foreach (int id in lst)
                    {
                        try
                        {
                            var cmd = DocumentCommandFactory.GetDocumentCommand(EnumDocumentActions.LaunchDocumentSendListItem, ctx, null, id);
                            _cmdService.ExecuteCommand(cmd);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ctx, $"AutoPlanService cannot process SendList Id={id} ", ex);
                            try
                            {
                                var docId = _docDb.GetDocumentIdBySendListId(ctx, id);
                                if (docId > 0)
                                {
                                    var cmdStop = DocumentCommandFactory.GetDocumentCommand(EnumDocumentActions.StopPlan, ctx, null, docId);
                                    _cmdService.ExecuteCommand(cmdStop);
                                }
                            }
                            catch (Exception ex2)
                            {
                                Logger.Error(ctx, $"AutoPlanService cannot Stop plan for SendList Id={id} ", ex2);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ctx, "Could not process autoplan", ex);
                }
                tmr.Change(md.TimeToUpdate * 60000, Timeout.Infinite); //start new iteration of the timer
            }) { CurrentContext = ctx };

            _workerSrv.AddNewTask(ctx, wrkUnit);

        }

        public override void Dispose()
        {
            foreach (var tmr in _timers.Values)
            {
                tmr.Change(Timeout.Infinite, Timeout.Infinite);
                tmr.Dispose();
            }
            _timers.Clear();
            _timers.Clear();
        }
    }
}