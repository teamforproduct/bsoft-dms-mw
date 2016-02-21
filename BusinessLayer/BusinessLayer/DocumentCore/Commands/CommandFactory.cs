using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public static class CommandFactory
    {
        public static ICommand GetDocumentCommand(EnumActions act, IContext ctx, InternalDocument doc, object param)
        {
            ICommand cmd;
            switch (act)
            {
                case EnumActions.AddDocument:
                    cmd = DmsResolver.Current.Get<AddDocumentCommand>();
                    break;
                case EnumActions.ModifyDocument:
                    cmd = DmsResolver.Current.Get<UpdateDocumentCommand>();
                    break;
                case EnumActions.DeleteDocument:
                    cmd = DmsResolver.Current.Get<DeleteDocumentCommand>();
                    break;

                case EnumActions.AddFavourite:
                    cmd = DmsResolver.Current.Get<AddFavouriteCommand>();
                    break;
                case EnumActions.DeleteFavourite:
                    cmd = DmsResolver.Current.Get<DeleteFavouriteCommand>();
                    break;
                case EnumActions.FinishWork:
                    cmd = DmsResolver.Current.Get<FinishWorkCommand>();
                    break;
                case EnumActions.StartWork:
                    cmd = DmsResolver.Current.Get<StartWorkCommand>();
                    break;
                case EnumActions.ControlChange:
                    cmd = DmsResolver.Current.Get<ControlChangeCommand>();
                    break;
                case EnumActions.ControlOff:
                    cmd = DmsResolver.Current.Get<ControlOffCommand>();
                    break;
                case EnumActions.ControlOn:
                    cmd = DmsResolver.Current.Get<ControlOnCommand>();
                    break;
                case EnumActions.AddNote:
                    cmd = DmsResolver.Current.Get<AddNoteCommand>();
                    break;
                case EnumActions.ChangeExecutor:
                    cmd = DmsResolver.Current.Get<ChangeExecutorCommand>();
                    break;
                case EnumActions.CopyDocument:
                    cmd = DmsResolver.Current.Get<CopyDocumentCommand>();
                    break;
                case EnumActions.RegisterDocument:
                    cmd = DmsResolver.Current.Get<RegisterDocumentCommand>();
                    break;
                case EnumActions.SendMessage:
                    cmd = DmsResolver.Current.Get<SendMessageCommand>();
                    break;

                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(ctx, doc, param);
            return cmd;
        }
    }
}