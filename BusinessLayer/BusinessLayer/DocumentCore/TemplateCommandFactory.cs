using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Commands;
using BL.Logic.DocumentCore.TemplateCommands;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore
{
    public static class TemplateCommandFactory
    {
        public static IDocumentCommand GetTemplateCommand(EnumDocumentActions act, IContext ctx, InternalDocument doc, object param)
        {
            IDocumentCommand cmd;
            switch (act)
            {
                case EnumDocumentActions.AddTemplateAttachedFile:
                    cmd = DmsResolver.Current.Get<AddTemplateFileCommand>();
                    break;
                case EnumDocumentActions.DeleteTemplateAttachedFile:
                    cmd = DmsResolver.Current.Get<DeleteTemplateFileCommand>();
                    break;
                case EnumDocumentActions.ModifyTemplateAttachedFile:
                    cmd = DmsResolver.Current.Get<ModifyTemplateFileCommand>();
                    break;
                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(ctx, doc, param, act);
            return cmd;
        }
    }
}