using System;
using System.Collections.Generic;
using System.Threading;
using BL.CrossCutting.Interfaces;
using BL.Model.Constants;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.FullTextSerach;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class FullTextSearchService: BaseSystemWorkerService
    {
        private readonly Dictionary<FullTextSettings, Timer> _timers;

        public FullTextSearchService(ISettings setting, ILogger logger):base(setting, logger)
        {
        }

        public IEnumerable<FullTextSearchResult> Search(string text)
        {
            return null;
        }

        public void UpdateIndex(InternalDocument document, EnumSearchObjectType partType,EnumOperationType operationType)
        {

        }

        protected override void InitializeServers()
        {
            foreach (var keyValuePair in _serverContext)
            {
                try
                {
                    var ftsSetting = new FullTextSettings
                    {
                        TimeToUpdate = _settings.GetSetting<int>(keyValuePair.Value, SettingConstants.FULLTEXT_TIMEOUT_MIN),
                        DatabaseKey = keyValuePair.Key,
                        StorePath = _settings.GetSetting<string>(keyValuePair.Value, SettingConstants.FULLTEXT_INDEX_PATH),
                    };

                    // start timer only once. Do not do it regulary in case we don't know how much time sending of email take. So we can continue sending only when previous iteration was comlete
                    var tmr = new Timer(SinchronizeIndexes, ftsSetting, ftsSetting.TimeToUpdate * 60000, Timeout.Infinite);
                    _timers.Add(ftsSetting, tmr);
                }
                catch (Exception ex)
                {
                    _logger.Error(keyValuePair.Value, "Could not start MeilSender for server", ex);
                }
            }
        }

        private Timer GetTimer(FullTextSettings key)
        {
            Timer res = null;
            lock (_lockObjectTimer)
            {
                if (_timers.ContainsKey(key))
                    res = _timers[key];
            }
            return res;
        }

        private void SinchronizeIndexes(object state)
        {
            var md = state as FullTextSettings;

            if (md == null) return;

            var tmr = GetTimer(md);
            var ctx = GetAdminContext(md.DatabaseKey);

            if (ctx == null) return;
            try
            {
                //TODO!!
            }
            catch (Exception ex)
            {
                _logger.Error(ctx, "Could not sinchronize fulltextsearch indexes",ex);
            }
            tmr.Change(md.TimeToUpdate * 60000, Timeout.Infinite);//start new iteration of the timer
        }

        public override void Dispose()
        {
            foreach (var tmr in _timers.Values)
            {
                tmr.Change(Timeout.Infinite, Timeout.Infinite);
                tmr.Dispose();
            }
        }
    }
}