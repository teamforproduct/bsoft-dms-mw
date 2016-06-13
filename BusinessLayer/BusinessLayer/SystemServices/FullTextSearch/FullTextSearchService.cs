using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BL.CrossCutting.Interfaces;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.Constants;
using BL.Model.Enums;
using BL.Model.FullTextSearch;

namespace BL.Logic.SystemServices.FullTextSearch
{
    public class FullTextSearchService : BaseSystemWorkerService, IFullTextSearchService
    {
        private const int MAX_ROW_PROCESS = 1000;
        private readonly Dictionary<FullTextSettings, Timer> _timers;
        List<IFullTextIndexWorker> _workers;
        ISystemDbProcess _systemDb;

        public FullTextSearchService(ISettings setting, ILogger logger, ISystemDbProcess systemDb) :base(setting, logger)
        {
            _timers = new Dictionary<FullTextSettings, Timer>();
            _workers = new List<IFullTextIndexWorker>();
            _systemDb = systemDb;
        }

        public void ReindexDatabase(IContext ctx)
        {
            var worker = _workers.FirstOrDefault(x => x.ServerKey == CommonSystemUtilities.GetServerKey(ctx));
            if (worker == null) return;
            
            worker.StartUpdate();
            try
            {
                int offset =0;
                var objToProcess = new EnumObjects[]
                {
                    EnumObjects.Documents, EnumObjects.DocumentEvents, EnumObjects.DocumentSendLists,
                    EnumObjects.DocumentSubscriptions, EnumObjects.DocumentFiles
                };
                //delete all current document before reindexing
                worker.DeleteAllDocuments();

                //going through the documents objects and add it. Select max 1000 row for one processing
                foreach (var dataType in objToProcess)
                {
                    do
                    {
                        var data = _systemDb.FullTextIndexDocumentsReindexDbPrepare(ctx, dataType, MAX_ROW_PROCESS,offset);
                        foreach (var itm in data)
                        {
                            worker.AddNewItem(itm);
                        }

                        if (data.Count() == MAX_ROW_PROCESS)
                        {
                            offset += MAX_ROW_PROCESS;
                        }
                        else
                        {
                            offset = 0;
                        }

                    } while (offset != 0);

                }

                // add to index dictionaries and templates 
                var additionaData = _systemDb.FullTextIndexNonDocumentsReindexDbPrepare(ctx);
                foreach (var itm in additionaData)
                {
                    worker.AddNewItem(itm);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ctx, "Error duing the reindexing database for client.", ex);
            }
            finally
            {
                worker.CommitChanges();
            }
            
        }

        private IFullTextIndexWorker GetWorker(IContext ctx)
        {
            return _workers.FirstOrDefault(x => x.ServerKey == CommonSystemUtilities.GetServerKey(ctx));
        }

        public IEnumerable<FullTextSearchResult> SearchDocument(IContext ctx, string text)
        {
            return GetWorker(ctx)?.SearchDocument(text);
        }

        public IEnumerable<FullTextSearchResult> SearchDictionary(IContext ctx, string text)
        {
            return GetWorker(ctx)?.SearchDictionary(text);
        }

        public IEnumerable<FullTextSearchResult> SearchInDocument(IContext ctx, string text, int documentId)
        {
            return GetWorker(ctx)?.SearchInDocument(text,  documentId);
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

            var processedIds = new List<int>();

            worker.StartUpdate();
            try
            {
                var toDelete = _systemDb.FullTextIndexToDeletePrepare(ctx);
                if (toDelete.Any())
                {
                    foreach (var itm in toDelete)
                    {
                        try
                        {
                            worker.DeleteItem(itm);
                            processedIds.Add(itm.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ctx, $"FullTextService cannot process docId={itm.DocumentId} ", ex);
                        }
                    }
                    _systemDb.FullTextIndexDeleteProcessed(ctx, processedIds, true);
                    processedIds.Clear();
                }

                var toUpdateNonDocuments =
                    _systemDb.FullTextIndexNonDocumentsReindexDbPrepare(ctx) as List<FullTextIndexItem>;
                if (toUpdateNonDocuments.Any())
                {
                    foreach (var itm in toDelete)
                    {
                        try
                        {
                            switch (itm.OperationType)
                            {
                                case EnumOperationType.AddNew:
                                    worker.AddNewItem(itm);
                                    break;
                                case EnumOperationType.Update:
                                    worker.UpdateItem(itm);
                                    break;
                            }
                            processedIds.Add(itm.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ctx, $"FullTextService cannot process docId={itm.DocumentId} ", ex);
                        }
                    }
                    _systemDb.FullTextIndexDeleteProcessed(ctx, processedIds);
                    processedIds.Clear();
                }

                var objToProcess = new EnumObjects[]
                {
                    EnumObjects.Documents, EnumObjects.DocumentEvents, EnumObjects.DocumentSendLists,
                    EnumObjects.DocumentSubscriptions, EnumObjects.DocumentFiles
                };

                var maxId = _systemDb.GetCurrentMaxCasheId(ctx);

                foreach (var objType in objToProcess)
                {
                    var toUpdate = _systemDb.FullTextIndexDocumentsPrepare(ctx, objType, MAX_ROW_PROCESS, maxId);
                    while (toUpdate.Any())
                    {
                        foreach (var itm in toUpdate)
                        {
                            try
                            {
                                switch (itm.OperationType)
                                {
                                    case EnumOperationType.AddNew:
                                        worker.AddNewItem(itm);
                                        break;
                                    case EnumOperationType.Update:
                                        worker.UpdateItem(itm);
                                        break;
                                }
                                processedIds.Add(itm.Id);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ctx, $"FullTextService cannot process docId={itm.DocumentId} ", ex);
                            }
                        }
                        _systemDb.FullTextIndexDeleteProcessed(ctx, processedIds);
                        processedIds.Clear();
                        toUpdate = _systemDb.FullTextIndexDocumentsPrepare(ctx, objType, MAX_ROW_PROCESS, maxId);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ctx, "FullTextService raise an exception when process cash. ", ex);
            }
            finally
            {
                worker.CommitChanges();
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