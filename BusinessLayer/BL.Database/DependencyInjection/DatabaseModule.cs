using BL.Database.Admins;
using BL.Database.Admins.Interfaces;
using BL.Database.Dictionaries;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents;
using BL.Database.Documents.Interfaces;
using BL.Database.Encryption;
using BL.Database.Encryption.Interfaces;
using BL.Database.FileWorker;
using BL.Database.Helper;
using BL.Database.Reports;
using BL.Database.SystemDb;
using Ninject.Modules;

namespace BL.Database.DependencyInjection
{
    public class DatabaseModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConnectionHelper>().To<ConnectionHelper>().InSingletonScope();
            RegistrateSystemProcess();
            RegistrateDocumentProcess();
            RegistrateEncryptionProcess();

            Bind<DmsReport>().ToSelf().InSingletonScope();
        }

        private void RegistrateDocumentProcess()
        {

            //Bind<IDictionariesDbProcess>().To<DictionariesDbProcess>().InSingletonScope();
            Bind<DictionariesDbProcess>().ToSelf().InSingletonScope();
            Bind<IDocumentsDbProcess>().To<DocumentsDbProcess>().InSingletonScope();
            Bind<ITemplateDocumentsDbProcess>().To<TemplateDocumentsDbProcess>().InSingletonScope();
            Bind<IDocumentFileDbProcess>().To<DocumentFileDbProcess>().InSingletonScope();

            Bind<IDocumentSendListsDbProcess>().To<DocumentSendListsDbProcess>().InSingletonScope();
            Bind<IDocumentFiltersDbProcess>().To<DocumentFiltersDbProcess>().InSingletonScope();
            Bind<IDocumentOperationsDbProcess>().To<DocumentOperationsDbProcess>().InSingletonScope();

            Bind<IDocumentTagsDbProcess>().To<DocumentTagsDbProcess>().InSingletonScope();

            Bind<IDocumentTasksDbProcess>().To<DocumentTasksDbProcess>().InSingletonScope();
        }

        private void RegistrateSystemProcess()
        {
            Bind<IAdminsDbProcess>().To<AdminsDbProcess>().InSingletonScope();
            Bind<ILanguagesDbProcess>().To<LanguagesDbProcess>().InSingletonScope();
            Bind<ISystemDbProcess>().To<SystemDbProcess>().InSingletonScope();
            Bind<IFileStore>().To<FileStore>().InSingletonScope();
            Bind<IFullTextDbProcess>().To<FullTextDbProcess>().InSingletonScope();
        }

        private void RegistrateEncryptionProcess()
        {
            Bind<IEncryptionDbProcess>().To<EncryptionDbProcess>().InSingletonScope();
        }

    }
}
