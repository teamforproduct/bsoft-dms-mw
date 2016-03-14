using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.CustomDictionary;
using BL.Logic.DictionaryCore.DocumentType;
using BL.Logic.DictionaryCore.Contact;
using BL.Logic.DictionaryCore.ContactType;
using BL.Logic.DictionaryCore.AgentAdresses;
using BL.Logic.DictionaryCore.AgentPerson;
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

                case EnumDictionaryActions.ModifyAgentPerson:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentPersonCommand>();
                    break;
                case EnumDictionaryActions.AddAgentPerson:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentPersonCommand>();
                    break;
                case EnumDictionaryActions.DeleteAgentPerson:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentPersonCommand>();
                    break;

                case EnumDictionaryActions.ModifyAgentAddress:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAgentAddressCommand>();
                    break;
                case EnumDictionaryActions.AddAgentAddress:
                    cmd = DmsResolver.Current.Get<AddDictionaryAgentAddressCommand>();
                    break;
                case EnumDictionaryActions.DeleteAgentAddress:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAgentAddressCommand>();
                    break;

                case EnumDictionaryActions.ModifyAddressType:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryAddressTypeCommand>();
                    break;
                case EnumDictionaryActions.AddAddressType:
                    cmd = DmsResolver.Current.Get<AddDictionaryAddressTypeCommand>();
                    break;
                case EnumDictionaryActions.DeleteAddressType:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryAddressTypeCommand>();
                    break;


                case EnumDictionaryActions.ModifyContact:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryContactCommand>();
                    break;
                case EnumDictionaryActions.AddContact:
                    cmd = DmsResolver.Current.Get<AddDictionaryContactCommand>();
                    break;
                case EnumDictionaryActions.DeleteContact:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryContactCommand>();
                    break;

                case EnumDictionaryActions.ModifyContactType:
                    cmd = DmsResolver.Current.Get<ModifyDictionaryContactTypeCommand>();
                    break;
                case EnumDictionaryActions.AddContactType:
                    cmd = DmsResolver.Current.Get<AddDictionaryContactTypeCommand>();
                    break;
                case EnumDictionaryActions.DeleteContactType:
                    cmd = DmsResolver.Current.Get<DeleteDictionaryContactTypeCommand>();
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
                    return null;
                    break;
                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}