using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using System.Threading;
using BL.CrossCutting.DependencyInjection;
using BL.Database.SystemDb;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Constants;
using BL.Model.ClearTrashDocuments;

namespace BL.Logic.SystemServices.ClearTrashDocuments
{
    public class ClearTrashDocumentsService : BaseSystemWorkerService, IClearTrashDocumentsService
    {
        private readonly Dictionary<ClearTrashDocumentsSettings, Timer> _timers;
        private ISystemDbProcess _sysDb;
        private ICommandService _cmdService;

        public ClearTrashDocumentsService(ISettings settings, ILogger logger, ICommandService cmdService) : base(settings, logger)
        {
            _sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
            _cmdService = cmdService;
            _timers = new Dictionary<ClearTrashDocumentsSettings, Timer>();
        }

        protected override void InitializeServers()
        {
            foreach (var keyValuePair in _serverContext)
            {
                try
                {
                    var ftsSetting = new ClearTrashDocumentsSettings
                    {
                        TimeToUpdate = _settings.GetSetting<int>(keyValuePair.Value, SettingConstants.CLEARTRASHDOCUMENTS_TIOMEOUT_MIN),
                        TimeForClearTrashDocuments = _settings.GetSetting<int>(keyValuePair.Value, SettingConstants.CLEARTRASHDOCUMENTS_TIOMEOUT_MIN_FOR_CLEAR),
                        DatabaseKey = keyValuePair.Key,
                    };
                    var tmr = new Timer(OnSinchronize, ftsSetting, ftsSetting.TimeToUpdate * 60000, Timeout.Infinite);
                    _timers.Add(ftsSetting, tmr);
                }
                catch (Exception ex)
                {
                    _logger.Error(keyValuePair.Value, "Could not start clear trash documents for server", ex);
                }
            }
        }

        private Timer GetTimer(ClearTrashDocumentsSettings key)
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
            var md = state as ClearTrashDocumentsSettings;

            if (md == null) return;

            var tmr = GetTimer(md);
            var ctx = GetAdminContext(md.DatabaseKey);

            if (ctx == null) return;
            try
            {
                var ids = _sysDb.GetDocumentIdsForClearTrashDocuments(ctx,md.TimeForClearTrashDocuments);
                foreach (int id in ids)
                {
                    try
                    {
                        var cmd =
                            DocumentCommandFactory.GetDocumentCommand(
                                Model.Enums.EnumDocumentActions.DeleteDocument, ctx, null, id);
                        _cmdService.ExecuteCommand(cmd);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ctx, $"ClearTrashDocumentsService cannot process Document Id={id} ", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ctx, "Could not process clear trash documents", ex);
            }
            tmr.Change(md.TimeToUpdate * 60000, Timeout.Infinite);//start new iteration of the timer
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}