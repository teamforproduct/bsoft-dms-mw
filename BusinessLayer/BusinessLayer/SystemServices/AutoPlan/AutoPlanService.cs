using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AutoPlan;
using System.Threading;
using BL.Database.SystemDb;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Constants;

namespace BL.Logic.SystemServices.AutoPlan
{
    public class AutoPlanService: BaseSystemWorkerService
    {
        private readonly Dictionary<AutoPlanSettings, Timer> _timers;
        private ISystemDbProcess _sysDb;
        private ICommandService _cmdService;

        public AutoPlanService(ISettings settings, ILogger logger) : base(settings, logger)
        {
            _sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
            _cmdService = DmsResolver.Current.Get<ICommandService>();
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
                    var tmr = new Timer(OnSinchronize, ftsSetting, ftsSetting.TimeToUpdate * 60000, Timeout.Infinite);
                    _timers.Add(ftsSetting, tmr);
                }
                catch (Exception ex)
                {
                    _logger.Error(keyValuePair.Value, "Could not start AutoPlan for server", ex);
                }
            }
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

        private void OnSinchronize(object state)
        {
            var md = state as AutoPlanSettings;

            if (md == null) return;

            var tmr = GetTimer(md);
            var ctx = GetAdminContext(md.DatabaseKey);

            if (ctx == null) return;
            try
            {
                var lst = _sysDb.GetSendListIdsForAutoPlan(ctx);
                foreach (int id in lst)
                {
                    var cmd = DocumentCommandFactory.GetDocumentCommand(Model.Enums.EnumDocumentActions.LaunchDocumentSendListItem, ctx, null, id);
                    _cmdService.ExecuteCommand(cmd);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ctx, "Could not sinchronize fulltextsearch indexes", ex);
            }
            tmr.Change(md.TimeToUpdate * 60000, Timeout.Infinite);//start new iteration of the timer
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}