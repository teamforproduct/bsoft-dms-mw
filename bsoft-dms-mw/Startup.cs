using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(bsoft_dms_mw.Startup))]
namespace bsoft_dms_mw
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
