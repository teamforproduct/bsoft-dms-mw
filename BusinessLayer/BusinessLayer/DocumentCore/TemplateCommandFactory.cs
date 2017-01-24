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
                case EnumDocumentActions.AddTemplateDocument:
                    cmd = DmsResolver.Current.Get<AddTemplateCommand>();
                    break;
                case EnumDocumentActions.CopyTemplateDocument:
                    cmd = DmsResolver.Current.Get<CopyTemplateCommand>();
                    break;
                case EnumDocumentActions.DeleteTemplateDocument:
                    cmd = DmsResolver.Current.Get<DeleteTemplateCommand>();
                    break;
                case EnumDocumentActions.ModifyTemplateDocument:
                    cmd = DmsResolver.Current.Get<ModifyTemplateCommand>();
                    break;
                case EnumDocumentActions.AddTemplateDocumentSendList:
                    cmd = DmsResolver.Current.Get<AddTemplateSendListCommand>();
                    break;
                case EnumDocumentActions.DeleteTemplateDocumentSendList:
                    cmd = DmsResolver.Current.Get<DeleteTemplateSendListCommand>();
                    break;
                case EnumDocumentActions.ModifyTemplateDocumentSendList:
                    cmd = DmsResolver.Current.Get<ModifyTemplateSendListCommand>();
                    break;
                case EnumDocumentActions.AddTemplateDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<AddTemplateRestrictedSendListCommand>();
                    break;
                case EnumDocumentActions.DeleteTemplateDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<DeleteTemplateRestrictedSendListCommand>();
                    break;
                case EnumDocumentActions.ModifyTemplateDocumentRestrictedSendList:
                    cmd = DmsResolver.Current.Get<ModifyTemplateRestrictedSendListCommand>();
                    break;
                case EnumDocumentActions.AddTemplateDocumentTask:
                    cmd = DmsResolver.Current.Get<AddTemplateTaskCommand>();
                    break;
                case EnumDocumentActions.DeleteTemplateDocumentTask:
                    cmd = DmsResolver.Current.Get<DeleteTemplateTaskCommand>();
                    break;
                case EnumDocumentActions.ModifyTemplateDocumentTask:
                    cmd = DmsResolver.Current.Get<ModifyTemplateTaskCommand>();
                    break;
                case EnumDocumentActions.AddTemplateDocumentPaper:
                    cmd = DmsResolver.Current.Get<AddTemplatePaperCommand>();
                    break;
                case EnumDocumentActions.DeleteTemplateDocumentPaper:
                    cmd = DmsResolver.Current.Get<DeleteTemplatePaperCommand>();
                    break;
                case EnumDocumentActions.ModifyTemplateDocumentPaper:
                    cmd = DmsResolver.Current.Get<ModifyTemplatePaperCommand>();
                    break;

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
                    throw new CommandNotDefinedError(act.ToString());
            }
            cmd.InitializeCommand(ctx, doc, param, act);
            return cmd;
        }
    }
}