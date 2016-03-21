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
            _timers = new Dictionary<FullTextSettings, Timer>();
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

        private List<FullTextIndexIem> PrepareObjectToIndexing(List<FullTextUpdateCashInfo> objList)
        {
            var res = new List<FullTextIndexIem>();
            //TODO check which data we store as description!
            foreach (var info in objList)
            {
                switch (info.PartType)
                {
                    case EnumSearchObjectType.Document:
                        res.Add(new FullTextIndexIem
                        {
                            DocumentId = info.Document.Id,
                            ItemType = info.PartType,
                            ObjectId = 0,
                            ObjectText = info.Document.Description,
                            OperationType = info.OperationType
                        });
                        break;
                    case EnumSearchObjectType.Event:
                        info.Document.Events.ToList().ForEach(x => res.Add(new FullTextIndexIem
                        {
                            DocumentId = x.DocumentId,
                            ItemType = info.PartType,
                            OperationType = info.OperationType,
                            ObjectId = x.Id,
                            ObjectText = x.Description
                        }));
                        break;
                    case EnumSearchObjectType.Subscription:
                        info.Document.Subscriptions.ToList().ForEach(x => res.Add(new FullTextIndexIem
                        {
                            DocumentId = x.DocumentId,
                            ItemType = info.PartType,
                            OperationType = info.OperationType,
                            ObjectId = x.Id,
                            ObjectText = x.Description
                        }));
                        break;
                    case EnumSearchObjectType.SendList:
                        info.Document.SendLists.ToList().ForEach(x => res.Add(new FullTextIndexIem
                        {
                            DocumentId = x.DocumentId,
                            ItemType = info.PartType,
                            OperationType = info.OperationType,
                            ObjectId = x.Id,
                            ObjectText = x.Description
                        }));
                        break;
                    case EnumSearchObjectType.Files:
                        info.Document.DocumentFiles.ToList().ForEach(x => res.Add(new FullTextIndexIem
                        {
                            DocumentId = x.DocumentId,
                            ItemType = info.PartType,
                            ObjectId = x.Id,
                            ObjectText = x.Name+"."+x.Extension,
                            OperationType = info.OperationType
                        }));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return res;
        }

        private void SinchronizeServer(FullTextSettings serverSetting, IContext ctx)
        {
            var objectToProcess = ExtractObjectsFromCash(serverSetting.DatabaseKey);
            if (!objectToProcess.Any()) return;

            var worker = _workers.FirstOrDefault(x => x.ServerKey == CommonSystemUtilities.GetServerKey(ctx));
            if (worker == null) return;

            var toUpdate = PrepareObjectToIndexing(objectToProcess);

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