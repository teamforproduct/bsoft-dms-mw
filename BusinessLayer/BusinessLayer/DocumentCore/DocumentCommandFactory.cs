using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.AdditionalCommands;
using BL.Logic.DocumentCore.Commands;
using BL.Logic.DocumentCore.PaperCommands;
using BL.Logic.DocumentCore.SendListCommands;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore
{
    public static class DocumentCommandFactory
    {
        public static IDocumentCommand GetDocumentCommand(EnumDocumentActions act, IContext ctx, InternalDocument doc, object param)
        {
            IDocumentCommand cmd;
            switch (act)
            {
                case EnumDocumentActions.AddDocument:
                    cmd = DmsResolver.Current.Get<AddDocumentCommand>();
                    break;
                case EnumDocumentActions.ModifyDocument:
                    cmd = DmsResolver.Current.Get<ModifyDocumentCommand>();
                    break;
                case EnumDocumentActions.DeleteDocument:
                    cmd = DmsResolver.Current.Get<DeleteDocumentCommand>();
                    break;

                case EnumDocumentActions.AddFavourite:
                    cmd = DmsResolver.Current.Get<AddFavouriteDocumentCommand>();
                    break;
                case EnumDocumentActions.DeleteFavourite:
                    cmd = DmsResolver.Current.Get<DeleteFavouriteDocumentCommand>();
                    break;
                case EnumDocumentActions.FinishWork:
                    cmd = DmsResolver.Current.Get<FinishWorkDocumentCommand>();
                    break;
                case EnumDocumentActions.StartWork:
                    cmd = DmsResolver.Current.Get<StartWorkDocumentCommand>();
                    break;
                case EnumDocumentActions.ControlChange:
                    cmd = DmsResolver.Current.Get<ControlChangeDocumentCommand>();
                    break;
                case EnumDocumentActions.ControlTargetChange:
                    cmd = DmsResolver.Current.Get<ControlTargetChangeDocumentCommand>();
                    break;
                case EnumDocumentActions.ControlOff:
                    cmd = DmsResolver.Current.Get<ControlOffDocumentCommand>();
                    break;
                case EnumDocumentActions.ControlOn:
                    cmd = DmsResolver.Current.Get<ControlOnDocumentCommand>();
                    break;
                case EnumDocumentActions.AddNote:
                    cmd = DmsResolver.Current.Get<AddNoteDocumentCommand>();
                    break;
                case EnumDocumentActions.ChangeExecutor:
                    cmd = DmsResolver.Current.Get<ChangeExecutorDocumentCommand>();
                    break;
                case EnumDocumentActions.CopyDocument:
                    cmd = DmsResolver.Current.Get<CopyDocumentCommand>();
                    break;
                case EnumDocumentActions.RegisterDocument:
                    cmd = DmsResolver.Current.Get<RegisterDocumentCommand>();
                    break;
                case EnumDocumentActions.SendMessage:
                    cmd = DmsResolver.Current.Get<SendMessageDocumentCommand>();
                    break;
                case EnumDocumentActions.AddDocumentLink:
                    cmd = DmsResolver.Current.Get<AddDocumentLinkCommand>();
                    break;
                case EnumDocumentActions.AddDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<AddDocumentRestrictedSendListCommand>();
                    break;
                case EnumDocumentActions.AddByStandartSendListDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<AddByStandartSendListDocumentRestrictedSendListCommand>();
                    break;
                case EnumDocumentActions.DeleteDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<DeleteDocumentRestrictedSendListCommand>();
                    break;
                case EnumDocumentActions.AddDocumentSendList:
                    cmd = DmsResolver.Current.Get<AddDocumentSendListCommand>();
                    break;
                case EnumDocumentActions.AddByStandartSendListDocumentSendList:
                    cmd = DmsResolver.Current.Get<AddByStandartSendListDocumentSendListCommand>();
                    break;
                case EnumDocumentActions.ModifyDocumentSendList:
                    cmd = DmsResolver.Current.Get<ModifyDocumentSendListCommand>();
                    break;
                case EnumDocumentActions.DeleteDocumentSendList:
                    cmd = DmsResolver.Current.Get<DeleteDocumentSendListCommand>();
                    break;
                case EnumDocumentActions.AddDocumentSendListStage:
                    cmd = DmsResolver.Current.Get<AddDocumentSendListStageCommand>();
                    break;
                case EnumDocumentActions.DeleteDocumentSendListStage:
                    cmd = DmsResolver.Current.Get<DeleteDocumentSendListStageCommand>();
                    break;
                case EnumDocumentActions.ModifyDocumentTags:
                    cmd = DmsResolver.Current.Get<ModifyDocumentTagsCommand>();
                    break;

                case EnumDocumentActions.DeleteDocumentFile:
                    cmd = DmsResolver.Current.Get<DeleteDocumentFileCommand>();
                    break;
                case EnumDocumentActions.AddDocumentFile:
                    cmd = DmsResolver.Current.Get<AddDocumentFileCommand>();
                    break;
                case EnumDocumentActions.ModifyDocumentFile:
                    cmd = DmsResolver.Current.Get<ModifyDocumentFileCommand>();
                    break;
                case EnumDocumentActions.SendForInformationExternal:
                    cmd = DmsResolver.Current.Get<SendForInformationDocumentCommand>();
                    break;
                case EnumDocumentActions.SendForInformation:
                    cmd = DmsResolver.Current.Get<SendForInformationDocumentCommand>();
                    break;
                case EnumDocumentActions.SendForConsideration:
                    cmd = DmsResolver.Current.Get<SendForInformationDocumentCommand>();
                    break;
                case EnumDocumentActions.SendForControl:
                    cmd = DmsResolver.Current.Get<SendForControlDocumentCommand>();
                    break;

                case EnumDocumentActions.SendForResponsibleExecution:
                    cmd = DmsResolver.Current.Get<SendForResponsibleExecutionDocumentCommand>();
                    break;
                case EnumDocumentActions.SendForExecution:
                    cmd = DmsResolver.Current.Get<SendForExecutionDocumentCommand>();
                    break;
                case EnumDocumentActions.MarkExecution:
                    cmd = DmsResolver.Current.Get<MarkExecutionDocumentCommand>();
                    break;
                case EnumDocumentActions.RejectResult:
                    cmd = DmsResolver.Current.Get<RejectResultDocumentCommand>();
                    break;
                case EnumDocumentActions.AcceptResult:
                    cmd = DmsResolver.Current.Get<AcceptResultDocumentCommand>();
                    break;

                case EnumDocumentActions.SendForSigning:
                    cmd = DmsResolver.Current.Get<SendForSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.SendForVisaing:
                    cmd = DmsResolver.Current.Get<SendForSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.SendForАgreement:
                    cmd = DmsResolver.Current.Get<SendForSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.SendForАpproval:
                    cmd = DmsResolver.Current.Get<SendForSigningDocumentCommand>();
                    break;

                case EnumDocumentActions.RejectSigning:
                    cmd = DmsResolver.Current.Get<RejectSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.RejectVisaing:
                    cmd = DmsResolver.Current.Get<RejectSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.RejectАgreement:
                    cmd = DmsResolver.Current.Get<RejectSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.RejectАpproval:
                    cmd = DmsResolver.Current.Get<RejectSigningDocumentCommand>();
                    break;

                case EnumDocumentActions.WithdrawSigning:
                    cmd = DmsResolver.Current.Get<WithdrawSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.WithdrawVisaing:
                    cmd = DmsResolver.Current.Get<WithdrawSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.WithdrawАgreement:
                    cmd = DmsResolver.Current.Get<WithdrawSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.WithdrawАpproval:
                    cmd = DmsResolver.Current.Get<WithdrawSigningDocumentCommand>();
                    break;

                case EnumDocumentActions.AffixSigning:
                    cmd = DmsResolver.Current.Get<AffixSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.AffixVisaing:
                    cmd = DmsResolver.Current.Get<AffixSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.AffixАgreement:
                    cmd = DmsResolver.Current.Get<AffixSigningDocumentCommand>();
                    break;
                case EnumDocumentActions.AffixАpproval:
                    cmd = DmsResolver.Current.Get<AffixSigningDocumentCommand>();
                    break;

                case EnumDocumentActions.AddSavedFilter:
                    cmd = DmsResolver.Current.Get<AddSavedFilterCommand>();
                    break;
                case EnumDocumentActions.DeleteSavedFilter:
                    cmd = DmsResolver.Current.Get<DeleteSavedFilterCommand>();
                    break;
                case EnumDocumentActions.ModifySavedFilter:
                    cmd = DmsResolver.Current.Get<ModifySavedFilterCommand>();
                    break;

                case EnumDocumentActions.LaunchDocumentSendListItem:
                    cmd = DmsResolver.Current.Get<LaunchDocumentSendListItemCommand>();
                    break;

                case EnumDocumentActions.LaunchPlan:
                    cmd = DmsResolver.Current.Get<LaunchPlanDocumentCommand>();
                    break;
                case EnumDocumentActions.StopPlan:
                    cmd = DmsResolver.Current.Get<StopPlanDocumentCommand>();
                    break;

                case EnumDocumentActions.MarkDocumentEventAsRead:
                    cmd = DmsResolver.Current.Get<MarkDocumentEventAsReadCommand>();
                    break;

                case EnumDocumentActions.AddDocumentTask:
                    cmd = DmsResolver.Current.Get<AddDocumentTaskCommand>();
                    break;

                case EnumDocumentActions.ModifyDocumentTask:
                    cmd = DmsResolver.Current.Get<ModifyDocumentTaskCommand>();
                    break;

                case EnumDocumentActions.DeleteDocumentTask:
                    cmd = DmsResolver.Current.Get<DeleteDocumentTaskCommand>();
                    break;

                case EnumDocumentActions.AddDocumentPaper:
                    cmd = DmsResolver.Current.Get<AddDocumentPaperCommand>();
                    break;

                case EnumDocumentActions.ModifyDocumentPaper:
                    cmd = DmsResolver.Current.Get<ModifyDocumentPaperCommand>();
                    break;

                case EnumDocumentActions.DeleteDocumentPaper:
                    cmd = DmsResolver.Current.Get<DeleteDocumentPaperCommand>();
                    break;

                case EnumDocumentActions.MarkOwnerDocumentPaper:
                    cmd = DmsResolver.Current.Get<MarkOwnerDocumentPaperCommand>();
                    break;

                case EnumDocumentActions.MarkСorruptionDocumentPaper:
                    cmd = DmsResolver.Current.Get<MarkСorruptionDocumentPaperCommand>();
                    break;

                case EnumDocumentActions.AddDocumentPaperList:
                    cmd = DmsResolver.Current.Get<AddDocumentPaperListCommand>();
                    break;

                case EnumDocumentActions.ModifyDocumentPaperList:
                    cmd = DmsResolver.Current.Get<ModifyDocumentPaperListCommand>();
                    break;

                case EnumDocumentActions.DeleteDocumentPaperList:
                    cmd = DmsResolver.Current.Get<DeleteDocumentPaperListCommand>();
                    break;

                case EnumDocumentActions.AddDocumentSendListItem:
                case EnumDocumentActions.DeleteDocumentLink:
                    return null;
                    break;
                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(ctx, doc, param, act);
            return cmd;
        }
    }
}