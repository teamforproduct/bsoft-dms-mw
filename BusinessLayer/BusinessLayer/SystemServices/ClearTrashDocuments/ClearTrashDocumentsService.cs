using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using System.Threading;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Database.SystemDb;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.ClearTrashDocuments;

namespace BL.Logic.SystemServices.ClearTrashDocuments
{
    public class ClearTrashDocumentsService : BaseSystemWorkerService, IClearTrashDocumentsService
    {
        private readonly Dictionary<ClearTrashDocumentsSettings, Timer> _timers;
        private ISystemDbProcess _sysDb;
        private ICommandService _cmdService;
        private readonly IDocumentFileDbProcess _docFileDb;
        private readonly IFileStore _fileStore;

        public ClearTrashDocumentsService(ISettings settings, ILogger logger, ICommandService cmdService, ISystemDbProcess sysDb, IDocumentFileDbProcess docFileDb, IFileStore fileStore) : base(settings, logger)
        {
            _sysDb = sysDb;
            _cmdService = cmdService;
            _docFileDb = docFileDb;
            _fileStore = fileStore;
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
                        TimeToUpdate = _settings.GetClearTrashDocumentsTimeoutMinute(keyValuePair.Value),
                        TimeForClearTrashDocuments = _settings.GetClearTrashDocumentsTimeoutMinuteForClear(keyValuePair.Value),
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
            
            // Clear trash documents.
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

            // CLEAR unused PDF copy of the files and their previews. 
            try
            {
                var pdfFilePeriod = _settings.GetClearOldPdfCopiesInDay(ctx);
                var fileTodelete = _docFileDb.GetOldPdfForAttachedFiles(ctx, pdfFilePeriod);
                foreach (var file in fileTodelete)
                {
                    _fileStore.DeletePdfCopy(ctx,file);
                    file.PdfCreated = false;
                    _docFileDb.UpdateFilePdfView(ctx, file);
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