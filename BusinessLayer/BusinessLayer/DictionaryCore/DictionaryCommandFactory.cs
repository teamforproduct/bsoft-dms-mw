using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.DocumentType;
using BL.Logic.DictionaryCore.Tag;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore
{
    public static class DictionaryCommandFactory
    {
        public static IDictionaryCommand GetDictionaryCommand(EnumDictionaryAction act, IContext ctx, object param)
        {
            IDictionaryCommand cmd;
            switch (act)
            {
                case EnumDictionaryAction.ModifyDocumentType:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryDocumentTypeCommand>();
                    break;
                case EnumDictionaryAction.AddDocumentType:
                    cmd = DmsResolver.Current.Get<AddDictionaryDocumentTypeCommand>();
                    break;
                case EnumDictionaryAction.ModifyTag:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryTagCommand>();
                    break;
                case EnumDictionaryAction.AddTag:
                    cmd = DmsResolver.Current.Get<AddDictionaryTagCommand>();
                    break;
                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(ctx, param);
            return cmd;
        }
    }
}