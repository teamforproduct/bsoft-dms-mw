using BL.Logic.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Logic.SystemServices.MailWorker;
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
            //var readXml = new Utilities.ReadXml("/servers.xml");
            //var dbs = readXml.Read();

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
        }
    }
}
