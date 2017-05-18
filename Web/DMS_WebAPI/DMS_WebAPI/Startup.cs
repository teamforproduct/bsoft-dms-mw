using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Logic.SystemServices.QueueWorker;
using BL.Model.Enums;
using BL.Model.WebAPI.Filters;
using DMS_WebAPI.DatabaseContext;
using DMS_WebAPI.Utilities;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Web;
using BL.Logic.SystemServices.TaskManagerService;

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


            // Столкнулись с проблемой вычитки настроек из центральной базы в транзакции (Нельзя использовть два дб. контекста под одной транзакцией). 
            var genSett = DmsResolver.Current.Get<IGeneralSettings>();
            genSett.ReadAll();

            //Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
            //var tt = Database.Exists("DefaultConnection");

            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();

            var dbs = dbProc.GetServersByAdminContext(new FilterAdminServers { ServerTypes = new List<EnumDatabaseType> { EnumDatabaseType.SQLServer } });
            var taskInit = DmsResolver.Current.Get<ICommonTaskInitializer>(); 

            FileLogger.AppendTextToFile("StartWorkers " + DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm") + " UTC", filePath);
//#if !DEBUG
            // Сервис бекграундной обработки задач/экшенов/команд. 
            var queueWorker = DmsResolver.Current.Get<IQueueWorkerService>();
            queueWorker.Initialize(dbs);
//#endif

//#if !DEBUG
            //TODO
            // Полнотекстовый поиск
           var indexService = DmsResolver.Current.Get<IFullTextSearchService>();
           indexService.Initialize(dbs);
//#endif


#if !DEBUG
            //TODO
            taskInit.InitializeAutoPlanTask(dbs);
#endif

#if !DEBUG
            //TODO
            taskInit.InitializeClearTrashTask(dbs);
#endif

#if !DEBUG
            //TODO
            //taskInit.InitializeMailWorkerTask(dbs);
#endif

#if !DEBUG
            // Очистка устаревших пользовательских контекстов
            WebCommonTaskInitializer.AddAuthWorker();

#endif

            FileLogger.AppendTextToFile("STARTUP END!!! " + DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm") + " UTC\r\n", filePath);
        }
    }
}
