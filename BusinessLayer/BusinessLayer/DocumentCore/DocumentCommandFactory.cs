using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Commands;
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

                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(ctx, doc, param);
            return cmd;
        }
    }
}