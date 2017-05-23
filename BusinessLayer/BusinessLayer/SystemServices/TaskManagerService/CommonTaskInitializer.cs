using System;
using System.Collections.Generic;
using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Dictionaries;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemServices.AutoPlan;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Logic.SystemServices.MailWorker;
using BL.Logic.SystemServices.QueueWorker;
using BL.Model.Context;
using BL.Model.DocumentCore.Filters;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using Ninject;
using Ninject.Parameters;

namespace BL.Logic.SystemServices.TaskManagerService
{
    public class CommonTaskInitializer : ICommonTaskInitializer
    {
        private readonly ITaskManager _taskManager;
        private readonly ISettingValues _settingValues;

        public CommonTaskInitializer(ITaskManager taskManager, ISettingValues settingValues)
        {
            _taskManager = taskManager;
            _settingValues = settingValues;
        }

        public void InitializeAutoPlanTask(List<DatabaseModelForAdminContext> dbs)
        {
            foreach (var db in dbs)
            {
                var ctx = new AdminContext(db);
                var tmrInterval = _settingValues.GetAutoplanTimeoutMinute(ctx);
                ((DmsContext) ctx.DbContext).Dispose();
                ctx.DbContext = null;
                _taskManager.AddTask(tmrInterval, (context, param) =>
                {
                    if (context == null) return;
                    var admCtx = new AdminContext(context);

                    var autoPlan = DmsResolver.Current.Get<IAutoPlanService>();
                    var logger = DmsResolver.Current.Get<ILogger>();
                    var dicDb = DmsResolver.Current.Get<DictionariesDbProcess>();
                    var workerSrv = DmsResolver.Current.Get<IQueueWorkerService>();

                    dicDb.UpdateExecutorsInPositions(admCtx);//TODO временно тут, потом может перенести в отдельный сервис

                    var wrkUnit = new QueueTask(() =>
                    {
                        try
                        {
                            autoPlan.AutoPlanTask(admCtx);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(admCtx, "Could not process autoplan", ex);
                        }
                    });

                    workerSrv.AddNewTask(admCtx, wrkUnit);
                }, db, ctx);
            }
        }

        public void InitializeMailWorkerTask(List<DatabaseModelForAdminContext> dbs)
        {
            foreach (var db in dbs)
            {
                var ctx = new AdminContext(db);
                var tmrInterval = _settingValues.GetMailDocumSenderTimeoutMin();
                var mailSrv = DmsResolver.Current.Get<IMailSenderWorkerService>();
                var mailParam = mailSrv.GetMailServerParameters(MailServers.Noreply,CommonSystemUtilities.GetServerKey(ctx));
                ((DmsContext)ctx.DbContext).Dispose();
                ctx.DbContext = null;
                _taskManager.AddTask(tmrInterval, (context, param) =>
                {
                    context.DbContext = DmsResolver.Current.Kernel.Get<IDmsDatabaseContext>(new ConstructorArgument("dbModel", context.CurrentDB));
                    var mSrv = DmsResolver.Current.Get<IMailSenderWorkerService>();
                    mSrv.CheckForNewMessages(context, (InternalSendMailServerParameters)param);
                    ((DmsContext)context.DbContext).Dispose();
                    context.DbContext = null;
                }, db, ctx, mailParam);
            }
        }

        public void InitializeClearTrashTask(List<DatabaseModelForAdminContext> dbs)
        {
            foreach (var db in dbs)
            {
                var ctx = new AdminContext(db);
                var tmrInterval = _settingValues.GetClearTrashDocumentsTimeoutMinute(ctx);
                ((DmsContext)ctx.DbContext).Dispose();
                ctx.DbContext = null;

                _taskManager.AddTask(tmrInterval, (context, param) =>
                {
                    if (context == null) return;

                    var documentServ = DmsResolver.Current.Get<IDocumentService>();
                    var sysDb = DmsResolver.Current.Get<ISystemDbProcess>();
                    var cmdService = DmsResolver.Current.Get<ICommandService>();
                    var docFileDb = DmsResolver.Current.Get<IDocumentFileDbProcess>();
                    var docOperDb = DmsResolver.Current.Get<IDocumentOperationsDbProcess>();
                    var fileStore = DmsResolver.Current.Get<IFileStore>();
                    var logger = DmsResolver.Current.Get<ILogger>();

                    context.DbContext = DmsResolver.Current.Kernel.Get<IDmsDatabaseContext>(new ConstructorArgument("dbModel",context.CurrentDB));
                    var timeForClearTrashDocuments = _settingValues.GetClearTrashDocumentsTimeoutMinuteForClear(context);
                    try
                    {
                        docOperDb.ModifyDocumentAccessesStatistics(context);
                        documentServ.CheckIsInWorkForControls(context, new FilterDocumentAccess());
                    }
                    catch (Exception ex)
                    {
                        logger.Error(context, "Could not process additional tasks", ex);
                    }

                    // Clear trash documents.
                    try
                    {
                        var ids = sysDb.GetDocumentIdsForClearTrashDocuments(context, timeForClearTrashDocuments);
                        foreach (int id in ids)
                        {
                            try
                            {
                                var cmd =DocumentCommandFactory.GetDocumentCommand(Model.Enums.EnumDocumentActions.DeleteDocument, context, null, id);
                                cmdService.ExecuteCommand(cmd);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(context, $"ClearTrashDocumentsService cannot process Document Id={id} ", ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(context, "Could not process clear trash documents", ex);
                    }

                    // CLEAR unused PDF copy of the files and their previews. 
                    try
                    {
                        var pdfFilePeriod = _settingValues.GetClearOldPdfCopiesInDay(context);
                        var fileTodelete = docFileDb.GetOldPdfForAttachedFiles(context, pdfFilePeriod);
                        foreach (var file in fileTodelete)
                        {
                            fileStore.DeletePdfCopy(context, file);
                            file.PdfCreated = false;
                            docFileDb.UpdateFilePdfView(context, file);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(context, "Could not process clear trash documents", ex);
                    }
                    ((DmsContext) context.DbContext).Dispose();
                    context.DbContext = null;
                }, db, ctx);
            }
        }

        public void InitWorkersForClient(DatabaseModelForAdminContext dbModel)
        {
            var dbs = new List<DatabaseModelForAdminContext> { dbModel };
            InitializeAutoPlanTask(dbs);
            InitializeClearTrashTask(dbs);
            InitializeMailWorkerTask(dbs);

            // add full text worker for new client. 
            var ftSrv = DmsResolver.Current.Get<IFullTextSearchService>();
            ftSrv.AddNewClient(new AdminContext(dbModel));

            var queueSrv = DmsResolver.Current.Get<IQueueWorkerService>();
            queueSrv.AddNewClient(new AdminContext(dbModel));
        }

        public void RemoveWorkersForClient(int clientId)
        {
            _taskManager.RemoveTaskForClient(clientId);
            var ftSrv = DmsResolver.Current.Get<IFullTextSearchService>();
            ftSrv.RemoveClient(clientId);
            var queueSrv = DmsResolver.Current.Get<IQueueWorkerService>();
            queueSrv.RemoveWorkersForClient(clientId);
        }
    }
}