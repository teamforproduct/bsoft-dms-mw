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
using BL.CrossCutting.Context;

namespace BL.Logic.SystemServices.AutoPlan
{
    public class AutoPlanService : BaseSystemWorkerService, IAutoPlanService
    {
        private readonly Dictionary<AutoPlanSettings, Timer> _timers;
        private readonly ISystemDbProcess _sysDb;
        private readonly IDocumentsDbProcess _docDb;
        private readonly DictionariesDbProcess _dicDb;
        private readonly IQueueWorkerService _workerSrv;
        private readonly ICommandService _cmdService;

        public AutoPlanService(ISettingValues settingValues, ILogger logger, ICommandService cmdService, IQueueWorkerService workerService) : base(settingValues, logger)
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
                        TimeToUpdate = SettingValues.GetAutoplanTimeoutMinute(keyValuePair.Value),
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
            var admCtx = new AdminContext(ctx);
            var wrkUnit = new QueueTask(() =>
            {
                bool isRepeat = true;

                try
                {
                    while (isRepeat)
                    {
                        var lst = _sysDb.GetSendListIdsForAutoPlan(admCtx, sendListId, documentId);
                        isRepeat = lst.Any() && documentId != null && sendListId == null;

                        foreach (int id in lst)
                        {
                            try
                            {
                                var cmd = DocumentCommandFactory.GetDocumentCommand(EnumDocumentActions.LaunchDocumentSendListItem, admCtx, null, id);
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
                                        var cmdStop = DocumentCommandFactory.GetDocumentCommand(EnumDocumentActions.StopPlan, admCtx, null, docId);
                                        _cmdService.ExecuteCommand(cmdStop);
                                    }
                                }
                                catch (Exception ex2)
                                {
                                    Logger.Error(admCtx, $"AutoPlanService cannot Stop plan for SendList Id={id} ", ex2);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ctx, "Could not process autoplan", ex);
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

        private void OnSinchronize(object state)
        {
            var md = state as AutoPlanSettings;

            if (md == null) return;

            var tmr = GetTimer(md);
            var ctx = GetAdminContext(md.DatabaseKey);

            if (ctx == null) return;
            var admCtx = new AdminContext(ctx);
            _dicDb.UpdateExecutorsInPositions(admCtx); //TODO временно тут, потом может перенести в отдельный сервис

            var wrkUnit = new QueueTask(() =>
            {
                try
                {
                    var lst = _sysDb.GetSendListIdsForAutoPlan(admCtx);
                    foreach (int id in lst)
                    {
                        try
                        {
                            var cmd = DocumentCommandFactory.GetDocumentCommand(EnumDocumentActions.LaunchDocumentSendListItem, admCtx, null, id);
                            _cmdService.ExecuteCommand(cmd);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(admCtx, $"AutoPlanService cannot process SendList Id={id} ", ex);
                            try
                            {
                                var docId = _docDb.GetDocumentIdBySendListId(admCtx, id);
                                if (docId > 0)
                                {
                                    var cmdStop = DocumentCommandFactory.GetDocumentCommand(EnumDocumentActions.StopPlan, admCtx, null, docId);
                                    _cmdService.ExecuteCommand(cmdStop);
                                }
                            }
                            catch (Exception ex2)
                            {
                                Logger.Error(admCtx, $"AutoPlanService cannot Stop plan for SendList Id={id} ", ex2);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(admCtx, "Could not process autoplan", ex);
                }
                tmr.Change(md.TimeToUpdate * 60000, Timeout.Infinite); //start new iteration of the timer
            });

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