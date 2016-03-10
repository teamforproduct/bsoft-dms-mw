using BL.Logic.DependencyInjection;
using BL.Logic.MailWorker;
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
            //var mailService = DmsResolver.Current.Get<IMailService>();
            //var readXml = new Utilities.ReadXml("/servers.xml");
            //var dbs = readXml.ReadDBs();
            //mailService.Initialize(dbs);
        }
    }
}
