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

[assembly: OwinStartup(typeof(DMS_WebAPI.Startup))]

namespace DMS_WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            var srv = new Servers();
            var dbs = srv.GetServers().Where(x=>x.ServerType == DatabaseType.SQLServer);

            //foreach (var srv in DmsResolver.Current.GetAll<ISystemWorkerService>())
            //{
            //    srv.Initialize(dbs);
            //}

            //var mailService = DmsResolver.Current.Get<MailSenderWorkerService>();
            //mailService.Initialize(dbs);

            //var indexService = DmsResolver.Current.Get<FullTextSearchService>();
            //indexService.Initialize(dbs);

            //var autoPlanService = DmsResolver.Current.Get<AutoPlanService>();
            //autoPlanService.Initialize(dbs);

            var userContextService = DmsResolver.Current.Get<UserContextWorkerService>();
            userContextService.Initialize();
        }
    }
}
