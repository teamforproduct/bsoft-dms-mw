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
using BL.Model.Constants;
using BL.Model.Enums;

namespace BL.Logic.SystemServices.AutoPlan
{
    public class AutoPlanService: BaseSystemWorkerService, IAutoPlanService
    {
        private readonly Dictionary<AutoPlanSettings, Timer> _timers;
        private ISystemDbProcess _sysDb;
        private IDocumentsDbProcess _docDb;
        private ICommandService _cmdService;
        private Dictionary<string, QueueWorker.QueueWorker> _workers;
        protected object _lockObjectWorker;

        public AutoPlanService(ISettings settings, ILogger logger, ICommandService cmdService) : base(settings, logger)
        {
            _sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
            _docDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            _cmdService = cmdService;
            _timers = new Dictionary<AutoPlanSettings, Timer>();
            _workers = new Dictionary<string, QueueWorker.QueueWorker>();
            _lockObjectWorker = new object();
        }

        protected override void InitializeServers()
        {
            foreach (var keyValuePair in _serverContext)
            {
                try
                {
                    var ftsSetting = new AutoPlanSettings
                    {
                        TimeToUpdate = _settings.GetSetting<int>(keyValuePair.Value, SettingConstants.AUTOPLAN_TIOMEOUT_MIN),
                        DatabaseKey = keyValuePair.Key,
                    };
                    _workers.Add(keyValuePair.Key, new QueueWorker.QueueWorker());
                    var tmr = new Timer(OnSinchronize, ftsSetting, ftsSetting.TimeToUpdate * 60000, Timeout.Infinite);
                    _timers.Add(ftsSetting, tmr);
                }
                catch (Exception ex)
                {
                    _logger.Error(keyValuePair.Value, "Could not start AutoPlan for server", ex);
                }
            }
        }

        private QueueWorker.QueueWorker GetWorker(string key)
        {
            QueueWorker.QueueWorker res = null;
            lock (_lockObjectWorker)
            {
                if (_workers.ContainsKey(key))
                    res = _workers[key];
            }
            return res;
        }

        private Timer GetTimer(AutoPlanSettings key)
        {
            Timer res = null;
            lock (_lockObjectTimer)
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

            var wrkr = GetWorker(srvKey);
            if (wrkr == null) return false;

            var wrkUnit = new QueueTask(() =>
            {
                try
                {
                    var lst = _sysDb.GetSendListIdsForAutoPlan(ctx, sendListId, documentId);
                    foreach (int id in lst)
                    {
                        try
                        {
                            var cmd = DocumentCommandFactory.GetDocumentCommand(Model.Enums.EnumDocumentActions.LaunchDocumentSendListItem, ctx, null, id);
                            _cmdService.ExecuteCommand(cmd);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ctx, $"AutoPlanService cannot process SendList Id={id} ", ex);
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
                                _logger.Error(ctx, $"AutoPlanService cannot Stop plan for SendList Id={id} ", ex2);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ctx, "Could not process autoplan", ex);
                }
            });
            wrkr.AddToQueue(wrkUnit);

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

            var wrkr = GetWorker(md.DatabaseKey);
            if (wrkr == null) return;

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
                            _logger.Error(ctx, $"AutoPlanService cannot process SendList Id={id} ", ex);
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
                                _logger.Error(ctx, $"AutoPlanService cannot Stop plan for SendList Id={id} ", ex2);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ctx, "Could not process autoplan", ex);
                }
                tmr.Change(md.TimeToUpdate*60000, Timeout.Infinite); //start new iteration of the timer
            });
            
            wrkr.AddToQueue(wrkUnit);

        }

        public override void Dispose()
        {
            foreach (var tmr in _timers.Values)
            {
                tmr.Change(Timeout.Infinite, Timeout.Infinite);
                tmr.Dispose();
            }
            _timers.Clear();

            foreach (var wrk in _workers.Values)
            {
                wrk.StopWorker();
            }
            _timers.Clear();
        }
    }
}