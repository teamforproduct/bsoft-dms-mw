using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Logic.Settings;
using BL.Model.SystemCore;
using BL.Logic.AdminCore.Interfaces;
using BL.CrossCutting.DependencyInjection;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class FullTextSearchService : BaseSystemWorkerService, IFullTextSearchService
    {
        private const int _MAX_ROW_PROCESS = 100000;
        private const int _MAX_ENTITY_FOR_THREAD = 500000;

        private readonly Dictionary<FullTextSettings, Timer> _timers;
        private readonly List<Timer> _stopTimersList = new List<Timer>();
        readonly List<IFullTextIndexWorker> _workers;
        readonly IFullTextDbProcess _systemDb;

        public FullTextSearchService(ISettings setting, ILogger logger, IFullTextDbProcess systemDb) : base(setting, logger)
        {
            _timers = new Dictionary<FullTextSettings, Timer>();
            _workers = new List<IFullTextIndexWorker>();
            _systemDb = systemDb;
        }

        private void ReindexObject(IContext ctx, IFullTextIndexWorker worker, EnumObjects dataType)
        {
            var data = _systemDb.GetItemsToReindex(ctx, dataType, null, null);
            foreach (var itm in data)
            {
                worker.AddNewItem(itm);
            }
        }

        private void ReindexBigObject(IContext ctx, IFullTextIndexWorker worker, EnumObjects dataType, int fromNumber, int toNumber)
        {
            int offset = fromNumber;
            do
            {
                var selectCount = (offset + _MAX_ROW_PROCESS < toNumber) ? _MAX_ROW_PROCESS : toNumber - offset;

                var data = _systemDb.GetItemsToReindex(ctx, dataType, selectCount, offset);
                foreach (var itm in data)
                {
                    worker.AddNewItem(itm);
                }
                if (data.Count() == _MAX_ROW_PROCESS)
                {
                    offset += _MAX_ROW_PROCESS;
                }
                else
                {
                    offset = 0;
                }
            } while (offset != 0 && offset < toNumber);
        }

        public void ReindexDatabase(IContext ctx)
        {
            var dbKey = CommonSystemUtilities.GetServerKey(ctx);
            var worker = _workers.FirstOrDefault(x => x.ServerKey == dbKey);
            if (worker == null) return;

            var md = _timers.Keys.First(x => x.DatabaseKey == dbKey);

            if (md == null) return;

            var tmr = GetTimer(md);
            tmr.Change(Timeout.Infinite, Timeout.Infinite); // stop the timer. But that should be checked. Probably timer event can be rased ones more
            _stopTimersList.Add(tmr); // to avoid additional raise of timer event
            var systemSetting = SettingsFactory.GetDefaultSetting(EnumSystemSettings.FULLTEXTSEARCH_WAS_INITIALIZED);
            systemSetting.Value = false.ToString();
            Settings.SaveSetting(ctx, systemSetting);
            //initiate the update of FT
            worker.StartUpdate();
            try
            {
                var currCashId = _systemDb.GetCurrentMaxCasheId(ctx);
                var objToProcess = _systemDb.ObjectToReindex();
                //delete all current document before reindexing
                worker.DeleteAllDocuments(ctx.CurrentClientId);
                var tskList = new List<Action>();
                foreach (var obj in objToProcess)//.Where(x=>x == EnumObjects.DocumentEvents).ToList())
                {
                    var itmsCount = _systemDb.GetItemsToUpdateCount(ctx, obj, false);
                    if (!itmsCount.Any() || itmsCount.All(x=>x == 0)) continue;

                    if (itmsCount.Count > 1 || itmsCount[0] <= _MAX_ENTITY_FOR_THREAD)
                    {
                        tskList.Add(() => { ReindexObject(ctx, worker, obj); });
                    }
                    else
                    {
                        int indFrom = 0;
                        do
                        {
                            int indTo = Math.Min(indFrom + _MAX_ENTITY_FOR_THREAD - 1, itmsCount[0]);
                            var @fromIndex = indFrom;
                            tskList.Add(() => { ReindexBigObject(ctx, worker, obj, @fromIndex, indTo); });
                            indFrom = indTo + 1;
                        } while (indFrom< itmsCount[0]);

                    }

                }

                Parallel.Invoke(new ParallelOptions() {MaxDegreeOfParallelism = 6},tskList.ToArray());

                //delete cash in case we just processed all that documents
                _systemDb.FullTextIndexDeleteCash(ctx, currCashId);

                //set indicator that full text for the client available
                systemSetting.Value = true.ToString();
                Settings.SaveSetting(ctx, systemSetting);
                md.IsFullTextInitialized = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ctx, ex, "Error duing the reindexing database for client.");
            }
            finally
            {
                worker.CommitChanges();
            }

            tmr.Change(md.TimeToUpdate * 60000, Timeout.Infinite); //start new iteration of the timer
        }

        private IFullTextIndexWorker GetWorker(IContext ctx)
        {
            return _workers.FirstOrDefault(x => x.ServerKey == CommonSystemUtilities.GetServerKey(ctx));
        }

        private void ReindexBeforeSearch()
        {
        }

        public List<int> SearchItemParentId(IContext ctx, string text, FullTextSearchFilter filter)
        {
            ReindexBeforeSearch();
            var ftRes = SearchItems(ctx, text, filter);
            if (ftRes == null) return new List<int>();

            var resWithRanges =
                ftRes.GroupBy(x => x.ParentId)
                    .Select(x => new { DocId = x.Key, Rate = x.Max(s => s.Score) })
                    .OrderByDescending(x => x.Rate);
            return resWithRanges.Select(x => x.DocId).ToList();
        }

        public IEnumerable<FullTextSearchResult> SearchItems(IContext ctx, string text, FullTextSearchFilter filter)
        {
            ReindexBeforeSearch();
            var admService = DmsResolver.Current.Get<IAdminService>();
            var perm = admService.GetUserPermissions(ctx, admService.GetFilterPermissionsAccessByContext(ctx, false, null, null, filter?.ModuleId)).Select(x=> Features.GetId(x.Feature)).ToList();
            var res = GetWorker(ctx)?.SearchItems(text, ctx.CurrentClientId, filter).Where(x=> perm.Contains(x.FeatureId));
            return res;
        }

        public IEnumerable<FullTextSearchResult> SearchItemsByDetail(IContext ctx, string text, FullTextSearchFilter filter)
        {
            ReindexBeforeSearch();
            var words = text.Split(' ').OrderBy(x=>x.Length);
            var res = new List<FullTextSearchResult>();
            var worker = GetWorker(ctx);
            if (worker == null) return res;
            var tempRes = new List<List<FullTextSearchResult>>();
            foreach (var word in words)
            {
                var r = worker.SearchItems(word, ctx.CurrentClientId, filter).ToList();
                if (!r.Any()) return res;
                tempRes.Add(r);
            }
            
            tempRes.RemoveAll(x => !x.Any());

            if (!tempRes.Any()) return res;
            res = tempRes.First();
            tempRes.Remove(res);
            foreach (var sRes in tempRes)
            {
                res = res.Join(sRes, a => new {a.ParentId, a.ParentObjectType},
                    b => new {b.ParentId, b.ParentObjectType}, (a, b) => a).ToList();
            }

            return res;
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

            foreach (var keyValuePair in ServerContext)
            {
                try
                {
                    var ftsSetting = new FullTextSettings
                    {
                        TimeToUpdate = Settings.GetFulltextRefreshTimeout(keyValuePair.Value),
                        DatabaseKey = keyValuePair.Key,
                        StorePath = Settings.GetFulltextDatastorePath(keyValuePair.Value),
                        IsFullTextInitialized = Settings.GetFulltextWasInitialized(keyValuePair.Value)
                    };
                    var worker = new FullTextIndexWorker(ftsSetting.DatabaseKey, ftsSetting.StorePath);
                    _workers.Add(worker);
                    // start timer only once. Do not do it regulary in case we don't know how much time sending of email take. So we can continue sending only when previous iteration was comlete
                    var tmr = new Timer(OnSinchronize, ftsSetting, ftsSetting.TimeToUpdate * 60000, Timeout.Infinite);
                    _timers.Add(ftsSetting, tmr);
                }
                catch (Exception ex)
                {
                    Logger.Error(keyValuePair.Value, ex, "Could not initialize Full text service for server");
                }
            }
        }

        private Timer GetTimer(FullTextSettings key)
        {
            Timer res = null;
            lock (LockObjectTimer)
            {
                if (_timers.ContainsKey(key))
                    res = _timers[key];
            }
            return res;
        }

        private void SinchronizeServer(IContext ctx)
        {
            //Logger.Information(ctx, "Start FullText sinchronization " + DateTime.Now);
            var worker = _workers.FirstOrDefault(x => x.ServerKey == CommonSystemUtilities.GetServerKey(ctx));
            if (worker == null) return;

            var processedIds = new List<int>();

            worker.StartUpdate();
            try
            {
                var currCashId = _systemDb.GetCurrentMaxCasheId(ctx);
                var cashList = _systemDb.FullTextIndexToUpdate(ctx, currCashId);

                foreach (var item in cashList)
                {
                    try
                    {
                        switch (item.OperationType)
                        {
                            case EnumOperationType.AddNew:
                                var toAdd = _systemDb.FullTextIndexPrepareNew(ctx, item.ObjectType, false, false, null, currCashId);
                                foreach (var itmAdd in toAdd)
                                {
                                    worker.AddNewItem(itmAdd);
                                }

                                break;
                            case EnumOperationType.Update:
                                var toUpd = _systemDb.FullTextIndexPrepareNew(ctx, item.ObjectType, false, false, null, currCashId);
                                foreach (var itmUpd in toUpd)
                                {
                                    worker.UpdateItem(itmUpd);
                                }
                                break;
                            case EnumOperationType.Delete:
                                var toDelete = _systemDb.FullTextIndexPrepareNew(ctx, item.ObjectType, false, false,null, currCashId);
                                foreach (var itmDel in toDelete)
                                {
                                    worker.DeleteItem(itmDel);
                                }
                                break;
                            case EnumOperationType.UpdateFull:
                                var toUpdFull = _systemDb.FullTextIndexPrepareNew(ctx, item.ObjectType, true, false, null, currCashId);
                                foreach (var itmUpd in toUpdFull)
                                {
                                    worker.UpdateItem(itmUpd);
                                }
                                break;
                            case EnumOperationType.DeleteFull:
                                var toDeleteFull = _systemDb.FullTextIndexPrepareNew(ctx, item.ObjectType, true, false, null, currCashId);
                                foreach (var itmDel in toDeleteFull)
                                {
                                    worker.DeleteItem(itmDel);
                                }
                                break;
                        }
                        processedIds.Add(item.Id);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ctx, ex, $"FullTextService cannot process item. CashID={item.Id} Type={item.ParentObjectId} ObjectId={item.ObjectId}");
                    }
                }

                if (processedIds.Any())
                {
                    _systemDb.FullTextIndexDeleteProcessed(ctx, processedIds.Distinct());
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ctx, "FullTextService raise an exception when process cash. ", ex);
            }
            finally
            {
                //Logger.Information(ctx, "Finisch FullText sinchronization " + DateTime.Now);
                worker.CommitChanges();
            }
        }

        private void OnSinchronize(object state)
        {
            var md = state as FullTextSettings;

            if (md == null) return;

            if (!md.IsFullTextInitialized) return; //Full text was not initialized for that client and that server

            var tmr = GetTimer(md);

            if (_stopTimersList.Contains(tmr)) return;

            var ctx = GetAdminContext(md.DatabaseKey);

            if (ctx == null) return;
            try
            {
                SinchronizeServer(ctx);
            }
            catch (Exception ex)
            {
                Logger.Error(ctx, ex, "Could not sinchronize fulltextsearch indexes");
                Logger.Error(ctx, ex, "Could not sinchronize fulltextsearch indexes");
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