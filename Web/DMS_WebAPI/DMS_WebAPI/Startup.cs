﻿using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.Database;
using DMS_WebAPI.Utilities;
using Microsoft.Owin;
using Owin;
using BL.Logic.SystemServices.ClearTrashDocuments;
using BL.Model.WebAPI.Filters;
using System.Collections.Generic;
using System.Data.Entity;
using DMS_WebAPI.Models;

[assembly: OwinStartup(typeof(DMS_WebAPI.Startup))]

namespace DMS_WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ApplicationDbContext.CreateDatabaseIfNotExists();

            ConfigureAuth(app);

            //Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
            //var tt = Database.Exists("DefaultConnection");

            var dbProc = new WebAPIDbProcess();

            var dbs = dbProc.GetServersByAdmin(new FilterAdminServers { ServerTypes = new List<DatabaseType> { DatabaseType.SQLServer } });

            //foreach (var srv in DmsResolver.Current.GetAll<ISystemWorkerService>())
            //{
            //    srv.Initialize(dbs);
            //}

            //var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
            //mailService.Initialize(dbs);

            //TODO
            //var indexService = DmsResolver.Current.Get<IFullTextSearchService>();
            //indexService.Initialize(dbs);

            //TODO
            var autoPlanService = DmsResolver.Current.Get<IAutoPlanService>();
            autoPlanService.Initialize(dbs);

            //TODO
            var clearTrashDocumentsService = DmsResolver.Current.Get<IClearTrashDocumentsService>();
            clearTrashDocumentsService.Initialize(dbs);

            var userContextService = DmsResolver.Current.Get<UserContextWorkerService>();
            userContextService.Initialize();

            var licencesService = DmsResolver.Current.Get<LicencesWorkerService>();
            licencesService.Initialize();
        }
    }
}
