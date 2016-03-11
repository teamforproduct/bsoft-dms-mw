using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore;
using BL.Logic.DictionaryCore.CustomDictionary;
using BL.Logic.DictionaryCore.DocumentType;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DictionaryCore.Tag;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.AdditionalCommands;
using BL.Logic.DocumentCore.Commands;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.DocumentCore.SendListCommands;
using BL.Logic.FileWorker;
using BL.Logic.Logging;
using BL.Logic.Observers;
using BL.Logic.PropertyCore;
using BL.Logic.PropertyCore.Commands;
using BL.Logic.PropertyCore.Interfaces;
using BL.Logic.Settings;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.Enums;
using Ninject.Modules;

namespace BL.Logic.DependencyInjection
{
    public class LogicModule : NinjectModule
    {
        public override void Load()
        {
            LoadSystemModule();
            LoadDocumentModule();
            LoadDocumentCommands();
            LoadDictionaryCommands();
            LoadObservers();
            LoadMailService();
        }

        private void LoadSystemModule()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();
            Bind<ISettings>().To<Setting>().InSingletonScope();
            Bind<ICommandService>().To<CommandService>().InSingletonScope();
            Bind<IFileStore>().To<FileStore>().InSingletonScope();
            Bind<IAdminService>().To<AdminService>().InSingletonScope();
            Bind<ISystemWorkerService>().To<MailSenderWorkerService>().InSingletonScope();
            Bind<ISystemWorkerService>().To<FullTextSearchService>().InSingletonScope();
        }

        private void LoadDocumentModule()
        {
            Bind<IDocumentService>().To<DocumentService>().InSingletonScope();
            Bind<ITemplateDocumentService>().To<TemplateDocumentService>().InSingletonScope();
            Bind<IDictionaryService>().To<DictionaryService>().InSingletonScope();
            Bind<IDocumentFileService>().To<DocumentFileService>().InSingletonScope();

            Bind<IDocumentFiltersService>().To<DocumentFiltersService>().InSingletonScope();
            Bind<IDocumentSendListService>().To<DocumentSendListService>().InSingletonScope();
            Bind<IDocumentTagService>().To<DocumentTagService>().InSingletonScope();

            //Bind<ICommandService>().To<CommandService>().InSingletonScope();

            Bind<IPropertyService>().To<PropertyService>().InSingletonScope();
        }

        private void LoadMailService()
        {
            Bind<IMailSender>().To<SSLMailSender>().InSingletonScope().Named(MailServerType.SSLServer.ToString());
            Bind<IMailSender>().To<BasicSmtpMailSender>().InSingletonScope().Named(MailServerType.BasicSmtp.ToString());
        }

        private void LoadObservers()
        {
            Bind<ICommandObserver>().To<AfterDocumentAddObserver>();
        }

        private void LoadPropertyCommands()
        {
            Bind<IPropertyCommand>().To<AddPropertyCommand>();
            Bind<IPropertyCommand>().To<DeletePropertyCommand>();
            Bind<IPropertyCommand>().To<ModifyPropertyCommand>();

            Bind<IPropertyCommand>().To<AddPropertyLinkCommand>();
            Bind<IPropertyCommand>().To<DeletePropertyLinkCommand>();
            Bind<IPropertyCommand>().To<ModifyPropertyLinkCommand>();
        }

        private void LoadDictionaryCommands()
        {
            Bind<IDictionaryCommand>().To<AddDictionaryDocumentTypeCommand>();
            Bind<IDictionaryCommand>().To<ModifyDictionaryDocumentTypeCommand>();

            Bind<IDictionaryCommand>().To<AddDictionaryTagCommand>();
            Bind<IDictionaryCommand>().To<ModifyDictionaryTagCommand>();

            Bind<IDictionaryCommand>().To<AddCustomDictionaryCommand>();
            Bind<IDictionaryCommand>().To<ModifyCustomDictionaryCommand>();
            Bind<IDictionaryCommand>().To<DeleteCustomDictionaryCommand>();

            Bind<IDictionaryCommand>().To<AddCustomDictionaryTypeCommand>();
            Bind<IDictionaryCommand>().To<ModifyCustomDictionaryTypeCommand>();
            Bind<IDictionaryCommand>().To<DeleteCustomDictionaryTypeCommand>();
        }

