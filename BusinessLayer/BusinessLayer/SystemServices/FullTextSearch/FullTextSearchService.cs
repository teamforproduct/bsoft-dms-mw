using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.Constants;
using BL.Model.FullTextSerach;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class FullTextSearchService : BaseSystemWorkerService, IFullTextSearchService
    {
        private readonly Dictionary<FullTextSettings, Timer> _timers;
        List<IFullTextIndexWorker> _workers;
        ISystemDbProcess _systemDb;

        public FullTextSearchService(ISettings setting, ILogger logger, ISystemDbProcess systemDb) :base(setting, logger)
        {
            _timers = new Dictionary<FullTextSettings, Timer>();
            _workers = new List<IFullTextIndexWorker>();
            _systemDb = systemDb;
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

        protected override void InitializeServers()
        {
            try
            {
                Dispose();
            }
            catch
            {
                // ignored
            }

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

        private void SinchronizeServer(IContext ctx)
        {
            var worker = _workers.FirstOrDefault(x => x.ServerKey == CommonSystemUtilities.GetServerKey(ctx));
            if (worker == null) return;

            var toUpdate = _systemDb.FullTextIndexPrepare(ctx) as List<FullTextIndexIem>;
            if (toUpdate == null || !toUpdate.Any()) return;

            worker.StartUpdate();

            var newItem = toUpdate.Where(x => x.OperationType == EnumOperationType.AddNew).ToList();
            foreach (var itm in newItem)
            {
                var toDelete = toUpdate.FirstOrDefault(x => x.DocumentId == itm.DocumentId && x.ItemType == itm.ItemType && x.ObjectId == itm.ObjectId && x.OperationType == EnumOperationType.Delete);
                if (toDelete != null)
                {
                    toUpdate.Remove(toDelete);
                }
                else
                {
                    worker.AddNewItem(itm);
                }
            }

            var deletedItem = toUpdate.Where(x => x.OperationType == EnumOperationType.Delete).ToList();
            foreach (var itm in deletedItem)
            {
                worker.DeleteItem(itm);
                toUpdate.RemoveAll(x => x.DocumentId == itm.DocumentId && x.ItemType == itm.ItemType && x.ObjectId == itm.ObjectId && x.OperationType == EnumOperationType.Update);
            }

            foreach (var itm in toUpdate.Where(x => x.OperationType == EnumOperationType.Update))
            {
                worker.UpdateItem(itm);
            }
            worker.CommitChanges();
            _systemDb.FullTextIndexDeleteProcessed(ctx, toUpdate.Select(x=>x.Id));
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
                SinchronizeServer(ctx);
            }
            catch (Exception ex)
            {
                _logger.Error(ctx, "Could not sinchronize fulltextsearch indexes", ex);
            }
            tmr.Change(md.TimeToUpdate*60000, Timeout.Infinite); //start new iteration of the timer
        }

        public override void Dispose()
        {
            foreach (var tmr in _timers.Values)
            {
                try
                {
                    tmr.Change(Timeout.Infinite, Timeout.Infinite);
                    tmr.Dispose();
                }
                catch
                {
                    // ignored
                }
            }
            _timers.Clear();

            _workers.ForEach(x =>
            {
                try
                {
                    x.Dispose();
                }
                catch
                {
                    // ignored
                }
            });
            _workers.Clear();
        }
    }
}