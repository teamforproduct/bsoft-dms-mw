using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.AdditionalCommands;
using BL.Logic.DocumentCore.Commands;
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
                    cmd = DmsResolver.Current.Get<DeleteDocumentRestrictedSendListCommand>();
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
                case EnumDocumentActions.SendForInformation:
                    cmd = DmsResolver.Current.Get<SendForInformationDocumentCommand>();
                    break;
                case EnumDocumentActions.SendForConsideration:
                    cmd = DmsResolver.Current.Get<SendForInformationDocumentCommand>();
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

                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(ctx, doc, param);
            return cmd;
        }
    }
}