        private void LoadDocumentCommands()
        {
            Bind<IDocumentCommand>().To<AddDocumentCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentCommand>();
            Bind<IDocumentCommand>().To<ModifyDocumentCommand>();
            Bind<IDocumentCommand>().To<CopyDocumentCommand>();

            Bind<IDocumentCommand>().To<ControlChangeDocumentCommand>();
            Bind<IDocumentCommand>().To<ControlOnDocumentCommand>();
            Bind<IDocumentCommand>().To<ControlOffDocumentCommand>();

            Bind<IDocumentCommand>().To<AddFavouriteDocumentCommand>();
            Bind<IDocumentCommand>().To<DeleteFavouriteDocumentCommand>();
            Bind<IDocumentCommand>().To<StartWorkDocumentCommand>();
            Bind<IDocumentCommand>().To<FinishWorkDocumentCommand>();

            Bind<IDocumentCommand>().To<AddNoteDocumentCommand>();
            Bind<IDocumentCommand>().To<SendMessageDocumentCommand>();
            Bind<IDocumentCommand>().To<ChangeExecutorDocumentCommand>();
            Bind<IDocumentCommand>().To<RegisterDocumentCommand>();
            Bind<IDocumentCommand>().To<LaunchPlanDocumentCommand>();
            Bind<IDocumentCommand>().To<StopPlanDocumentCommand>();

            Bind<IDocumentCommand>().To<AddDocumentLinkCommand>();

            Bind<IDocumentCommand>().To<AddDocumentFileCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentFileCommand>();
            Bind<IDocumentCommand>().To<ModifyDocumentFileCommand>();

            Bind<IDocumentCommand>().To<SendForInformationDocumentCommand>();
            Bind<IDocumentCommand>().To<SendForControlDocumentCommand>();
            Bind<IDocumentCommand>().To<SendForResponsibleExecutionDocumentCommand>();
            Bind<IDocumentCommand>().To<SendForExecutionDocumentCommand>();
            Bind<IDocumentCommand>().To<MarkExecutionDocumentCommand>();
            Bind<IDocumentCommand>().To<RejectResultDocumentCommand>();
            Bind<IDocumentCommand>().To<AcceptResultDocumentCommand>();

            Bind<IDocumentCommand>().To<SendForSigningDocumentCommand>();
            Bind<IDocumentCommand>().To<RejectResultDocumentCommand>();
            Bind<IDocumentCommand>().To<WithdrawSigningDocumentCommand>();
            Bind<IDocumentCommand>().To<AffixSigningDocumentCommand>();

            Bind<IDocumentCommand>().To<AddSavedFilterCommand>();
            Bind<IDocumentCommand>().To<DeleteSavedFilterCommand>();
            Bind<IDocumentCommand>().To<ModifySavedFilterCommand>();

            Bind<IDocumentCommand>().To<AddByStandartSendListDocumentRestrictedSendListCommand>();
            Bind<IDocumentCommand>().To<AddByStandartSendListDocumentSendListCommand>();
            Bind<IDocumentCommand>().To<AddDocumentRestrictedSendListCommand>();
            Bind<IDocumentCommand>().To<AddDocumentSendListCommand>();
            Bind<IDocumentCommand>().To<AddDocumentSendListStageCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentRestrictedSendListCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentSendListCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentSendListStageCommand>();
            Bind<IDocumentCommand>().To<ModifyDocumentSendListCommand>();
            Bind<IDocumentCommand>().To<LaunchDocumentSendListItemCommand>();

        }

    }
}