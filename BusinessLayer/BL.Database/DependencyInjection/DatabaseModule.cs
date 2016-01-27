using BL.Database.Documents;
using BL.Database.Helpers;
using BL.Database.Manager;
using Ninject.Modules;

namespace BL.Database.DependencyInjection
{
    public class DatabaseModule : NinjectModule
    {
        public override void Load()
        {
            InternalClassRegistration();

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