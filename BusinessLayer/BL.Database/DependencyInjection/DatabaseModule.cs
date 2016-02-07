using BL.Database.Admins;
using BL.Database.Admins.Interfaces;
using BL.Database.Dictionaries;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents;
using BL.Database.Documents.Interfaces;
using BL.Database.Manager;
using BL.Database.Security;
using BL.Database.SystemDb;
using Ninject.Modules;

namespace BL.Database.DependencyInjection
{
    public class DatabaseModule : NinjectModule
    {
        public override void Load()
        {
            InternalClassRegistration();
            RegistrateSystemProcess();
            RegistrateDocumentProcess();
        }

        private void RegistrateDocumentProcess()
        {
            Bind<IDictionariesDbProcess>().To<DictionariesDbProcess>().InSingletonScope();
            Bind<IDocumentsDbProcess>().To<DocumentsDbProcess>().InSingletonScope();
            Bind<ITemplateDocumentsDbProcess>().To<TemplateDocumentsDbProcess>().InSingletonScope();
            Bind<IDocumentFileDbProcess>().To<DocumentFileDbProcess>().InSingletonScope();
        }

        private void RegistrateSystemProcess()
        {
            Bind<ISecurityDbProcess>().To<SecurityDbProcess>().InSingletonScope();
            Bind<IAdminsDbProcess>().To<AdminsDbProcess>().InSingletonScope();
            Bind<ISystemDbProcess>().To<SystemDbProcess>().InSingletonScope();
        }

        private void InternalClassRegistration()
        {
            // this class should be used only n DB layer
            Bind<IConnectionManager>().To<ConnectionManager>().InSingletonScope();
        }
    }
}