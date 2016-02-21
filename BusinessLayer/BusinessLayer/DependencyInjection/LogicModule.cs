using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Commands;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.Logging;
using BL.Logic.Secure;
using BL.Logic.Settings;
using BL.Logic.SystemLogic;
using Ninject.Modules;

namespace BL.Logic.DependencyInjection
{
    public class LogicModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();
            Bind<ISettings>().To<Setting>().InSingletonScope();
            Bind<IDocumentService>().To<DocumentService>().InSingletonScope();
            Bind<ITemplateDocumentService>().To<TemplateDocumentService>().InSingletonScope();
            Bind<IDictionaryService>().To<DictionaryService>().InSingletonScope();
            Bind<IAdminService>().To<AdminService>().InSingletonScope();
            Bind<ISecureService>().To<SecureService>().InSingletonScope();
            Bind<IFileStore>().To<FileStore>().InSingletonScope();
            Bind<IDocumentFileService>().To<DocumentFileService>().InSingletonScope();

            Bind<IDocumentOperationsService>().To<DocumentOperationsService>().InSingletonScope();
            Bind<IDocumentFiltersService>().To<DocumentFiltersService>().InSingletonScope();
            Bind<IDocumentSendListService>().To<DocumentSendListService>().InSingletonScope();
            Bind<IDocumentTagService>().To<DocumentTagService>().InSingletonScope();

            Bind<ICommandService>().To<CommandService>().InSingletonScope();
            LoadCommands();
        }

        private void LoadCommands()
        {
            Bind<ICommand>().To<AddDocumentCommand>();
            Bind<ICommand>().To<DeleteDocumentCommand>();
            Bind<ICommand>().To<UpdateDocumentCommand>();
            Bind<ICommand>().To<CopyDocumentCommand>();

            Bind<ICommand>().To<ControlChangeCommand>();
            Bind<ICommand>().To<ControlOnCommand>();
            Bind<ICommand>().To<ControlOffCommand>();

            Bind<ICommand>().To<AddFavouriteCommand>();
            Bind<ICommand>().To<DeleteFavouriteCommand>();

            Bind<ICommand>().To<StartWorkCommand>();
            Bind<ICommand>().To<FinishWorkCommand>();

            Bind<ICommand>().To<AddNoteCommand>();
            Bind<ICommand>().To<SendMessageCommand>();
            Bind<ICommand>().To<ChangeExecutorCommand>();
            Bind<ICommand>().To<RegisterDocumentCommand>();
        }
    }
}