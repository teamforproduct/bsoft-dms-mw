using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.ClientCore;
using BL.Logic.ClientCore.Interfaces;
using BL.Logic.DictionaryCore;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.AdditionalCommands;
using BL.Logic.DocumentCore.Commands;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.DocumentCore.PaperCommands;
using BL.Logic.DocumentCore.ReportsCommands;
using BL.Logic.DocumentCore.SendListCommands;
using BL.Logic.DocumentCore.TemplateCommands;
using BL.Logic.EncryptionCore;
using BL.Logic.EncryptionCore.Certificate;
using BL.Logic.EncryptionCore.Commands;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Logic.Logging;
using BL.Logic.Observers;
using BL.Logic.PropertyCore;
using BL.Logic.PropertyCore.Commands;
using BL.Logic.PropertyCore.Interfaces;
using BL.Logic.Settings;
using BL.Logic.SystemCore;
using BL.Logic.SystemCore.Interfaces;
using BL.Logic.SystemServices.AutoPlan;
using BL.Logic.SystemServices.FileService;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Logic.SystemServices.MailWorker;
using BL.Logic.SystemServices.QueueWorker;
using BL.Logic.SystemServices.TaskManagerService;
using BL.Logic.SystemServices.TempStorage;
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
            LoadReportCommands();
            LoadEncryptionModule();
            LoadEncryptionCommands();
            LoadPropertyCommands();
        }

        private void LoadSystemModule()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();
            Bind<ISettings>().To<Setting>().InSingletonScope();
            Bind<ISettingValues>().To<SettingValues>().InSingletonScope();
            Bind<ICommandService>().To<CommandService>().InSingletonScope();
            Bind<IAdminService>().To<AdminService>().InSingletonScope();
            Bind<IClientService>().To<ClientService>().InSingletonScope();
            Bind<IFullTextSearchService>().To<FullTextSearchService>().InSingletonScope();
            Bind<IQueueWorkerService>().To<QueueWorkerService>().InSingletonScope();
            Bind<ITempStorageService>().To<TempStorageService>().InSingletonScope();
            Bind<ISystemService>().To<SystemService>().InSingletonScope();
            Bind<IFileService>().To<FileService>().InSingletonScope();
            Bind<ICommonTaskInitializer>().To<CommonTaskInitializer>().InSingletonScope();
            Bind<ITaskManager>().To<TaskManager>().InSingletonScope();
            Bind<IAutoPlanService>().To<AutoPlanService>().InSingletonScope();
            Bind<IMailSenderWorkerService>().To<MailSenderWorkerService>().InSingletonScope();
        }

        private void LoadDocumentModule()
        {
            Bind<IDocumentService>().To<DocumentService>().InSingletonScope();
            Bind<ITemplateService>().To<TemplateService>().InSingletonScope();
            Bind<IDictionaryService>().To<DictionaryService>().InSingletonScope();
            Bind<IDocumentFileService>().To<DocumentFileService>().InSingletonScope();

            Bind<IDocumentFiltersService>().To<DocumentFiltersService>().InSingletonScope();
            Bind<IDocumentSendListService>().To<DocumentSendListService>().InSingletonScope();
            Bind<IDocumentTagService>().To<DocumentTagService>().InSingletonScope();
            Bind<IDocumentTaskService>().To<DocumentTaskService>().InSingletonScope();

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
            Bind<IDocumentCommand>().To<ControlTargetChangeDocumentCommand>();
            Bind<IDocumentCommand>().To<ControlOnDocumentCommand>();
            Bind<IDocumentCommand>().To<ControlOffDocumentCommand>();

            Bind<IDocumentCommand>().To<AddFavouriteDocumentCommand>();
            Bind<IDocumentCommand>().To<DeleteFavouriteDocumentCommand>();
            Bind<IDocumentCommand>().To<StartWorkDocumentCommand>();
            Bind<IDocumentCommand>().To<FinishWorkDocumentCommand>();

            Bind<IDocumentCommand>().To<AddNoteDocumentCommand>();
            Bind<IDocumentCommand>().To<SendMessageDocumentCommand>();
            Bind<IDocumentCommand>().To<ChangeExecutorDocumentCommand>();
            Bind<IDocumentCommand>().To<ChangePositionDocumentCommand>();
            Bind<IDocumentCommand>().To<RegisterDocumentCommand>();
            Bind<IDocumentCommand>().To<LaunchPlanDocumentCommand>();
            Bind<IDocumentCommand>().To<StopPlanDocumentCommand>();

            Bind<IDocumentCommand>().To<AddDocumentLinkCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentLinkCommand>();

            Bind<IDocumentCommand>().To<AddDocumentFileCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentFileCommand>();
            Bind<IDocumentCommand>().To<RestoreDocumentFileCommand>();

            Bind<IDocumentCommand>().To<ModifyDocumentFileCommand>();

            Bind<IDocumentCommand>().To<AcceptDocumentFileCommand>();
            Bind<IDocumentCommand>().To<RejectDocumentFileCommand>();
            Bind<IDocumentCommand>().To<RenameDocumentFileCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentFileVersionCommand>();

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
            Bind<IDocumentCommand>().To<SelfAffixSigningDocumentCommand>();
            Bind<IDocumentCommand>().To<VerifySigningDocumentCommand>();

            Bind<IDocumentCommand>().To<AddSavedFilterCommand>();
            Bind<IDocumentCommand>().To<DeleteSavedFilterCommand>();
            Bind<IDocumentCommand>().To<ModifySavedFilterCommand>();

            Bind<IDocumentCommand>().To<AddByStandartSendListDocumentRestrictedSendListCommand>();
            Bind<IDocumentCommand>().To<AddDocumentRestrictedSendListCommand>();
            Bind<IDocumentCommand>().To<AddDocumentSendListCommand>();
            Bind<IDocumentCommand>().To<AddDocumentSendListStageCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentRestrictedSendListCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentSendListCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentSendListStageCommand>();
            Bind<IDocumentCommand>().To<ModifyDocumentSendListCommand>();
            Bind<IDocumentCommand>().To<LaunchDocumentSendListItemCommand>();

            Bind<IDocumentCommand>().To<AddDocumentTaskCommand>();
            Bind<IDocumentCommand>().To<ModifyDocumentTaskCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentTaskCommand>();

            Bind<IDocumentCommand>().To<AddDocumentPaperCommand>();
            Bind<IDocumentCommand>().To<ModifyDocumentPaperCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentPaperCommand>();

            Bind<IDocumentCommand>().To<AddDocumentPaperListCommand>();
            Bind<IDocumentCommand>().To<ModifyDocumentPaperListCommand>();
            Bind<IDocumentCommand>().To<DeleteDocumentPaperListCommand>();

            Bind<IDocumentCommand>().To<MarkOwnerDocumentPaperCommand>();
            Bind<IDocumentCommand>().To<MarkСorruptionDocumentPaperCommand>();

            Bind<IDocumentCommand>().To<SendDocumentPaperEventCommand>();
            Bind<IDocumentCommand>().To<CancelSendDocumentPaperEventCommand>();
            Bind<IDocumentCommand>().To<RecieveDocumentPaperEventCommand>();
            Bind<IDocumentCommand>().To<PlanDocumentPaperEventCommand>();
            Bind<IDocumentCommand>().To<CancelPlanDocumentPaperEventCommand>();

            Bind<IDocumentCommand>().To<AddTemplateCommand>();
            Bind<IDocumentCommand>().To<AddTemplateFileCommand>();
            Bind<IDocumentCommand>().To<AddTemplateRestrictedSendListCommand>();
            Bind<IDocumentCommand>().To<AddTemplateSendListCommand>();
            Bind<IDocumentCommand>().To<AddTemplateTaskCommand>();
            Bind<IDocumentCommand>().To<ModifyTemplateCommand>();
            Bind<IDocumentCommand>().To<ModifyTemplateFileCommand>();
            Bind<IDocumentCommand>().To<ModifyTemplateRestrictedSendListCommand>();
            Bind<IDocumentCommand>().To<ModifyTemplateSendListCommand>();
            Bind<IDocumentCommand>().To<ModifyTemplateTaskCommand>();
            Bind<IDocumentCommand>().To<DeleteTemplateCommand>();
            Bind<IDocumentCommand>().To<DeleteTemplateFileCommand>();
            Bind<IDocumentCommand>().To<DeleteTemplateRestrictedSendListCommand>();
            Bind<IDocumentCommand>().To<DeleteTemplateSendListCommand>();
            Bind<IDocumentCommand>().To<DeleteTemplateTaskCommand>();

        }

        private void LoadReportCommands()
        {
            Bind<IDocumentCommand>().To<ReportRegistrationCardDocumentCommand>();
            Bind<IDocumentCommand>().To<ReportRegisterTransmissionDocumentsCommand>();
            Bind<IDocumentCommand>().To<ReportDocumentForDigitalSignature>();
        }

        private void LoadEncryptionModule()
        {
            Bind<IEncryptionService>().To<EncryptionService>().InSingletonScope();
        }

        private void LoadEncryptionCommands()
        {
            Bind<IEncryptionCommand>().To<AddEncryptionCertificateCommand>();
            Bind<IEncryptionCommand>().To<DeleteEncryptionCertificateCommand>();
            Bind<IEncryptionCommand>().To<ModifyEncryptionCertificateCommand>();

            Bind<IEncryptionCommand>().To<VerifyPdfCommand>();
        }

    }
}