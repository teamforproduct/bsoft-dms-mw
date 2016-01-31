using BL.CrossCutting.Helpers;
using BL.Database.Documents;
using BL.Database.Documents.Interfaces;
using BL.Database.Manager;
using BL.Database.Security;
using Ninject.Modules;

namespace BL.Database.DependencyInjection
{
    public class DatabaseModule : NinjectModule
    {
        public override void Load()
        {
            InternalClassRegistration();

            Bind<ISecurityDbProcess>().To<SecurityDbProcess>().InSingletonScope();
            Bind<IDocumnetsDbProcess>().To<DocumnetsDbProcess>().InSingletonScope();
            Bind<ITemplateDocumnetsDbProcess>().To<TemplateDocumnetsDbProcess>().InSingletonScope();
        }

        private void InternalClassRegistration()
        {
            // this class should be used only n DB layer
            Bind<IConnectionStringHelper>().To<ConnectionStringHelper>().InSingletonScope();
            Bind<IConnectionManager>().To<ConnectionManager>().InSingletonScope();
        }
    }
}