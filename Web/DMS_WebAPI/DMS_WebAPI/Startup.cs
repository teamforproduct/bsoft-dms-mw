using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemCore.Interfaces;
using BL.Logic.SystemServices.AutoPlan;
using BL.Logic.SystemServices.ClearTrashDocuments;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.Enums;
using BL.Model.WebAPI.Filters;
using DMS_WebAPI.Models;
using DMS_WebAPI.Utilities;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(DMS_WebAPI.Startup))]

namespace DMS_WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ApplicationDbContext.CreateDatabaseIfNotExists();

            // configuring authentication
            ConfigureAuth(app);

            // Проверка на целостность Actions в процедуре импорта 
            var systemService = DmsResolver.Current.Get<ISystemService>();
            systemService.CheckSystemActions();

            // Проверка на целостность переводов
            ApplicationDbImportData.CheckLanguages();


        //Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
        //var tt = Database.Exists("DefaultConnection");

            var dbProc = new WebAPIDbProcess();

            var dbs = dbProc.GetServersByAdmin(new FilterAdminServers { ServerTypes = new List<EnumDatabaseType> { EnumDatabaseType.SQLServer } });

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
        }
    }
}
