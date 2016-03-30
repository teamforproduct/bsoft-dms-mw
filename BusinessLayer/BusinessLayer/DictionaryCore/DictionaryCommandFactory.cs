using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.CustomDictionary;
using BL.Logic.DictionaryCore.DocumentType;
using BL.Logic.DictionaryCore.Tag;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore
{
    public static class DictionaryCommandFactory
    {
        public static IDictionaryCommand GetDictionaryCommand(EnumDictionaryActions act, IContext ctx, object param)
        {
            IDictionaryCommand cmd;
            switch (act)
            {
                case EnumDictionaryActions.ModifyDocumentType:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryDocumentTypeCommand>();
                    break;
                case EnumDictionaryActions.AddDocumentType:
                    cmd = DmsResolver.Current.Get<AddDictionaryDocumentTypeCommand>();
                    break;
                case EnumDictionaryActions.DeleteDocumentType:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryDocumentTypeCommand>();
                    break;
                case EnumDictionaryActions.ModifyTag:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryTagCommand>();
                    break;
                case EnumDictionaryActions.AddTag:
                    cmd = DmsResolver.Current.Get<AddDictionaryTagCommand>();
                    break;
                case EnumDictionaryActions.AddCustomDictionaryType:
                    cmd = DmsResolver.Current.Get<AddCustomDictionaryTypeCommand>();
                    break;
                case EnumDictionaryActions.ModifyCustomDictionaryType:
                    cmd = DmsResolver.Current.Get<ModifyCustomDictionaryTypeCommand>();
                    break;
                case EnumDictionaryActions.DeleteCustomDictionaryType:
                    cmd = DmsResolver.Current.Get<DeleteCustomDictionaryTypeCommand>();
                    break;
                case EnumDictionaryActions.AddCustomDictionary:
                    cmd = DmsResolver.Current.Get<AddCustomDictionaryCommand>();
                    break;
                case EnumDictionaryActions.ModifyCustomDictionary:
                    cmd = DmsResolver.Current.Get<ModifyCustomDictionaryCommand>();
                    break;
                case EnumDictionaryActions.DeleteCustomDictionary:
                    cmd = DmsResolver.Current.Get<DeleteCustomDictionaryCommand>();
                    break;
                case EnumDictionaryActions.DeleteTag:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryTagCommand>();
                    break;
                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}