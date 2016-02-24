using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.AdditionalCommands;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore
{
    public static class DocumentAdditionCommandFactory
    {
        public static IDocumentAdditionCommand GetDocumentCommand(EnumDocumentAdditionActions act, IContext ctx, InternalDocument doc, object param)
        {
            IDocumentAdditionCommand cmd;
            switch (act)
            {
                case EnumDocumentAdditionActions.AddDocumentLink:
                    cmd = DmsResolver.Current.Get<AddDocumentLinkCommand>();
                    break;
                case EnumDocumentAdditionActions.AddDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<AddDocumentRestrictedSendListCommand>();
                    break;
                case EnumDocumentAdditionActions.AddByStandartSendListDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<AddByStandartSendListDocumentRestrictedSendListCommand>();
                    break;
                case EnumDocumentAdditionActions.DeleteDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<DeleteDocumentRestrictedSendListCommand>();
                    break;
                case EnumDocumentAdditionActions.AddDocumentSendList:
                    cmd = DmsResolver.Current.Get<AddDocumentSendListCommand>();
                    break;
                case EnumDocumentAdditionActions.AddByStandartSendListDocumentSendList:
                    cmd = DmsResolver.Current.Get<AddByStandartSendListDocumentSendListCommand>();
                    break;
                case EnumDocumentAdditionActions.ModifyDocumentSendList:
                    cmd = DmsResolver.Current.Get<ModifyDocumentSendListCommand>();
                    break;
                case EnumDocumentAdditionActions.DeleteDocumentSendList:
                    cmd = DmsResolver.Current.Get<DeleteDocumentRestrictedSendListCommand>();
                    break;
                case EnumDocumentAdditionActions.AddDocumentSendListStage:
                    cmd = DmsResolver.Current.Get<AddDocumentSendListStageCommand>();
                    break;
                case EnumDocumentAdditionActions.DeleteDocumentSendListStage:
                    cmd = DmsResolver.Current.Get<DeleteDocumentSendListStageCommand>();
                    break;
                case EnumDocumentAdditionActions.ModifyDocumentTags:
                    cmd = DmsResolver.Current.Get<ModifyDocumentTagsCommand>();
                    break;
                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(ctx, doc, param);
            return cmd;
        }
    }
}