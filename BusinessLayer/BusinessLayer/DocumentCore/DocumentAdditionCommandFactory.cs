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

                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(ctx, doc, param);
            return cmd;
        }
    }
}