using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.Logic.SystemServices.AutoPlan;
using BL.Logic.SystemServices.ClearTrashDocuments;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Logic.SystemServices.QueueWorker;
using BL.Model.Enums;
using BL.Model.WebAPI.Filters;
using DMS_WebAPI.Models;
using DMS_WebAPI.Utilities;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Web;

[assembly: OwinStartup(typeof(DMS_WebAPI.Startup))]

namespace DMS_WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var filePath = HttpContext.Current.Server.MapPath("~/SiteErrors.txt");

            FileLogger.AppendTextToFile("STARTUP BEGIN!!! " + DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm") + " UTC", filePath); 

            ApplicationDbContext.CreateDatabaseIfNotExists();

            // configuring authentication
            ConfigureAuth(app);
            
            // Проверка на целостность Actions в процедуре импорта 
            //var systemService = DmsResolver.Current.Get<ISystemService>();
            Properties.Settings.Default["ServerPath"] = HttpContext.Current.Server.MapPath("~/");
            Properties.Settings.Default.Save();
            // Проверка на целостность переводов
            ApplicationDbImportData.CheckLanguages();
            

            //Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
            //var tt = Database.Exists("DefaultConnection");

            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();

            var dbs = dbProc.GetServersByAdminContext(new FilterAdminServers { ServerTypes = new List<EnumDatabaseType> { EnumDatabaseType.SQLServer } });


            FileLogger.AppendTextToFile("StartWorkers " + DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm") + " UTC", filePath);
            //#if !DEBUG
            // Сервис бекграундной обработки задач/экшенов/команд. 
            var queueWorker = DmsResolver.Current.Get<IQueueWorkerService>();
            queueWorker.Initialize(dbs);
//#endif

            //foreach (var srv in DmsResolver.Current.GetAll<ISystemWorkerService>())
            //{
            //    srv.Initialize(dbs);
            //}

            //var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
            //mailService.Initialize(dbs);


#if !DEBUG
            //TODO
            // Полнотекстовый поиск
           var indexService = DmsResolver.Current.Get<IFullTextSearchService>();
           indexService.Initialize(dbs);
#endif


//#if !DEBUG
            //TODO
            var autoPlanService = DmsResolver.Current.Get<IAutoPlanService>();
            autoPlanService.Initialize(dbs);
//#endif
#if !DEBUG
            //TODO
            var clearTrashDocumentsService = DmsResolver.Current.Get<IClearTrashDocumentsService>();
            clearTrashDocumentsService.Initialize(dbs);
#endif

#if !DEBUG
            // Очистка устаревших пользовательских контекстов
            var userContextService = DmsResolver.Current.Get<UserContextsWorkerService>();
            userContextService.Initialize();
#endif

#if !DEBUG
            // Проверка лицензии
            var licencesService = DmsResolver.Current.Get<LicencesWorkerService>();
            licencesService.Initialize();
#endif

            FileLogger.AppendTextToFile("STARTUP END!!! " + DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm") + " UTC", filePath);
        }
    }
}
