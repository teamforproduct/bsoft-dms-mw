using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BL.CrossCutting.Interfaces;
using BL.Logic.Common;
using BL.Model.Constants;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.FullTextSerach;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class FullTextSearchService : BaseSystemWorkerService, IFullTextSearchService
    {
        private readonly Dictionary<FullTextSettings, Timer> _timers;
        List<FullTextUpdateCashInfo> _updateQueue;
        List<IFullTextIndexWorker> _workers;
        private object _lockQueue;


        public FullTextSearchService(ISettings setting, ILogger logger):base(setting, logger)
        {
            _updateQueue = new List<FullTextUpdateCashInfo>();
            _lockQueue = new object();
            _workers = new List<IFullTextIndexWorker>();
        }

        public IEnumerable<FullTextSearchResult> Search(IContext ctx, string text)
        {
            var worker = _workers.FirstOrDefault(x => x.ServerKey == CommonSystemUtilities.GetServerKey(ctx));
            return worker?.Search(text);
        }

        public IEnumerable<FullTextSearchResult> Search(IContext ctx, string text, EnumSearchObjectType objectType, int documentId)
        {
            var worker = _workers.FirstOrDefault(x => x.ServerKey == CommonSystemUtilities.GetServerKey(ctx));
            return worker?.Search(text, objectType, documentId);
        }

        public void UpdateIndex(IContext ctx, InternalDocument doc, EnumSearchObjectType objectType,EnumOperationType operType)
        {
            var si = new FullTextUpdateCashInfo
            {
                Document = doc,
                OperationType = operType,
                PartType = objectType,
                ServerKey = CommonSystemUtilities.GetServerKey(ctx)
            };
            AddNewInfo(si);
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
                    var worker = new FullTextIndexWorker(ftsSetting.DatabaseKey, ftsSetting.StorePath);
                    _workers.Add(worker);
                    // start timer only once. Do not do it regulary in case we don't know how much time sending of email take. So we can continue sending only when previous iteration was comlete
                    var tmr = new Timer(OnSinchronize, ftsSetting, ftsSetting.TimeToUpdate * 60000, Timeout.Infinite);
                    _timers.Add(ftsSetting, tmr);
                }
                catch (Exception ex)
                {
                    _logger.Error(keyValuePair.Value, "Could not start MeilSender for server", ex);
                }
            }
        }

        private void AddNewInfo(FullTextUpdateCashInfo info)
        {
            lock (_lockQueue)
            {
                _updateQueue.Add(info);
            }
        }

        private List<FullTextUpdateCashInfo> ExtractObjectsFromCash(string serverKey)
        {
            List<FullTextUpdateCashInfo> res;
            lock (_lockQueue)
            {
                res = _updateQueue.Where(x => x.ServerKey == serverKey).ToList();
                _updateQueue.RemoveAll(x => x.ServerKey == serverKey);
            }
            return res;
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

        private void SinchronizeServer(FullTextSettings serverSetting, IContext ctx)
        {
            var toUpdate = ExtractObjectsFromCash(serverSetting.DatabaseKey);
            var worker = _workers.FirstOrDefault(x => x.ServerKey == CommonSystemUtilities.GetServerKey(ctx));
            if (worker == null) return;

            foreach (var itm in toUpdate.Where(x => x.OperationType == EnumOperationType.Delete))
            {
                worker.DeleteItem();
            }

            foreach (var itm in toUpdate.Where(x => x.OperationType == EnumOperationType.Update))
            {
                worker.UpdateItem();
            }

            foreach (var itm in toUpdate.Where(x => x.OperationType == EnumOperationType.AddNew))
            {
                worker.AddNewItem();
            }
        }

        private void OnSinchronize(object state)
        {
            var md = state as FullTextSettings;

            if (md == null) return;

            var tmr = GetTimer(md);
            var ctx = GetAdminContext(md.DatabaseKey);

            if (ctx == null) return;
            try
            {
                SinchronizeServer(md, ctx);
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