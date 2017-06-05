using BL.CrossCutting.Interfaces;
using BL.Database.Admins;
using BL.Database.DatabaseContext;
using BL.Database.Dictionaries;
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
            Bind<IDmsDatabaseContext>().To<DmsContext>();
        }

        private void RegistrateDocumentProcess()
        {

            //Bind<IDictionariesDbProcess>().To<DictionariesDbProcess>().InSingletonScope();
            Bind<DictionariesDbProcess>().ToSelf().InSingletonScope();
            Bind<IDocumentsDbProcess>().To<DocumentsDbProcess>().InSingletonScope();
            Bind<ITemplateDbProcess>().To<TemplateDbProcess>().InSingletonScope();
            Bind<IDocumentFileDbProcess>().To<DocumentFileDbProcess>().InSingletonScope();

            Bind<IDocumentSendListsDbProcess>().To<DocumentSendListsDbProcess>().InSingletonScope();
            Bind<IDocumentFiltersDbProcess>().To<DocumentFiltersDbProcess>().InSingletonScope();
            Bind<IDocumentOperationsDbProcess>().To<DocumentOperationsDbProcess>().InSingletonScope();

            Bind<IDocumentTagsDbProcess>().To<DocumentTagsDbProcess>().InSingletonScope();

            Bind<IDocumentTasksDbProcess>().To<DocumentTasksDbProcess>().InSingletonScope();
        }

        private void RegistrateSystemProcess()
        {
            //Bind<IAdminsDbProcess>().To<AdminsDbProcess>().InSingletonScope();
            Bind<AdminsDbProcess>().ToSelf().InSingletonScope();
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
