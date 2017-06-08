using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.AdditionalCommands;
using BL.Logic.DocumentCore.Commands;
using BL.Logic.DocumentCore.PaperCommands;
using BL.Logic.DocumentCore.ReportsCommands;
using BL.Logic.DocumentCore.SendListCommands;
using BL.Logic.DocumentCore.TemplateCommands;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore
{
    public static class DocumentCommandFactory
    {
        public static IDocumentCommand GetDocumentCommand(EnumActions act, IContext ctx, InternalDocument doc, object param)
        {
            if (ctx.ClientLicence?.LicenceError != null)
            {
                throw ctx.ClientLicence.LicenceError as DmsExceptions;
            }

            IDocumentCommand cmd;
            switch (act)
            {
                case EnumActions.AddDocument:
                    cmd = DmsResolver.Current.Get<AddDocumentCommand>();
                    break;
                case EnumActions.AddLinkedDocument:
                    cmd = DmsResolver.Current.Get<AddDocumentCommand>();
                    break;
                case EnumActions.ModifyDocument:
                    cmd = DmsResolver.Current.Get<ModifyDocumentCommand>();
                    break;
                case EnumActions.DeleteDocument:
                    cmd = DmsResolver.Current.Get<DeleteDocumentCommand>();
                    break;

                case EnumActions.AddFavourite:
                    cmd = DmsResolver.Current.Get<AddFavouriteDocumentCommand>();
                    break;
                case EnumActions.DeleteFavourite:
                    cmd = DmsResolver.Current.Get<DeleteFavouriteDocumentCommand>();
                    break;
                case EnumActions.FinishWork:
                    cmd = DmsResolver.Current.Get<FinishWorkDocumentCommand>();
                    break;
                case EnumActions.StartWork:
                    cmd = DmsResolver.Current.Get<StartWorkDocumentCommand>();
                    break;
                case EnumActions.ControlChange:
                    cmd = DmsResolver.Current.Get<ControlChangeDocumentCommand>();
                    break;
                case EnumActions.SendForExecutionChange:
                    cmd = DmsResolver.Current.Get<ControlChangeDocumentCommand>();
                    break;
                //case EnumDocumentActions.SendForControlChange:
                //    cmd = DmsResolver.Current.Get<ControlChangeDocumentCommand>();
                //    break;
                case EnumActions.ControlTargetChange:
                    cmd = DmsResolver.Current.Get<ControlTargetChangeDocumentCommand>();
                    break;
                case EnumActions.ControlOff:
                    cmd = DmsResolver.Current.Get<ControlOffDocumentCommand>();
                    break;
                case EnumActions.ControlOn:
                    cmd = DmsResolver.Current.Get<ControlOnDocumentCommand>();
                    break;
                case EnumActions.AddNote:
                    cmd = DmsResolver.Current.Get<AddNoteDocumentCommand>();
                    break;
                case EnumActions.ChangeExecutor:
                    cmd = DmsResolver.Current.Get<ChangeExecutorDocumentCommand>();
                    break;
                case EnumActions.ChangePosition:
                    cmd = DmsResolver.Current.Get<ChangePositionDocumentCommand>();
                    break;
                case EnumActions.CopyDocument:
                    cmd = DmsResolver.Current.Get<CopyDocumentCommand>();
                    break;
                case EnumActions.RegisterDocument:
                    cmd = DmsResolver.Current.Get<RegisterDocumentCommand>();
                    break;
                case EnumActions.SendMessage:
                    cmd = DmsResolver.Current.Get<SendMessageDocumentCommand>();
                    break;
                case EnumActions.AddDocumentLink:
                    cmd = DmsResolver.Current.Get<AddDocumentLinkCommand>();
                    break;
                case EnumActions.DeleteDocumentLink:
                    cmd = DmsResolver.Current.Get<DeleteDocumentLinkCommand>();
                    break;
                case EnumActions.AddDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<AddDocumentRestrictedSendListCommand>();
                    break;
                case EnumActions.AddByStandartSendListDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<AddByStandartSendListDocumentRestrictedSendListCommand>();
                    break;
                case EnumActions.DeleteDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<DeleteDocumentRestrictedSendListCommand>();
                    break;
                case EnumActions.AddDocumentSendList:
                case EnumActions.CopyDocumentSendList:
                    cmd = DmsResolver.Current.Get<AddDocumentSendListCommand>();
                    break;
                case EnumActions.SendDocument:
                    cmd = DmsResolver.Current.Get<SendDocumentCommand>();
                    break;
                //case EnumDocumentActions.AddByStandartSendListDocumentSendList:
                //    cmd = DmsResolver.Current.Get<AddByStandartSendListDocumentSendListCommand>();
                //    break;
                case EnumActions.ModifyDocumentSendList:
                    cmd = DmsResolver.Current.Get<ModifyDocumentSendListCommand>();
                    break;
                case EnumActions.DeleteDocumentSendList:
                    cmd = DmsResolver.Current.Get<DeleteDocumentSendListCommand>();
                    break;
                case EnumActions.AddDocumentSendListStage:
                    cmd = DmsResolver.Current.Get<AddDocumentSendListStageCommand>();
                    break;
                case EnumActions.DeleteDocumentSendListStage:
                    cmd = DmsResolver.Current.Get<DeleteDocumentSendListStageCommand>();
                    break;
                case EnumActions.ModifyDocumentTags:
                    cmd = DmsResolver.Current.Get<ModifyDocumentTagsCommand>();
                    break;


//                case EnumDocumentActions.AddDocumentFileUseMainNameFile:
                case EnumActions.AddDocumentFile:
                    cmd = DmsResolver.Current.Get<AddDocumentFileCommand>();
                    break;
                case EnumActions.ModifyDocumentFile:
                    cmd = DmsResolver.Current.Get<ModifyDocumentFileCommand>();
                    break;

                case EnumActions.AcceptDocumentFile:
                case EnumActions.AcceptMainVersionDocumentFile:
                    cmd = DmsResolver.Current.Get<AcceptDocumentFileCommand>();
                    break;
                case EnumActions.RejectDocumentFile:
                    cmd = DmsResolver.Current.Get<RejectDocumentFileCommand>();
                    break;
                case EnumActions.RenameDocumentFile:
                    cmd = DmsResolver.Current.Get<RenameDocumentFileCommand>();
                    break;
                case EnumActions.DeleteDocumentFileVersion:
                case EnumActions.DeleteDocumentFileVersionFinal:
                //                case EnumDocumentActions.DeleteDocumentFileVersionRecord:
                //cmd = DmsResolver.Current.Get<DeleteDocumentFileVersionCommand>();
                //break;
                case EnumActions.DeleteDocumentFile:
                    cmd = DmsResolver.Current.Get<DeleteDocumentFileCommand>();
                    break;
                case EnumActions.RestoreDocumentFileVersion:
                    cmd = DmsResolver.Current.Get<RestoreDocumentFileCommand>();
                    break;
                case EnumActions.SendForInformationExternal:
                    cmd = DmsResolver.Current.Get<SendForInformationDocumentCommand>();
                    break;
                case EnumActions.SendForInformation:
                    cmd = DmsResolver.Current.Get<SendForInformationDocumentCommand>();
                    break;
                case EnumActions.SendForConsideration:
                    cmd = DmsResolver.Current.Get<SendForInformationDocumentCommand>();
                    break;
                case EnumActions.SendForExecution:
                    cmd = DmsResolver.Current.Get<SendForExecutionDocumentCommand>();
                    break;
                case EnumActions.AskPostponeDueDate:
                    cmd = DmsResolver.Current.Get<AskPostponeDueDateDocumentCommand>();
                    break;
                case EnumActions.CancelPostponeDueDate:
                    cmd = DmsResolver.Current.Get<CancelPostponeDueDateDocumentCommand>();
                    break;
                case EnumActions.MarkExecution:
                    cmd = DmsResolver.Current.Get<MarkExecutionDocumentCommand>();
                    break;
                case EnumActions.RejectResult:
                    cmd = DmsResolver.Current.Get<RejectResultDocumentCommand>();
                    break;
                case EnumActions.AcceptResult:
                    cmd = DmsResolver.Current.Get<AcceptResultDocumentCommand>();
                    break;
                case EnumActions.CancelExecution:
                    cmd = DmsResolver.Current.Get<AcceptResultDocumentCommand>();
                    break;
                case EnumActions.SendForSigning:
                    cmd = DmsResolver.Current.Get<SendForSigningDocumentCommand>();
                    break;
                case EnumActions.SendForVisaing:
                    cmd = DmsResolver.Current.Get<SendForSigningDocumentCommand>();
                    break;
                case EnumActions.SendForАgreement:
                    cmd = DmsResolver.Current.Get<SendForSigningDocumentCommand>();
                    break;
                case EnumActions.SendForАpproval:
                    cmd = DmsResolver.Current.Get<SendForSigningDocumentCommand>();
                    break;

                case EnumActions.RejectSigning:
                    cmd = DmsResolver.Current.Get<RejectSigningDocumentCommand>();
                    break;
                case EnumActions.RejectVisaing:
                    cmd = DmsResolver.Current.Get<RejectSigningDocumentCommand>();
                    break;
                case EnumActions.RejectАgreement:
                    cmd = DmsResolver.Current.Get<RejectSigningDocumentCommand>();
                    break;
                case EnumActions.RejectАpproval:
                    cmd = DmsResolver.Current.Get<RejectSigningDocumentCommand>();
                    break;

                case EnumActions.WithdrawSigning:
                    cmd = DmsResolver.Current.Get<WithdrawSigningDocumentCommand>();
                    break;
                case EnumActions.WithdrawVisaing:
                    cmd = DmsResolver.Current.Get<WithdrawSigningDocumentCommand>();
                    break;
                case EnumActions.WithdrawАgreement:
                    cmd = DmsResolver.Current.Get<WithdrawSigningDocumentCommand>();
                    break;
                case EnumActions.WithdrawАpproval:
                    cmd = DmsResolver.Current.Get<WithdrawSigningDocumentCommand>();
                    break;

                case EnumActions.AffixSigning:
                    cmd = DmsResolver.Current.Get<AffixSigningDocumentCommand>();
                    break;
                case EnumActions.AffixVisaing:
                    cmd = DmsResolver.Current.Get<AffixSigningDocumentCommand>();
                    break;
                case EnumActions.AffixАgreement:
                    cmd = DmsResolver.Current.Get<AffixSigningDocumentCommand>();
                    break;
                case EnumActions.AffixАpproval:
                    cmd = DmsResolver.Current.Get<AffixSigningDocumentCommand>();
                    break;

                case EnumActions.SelfAffixSigning:
                    cmd = DmsResolver.Current.Get<SelfAffixSigningDocumentCommand>();
                    break;
                case EnumActions.VerifySigning:
                    cmd = DmsResolver.Current.Get<VerifySigningDocumentCommand>();
                    break;

                case EnumActions.AddSavedFilter:
                    cmd = DmsResolver.Current.Get<AddSavedFilterCommand>();
                    break;
                case EnumActions.DeleteSavedFilter:
                    cmd = DmsResolver.Current.Get<DeleteSavedFilterCommand>();
                    break;
                case EnumActions.ModifySavedFilter:
                    cmd = DmsResolver.Current.Get<ModifySavedFilterCommand>();
                    break;

                case EnumActions.LaunchDocumentSendListItem:
                    cmd = DmsResolver.Current.Get<LaunchDocumentSendListItemCommand>();
                    break;

                case EnumActions.LaunchPlan:
                    cmd = DmsResolver.Current.Get<LaunchPlanDocumentCommand>();
                    break;
                case EnumActions.StopPlan:
                    cmd = DmsResolver.Current.Get<StopPlanDocumentCommand>();
                    break;

                case EnumActions.MarkDocumentEventAsRead:
                    cmd = DmsResolver.Current.Get<MarkDocumentEventAsReadCommand>();
                    break;

                case EnumActions.AddDocumentTask:
                    cmd = DmsResolver.Current.Get<AddDocumentTaskCommand>();
                    break;

                case EnumActions.ModifyDocumentTask:
                    cmd = DmsResolver.Current.Get<ModifyDocumentTaskCommand>();
                    break;

                case EnumActions.DeleteDocumentTask:
                    cmd = DmsResolver.Current.Get<DeleteDocumentTaskCommand>();
                    break;

                case EnumActions.AddDocumentPaper:
                    cmd = DmsResolver.Current.Get<AddDocumentPaperCommand>();
                    break;

                case EnumActions.ModifyDocumentPaper:
                    cmd = DmsResolver.Current.Get<ModifyDocumentPaperCommand>();
                    break;

                case EnumActions.DeleteDocumentPaper:
                    cmd = DmsResolver.Current.Get<DeleteDocumentPaperCommand>();
                    break;

                case EnumActions.MarkOwnerDocumentPaper:
                    cmd = DmsResolver.Current.Get<MarkOwnerDocumentPaperCommand>();
                    break;

                case EnumActions.SendDocumentPaperEvent:
                    cmd = DmsResolver.Current.Get<SendDocumentPaperEventCommand>();
                    break;

                case EnumActions.CancelSendDocumentPaperEvent:
                    cmd = DmsResolver.Current.Get<CancelSendDocumentPaperEventCommand>();
                    break;
                case EnumActions.RecieveDocumentPaperEvent:
                    cmd = DmsResolver.Current.Get<RecieveDocumentPaperEventCommand>();
                    break;
                case EnumActions.PlanDocumentPaperEvent:
                    cmd = DmsResolver.Current.Get<PlanDocumentPaperEventCommand>();
                    break;
                case EnumActions.CancelPlanDocumentPaperEvent:
                    cmd = DmsResolver.Current.Get<CancelPlanDocumentPaperEventCommand>();
                    break;
                case EnumActions.MarkСorruptionDocumentPaper:
                    cmd = DmsResolver.Current.Get<MarkСorruptionDocumentPaperCommand>();
                    break;

                case EnumActions.AddDocumentPaperList:
                    cmd = DmsResolver.Current.Get<AddDocumentPaperListCommand>();
                    break;

                case EnumActions.ModifyDocumentPaperList:
                    cmd = DmsResolver.Current.Get<ModifyDocumentPaperListCommand>();
                    break;

                case EnumActions.DeleteDocumentPaperList:
                    cmd = DmsResolver.Current.Get<DeleteDocumentPaperListCommand>();
                    break;

                case EnumActions.ReportRegistrationCardDocument:
                    cmd = DmsResolver.Current.Get<ReportRegistrationCardDocumentCommand>();
                    break;
                case EnumActions.ReportRegisterTransmissionDocuments:
                    cmd = DmsResolver.Current.Get<ReportRegisterTransmissionDocumentsCommand>();
                    break;
                case EnumActions.ReportDocumentForDigitalSignature:
                    cmd = DmsResolver.Current.Get<ReportDocumentForDigitalSignature>();
                    break;

                case EnumActions.AddTemplate:
                    cmd = DmsResolver.Current.Get<AddTemplateCommand>();
                    break;
                case EnumActions.CopyTemplate:
                    cmd = DmsResolver.Current.Get<CopyTemplateCommand>();
                    break;
                case EnumActions.DeleteTemplate:
                    cmd = DmsResolver.Current.Get<DeleteTemplateCommand>();
                    break;
                case EnumActions.ModifyTemplate:
                    cmd = DmsResolver.Current.Get<ModifyTemplateCommand>();
                    break;
                case EnumActions.AddTemplateSendList:
                    cmd = DmsResolver.Current.Get<AddTemplateSendListCommand>();
                    break;
                case EnumActions.DeleteTemplateSendList:
                    cmd = DmsResolver.Current.Get<DeleteTemplateSendListCommand>();
                    break;
                case EnumActions.ModifyTemplateSendList:
                    cmd = DmsResolver.Current.Get<ModifyTemplateSendListCommand>();
                    break;
                case EnumActions.AddTemplateRestrictedSendList:
                    cmd = DmsResolver.Current.Get<AddTemplateRestrictedSendListCommand>();
                    break;
                case EnumActions.DeleteTemplateRestrictedSendList:
                    cmd = DmsResolver.Current.Get<DeleteTemplateRestrictedSendListCommand>();
                    break;
                case EnumActions.ModifyTemplateRestrictedSendList:
                    cmd = DmsResolver.Current.Get<ModifyTemplateRestrictedSendListCommand>();
                    break;
                case EnumActions.AddTemplateAccess:
                    cmd = DmsResolver.Current.Get<AddTemplateAccessCommand>();
                    break;
                case EnumActions.DeleteTemplateAccess:
                    cmd = DmsResolver.Current.Get<DeleteTemplateAccessCommand>();
                    break;
                case EnumActions.ModifyTemplateAccess:
                    cmd = DmsResolver.Current.Get<ModifyTemplateAccessCommand>();
                    break;

                case EnumActions.AddTemplateTask:
                    cmd = DmsResolver.Current.Get<AddTemplateTaskCommand>();
                    break;
                case EnumActions.DeleteTemplateTask:
                    cmd = DmsResolver.Current.Get<DeleteTemplateTaskCommand>();
                    break;
                case EnumActions.ModifyTemplateTask:
                    cmd = DmsResolver.Current.Get<ModifyTemplateTaskCommand>();
                    break;
                case EnumActions.AddTemplatePaper:
                    cmd = DmsResolver.Current.Get<AddTemplatePaperCommand>();
                    break;
                case EnumActions.DeleteTemplatePaper:
                    cmd = DmsResolver.Current.Get<DeleteTemplatePaperCommand>();
                    break;
                case EnumActions.ModifyTemplatePaper:
                    cmd = DmsResolver.Current.Get<ModifyTemplatePaperCommand>();
                    break;

                case EnumActions.AddTemplateFile:
                    cmd = DmsResolver.Current.Get<AddTemplateFileCommand>();
                    break;
                case EnumActions.DeleteTemplateFile:
                    cmd = DmsResolver.Current.Get<DeleteTemplateFileCommand>();
                    break;
                case EnumActions.ModifyTemplateFile:
                    cmd = DmsResolver.Current.Get<ModifyTemplateFileCommand>();
                    break;

                default:
                    throw new CommandNotDefinedError(act.ToString());
            }
            cmd.InitializeCommand(ctx, doc, param, act);
            return cmd;
        }
    }
}