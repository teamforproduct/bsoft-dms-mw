﻿using System;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using System.Threading;
using BL.CrossCutting.DependencyInjection;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Database.SystemDb;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.ClearTrashDocuments;
using BL.Model.DocumentCore.Filters;
using Ninject;
using Ninject.Parameters;

namespace BL.Logic.SystemServices.ClearTrashDocuments
{
    public class ClearTrashDocumentsService : BaseSystemWorkerService, IClearTrashDocumentsService
    {
        private readonly Dictionary<ClearTrashDocumentsSettings, Timer> _timers;
        private readonly IDocumentService _documentServ;
        private readonly ISystemDbProcess _sysDb;
        private readonly ICommandService _cmdService;
        private readonly IDocumentFileDbProcess _docFileDb;
        private readonly IDocumentOperationsDbProcess _docOperDb;        
        private readonly IFileStore _fileStore;

        public ClearTrashDocumentsService(IDocumentService documentServ, IDocumentOperationsDbProcess docOperDb, ISettingValues settingValues, ILogger logger, ICommandService cmdService, ISystemDbProcess sysDb, IDocumentFileDbProcess docFileDb, IFileStore fileStore) : base(settingValues, logger)
        {
            _documentServ = documentServ;
            _sysDb = sysDb;
            _cmdService = cmdService;
            _docFileDb = docFileDb;
            _docOperDb = docOperDb;
            _fileStore = fileStore;
            _timers = new Dictionary<ClearTrashDocumentsSettings, Timer>();            
        }

        protected override void InitializeServers()
        {
            foreach (var keyValuePair in ServerContext)
            {
                try
                {
                    var ftsSetting = new ClearTrashDocumentsSettings
                    {
                        TimeToUpdate = SettingValues.GetClearTrashDocumentsTimeoutMinute(keyValuePair.Value),
                        TimeForClearTrashDocuments = SettingValues.GetClearTrashDocumentsTimeoutMinuteForClear(keyValuePair.Value),
                        DatabaseKey = keyValuePair.Key,
                    };
                    var tmr = new Timer(OnSinchronize, ftsSetting, ftsSetting.TimeToUpdate * 60000, Timeout.Infinite);
                    _timers.Add(ftsSetting, tmr);
                }
                catch (Exception ex)
                {
                    Logger.Error(keyValuePair.Value, "Could not start clear trash documents for server", ex);
                }
            }
        }

        private Timer GetTimer(ClearTrashDocumentsSettings key)
        {
            Timer res = null;
            lock (LockObjectTimer)
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
            ctx.DbContext = DmsResolver.Current.Kernel.Get<IDmsDatabaseContext>(new ConstructorArgument("dbModel", ctx.CurrentDB));
//            _docOperDb.MarkDocumentEventAsReadAuto(ctx);
            _docOperDb.ModifyDocumentAccessesStatistics(ctx);
            _documentServ.CheckIsInWorkForControls(ctx, new FilterDocumentAccess());

            // Clear trash documents.
            try
            {
                var ids = _sysDb.GetDocumentIdsForClearTrashDocuments(ctx,md.TimeForClearTrashDocuments);
                foreach (int id in ids)
                {
                    try
                    {
                        var cmd = DocumentCommandFactory.GetDocumentCommand(Model.Enums.EnumDocumentActions.DeleteDocument, ctx, null, id);
                        _cmdService.ExecuteCommand(cmd);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ctx, $"ClearTrashDocumentsService cannot process Document Id={id} ", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ctx, "Could not process clear trash documents", ex);
            }

            // CLEAR unused PDF copy of the files and their previews. 
            try
            {
                var pdfFilePeriod = SettingValues.GetClearOldPdfCopiesInDay(ctx);
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
                Logger.Error(ctx, "Could not process clear trash documents", ex);
            }
            ((DmsContext)ctx.DbContext).Dispose();
            ctx.DbContext = null;
            tmr.Change(md.TimeToUpdate * 60000, Timeout.Infinite);//start new iteration of the timer
        }

        public override void Dispose()
        {
           
        }
    }
}